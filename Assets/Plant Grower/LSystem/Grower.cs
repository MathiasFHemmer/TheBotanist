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

    public float WaterMeterCurrent = 100f;
    public float WaterMeterMax = 100f;

    int LastGrowthIndex = 0;
    Stack<Context> CurrentPlantContext;

    public delegate void OnPlantFullGrown();
    public event OnPlantFullGrown OnPlantFullGrownHandler;
    public delegate void OnWaterUsed(float current, float max);
    public event OnWaterUsed OnWaterUsedHandler;
    public delegate void OnWaterAdded(float ammount, float max);
    public event OnWaterAdded OnWaterAddedHandler;
    public delegate void OnWaterDepleted();
    public event OnWaterDepleted OnWaterDepletedHandler;

    public delegate void OnGrowth(float current, float max);
    public event OnGrowth OnGrowthHandler;

    void Awake()
    {
        Plant = new List<Twig>();
        Regrow();       
    }

    private float _timer = 0f;
    private float _timerMax = 0.1f;

    private void Update()
    {
        var iterationFinished = Iteration >= (PlantDNA.AvarageLifetime / _timerMax);

        if (Input.GetMouseButtonUp(0))
        {
            WaterMeterCurrent = Mathf.Clamp(WaterMeterCurrent + 30, 0, WaterMeterMax);
            OnWaterAddedHandler?.Invoke(30, WaterMeterMax);
        }

        if (WaterMeterCurrent < 0)
            return;

        _timer += Time.deltaTime;
        if(_timer >= _timerMax && !iterationFinished)
        {
            _timer = 0f;
            Draw();
            OnGrowthHandler?.Invoke(Iteration, PlantDNA.AvarageLifetime / _timerMax);
        }

        WaterMeterCurrent -= Time.deltaTime * PlantDNA.WaterGrowthUsagePerSecond;
        OnWaterUsedHandler?.Invoke(WaterMeterCurrent, WaterMeterMax);

        if (WaterMeterCurrent <= 0)
            OnWaterDepletedHandler?.Invoke();

        if (iterationFinished)
            OnPlantFullGrownHandler?.Invoke();
    }
    public void Regrow()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);

        foreach (var t in Plant)
            Destroy(t.Dummy);

        Plant = new List<Twig>();
        Parser = new Parser(PlantDNA);
        CurrentPlantContext = new Stack<Context>();
        var defaultContext = new Context
        {
            TwigWidth = PlantDNA.RootTwigWidth,
            LastTwigEnd = (Vector2)transform.position,
        };
        CurrentPlantContext.Push(defaultContext);
        LastGrowthIndex = 0;
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
        var currentPercent = (float)Iteration / (PlantDNA.AvarageLifetime / _timerMax);
        var maxIndex = (int)(PlantSkeleton.Count * currentPercent);
        
        Iteration++;

        if (maxIndex == LastGrowthIndex) return;
        var parametrizedTree = PlantSkeleton.Skip(LastGrowthIndex).Take(maxIndex - LastGrowthIndex);
        LastGrowthIndex = maxIndex;

        foreach (var data in parametrizedTree)
        {
            if(data.Symbol == 'F')
            {
                var endLenght = (Vector2.up * PlantDNA.AvarageTwigLenght * PlantDNA.TwigBranchDepthLengthFalloff);
                var twigRotation = (data.TwigAngleVariation ?? 0) + CurrentPlantContext.Peek().BaseRotation;

                var dummy = new GameObject();
                dummy.transform.parent = transform;
                var lr = dummy.AddComponent<LineRenderer>();

                var twig = new Twig()
                {
                    StartPosition = CurrentPlantContext.Peek().LastTwigEnd,
                    EndPosition = CurrentPlantContext.Peek().LastTwigEnd + (Vector2)(Quaternion.AngleAxis(twigRotation, Vector3.forward) * endLenght),
                    Dummy = dummy,
                    LineRenderer = lr
                };

                lr.positionCount = 2;
                lr.useWorldSpace = false;
                lr.startWidth = lr.endWidth = CurrentPlantContext.Peek().TwigWidth;
                lr.material = new Material(Shader.Find("Sprites/Default"));
                lr.startColor = lr.endColor = PlantDNA.TwigColor;
                lr.SetPositions(new Vector3[]
                {
                    (Vector2)twig.StartPosition,
                    (Vector2)twig.EndPosition,
                });

                CurrentPlantContext.Peek().LastTwigEnd = twig.EndPosition;
                CurrentPlantContext.Peek().TwigWidth *= .98f;

                Plant.Add(twig);
            }
            else if(data.Symbol == '+' || data.Symbol == '-')
            {
                CurrentPlantContext.Peek().BaseRotation += data.BranchAngleVariation ?? 0;
            }
            else if(data.Symbol == '[')
            {
                var newContext = CurrentPlantContext.Peek().Clone();
                newContext.Depth++;
                newContext.TwigWidth *= PlantDNA.TwigBranchDepthWidthFalloff;
                CurrentPlantContext.Push(newContext);
            }
            else if(data.Symbol == ']')
            {
                CurrentPlantContext.Pop();
            }
            else if (data.Symbol == 'A')
            {
                var twig = Plant.Last();
                var dir = (twig.EndPosition - twig.StartPosition).normalized;
                twig.EndPosition += dir * (PlantDNA.AvarageTwigLenght);

                CurrentPlantContext.Peek().LastTwigEnd = twig.EndPosition;
                CurrentPlantContext.Peek().TwigWidth *= .98f;

                twig.LineRenderer.SetPosition(1, twig.EndPosition);
                twig.LineRenderer.endWidth = CurrentPlantContext.Peek().TwigWidth;
            }
        }
    }
}
