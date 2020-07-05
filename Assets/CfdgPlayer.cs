using System.Collections.Generic;
using UnityEngine;

public interface CfdgRenderer
{
    void Init(List<Shape> shapes);
    void Draw(float relativeTime);
    void ShutDown();
}

public class CfdgPlayer : MonoBehaviour
{
    public Camera camera;
    public float animationDuration;
    public bool repeat;

    [HideInInspector]
    public string grammarStr;
    public float currentTime { get; set; }

    private CfdgRenderer m_Renderer;

    private HsvColor m_BackgroundColor;
    private Rect m_Extent;
    private int m_MaxShapeCount;
    private float m_RelativeTime;

    public void Run()
    {
        Shutdown();

        List<Shape> shapes;
        using (new DebugLogTimer("Evaluation"))
            shapes = Evaluate();

        using (new DebugLogTimer("Renderer Init"))
            m_Renderer.Init(shapes);
        
        SetupCamera();
        currentTime = 0;
    }

    public void Shutdown()
    {
        m_Renderer.ShutDown();
    }

    void OnEnable()
    {
        if (string.IsNullOrEmpty(grammarStr))
        {
            grammarStr = SampleGrammars.SimpleBubbleGrammar;
        }
    }

    void Start()
    {
        var components = GetComponents<Component>();
        foreach (var component in components)
        {
            if (component is CfdgRenderer cfdgRenderer)
            {
                m_Renderer = cfdgRenderer;
                break;
            }
        }

        Run();
    }

    void SetupCamera()
    {
        camera.backgroundColor = m_BackgroundColor.ToRGBA();
        camera.transform.position = new Vector3(m_Extent.center.x, m_Extent.center.y, camera.transform.position.z);
        var size = m_Extent.height/* / 2f*/;
        camera.orthographicSize = size;
    }

    private List<Shape> Evaluate()
    {
        var grammar = grammarStr == null ? SampleGrammars.SimpleBubble() : CfdgCompiler.Compile(grammarStr);
        var evaluator = new Evaluator();
        evaluator.grammar = grammar;
        evaluator.Evaluate();

        m_BackgroundColor = new HsvColor() { h = 0, s = 0, v = 1, a = 1 };
        m_BackgroundColor = Utils.AdjustColor(m_BackgroundColor, grammar.backgroundColor);

        m_MaxShapeCount = evaluator.shapes.Count;

        m_Extent = new Rect();
        foreach (var shape in evaluator.shapes)
        {
            var shapeRect = new Rect();
            shapeRect.center = new Vector2(shape.x, shape.y);
            shapeRect.width = shape.width;
            shapeRect.height = shape.height;

            if (shapeRect.xMin < m_Extent.xMax)
                m_Extent.xMin = shapeRect.xMin;
            if (shapeRect.xMax > m_Extent.xMax)
                m_Extent.xMax = shapeRect.xMax;
            if (shapeRect.yMin < m_Extent.yMin)
                m_Extent.yMin = shapeRect.yMin;
            if (shapeRect.yMax > m_Extent.yMax)
                m_Extent.yMax = shapeRect.yMax;

        }

        return evaluator.shapes;
    }

    void Update()
    {
        currentTime += Time.deltaTime;
        m_RelativeTime = animationDuration == 0 || currentTime > animationDuration ? 1f : currentTime / animationDuration;
        m_RelativeTime = Mathf.Clamp(m_RelativeTime, 0, 1);
        m_RelativeTime = EaseInCubic(0, 1, m_RelativeTime);
        
        m_Renderer.Draw(m_RelativeTime);

        if (repeat)
        {
            currentTime = currentTime % animationDuration;
        }
    }

    public static float EaseInCubic(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value + start;
    }

    void OnGUI()
    {
        var msg = $"Time: {m_RelativeTime.ToString("F5")} Shapes: {m_MaxShapeCount}";
        GUILayout.Label(msg);
    }

    private void OnDisable()
    {
        Shutdown();
    }
}
