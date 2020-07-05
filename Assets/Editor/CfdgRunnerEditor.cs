using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CfdgPlayer))]
public class CfdgRunnerEditor : Editor
{
    private string[] m_GrammarNames;
    private string[] m_Grammars;
    private CfdgPlayer m_Runner;
    private int m_CurrentGrammarIndex;

    void OnEnable()
    {
        m_Runner = target as CfdgPlayer;

        var sampleGrammarType = typeof(SampleGrammars);
        var fields = sampleGrammarType.GetFields();
        var grammarInfos = fields.Where(fieldInfo => fieldInfo.Name.EndsWith("Grammar") && fieldInfo.FieldType == typeof(string)).Select(fieldInfo =>
        {
            var grammaName = fieldInfo.Name.Replace("Grammar", "");
            var grammarValue = fieldInfo.GetValue(target) as string;
            return Tuple.Create(grammaName, grammarValue);
        });
        m_GrammarNames = grammarInfos.Select(t => t.Item1).ToArray();
        m_Grammars = grammarInfos.Select(t => t.Item2).ToArray();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Reset"))
        {
            m_Runner.currentTime = 0f;
        }

        if (GUILayout.Button("Generate"))
        {
            m_Runner.Run();
        }

        if (GUILayout.Button("Save"))
        {
            SaveGrammar();
        }

        if (GUILayout.Button("Load"))
        {
            LoadGrammar();
        }

        EditorGUI.BeginChangeCheck();
        m_CurrentGrammarIndex = EditorGUILayout.Popup("Sample Grammar", m_CurrentGrammarIndex, m_GrammarNames);
        if (EditorGUI.EndChangeCheck())
        {
            m_Runner.grammarStr = m_Grammars[m_CurrentGrammarIndex];
        }

        m_Runner.grammarStr = EditorGUILayout.TextArea(m_Runner.grammarStr);
    }

    void SaveGrammar()
    {
        var assetName = EditorUtility.SaveFilePanel("Save Grammar Image", "Assets/cfdg/GrammarImgs", "cfdg", "png");
        if (string.IsNullOrEmpty(assetName))
        {
            return;
        }

        var camera = m_Runner.camera;
        var rect = new Rect(0, 0, camera.pixelWidth, camera.pixelHeight);
        var renderTexture = new RenderTexture((int)rect.width, (int)rect.height, 24);
        var snapshotTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);

        var oldCameraRenderTexture = camera.targetTexture;
        camera.targetTexture = renderTexture;
        camera.Render();

        var old = RenderTexture.active;
        RenderTexture.active = renderTexture;
        snapshotTexture.ReadPixels(rect, 0, 0);
        RenderTexture.active = old;
        camera.targetTexture = oldCameraRenderTexture;

        // Don't forget to apply so that all operations are done.
        snapshotTexture.Apply();

        var currentGrammar = m_Runner.grammarStr;
        var encodedGrammar = Steganography.Encode(snapshotTexture, currentGrammar);

        var grammarStr = Steganography.DecodeAsString(encodedGrammar);

        var bytes = encodedGrammar.EncodeToPNG();
        System.IO.File.WriteAllBytes(assetName, bytes);

        EditorUtility.RevealInFinder(assetName);
    }

    void LoadGrammar()
    {
        var grammarFile = EditorUtility.OpenFilePanel("Load Grammar Image", "Assets/cfdg/GrammarImgs", "png");
        if (string.IsNullOrEmpty(grammarFile))
            return;

        try
        {
            var bytes = System.IO.File.ReadAllBytes(grammarFile);
            var tex = new Texture2D(2, 2);
            if (!tex.LoadImage(bytes))
            {
                Debug.LogError($"Cannot load Grammar File: {grammarFile}");
                return;
            }

            var grammarStr = Steganography.DecodeAsString(tex);
            m_Runner.grammarStr = grammarStr;
            m_Runner.Run();
        }
        catch (Exception e)
        {
            Debug.LogError($"Cannot load Grammar File: {grammarFile} {e}");
        }
    }
}
