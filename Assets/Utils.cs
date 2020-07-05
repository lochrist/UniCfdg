using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

internal class DebugLogTimer : IDisposable
{
    private System.Diagnostics.Stopwatch m_Timer;
    public string msg { get; set; }

    public DebugLogTimer(string m)
    {
        msg = m;
        m_Timer = System.Diagnostics.Stopwatch.StartNew();
    }

    public static DebugLogTimer Start(string m)
    {
        return new DebugLogTimer(m);
    }

    public void Dispose()
    {
        m_Timer.Stop();
        Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, msg + " - " + m_Timer.ElapsedMilliseconds + "ms");
    }
}

public static class Utils
{
    [MenuItem("Tools/Test Matrix")]
    private static void TestMatrix()
    {
        var twoD = Identity();
        twoD = AddTranslation(twoD, 1, 2);
        twoD = AddRotation(twoD, 37);
        twoD = AddScale(twoD, 3, 4);

        var mat = Matrix4x4.identity;
        mat *= Matrix4x4.Translate(new Vector3(1, 2, 0));
        mat *= Matrix4x4.Rotate(Quaternion.Euler(0, 0, 37));
        mat *= Matrix4x4.Scale(new Vector3(3, 4, 1));
    }

    public static float[] ToTransform2D(Matrix4x4 mat)
    {
        var t = new float[6];
        t[0] = mat.m00;
        t[1] = mat.m10;
        t[2] = mat.m01;
        t[3] = mat.m11;
        t[4] = mat.m03;
        t[5] = mat.m13;
        return t;
    }

    public static void SetTransform2D(ref Matrix4x4 mat, float[] t)
    {
        mat.m00 = t[0];
        mat.m10 = t[1];
        mat.m01 = t[2];
        mat.m11 = t[3];
        mat.m03 = t[4];
        mat.m13 = t[5];
    }

    public static float[] Identity()
    {
        return new float[] { 1, 0, 0, 1, 0, 0 };
    }

    public static float[] AddTranslation(float[] transform, float x, float y)
    {
        return AdjustTransform2D(transform, new float[]
        {
            1, 0, 0, 1, x, y
        });
    }

    public static float[] AddRotation(float[] transform, float degrees)
    {
        var c = Mathf.Cos(Mathf.PI * degrees / 180);
        var s = Mathf.Sin(Mathf.PI * degrees / 180);
        return AdjustTransform2D(transform, new float[]
        {
            c, s, -s, c, 0, 0
        });
    }

    public static float[] AddScale(float[] transform, float x, float y)
    {
        return AdjustTransform2D(transform, new float[]
        {
            x, 0, 0, y, 0, 0
        });
    }

    public static float[] AdjustTransform2D(float[] transform, float [] adjustment)
    {
        return new float[]
        {
            transform[0] * adjustment[0] + transform[2] * adjustment[1],
            transform[1] * adjustment[0] + transform[3] * adjustment[1],
            transform[0] * adjustment[2] + transform[2] * adjustment[3],
            transform[1] * adjustment[2] + transform[3] * adjustment[3],
            transform[0] * adjustment[4] + transform[2] * adjustment[5] + transform[4],
            transform[1] * adjustment[4] + transform[3] * adjustment[5] + transform[5]
        };
    }

    public static HsvColor AdjustColor(HsvColor color, HsvColor adjustment)
    {
        var h = (color.h + adjustment.h) % 360;
        if (h < 0)
            h += 360;
        var s = AdjustColorComponent(color.s, adjustment.s);
        var v = AdjustColorComponent(color.v, adjustment.v);
        var a = AdjustColorComponent(color.a, adjustment.a);
        return new HsvColor() { h = h, s = s, v = v, a = a };
    }

    public static float AdjustColorComponent(float value, float adj)
    {
        return adj > 0 ? value + ((1 - value) * adj) : value + (adj * value);
    }
}
