using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Twig
{
    public Vector3 StartPosition;
    public Vector3 EndPosition;

    public GameObject Dummy = null;
    public LineRenderer LineRenderer = null;
}

public class Context
{
    public float TwigWidth = 0f;
    public Vector2 LastTwigEnd = new Vector2(0,0);
    public float BaseRotation = 0f;
    public int Depth = 0;

    public Context Clone()
    {
        return new Context()
        {
            LastTwigEnd = this.LastTwigEnd, 
            BaseRotation = this.BaseRotation, 
            TwigWidth = this.TwigWidth,
            Depth = this.Depth
        };
    }
}

public class Grower : MonoBehaviour
{
    public PlantDNA PlantDNA;

    public Parser Parser;
    public int Iteration = 0;

    public List<Twig> Plant;
    public List<ParametrizedTreeNode> PlantSkeleton;

    public float CurrentWaterLevel = 100f;
    public Transform WaterFillUI;

    void Awake()
    {
        Plant = new List<Twig>();
        Regrow();       
    }

    private float _timer = 0f;
    private float _timerMax = 0.1f;

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
            CurrentWaterLevel += 30;
        if (CurrentWaterLevel < 0)
            return;

        _timer += Time.deltaTime;
        if(_timer > _timerMax && Iteration <= (PlantDNA.AvarageLifetime / _timerMax))
        {
            _timer = 0f;
            Draw();
            
        }
        CurrentWaterLevel -= Time.deltaTime * PlantDNA.WaterGrowthUsagePerSecond;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if(WaterFillUI != null)
        {
            var newScale = new Vector3(1, CurrentWaterLevel / 100, 1);
            WaterFillUI.localScale = newScale;
        }
    }

    public void Regrow()
    {
        Debug.Log("Regrow");
        Random.InitState((int)System.DateTime.Now.Ticks);

        foreach (var t in Plant)
            Destroy(t.Dummy);

        Plant = new List<Twig>();
        Parser = new Parser(PlantDNA);
        Iteration = 0;

        Iterate();
    }

    public void Iterate()
    {
        for (int i = 0; i < PlantDNA.MaxIterations; i++)
            Parser.Iterate();

        PlantSkeleton = Parser.Parse();
    }


    public void Draw()
    {
        foreach (var t in Plant)
            Destroy(t.Dummy);

        Plant = new List<Twig>();
        var contextStack = new Stack<Context>();
        contextStack.Push(new Context());
        contextStack.Peek().TwigWidth = PlantDNA.RootTwigWidth;
        contextStack.Peek().LastTwigEnd = (Vector2)transform.position;

        var currentPercent = (float)Iteration / (PlantDNA.AvarageLifetime / 0.1f);
        var maxIndex = (int)(PlantSkeleton.Count * currentPercent);
        var parametrizedTree = PlantSkeleton.Take(maxIndex);

        foreach (var data in parametrizedTree)
        {
            if(data.Symbol == 'F')
            {
                var endLenght = (Vector2.up * PlantDNA.AvarageTwigLenght * PlantDNA.TwigBranchDepthLengthFalloff);
                var twigRotation = (data.TwigAngleVariation ?? 0) + contextStack.Peek().BaseRotation;

                var dummy = new GameObject();
                dummy.transform.parent = transform;
                var lr = dummy.AddComponent<LineRenderer>();

                var twig = new Twig()
                {
                    StartPosition = contextStack.Peek().LastTwigEnd,
                    EndPosition = contextStack.Peek().LastTwigEnd + (Vector2)(Quaternion.AngleAxis(twigRotation, Vector3.forward) * endLenght),
                    Dummy = dummy,
                    LineRenderer = lr
                };

                lr.positionCount = 2;
                lr.useWorldSpace = false;
                lr.startWidth = lr.endWidth = contextStack.Peek().TwigWidth;
                lr.material = new Material(Shader.Find("Sprites/Default"));
                lr.startColor = lr.endColor = PlantDNA.TwigColor;
                lr.SetPositions(new Vector3[]
                {
                    (Vector2)twig.StartPosition,
                    (Vector2)twig.EndPosition,
                });

                contextStack.Peek().LastTwigEnd = twig.EndPosition;
                contextStack.Peek().TwigWidth *= .98f;

                Plant.Add(twig);
            }
            else if(data.Symbol == '+' || data.Symbol == '-')
            {
                contextStack.Peek().BaseRotation += data.BranchAngleVariation ?? 0;
            }
            else if(data.Symbol == '[')
            {
                var newContext = contextStack.Peek().Clone();
                newContext.Depth++;
                newContext.TwigWidth *= PlantDNA.TwigBranchDepthWidthFalloff;
                contextStack.Push(newContext);
            }
            else if(data.Symbol == ']')
            {
                contextStack.Pop();
            }
            else if (data.Symbol == 'A')
            {
                var twig = Plant.Last();
                var dir = (twig.EndPosition - twig.StartPosition).normalized;
                twig.EndPosition += dir * (PlantDNA.AvarageTwigLenght);

                contextStack.Peek().LastTwigEnd = twig.EndPosition;
                contextStack.Peek().TwigWidth *= .98f;

                twig.LineRenderer.SetPosition(1, twig.EndPosition);
                twig.LineRenderer.endWidth = contextStack.Peek().TwigWidth;
            }
        }
        Iteration++;
    }
}