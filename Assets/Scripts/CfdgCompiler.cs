using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using UnityEngine;
using UnityEditor;
using UnityEngine.Assertions;

public class CfdgCompiler : CfdgBaseVisitor<CfdgCompiler>
{
    public Stack<Replacement> currentReplacement;
    public Stack<List<Replacement>> currentReplacements;
    public Rule currentRule;
    public Grammar currentGrammar;

    public CfdgCompiler()
    {
        currentGrammar = new Grammar();
        currentReplacement = new Stack<Replacement>();
        currentReplacements = new Stack<List<Replacement>>();
    }

    protected override CfdgCompiler DefaultResult => this;

    public override CfdgCompiler VisitContextfree([NotNull] CfdgParser.ContextfreeContext context)
    {
        var result = VisitChildren(context);
        return result;
    }

    public override CfdgCompiler VisitStatements([NotNull] CfdgParser.StatementsContext context)
    {
        var result = VisitChildren(context);
        return result;
    }

    public override CfdgCompiler VisitStatement([NotNull] CfdgParser.StatementContext context)
    {
        var result = VisitChildren(context);
        return result;
    }

    public override CfdgCompiler VisitStartshape([NotNull] CfdgParser.StartshapeContext context)
    {
        currentGrammar.startShape = context.NAME().GetText();
        var result = VisitChildren(context);
        return result;
    }

    public override CfdgCompiler VisitBackground([NotNull] CfdgParser.BackgroundContext context)
    {
        var result = VisitChildren(context);
        return result;
    }

    public override CfdgCompiler VisitDesign_rule([NotNull] CfdgParser.Design_ruleContext context)
    {
        currentRule = new Rule();
        currentReplacements.Push(new List<Replacement>());
        currentRule.name = context.NAME().GetText();
        if (context.num() != null)
        {
            currentRule.weight = Convert.ToSingle(context.num().GetText());
        }

        var result = VisitChildren(context);

        currentRule.replacements = currentReplacements.Pop().ToArray();
        currentGrammar.AddRule(currentRule);
        currentRule = null;

        return result;
    }

    public override CfdgCompiler VisitReplacements([NotNull] CfdgParser.ReplacementsContext context)
    {
        var result = VisitChildren(context);
        return result;
    }
    
    public override CfdgCompiler VisitReplacement_loop([NotNull] CfdgParser.Replacement_loopContext context)
    {
        var hasLoop = context.NUMBER() != null;
        if (hasLoop)
        {
            // This is the replacement for our modification.
            currentReplacement.Push(new Replacement());

            currentReplacement.Peek().loop = Convert.ToInt32(context.NUMBER().GetText());
            currentReplacements.Push(new List<Replacement>());
        }

        var result = VisitChildren(context);

        if (hasLoop)
        {
            currentReplacement.Peek().replacements = currentReplacements.Pop().ToArray();
            currentReplacements.Peek().Add(currentReplacement.Pop());
        }
        
        return result;
    }

    public override CfdgCompiler VisitReplacement([NotNull] CfdgParser.ReplacementContext context)
    {
        currentReplacement.Push(new Replacement());

        var id = context.NAME().GetText();
        currentReplacement.Peek().id = id;
        currentReplacement.Peek().isTerminal = id == "SQUARE" || id == "CIRCLE" || id == "TRIANGLE";

        var result = VisitChildren(context);

        currentReplacements.Peek().Add(currentReplacement.Pop());

        return result;
    }

    public override CfdgCompiler VisitModification([NotNull] CfdgParser.ModificationContext context)
    {
        var result = VisitChildren(context);
        return result;
    }

    public override CfdgCompiler VisitAdjustments([NotNull] CfdgParser.AdjustmentsContext context)
    {
        var result = VisitChildren(context);
        return result;
    }

    public override CfdgCompiler VisitAdjustment([NotNull] CfdgParser.AdjustmentContext context)
    {
        var result = VisitChildren(context);
        return result;
    }

    public override CfdgCompiler VisitColor_adjustments([NotNull] CfdgParser.Color_adjustmentsContext context)
    {
        var result = VisitChildren(context);
        return result;
    }

