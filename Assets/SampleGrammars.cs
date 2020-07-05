using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SampleGrammars
{
    public static string NumberTestGrammar = @"startshape init
rule init {
    square [a 0.1 h -100 sat .2 b -.5]
}
";

    public static Grammar NumberTest()
    {
        var grammar = new Grammar();
        grammar.name = "NumberTest";
        grammar.startShape = "init";

        {
            var initRule = new Rule();
            initRule.name = "init";

            var s1 = new Replacement();
            s1.id = "square";
            s1.color = new HsvColor() { a = 0.1f, h = -100, s = 0.2f, v = -0.5f };

            initRule.replacements = new[] { s1 };
            grammar.AddRule(initRule);
        }
        return grammar;
    }

    public static string WithLoopGrammar = @"startshape init
rule init {
   2 * {r 30} SQUARE{}
   3 * {r 30} { 
     TRIANGLE { hue 130 b 1 sat 1} 
     4 * { hue 20 b 1 sat 0.5 } { 
         TRIANGLE { r 20 s .1 } 
     }
     CIRCLE { s 0.3 x 0.3 } 
   }
}
";

    public static string PetalLoopGrammar = @"startshape flower
rule flower {
    // petals
    6 * [r 60] CIRCLE [ r 30 x 0.5 s 1 0.25 ]
    //center
    CIRCLE [ s 0.25 b 1 ]
}";

    public static string PetalLoopUnrolledGrammar = @"startshape flower
rule flower {
    // petals
    CIRCLE [ r 30 x 0.5 s 1 0.25 ]
    CIRCLE [ r 60 r 30 x 0.5 s 1 0.25 ]
    CIRCLE [ r 60 r 60 r 30 x 0.5 s 1 0.25 ]
    CIRCLE [ r 60 r 60 r 60 r 30 x 0.5 s 1 0.25 ]
    CIRCLE [ r 60 r 60 r 60 r 60 r 30 x 0.5 s 1 0.25 ]
    CIRCLE [ r 60 r 60 r 60 r 60 r 60 r 30 x 0.5 s 1 0.25 ]
    //center
    CIRCLE [ s 0.25 b 1 ]
}";

    public static string WithCommentsGrammar = @"startshape init
background { h 20 sat 0.7 b 0.9 }

// This is a single line comment

# this is another single line comment

rule init {
    square {h 200 sat 0.7 b 0.7 a 0.5 s 0.5}
}

/*
this is a multi line comment

*/
rule square {
    SQUARE [r 45 h 45]
}";

    public static string SimpleSquareGrammar = @"startshape init
background { h 20 sat 0.7 b 0.9 }
rule init {
    square [h 100 sat 0.5 b 0.5]
    square {h 200 sat 0.7 b 0.7 a 0.5 s 0.5}
}

rule square {
    SQUARE [r 45 h 45]
}";

    public static Grammar SimpleSquare()
    {
        var grammar = new Grammar();
        grammar.name = "SimpleSquare";
        grammar.backgroundColor = new HsvColor() { h = 20, s = 0.7f, v = 0.9f };

        {
            var initRule = new Rule();
            initRule.name = "init";

            var s1 = new Replacement();
            s1.id = "square";
            s1.color = new HsvColor() { h = 100, s = 0.5f, v = 0.5f };

            var s2 = new Replacement();
            s2.id = "square";
            s2.color = new HsvColor() { h = 200, s = 0.7f, v = 0.7f, a = 0.5f };
            s2.transform *= Matrix4x4.Scale(new Vector3(0.5f, 0.5f, 1));

            /*
            var s3 = new ShapeDesc();
            s3.id = "square";
            s3.color = new HsvColor() { a = 1, h = 150, s = 0.2f, v = 0.1f };
            s3.transform *= Matrix4x4.Scale(new Vector3(0.2f, 0.2f, 0.2f));
            */

            initRule.replacements = new[] { s1, s2 };

            grammar.startShape = "init";
            grammar.AddRule(initRule);
        }

        {
            var squareRule = new Rule();
            squareRule.name = "square";
            var s1 = new Replacement();
            s1.SetSquare();
            s1.transform = Matrix4x4.Rotate(Quaternion.Euler(new Vector3(0, 0, 45)));
            s1.color = new HsvColor() { h = 45 };
            squareRule.replacements = new[] { s1 };

            grammar.AddRule(squareRule);
        }

        return grammar;
    }

    public static string UnitShapesGrammar = @"startshape init

rule init {
    layer {x 0 y 0}
    layer { x 0 y 1 }
    layer { x 0 y  2 }
}

rule layer {
   SQUARE { x 0 y 0 hue 160 sat 1 b 1 }      
   CIRCLE { x 0 y 0.25 hue 100 sat 1 b 1 }
   TRIANGLE { x 0 y 0.5 hue 60 sat 1 b 1 }
}";

    public static Grammar UnitShapes()
    {
        var grammar = new Grammar();
        grammar.name = "UnitShapes";

        {
            var initRule = new Rule();
            initRule.name = "init";

            var s1 = new Replacement();
            s1.id = "layer";
            s1.transform *= Matrix4x4.Translate(new Vector3(0f, 0, 0));

            var s2 = new Replacement();
            s2.id = "layer";
            s2.transform *= Matrix4x4.Translate(new Vector3(0f, 1, 0));

            var s3 = new Replacement();
            s3.id = "layer";
            s3.transform *= Matrix4x4.Translate(new Vector3(0f, 2, 0));

            initRule.replacements = new[] { s1, s2, s3 };

            grammar.startShape = "init" ;
            grammar.AddRule(initRule);
        }

        {
            var initRule = new Rule();
            initRule.name = "layer";

            var s1 = new Replacement();
            s1.SetSquare();
            s1.transform *= Matrix4x4.Translate(new Vector3(0f, 0, 0));
            s1.color = new HsvColor() { h = 160, s = 1, v = 1 };

            var s2 = new Replacement();
            s2.SetCircle();
            s2.transform *= Matrix4x4.Translate(new Vector3(0f, 0.25f, 0));
            s2.color = new HsvColor() { h = 100, s = 1, v = 1 };

            var s3 = new Replacement();
            s3.SetTriangle();
            s3.transform *= Matrix4x4.Translate(new Vector3(0f, 0.5f, 0));
            s3.color = new HsvColor() { h = 60, s = 1, v = 1 };

            initRule.replacements = new[] { s1, s2, s3 };
            grammar.AddRule(initRule);
        }

        return grammar;
    }

    public static string FourCirclesGrammar = @"startshape Circles
rule Circles {
  FourCircles {}
}

rule FourCircles {
  CIRCLE {x 1.5 s 0.9 hue 60 sat 1 b 1 }
  CIRCLE {x -1.5 s 0.3 hue 60 sat 0.1 b 1 }
  CIRCLE {y 1.5 s 0.5 hue 60 sat -1 b 0.5}
  CIRCLE {y -1.5 s 0.7 hue 60 sat .1 b 1}
}";

    public static Grammar FourCircles()
    {
        var grammar = new Grammar();
        grammar.name = "FourCircles";

        {
            var initRule = new Rule();
            initRule.name = "Circles";

            var s1 = new Replacement();
            s1.id = "FourCircles";

            initRule.replacements = new[] { s1 };

            grammar.startShape = "Circles";
            grammar.AddRule(initRule);
        }

        {
            var squareRule = new Rule();
            squareRule.name = "FourCircles";

            var s1 = new Replacement();
            s1.SetCircle();
            s1.transform *= Matrix4x4.Translate(new Vector3(1.5f, 0, 0));
            s1.transform *= Matrix4x4.Scale(new Vector3(0.9f, 0.9f, 1f));
            s1.color = new HsvColor() { h = 60, s = 1, v = 1 };

            var s2 = new Replacement();
            s2.SetCircle();
            s2.transform *= Matrix4x4.Translate(new Vector3(-1.5f, 0, 0));
            s2.transform *= Matrix4x4.Scale(new Vector3(0.3f, 0.3f, 1f));
            s2.color = new HsvColor() { h = 60, s = 0.1f, v = 1 };

            var s3 = new Replacement();
            s3.SetCircle();
            s3.transform *= Matrix4x4.Translate(new Vector3(0, 1.5f, 0));
            s3.transform *= Matrix4x4.Scale(new Vector3(0.5f, 0.5f, 1f));
            s3.color = new HsvColor() { h = 60, s = -1, v = 0.5f };

            var s4 = new Replacement();
            s4.SetCircle();
            s4.transform *= Matrix4x4.Translate(new Vector3(0f, -1.5f, 0));
            s4.transform *= Matrix4x4.Scale(new Vector3(0.7f, 0.7f, 1f));
            s4.color = new HsvColor() { h = 60, s = 0.1f, v = 1 };

            squareRule.replacements = new[] { s1, s2, s3, s4 };

            grammar.AddRule(squareRule);
        }

        return grammar;
    }

    public static string SimpleBubbleGrammar = @"startshape BULB
rule BULB {
    WHEEL { }
    BULB { x 2 r 95 s .9 }
}

rule WHEEL {
    CIRCLE { }
    CIRCLE { s .9 b 1 }
}";

    public static Grammar SimpleBubble()
    {
        var grammar = new Grammar();
        grammar.name = "SimpleBubble";

        {
            var initRule = new Rule();
            initRule.name = "BULB";

            var s1 = new Replacement();
            s1.id = "WHEEL";

            var s2 = new Replacement();
            s2.id = "BULB";
            s2.transform *= Matrix4x4.Translate(new Vector3(2f, 0, 0));
            s2.transform *= Matrix4x4.Rotate(Quaternion.Euler(new Vector3(0, 0, 95)));
            s2.transform *= Matrix4x4.Scale(new Vector3(0.9f, 0.9f, 1f));

            initRule.replacements = new[] { s1, s2 };

            grammar.startShape = "BULB";
            grammar.AddRule(initRule);
        }

        {
            var squareRule = new Rule();
            squareRule.name = "WHEEL";
            var s1 = new Replacement();
            s1.SetCircle();
            
            var s2 = new Replacement();
            s2.SetCircle();
            s2.color = new HsvColor() { v = 1 };
            s2.transform *= Matrix4x4.Scale(new Vector3(0.9f, 0.9f, 1f));

            squareRule.replacements = new[] { s1, s2 };

            grammar.AddRule(squareRule);
        }

        return grammar;
    }

    public static string SimpleSpiralSquaresGrammar = @"startshape START
rule START {
   SPIRAL {}
   SPIRAL { r 120 }
   SPIRAL { r 240 }
}

rule SPIRAL {
   F_SQUARES { }
   F_TRIANGLES { x 0.5 y 0.5 r 45 }
}

rule F_SQUARES {
  SQUARE {  hue 220 sat 0.9 b 0.33  }
  SQUARE { s 0.9  sat 0.75 b 1 }
}

rule F_TRIANGLES {
  SQUARE { s 1.9 0.4 sat 0.7 b 1 }
} ";

    public static Grammar SimpleSpiralSquares()
    {
        var grammar = new Grammar();
        grammar.name = "SimpleSpiralSquares";

        {
            var initRule = new Rule();
            initRule.name = "START";

            var s1 = new Replacement();
            s1.id = "SPIRAL";

            var s2 = new Replacement();
            s2.id = "SPIRAL";
            s2.transform *= Matrix4x4.Rotate(Quaternion.Euler(new Vector3(0, 0, 120)));

            var s3 = new Replacement();
            s3.id = "SPIRAL";
            s3.transform *= Matrix4x4.Rotate(Quaternion.Euler(new Vector3(0, 0, 240)));

            initRule.replacements = new[] { s1, s2, s3 };

            grammar.startShape = "START";
            grammar.AddRule(initRule);
        }

        {
            var squareRule = new Rule();
            squareRule.name = "SPIRAL";
            var s1 = new Replacement();
            s1.id = "F_SQUARES";

            var s2 = new Replacement();
            s2.id = "F_TRIANGLES";
            s2.transform *= Matrix4x4.Translate(new Vector3(0.5f, 0.5f, 0));
            s2.transform *= Matrix4x4.Rotate(Quaternion.Euler(new Vector3(0, 0, 45)));

            squareRule.replacements = new[] { s1, s2 };

            grammar.AddRule(squareRule);
        }

        {
            var squareRule = new Rule();
            squareRule.name = "F_SQUARES";
            var s1 = new Replacement();
            s1.SetSquare();
            s1.color = new HsvColor() { h = 220, s = 0.9f, v = 0.33f };

            var s2 = new Replacement();
            s2.SetSquare();
            s2.transform *= Matrix4x4.Scale(new Vector3(0.9f, 0.9f, 1f));
            s2.color = new HsvColor() { s = 0.75f, v = 1 };

            squareRule.replacements = new[] { s1, s2 };

            grammar.AddRule(squareRule);
        }

        {
            var squareRule = new Rule();
            squareRule.name = "F_TRIANGLES";
            var s1 = new Replacement();
            s1.SetSquare();
            s1.transform *= Matrix4x4.Scale(new Vector3(1.9f, 0.4f, 1f));
            s1.color = new HsvColor() { s = 0.7f, v = 1f };

            squareRule.replacements = new[] { s1 };

            grammar.AddRule(squareRule);
        }

        return grammar;
    }

    public static string LotsOfSquarePatternGrammar = @"startshape scene

rule scene {
    SQUARE { x 0.5 y 0.5 s 2 hue 240 sat 1 b 0.3}
    rectangle { x 0 y 0 sat 1 b 0 hue 0 s 0.71 1}
    rectangle { x 1 y 0 sat 0 b 1 hue 0 s 0.71 1}
    rectangle { x 0 y 1 sat 1 b 1 hue 0 s 0.71 1}
    rectangle { x 1 y 1 sat 0.5 b 0.5 hue 0 s 0.71 1}
}

rule rectangle {
    SQUARE {  }
    rectangle [ r 90 s 0.71 y 0.5 alpha -0.4 b -0.1 sat -0.2 hue -4]
    rectangle [ r -90 s 0.71 y 0.5 alpha 0.02 b 0.2 sat 0.3 hue 4]
}";

    /*
     rule rectangle {
    SQUARE {  }
    rectangle [ r -90 s 0.71 y 0.5 alpha -0.4 b -0.1 sat -0.2 hue -4]
    rectangle [ r 90 s 0.71 y 0.5 alpha 0.02 b 0.2 sat 0.3 hue 4]
}
    */
    public static Grammar LotsOfSquarePattern()
    {
        var grammar = new Grammar();
        grammar.name = "LotsOfSquarePattern";

        {
            var initRule = new Rule();
            initRule.name = "scene";

            var s1 = new Replacement();
            s1.SetSquare();
            s1.transform *= Matrix4x4.Translate(new Vector3(0.5f, 0.5f, 0));
            s1.transform *= Matrix4x4.Scale(new Vector3(2.0f, 2f, 1));
            s1.color = new HsvColor() { h = 240, s = 1f, v = 0.3f };

            var s2 = new Replacement();
            s2.id = "rectangle";
            s2.transform *= Matrix4x4.Translate(new Vector3(0f, 0f, 0));
            s2.transform *= Matrix4x4.Scale(new Vector3(0.71f, 1f, 1));
            s2.color = new HsvColor() { h = 0, s = 1f, v = 0f };

            var s3 = new Replacement();
            s3.id = "rectangle";
            s3.transform *= Matrix4x4.Translate(new Vector3(1f, 0f, 0));
            s3.transform *= Matrix4x4.Scale(new Vector3(0.71f, 1f, 1));
            s3.color = new HsvColor() { h = 0, s = 0f, v = 1f };
            
            var s4 = new Replacement();
            s4.id = "rectangle";
            s4.transform *= Matrix4x4.Translate(new Vector3(0f, 1f, 0));
            s4.transform *= Matrix4x4.Scale(new Vector3(0.71f, 1f, 1));
            s4.color = new HsvColor() { h = 0, s = 1f, v = 1f };
            
            var s5 = new Replacement();
            s5.id = "rectangle";
            s5.transform *= Matrix4x4.Translate(new Vector3(1f, 1f, 0));
            s5.transform *= Matrix4x4.Scale(new Vector3(0.71f, 1f, 1));
            s5.color = new HsvColor() { h = 0, s = 0.5f, v = 0.5f };

            initRule.replacements = new[] { s1, s2, s3, s4, s5 };

            grammar.startShape = "scene";
            grammar.AddRule(initRule);
        }

        {
            var squareRule = new Rule();
            squareRule.name = "rectangle";
            var s1 = new Replacement();
            s1.SetSquare();
            
            var s2 = new Replacement();
            s2.id = "rectangle";
            s2.transform *= Matrix4x4.Rotate(Quaternion.Euler(new Vector3(0, 0, 90)));
            s2.transform *= Matrix4x4.Scale(new Vector3(0.71f, 0.71f, 1));
            s2.transform *= Matrix4x4.Translate(new Vector3(0f, 0.5f, 0));
            s2.color = new HsvColor() { a = -0.4f, h = -4, s = -0.2f, v = -0.1f };

            var s3 = new Replacement();
            s3.id = "rectangle";
            s3.transform *= Matrix4x4.Rotate(Quaternion.Euler(new Vector3(0, 0, -90)));
            s3.transform *= Matrix4x4.Scale(new Vector3(0.71f, 0.71f, 1));
            s3.transform *= Matrix4x4.Translate(new Vector3(0f, 0.5f, 0));
            s3.color = new HsvColor() { a = 0.02f, h = 4, s = 0.3f, v = 0.2f };

            squareRule.replacements = new[] { s1, s2, s3 };
            
            grammar.AddRule(squareRule);
        }

        return grammar;
    }

    public static string SnowFlakesGrammar = @"startshape init
rule init{turn[b 1 a -1 sat 1 h 310]}
rule turn{
    SQUARE[x .5 y -.05 s 1 .1]
    CIRCLE[x 1.77 y .13 s .5]
    turn[z 1 r -2.07028 s 0.298966      a .1 h 90 sat -.3]
    turn[z 1 x 1 r 45.9297 s .324258    a .1 sat .1]
    turn[z 1 x 1.72574 y .806014 r -48 s .922]
}";

    public static string SpiralSquaresGrammar = @"startshape START
background { b -1 }

rule START {
   SPIRAL{}
   SPIRAL { r 120 }
   SPIRAL { r 240 }
}

rule SPIRAL {
   F_SQUARES { }
   F_TRIANGLES { x 0.5 y 0.5 r 45 }
   SPIRAL { y 1 r 35 s 0.97 }
}

rule F_SQUARES {
  SQUARE {  hue 220 sat 0.9 b 0.33  }
   SQUARE { s 0.9  sat 0.75 b 1 }
   F_SQUARES { s 0.7 r 15 hue 55}
}

rule F_TRIANGLES {
  TRIANGLE { s 1.9 0.4 sat 0.7 b 1 }
  F_TRIANGLES { s 0.8 r 5 hue 25}
}";

    public static string ExplositionGrammar = @"startshape START

rule START {
    SCENE { b 0.01 hue 0  sat 0.8 }
}

rule SCENE {
    CURVE {  }	
    START { s 0.995 r 20 b 0.01 hue 0.1  sat 0.8}
}

rule CURVE {
    SQUARE { }
    CURVE { y 1 s 0.997 r 5  }
}

rule CURVE 0.007 {
    CIRCLE { s 3.5  b 0.5}
    CURVE { y 1 s 0.99 r 10 }
}

rule CURVE 0.01 {
    FLOWER { }
    SQUARE { }
    CURVE { y 1 s 0.99 r -40 skew 10 0 }
}

rule FLOWER {
    TRIANGLE { s 15 1 r 45 }
    FLOWER { s 0.9 r 45 }
}";

    public static string HairyBallGrammar = @"startshape sticks
rule sticks {
    line { r 15 }
    line { r -15 }
    line { r 45 }
    line { r -45 }    
    sticks { s 0.9999 x 0.2 y 0.12 r 1.333 }
}

rule line {
    SQUARE { s 0.06 5 hue 40 sat 0.6 b 0.5  }
} 

rule line {
    SQUARE { s 0.02 3 hue 40 sat 0.6 b 0.333 }
}

rule line {
    SQUARE { s 0.05 6 hue 40 sat 0.6 b 0.75  }
}

rule line {
    SQUARE { s 0.02 4 hue 40 sat 0.6 b 0.2 }
}
";

    public static string WormGrammar = @"startshape fear_worm
rule fear_worm { fear_worm1 { } }
rule fear_worm { fear_worm2 { } }
rule fear_worm 0.1 {
    antenna { s 0.01 y 0.02 }
    antenna { s 0.01 r 20 y 0.02 }
}

rule fear_worm1 {
    fear_circle { s 0.1 b 1}
    fear_circle { s 0.1 b 1 r 10}
    fear_worm1 { x 0.03 r 4.42 s 1.005}
}

rule fear_worm2 {
    fear_circle { s 0.1 b 1}
    fear_circle { s 0.1 b 1 r 10}
    fear_worm2 { x 0.02 r -4.42 s 1.005}
}

rule fear_worm1 0.3 {
    fear_worm { }
}

rule fear_worm2 0.3 {
    fear_worm { }
}

rule fear_circle {
    CIRCLE { }
    fear_circle { x -0.005 y -0.005 s 0.98 0.998 b -0.001 }
}
rule fear_circle {
    CIRCLE { }
    fear_circle { x -0.005 y -0.000 s 0.97 0.998 b -0.05 }
}

rule antenna {
    CIRCLE { }
    antenna { x 1 r 10 s 0.8}
}";

    public static string BeatingHeartGrammar = @"startshape START
rule START {
    BLOOD{x -20 r 90}
    BLOOD{x 20 flip 0 r 90}
    CIRCLE{size 50}
    CIRCLE{sat 1 size 48 b 1}
}

rule BLOOD 2000 {
    VESSEL{}
    BLOOD{y 0.9 s 0.997 r 3 hue -0.2}
}

rule BLOOD 75 {
    BLOOD{s 0.997 hue -0.2 flip 90}
}

rule BLOOD 5 {
    BRANCH{}
    BLOOD{y 0.9 s 0.997 r  3 hue -0.2}
    BLOOD{y 0.9 s 0.997 r -3 hue -0.2}
}

rule BRANCH {
    VESSEL{}
    BLOOD{y 1 s 1}
    BLOOD{y 1 s 1}
}

rule VESSEL {
    SQUARE{x -3}
    SQUARE{x  3}

    SQUARE{x -2 hue 1 sat 1 b 1}
    SQUARE{x -1 hue 1 sat 1 b 1}
    SQUARE{hue 1 sat 1 b 1}
    SQUARE{x 1 hue 1 sat 1 b 1}
    SQUARE{x 2 hue 1 sat 1 b 1}
}";

    public static string CubeCastleGrammar = @"startshape ZCUBES
background {b -.5}

rule ZCUBES {
    2*{s -1 1} ZCUBE {}
}

rule ZCUBE {
    XCUBE {}
    XCUBE {x -1 y .58 s .98 z -1}
    DCUBE {y -1 s .99 z -1}
    ZCUBE {x 1 y .58 s .98 z -1}
}

rule XCUBE {
    CUBE {}
    XCUBE {x -1 y .58 s .99 z -1}
}

rule XCUBE {
    CUBE {}
    XCUBE {x 1 y .58 s .99 z -1}
}

rule XCUBE {
    CUBE {}
    XCUBE {y 1 s 1.010101 z 1}
}

rule XCUBE {
    CUBE {}
    XCUBE {y -1 s .99 z -1}
}

rule DCUBE {
    CUBE {}
    DCUBE {y -1 s .99 z -1}
}

rule DCUBE .09{
    CUBE {}
}

rule CUBE{ SIDE{s -1 1}SIDE{s 1}TOP{}}

rule SIDE {FACE{skew 0 30}}

rule TOP {FACE[s 1.413 .816 r 135 b .8]}

rule FACE {SQUARE{x .5 y -.5}}";

    public static string ColorSplosionGrammar = @"
startshape START
background { b -0.8 }

rule START {
    NODE { hue 60 sat 0.9 b 0.3 r 17}
    NODE { hue 20 sat 0.8 b 0.3 r 89 }
    NODE { hue 50 sat 0.85 b 0.3 r 127}
    NODE { hue 30 sat 0.7 b 0.3 r 74}
}

rule NODE {
    STAR { skew 0 20 }
    NODE { s 0.97 y 1.2 r 15 hue -20.5 alpha -0.02 skew 5 0 }
}

rule NODE 0.2 { 
    NODE { flip 197 skew 3 5}
}

rule NODE 0.2 { 
    NODE { flip 17 skew 2 9}
}

rule STAR {
    SHAPE { }
    SHAPE { flip 180 alpha -0.3}
    SHAPE { r 90 alpha -0.4}
    SHAPE { r -90 alpha -0.2}
    STAR { s 0.94 b 0.7  r 47 alpha -0.07}
}

rule SHAPE {
    CIRCLE { x 1 }
    TRIANGLE{ s 3.5 0.09 x 3 r 17}
    CIRCLE { s 0.5 x 4 y 4 }
    CIRCLE { s 0.4 x 4 y 4.1 alpha 0 b 0.8 hue 0}
}

rule SHAPE 0.2 {
    SHAPE { skew 10 5 }
}

rule SHAPE 0.2 {
    SHAPE { skew 3 10}
}";

    public static string SpiralAllShapesGrammar = @"startshape BULB
rule BULB {
    WHEEL { }
    BULB { x 2 r 95 s .9 }
}

rule WHEEL {
    SQUARE { }   
    CIRCLE { s .9 b 1 }
    TRIANGLE { s .8 b 0.5 }
}";

    public static string PetalsGrammar = @"
startshape bouquet

rule bouquet {
    flower6 { x 2 r 15 }
}

rule flower6 {
    demiflower6 { }
    demiflower6 { flip 90 }
}

rule flower5 {
    demiflower5 { }
    demiflower5 { flip 90 }
}

rule demiflower6 {
    petal6 { }
    petal6 { r 30 }
    petal6 { r 60 }
    petal6 { r 90 }
    petal6 { r 120 }
    petal6 { r 150 }
    petal6 { r 180 }
    petal6 { r 210 }
    petal6 { r 240 }
    petal6 { r 270 }
    petal6 { r 300 }
    petal6 { r 330 }
}
rule demiflower5 {
    petal5 { }
    petal5 { r 72 }
    petal5 { r 144 }
    petal5 { r 216 }
    petal5 { r 288 }
}

rule petal5 {
    SQUARE { s 1 0.02 }
    CIRCLE { x -0.5 s 0.02 }
    petal5 [ x 0.5 r 144.1 s 0.998 x 0.5 b 0.0015 ]
}

rule petal6 {
    SQUARE { s 1 0.02 }
    CIRCLE { x -0.5 s 0.02 }
    petal6 [ x 0.5 r 120.205 s 0.996 x 0.5 b 0.002 ]
}";

    public static string TreeWithFruitsGrammar = @"startshape A

rule A
{
  SQUARE {h 0 sat .89 b .25 }
  A {y .5 r 2 s .9999 b .0002}
}
rule A
{
  SQUARE {h 0 sat .89 b .25 }
  A {y .5 r -2 s .9999 b .0002}
}
rule A .05
{
  SQUARE {h 0 sat .89 b .25 }
  A {y .5 r 15 s .8 sat .9 b .0002  a -.15}  
  A {y .5 r -15 s .8 sat .9 b .0002 a -.15}  
}
rule A .007
{
  SQUARE {h 0 sat .89 b .25 }
  B {y .5 r 15 s .9}  
  B {y .5 r -15 s .9}  
}

rule B
{
  SQUARE {h 0 sat .89 b .25 }
  B {y .5 r 2 s .99 b .0002}
}
rule B
{
  SQUARE {h 0 sat .89 b .25 }
  B {y .5 r -2 s .99 b .0002}
}

rule B .1
{
  SQUARE {h 0 sat .89 b .25 }
  ponItem {}
  B {y .5 r 2 s .99 b .0002}
}
rule B .1
{
  SQUARE {h 0 sat .89 b .25 }
  ponItem {}
  B {y .5 r -2 s .99 b .0002}
}

rule ponItem { ponHoja{s 1.5} }
rule ponItem .25 { fruto{s 5} }

rule ponHoja { hoja {x 1.75 s 5 r 15} }
rule ponHoja { hoja {x -1.75 s 5 r -15} }

rule fruto 
{
  CIRCLE {h 0 sat .7 b .8}
  CIRCLE {x .25 y -.25 s .9 h 0 sat .7 b .5}
  CIRCLE {y -.5 s .8 h 0 sat .7 b .4}
  CIRCLE {x .2 y -.75 s .7 h 0 sat .7 b .3}
}

rule hoja 
{
  CIRCLE {s 1 .7 h 133 sat .79 b .41 }
  CIRCLE {s 1 .1 h 126 sat .78 b .75 }
}";

    public static string SimpleTreeGrammar = @"startshape TREE
rule TREE 20 {
    CIRCLE [ size 0.25 ]
    TREE [ y 0.1 size 0.97 ]
}

rule TREE 1.5 {
    BRANCH [  ]
}

rule BRANCH
{
    BRANCH_LEFT [ ]
    BRANCH_RIGHT [ ]
}

rule BRANCH_LEFT {
    TREE [ rotate 20 ]
}
rule BRANCH_LEFT {
    TREE [ rotate 30 ]
}
rule BRANCH_LEFT {
    TREE [ rotate 40 ]
}
rule BRANCH_LEFT {

}

rule BRANCH_RIGHT {
    TREE [ rotate -20 ]
}
rule BRANCH_RIGHT {
    TREE [ rotate -30 ]
}
rule BRANCH_RIGHT {
    TREE [ rotate -40 ]
}
rule BRANCH_RIGHT {
}
";

    public static string GlassCubesGrammar = @"startshape glasscubes
rule glasscubes {
 cuberow {b 1 sat 0.8 hue 40 alpha -0.2}
 cuberow {y 1.1 b 1 sat 0.5 hue 80 alpha -0.4}
 cuberow {y 2.2 b 0.9 sat 0.1 hue 120 alpha -0.6}
}

rule cuberow {
 cube {sat 0}
 cube {x 1.1 sat 0.5 hue 90}
 cube {x 2.2 sat 0.8 hue 200}
}


rule cube {
 SQUARE {x 0.281 y 0.281 hue 240}
 SQUARE [r 45 skew 45 0 x -0.5125 y 0.355 s 0.4 0.707 b -0.5 hue 120 ]
 SQUARE [r -45 skew 0 45 x 0.355 y -0.5125 s 0.707 0.4 b -0.5 hue 40 ]
 SQUARE [r 45 skew 45 0 x 0.9095 y -0.355 s 0.4 0.707 hue 80 ]
 SQUARE [r -45 skew 0 45 x -0.355 y 0.9095 s 0.707 0.4 hue 160 ]
 SQUARE { }
}";

    public static string ForestGrammar = @"startshape FOREST
rule FOREST
{
     SEED []
     SEED [x -20]
     SEED [x -40]
}

rule SEED {BRANCH []}
rule SEED {BRANCH [rotate 1]}
rule SEED {BRANCH [rotate -1]}
rule SEED {BRANCH [rotate 2]}
rule SEED {BRANCH [rotate -2]}
rule SEED {FORK []}

rule BRANCH {LEFTBRANCH [flip 90]}
rule BRANCH {LEFTBRANCH []}

rule LEFTBRANCH 4 {BLOCK [] LEFTBRANCH [y 0.885 rotate 0.1 size 0.99]}
rule LEFTBRANCH 4 {BLOCK [] LEFTBRANCH [y 0.885 rotate 0.2 size 0.99]}
rule LEFTBRANCH {BLOCK [] LEFTBRANCH [y 0.885 rotate 4 size 0.99]}
rule LEFTBRANCH {BLOCK [] FORK []}

rule BLOCK
{
     SQUARE [rotate 1]
     SQUARE [rotate -1]
     SQUARE []
}

rule FORK {
     BRANCH [ ]
     BRANCH [size 0.5 rotate 40]
}
rule FORK {
     BRANCH [ ]
     BRANCH [size 0.5 rotate -40]
}
rule FORK {
     BRANCH [size 0.5 rotate -20]
     BRANCH [ ]
}
rule FORK {
     BRANCH [size 0.7 y 0.1 rotate 20]
     BRANCH [size 0.7 y 0.1 rotate -20]
}
";
    public static string AlphaSquareTestGrammar = @"startshape glasscubes
rule glasscubes {
 cuberow {b 1 sat 0.8 hue 40 alpha -0.2}
}

rule cuberow {
 SQUARE {x 0.281 y 0.281 hue 240}
 SQUARE { }
}";
}
