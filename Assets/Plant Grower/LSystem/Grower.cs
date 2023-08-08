using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Twig
{
    public Vector3 StartPosition;
    public Vector3 EndPosition;
}

public class Context
{
    public float TwigWidth = 0f;
    public float AvarageTwigLength = 0f;
    public Vector2 LastTwigEnd = new Vector2(0,0);
    public float BaseRotation = 0f;
    public int Depth = 0;

    public GameObject Dummy;
    public LineRenderer LineRenderer;

    public Context Clone()
    {
        return new Context()
        {
            LastTwigEnd = this.LastTwigEnd, 
            BaseRotation = this.BaseRotation, 
            TwigWidth = this.TwigWidth,
            Depth = this.Depth,
            AvarageTwigLength= this.AvarageTwigLength
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
        CurrentPlantContext = new Stack<Context>();
        Regrow();       
    }

    private float _timer = 0f;
    private float _timerMax = 0.1f;

    private void Update()
    {
        var iterationFinished = Iteration >= (PlantDNA.AvarageLifetime / _timerMax);

        if (iterationFinished) return;

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

        foreach (var c in CurrentPlantContext)
            Destroy(c.Dummy);

        Plant = new List<Twig>();
        Parser = new Parser(PlantDNA);
        CurrentPlantContext = new Stack<Context>();

        var dummy = new GameObject();
        dummy.transform.parent = transform;
        var renderer = dummy.AddComponent<LineRenderer>();

        renderer.positionCount = 1;
        renderer.material = new Material(Shader.Find("Sprites/Default"));
        renderer.SetPosition(renderer.positionCount - 1, (Vector2)transform.position);
        renderer.startWidth = renderer.endWidth = PlantDNA.RootTwigWidth;
        renderer.startColor = renderer.endColor = PlantDNA.TwigColor;

        var defaultContext = new Context
        {
            TwigWidth = PlantDNA.RootTwigWidth,
            LastTwigEnd = (Vector2)transform.position,
            Dummy = dummy,
            LineRenderer = renderer,
            AvarageTwigLength = PlantDNA.AvarageTwigLenght
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
            var currentContext = CurrentPlantContext.Peek();

            if (data.Symbol == 'F')
            {
                var endLenght = Vector2.up * currentContext.AvarageTwigLength;
                var twigRotation = (data.TwigAngleVariation ?? 0) + CurrentPlantContext.Peek().BaseRotation;

                var twig = new Twig()
                {
                    StartPosition = currentContext.LastTwigEnd,
                    EndPosition = currentContext.LastTwigEnd + (Vector2)(Quaternion.AngleAxis(twigRotation, Vector3.forward) * endLenght),
                };
                currentContext.LastTwigEnd = twig.EndPosition;
                currentContext.TwigWidth *= .98f;
                Plant.Add(twig);

                currentContext.LineRenderer.positionCount++;
                currentContext.LineRenderer.SetPosition(currentContext.LineRenderer.positionCount - 1, twig.EndPosition);
                currentContext.LineRenderer.endWidth = currentContext.TwigWidth;
            }
            else if(data.Symbol == '+' || data.Symbol == '-')
                CurrentPlantContext.Peek().BaseRotation += data.BranchAngleVariation ?? 0;
            else if(data.Symbol == '[')
            {
                var dummy = new GameObject();
                dummy.transform.parent = currentContext.Dummy.transform;
                var renderer = dummy.AddComponent<LineRenderer>();

                var newTwighStartWidth = currentContext.TwigWidth *= PlantDNA.TwigBranchDepthWidthFalloff;

                renderer.positionCount = 1;
                renderer.material = new Material(Shader.Find("Sprites/Default"));
                renderer.startColor = renderer.endColor = PlantDNA.TwigColor;
                renderer.SetPosition(renderer.positionCount - 1, currentContext.LastTwigEnd);
                renderer.startWidth = renderer.endWidth = newTwighStartWidth;

                var newContext = currentContext.Clone();
                newContext.Depth++;
                newContext.TwigWidth = newTwighStartWidth;
                newContext.Dummy = dummy;
                newContext.LineRenderer = renderer;
                newContext.AvarageTwigLength *= PlantDNA.TwigBranchDepthLengthFalloff;

                CurrentPlantContext.Push(newContext);
            }
            else if(data.Symbol == ']')
                CurrentPlantContext.Pop();
            
        }
    }
}