    public override CfdgCompiler VisitGeom_adjustment([NotNull] CfdgParser.Geom_adjustmentContext context)
    {
        var modification = currentReplacement.Peek();
        if (context.XSHIFT() != null)
        {
            var value = Convert.ToSingle(context.num()[0].GetText());
            modification.transform *= Matrix4x4.Translate(new Vector3(value, 0, 0));
        }
        else if (context.YSHIFT() != null)
        {
            var value = Convert.ToSingle(context.num()[0].GetText());
            modification.transform *= Matrix4x4.Translate(new Vector3(0, value, 0));
        }
        else if (context.ZSHIFT() != null)
        {
            var value = Convert.ToSingle(context.num()[0].GetText());
            modification.transform *= Matrix4x4.Translate(new Vector3(0, 0, value));
        }
        else if (context.ROTATE() != null)
        {
            var value = Convert.ToSingle(context.num()[0].GetText());
            modification.transform *= Matrix4x4.Rotate(Quaternion.Euler(new Vector3(0, 0, value)));
        }
        else if (context.SIZE() != null)
        {
            var num = context.num();
            if (num.Length == 1)
            {
                var value = Convert.ToSingle(context.num()[0].GetText());
                modification.transform *= Matrix4x4.Scale(new Vector3(value, value, 1));
            }
            else if (num.Length == 2)
            {
                var value = Convert.ToSingle(context.num()[0].GetText());
                var value2 = Convert.ToSingle(context.num()[1].GetText());
                modification.transform *= Matrix4x4.Scale(new Vector3(value, value2, 1));
            }
            else if (num.Length == 3)
            {
                var value = Convert.ToSingle(context.num()[0].GetText());
                var value2 = Convert.ToSingle(context.num()[1].GetText());
                var value3 = Convert.ToSingle(context.num()[2].GetText());
                modification.transform *= Matrix4x4.Scale(new Vector3(value, value2, value3));
            }
        }
        else if (context.SKEW() != null)
        {
            var value = Convert.ToSingle(context.num()[0].GetText());
            var value2 = Convert.ToSingle(context.num()[1].GetText());
            var x = Mathf.Tan(Mathf.PI * value / 180);
            var y = Mathf.Tan(Mathf.PI * value2 / 180);
            var transform = Utils.ToTransform2D(modification.transform);
            var shear = new [] { 1, y, x, 1, 0, 0 };
            transform = Utils.AdjustTransform2D(transform, shear);
            Utils.SetTransform2D(ref modification.transform, transform);
        }
        else if (context.FLIP() != null)
        {
            var value = Convert.ToSingle(context.num()[0].GetText());
            var c = Mathf.Cos(Mathf.PI * value / 90);
            var s = Mathf.Sin(Mathf.PI * value / 90);
            var transform = Utils.ToTransform2D(modification.transform);
            var flip = new[] { c, s, s, -c, 0, 0 };
            transform = Utils.AdjustTransform2D(transform, flip);
            Utils.SetTransform2D(ref modification.transform, transform);
        }

        var result = VisitChildren(context);
        return result;
    }

    public override CfdgCompiler VisitColor_adjustment([NotNull] CfdgParser.Color_adjustmentContext context)
    {
        var baseColor = currentReplacement.Count > 0 ? currentReplacement.Peek().color : currentGrammar.backgroundColor;
        if (context.HUE() != null)
        {
            baseColor.h = Convert.ToSingle(context.num().GetText());
        }
        else if (context.SATURATION() != null)
        {
            baseColor.s = Convert.ToSingle(context.num().GetText());
        }
        else if (context.BRIGHTNESS() != null)
        {
            baseColor.v = Convert.ToSingle(context.num().GetText());
        }
        else if (context.ALPHA() != null)
        {
            baseColor.a = Convert.ToSingle(context.num().GetText());
        }

        if (currentReplacement.Count > 0)
        {
            currentReplacement.Peek().color = baseColor;
        }
        else
        {
            currentGrammar.backgroundColor = baseColor;
        }

        var result = VisitChildren(context);
        return result;
    }

    public static Grammar Compile(string grammar, string grammarName = null)
    {
        using (new DebugLogTimer($"Compiling Grammar"))
        {
            var antlerStream = new AntlrInputStream(grammar);
            var lexer = new CfdgLexer(antlerStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new CfdgParser(tokenStream);

            var compiler = new CfdgCompiler();
            compiler.Visit(parser.contextfree());
            compiler.currentGrammar.name = grammarName;

            Assert.IsTrue(compiler.currentReplacement.Count == 0);
            Assert.IsTrue(compiler.currentReplacements.Count == 0);

            return compiler.currentGrammar;
        }
    }

}
