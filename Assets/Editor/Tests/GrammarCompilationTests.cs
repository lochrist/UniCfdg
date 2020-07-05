using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class GrammarCompilationTests
{
    static bool Close(float f1, float f2, float epsilon = 0.00001f)
    {
        return Math.Abs(f1 - f2) < epsilon;
    }

    static bool CompareTransform(Matrix4x4 m1, Matrix4x4 m2)
    {
        for (int i = 0; i < 16; i++)
        {
            if (!Close(m1[i], m2[i]))
                return false;
        }

        return true;
    }

    static bool CompareColor(HsvColor c1, HsvColor c2)
    {
        return Close(c1.h, c2.h) && Close(c1.s, c2.s) && Close(c1.v, c2.v) && Close(c1.a, c2.a);
    }

    static bool CompareColor(Color c1, Color c2)
    {
        return Close(c1.r, c2.r) && Close(c1.g, c2.g) && Close(c1.b, c2.b) && Close(c1.a, c2.a);
    }

    public void CompareGrammar(Grammar expected, string title, string grammarStr)
    {
        Assert.DoesNotThrow(() =>
        {
            CfdgCompiler.Compile(grammarStr);
        }, $"Cannot compile Grammar: {title}");

        var grammar = CfdgCompiler.Compile(grammarStr, title);
        Assert.AreEqual(expected.name, grammar.name);
        Assert.IsTrue(CompareColor(expected.backgroundColor, grammar.backgroundColor));
        Assert.AreEqual(expected.startShape, grammar.startShape);

        foreach (var expectedRule in expected.ruleGroups)
        {
            Assert.IsTrue(grammar.ruleGroups.ContainsKey(expectedRule.Key), $"Missing rule {expectedRule.Key}");
        }
        foreach (var rule in grammar.ruleGroups)
        {
            Assert.IsTrue(expected.ruleGroups.ContainsKey(rule.Key), $"Unexpected rule {rule.Key}");
        }

        foreach (var kvp in expected.ruleGroups)
        {
            var ruleName = kvp.Key;
            var expectedRules = expected.ruleGroups[ruleName];
            var rules = grammar.ruleGroups[ruleName];
            Assert.AreEqual(expectedRules.Count, rules.Count, "Not same number of rules");

            for (var i = 0; i < expectedRules.Count; i++)
            {
                var expectedRule = expectedRules[i];
                var rule = rules[i];
                Assert.AreEqual(expectedRule.name, rule.name);
                Assert.IsTrue(Close(expectedRule.probability, rule.probability), $"Different rule probability {ruleName}");
                Assert.IsTrue(Close(expectedRule.weight, rule.weight), $"Different rule weight {ruleName}");
                Assert.AreEqual(expectedRule.replacements.Length, rule.replacements.Length, $"Not same number of replacement {ruleName}");
                for (int replacementIndex = 0; replacementIndex < expectedRule.replacements.Length; replacementIndex++)
                {
                    var expectedReplacement = expectedRule.replacements[replacementIndex];
                    var replacement = rule.replacements[replacementIndex];
                    Assert.AreEqual(expectedReplacement.id, replacement.id, $"Not same replacement id in {ruleName} #{replacementIndex}");
                    Assert.AreEqual(expectedReplacement.isTerminal, replacement.isTerminal, $"Not same isTerminal in {ruleName} #{replacementIndex}");
                    Assert.IsTrue(CompareColor(expectedReplacement.color, replacement.color), $"Not same color in {ruleName} #{replacementIndex}");
                    Assert.IsTrue(CompareTransform(expectedReplacement.transform, replacement.transform), $"Not same transform in {ruleName} #{replacementIndex}");
                }
            }
        }
    }

    [Test]
    public void NumberTest()
    {
        CompareGrammar(SampleGrammars.NumberTest(), "NumberTest", SampleGrammars.NumberTestGrammar);
    }

    [Test]
    public void SimpleSquare()
    {
        CompareGrammar(SampleGrammars.SimpleSquare(), "SimpleSquare", SampleGrammars.SimpleSquareGrammar);
    }

    [Test]
    public void UnitShapes()
    {
        CompareGrammar(SampleGrammars.UnitShapes(), "UnitShapes", SampleGrammars.UnitShapesGrammar);
    }

    [Test]
    public void FourCircles()
    {
        CompareGrammar(SampleGrammars.FourCircles(), "FourCircles", SampleGrammars.FourCirclesGrammar);
    }

    [Test]
    public void SimpleBubble()
    {
        CompareGrammar(SampleGrammars.SimpleBubble(), "SimpleBubble", SampleGrammars.SimpleBubbleGrammar);
    }

    [Test]
    public void SpiralSquares()
    {
        CompareGrammar(SampleGrammars.SimpleSpiralSquares(), "SimpleSpiralSquares", SampleGrammars.SimpleSpiralSquaresGrammar);
    }

    [Test]
    public void LotsOfSquarePattern()
    {
        CompareGrammar(SampleGrammars.LotsOfSquarePattern(), "LotsOfSquarePattern", SampleGrammars.LotsOfSquarePatternGrammar);
    }

    [Test]
    public void ParseLoopGrammar()
    {
        CfdgCompiler.Compile(SampleGrammars.WithLoopGrammar);
    }
}
