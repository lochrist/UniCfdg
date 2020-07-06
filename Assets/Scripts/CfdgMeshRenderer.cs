using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct MeshProperties
{
    public Matrix4x4 mat;
    public Vector4 color;

    public static int Size()
    {
        return
            sizeof(float) * 4 * 4 + // matrix;
            sizeof(float) * 4;      // color;
    }
}

class ShapeRenderer
{
    public string id;
    public ComputeBuffer meshPropertiesBuffer;
    public ComputeBuffer argsBuffer;
    public Mesh mesh;
    public Material material;
    public List<Shape> shapes;
    public bool isEnable;
    public int nbVisibleShapes;

    public ShapeRenderer(string id, Material srcMaterial, Mesh srcMesh)
    {
        this.id = id;
        mesh = srcMesh;
        material = new Material(srcMaterial);
        shapes = new List<Shape>();
    }

    uint[] GetArgBufferData(int shapeCount)
    {
        var args = new uint[5] { 0, 0, 0, 0, 0 };
        args[0] = mesh.GetIndexCount(0);
        args[1] = (uint)shapeCount;
        args[2] = mesh.GetIndexStart(0);
        args[3] = mesh.GetBaseVertex(0);
        return args;
    }

    public void Init()
    {
        isEnable = shapes.Count > 0;
        if (!isEnable)
            return;

        var args = GetArgBufferData(shapes.Count);
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        argsBuffer.SetData(args);

        // Initialize buffer with the given population.
        var properties = new MeshProperties[shapes.Count];
        for (int i = 0; i < shapes.Count; i++)
        {
            var props = new MeshProperties();
            props.mat = shapes[i].transform;
            props.color = shapes[i].color;
            properties[i] = props;
        }

        meshPropertiesBuffer = new ComputeBuffer(shapes.Count, MeshProperties.Size());
        meshPropertiesBuffer.SetData(properties);
        material.SetBuffer("_Properties", meshPropertiesBuffer);
    }

    public void Draw(Bounds bounds, int nextVisibleShapeIndex)
    {
        if (isEnable)
        {
            if (nextVisibleShapeIndex < nbVisibleShapes)
                nbVisibleShapes = 0;
            for (; nbVisibleShapes < shapes.Count; ++nbVisibleShapes)
            {
                if (shapes[nbVisibleShapes].index > nextVisibleShapeIndex)
                    break;
            }

            var args = GetArgBufferData(Mathf.Min(nbVisibleShapes, shapes.Count));
            argsBuffer.SetData(args);
            Graphics.DrawMeshInstancedIndirect(mesh, 0, material, bounds, argsBuffer);
        }
    }

    public void Release()
    {
        meshPropertiesBuffer?.Release();
        meshPropertiesBuffer = null;
        argsBuffer?.Release();
        argsBuffer = null;
    }
}

public class CfdgMeshRenderer : MonoBehaviour, CfdgRenderer
{
    public Mesh triangleMesh;
    public Mesh circleMesh;
    public Mesh quadMesh;
    public Material material;

    private List<Shape> m_Shapes;
    private Dictionary<string, ShapeRenderer> m_Renderers;
    private Bounds m_Bounds;

    public void Init(List<Shape> shapes)
    {
        m_Shapes = shapes;
        m_Renderers = new Dictionary<string, ShapeRenderer>();
        m_Renderers.Add("SQUARE", new ShapeRenderer("SQUARE", material, quadMesh));
        m_Renderers.Add("CIRCLE", new ShapeRenderer("CIRCLE", material, circleMesh));
        m_Renderers.Add("TRIANGLE", new ShapeRenderer("TRIANGLE", material, triangleMesh));

        using (new DebugLogTimer("MeshRenderer - Assign Shapes"))
            AssignShapes(shapes);

        using (new DebugLogTimer("MeshRenderer - Setup Buffers"))
            SetupBuffers();

        m_Bounds = new Bounds(transform.position, Vector3.up * 2);
    }

    public void Draw(float relativeTime)
    {
        var nbShapesVisible = (int)(m_Shapes.Count * relativeTime);
        foreach (var renderer in m_Renderers.Values)
        {
            renderer.Draw(m_Bounds, nbShapesVisible);
        }
    }

    public void ShutDown()
    {
        if (m_Renderers == null)
            return;

        foreach (var renderer in m_Renderers.Values)
        {
            renderer.Release();
        }
        m_Renderers.Clear();
    }

    void AssignShapes(List<Shape> shapes)
    {
        string currentShapeId = null;
        var z = 20f;
        var zDelta = 20f / shapes.Count;
        for (var i = 0; i < shapes.Count; ++i)
        {
            var shape = shapes[i];
            shape.index = i;
            if (!m_Renderers.TryGetValue(shape.shapeId, out var shapeRenderer))
            {
                continue;
            }

            if (shape.shapeId != currentShapeId)
            {
                currentShapeId = shape.shapeId;
                z -= zDelta;
            }
            shape.transform *= Matrix4x4.Translate(new Vector3(0, 0, z));
            shapeRenderer.shapes.Add(shape);
        }
    }

    void SetupBuffers()
    {
        foreach (var shapeRenderer in m_Renderers.Values)
        {
            shapeRenderer.Init();
        }
    }
}
