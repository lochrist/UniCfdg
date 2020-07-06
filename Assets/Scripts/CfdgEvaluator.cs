using System;
using System.Collections.Generic;
using System.Linq;
using TreeEditor;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Diagnostics.DebuggerDisplay("{h},{s},{v},{a}")]
public struct HsvColor
{
    public float h;
    public float s;
    public float v;
    public float a;

    public override string ToString()
    {
        return $"{h},{s},{v},{a}";
    }

    public Color ToRGBA()
    {
        var rgb = Color.HSVToRGB(h / 360f, s, v);
        return new Color(rgb.r, rgb.g, rgb.b, a);
    }
}

[System.Diagnostics.DebuggerDisplay("{id}")]
public class Replacement
{
    public static Replacement defaultShape = new Replacement();

    public Replacement()
    {
        transform = Matrix4x4.identity;
        color = new HsvColor() { a = 0, h = 0, s = 0, v = 0 };
    }

    public void SetSquare()
    {
        id = "SQUARE";
        isTerminal = true;
    }

    public void SetCircle()
    {
        id = "CIRCLE";
        isTerminal = true;
    }

    public void SetTriangle()
    {
        id = "TRIANGLE";
        isTerminal = true;
    }

    public string id;
    public Matrix4x4 transform;
    public float[] transform2D => Utils.ToTransform2D(transform);
    public HsvColor color;
    public bool isTerminal;
    public bool isLoop => loop > 0;
    public int loop;
    public Replacement[] replacements;
}

[System.Diagnostics.DebuggerDisplay("{name}")]
public class Rule
{
    public Rule()
    {
        weight = 1f;
    }

    public string name;
    public float weight;
    public Replacement[] replacements;
    public float probability;
}

public class Grammar
{
    public string name;
    public string startShape;
    public HsvColor backgroundColor;
    public Dictionary<string, List<Rule>> ruleGroups;

    public Grammar()
    {
        ruleGroups = new Dictionary<string, List<Rule>>();
        backgroundColor = new HsvColor() { h = 0, s = 0, v = 0, a = 0 };
    }

    public void AddRule(Rule rule)
    {
        if (!ruleGroups.TryGetValue(rule.name, out var rules))
        {
            rules = new List<Rule>();
            ruleGroups.Add(rule.name, rules);
        }
        rules.Add(rule);
    }

    public Rule GetRule(string ruleName, float probability = -1)
    {
        if (!ruleGroups.ContainsKey(ruleName))
            throw new Exception($"Rule not found: {ruleName}");

        var rules = ruleGroups[ruleName];
        if (rules.Count == 0)
            throw new Exception("Rules empty");

        if (rules.Count == 1)
        {
            return rules[0];
        }

        var total = 0f;
        probability = probability < 0 ? Random.value : probability;
        for (var i = 0; i < rules.Count; ++i)
        {
            total += rules[i].probability;
            if (probability < total)
                return rules[i];
        }

        throw new Exception($"Cannot find rule with probability for : {ruleName}");
    }

    public void FinishSetup()
    {
        foreach (var rules in ruleGroups.Values)
        {
            var sum = rules.Sum(r => r.weight);
            foreach (var rule in rules)
            {
                rule.probability = rule.weight / sum;
            }
        }
    }
}

public struct Shape
{
    public Matrix4x4 transform;
    public Color color;
    public HsvColor hsv;
    public string shapeId;
    public float area;
    public int index;

    public override string ToString()
    {
        var t2D = transform2D;
        var tt = $"{t2D[0]},{t2D[1]},{t2D[2]},{t2D[3]},{t2D[4]},{t2D[5]}";
        return $"{shapeId} area: {area} hsv: {hsv} t:{tt}";
    }

    public float x => transform.m03;
    public float y => transform.m13;
    public float width => transform.m00;
    public float height => transform.m11;

    public float[] transform2D => Utils.ToTransform2D(transform);
}

public struct EvaluationStackFrame
{
    public EvaluationStackFrame(Replacement shape)
    {
        this.replacement = shape;
        transform = Matrix4x4.identity;
        color = new HsvColor() { h = 0, s = 0, v = 0, a = 1 };
    }

    public float[] transform2D => Utils.ToTransform2D(transform);

    public Matrix4x4 transform;
    public HsvColor color;
    public Replacement replacement;
}

public class TooManyShapesException : Exception
{
    public TooManyShapesException(string msg)
        : base(msg)
    {

    }
}

public class Evaluator
{
    public Grammar grammar;
    public List<Shape> shapes;
    public Queue<EvaluationStackFrame> frames;

    public int maxNbShapes = 500000;
    public float areaCutoff = 0.000003f;

    private int m_RuleCount;
    private int m_ShapeCount;
    private int m_Culled;

    public Evaluator()
    {
        frames = new Queue<EvaluationStackFrame>();
        shapes = new List<Shape>(maxNbShapes);
    }

    public void Evaluate()
    {
        grammar.FinishSetup();
        using (new DebugLogTimer("Evaluate Frames"))
        {
            try
            {
                m_Culled = 0;
                m_RuleCount = 0;
                var initialState = new Replacement();
                initialState.id = grammar.startShape;
                frames.Enqueue(new EvaluationStackFrame(initialState));
                while (frames.Count > 0)
                {
                    var frame = frames.Dequeue();
                    EvaluateFrame(frame);
                }
            }
            catch (TooManyShapesException e)
            {
                Debug.Log(e);
            }
        }

        using (new DebugLogTimer("Shape Sorting"))
            shapes.Sort((s1, s2) => s2.area.CompareTo(s1.area));
        Debug.Log($"Shapes: {shapes.Count} Culled: {m_Culled}");
    }

    private void EvaluateFrame(EvaluationStackFrame frame)
    {
        var scale = frame.transform.lossyScale;
        var area = Math.Abs(scale.x * scale.y);
        if (area < areaCutoff)
        {
            m_Culled++;
            return;
        }

        if (frame.replacement.isLoop)
        {
            /*
            var loopReplacement = frame.replacement;
            var transform = frame.transform;
            var color = frame.color;
            for (var loop = 0; loop < loopReplacement.loop; ++loop)
            {
                for (var replacementIndex = loopReplacement.replacements.Length - 1; replacementIndex >=0; --loopReplacement)
                {
                    frames.Enqueue(new EvaluationStackFrame(replacement) { transform = transform, color = color });
                    transform = transform * loopReplacement.transform;
                    color = Utils.AdjustColor(color, loopReplacement.color);
                }
            }
            */
        }
        else
        {
            var transform = frame.transform * frame.replacement.transform;
            var frameTransform2D = Utils.ToTransform2D(frame.transform);
            var shapeTransform2D = Utils.ToTransform2D(frame.replacement.transform);
            var transform2D = Utils.ToTransform2D(transform);
            var color = Utils.AdjustColor(frame.color, frame.replacement.color);
            
            if (frame.replacement.isTerminal)
            {
                shapes.Add(new Shape()
                {
                    hsv = color,
                    color = color.ToRGBA(),
                    transform = transform,
                    shapeId = frame.replacement.id,
                    area = area
                });

                if (shapes.Count > maxNbShapes)
                    throw new TooManyShapesException($"Stopped rule generation. Too many shapes: {maxNbShapes}");
                return;
            }

            var newRule = grammar.GetRule(frame.replacement.id);
            foreach (var shapeDesc in newRule.replacements)
            {
                frames.Enqueue(new EvaluationStackFrame(shapeDesc) { transform = transform, color = color });
            }
        }
    }    
}