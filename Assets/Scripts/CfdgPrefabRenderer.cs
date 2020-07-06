using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CfdgPrefabRenderer : MonoBehaviour, CfdgRenderer
{
    public GameObject root;
    public GameObject circlePrefab;
    public GameObject squarePrefab;
    public GameObject trianglePrefab;
    public Material combineMeshMaterial;
    public bool combineMeshes;

    private List<Shape> m_Shapes;
    private int m_NbShapeVisible;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(List<Shape> shapes)
    {
        m_NbShapeVisible = 0;
        m_Shapes = shapes;
        if (root == null)
        {
            root = new GameObject("Shapes");
        }

        var block = new MaterialPropertyBlock();

        CombineInstance[] combine = null;
        if (combineMeshes)
            combine = new CombineInstance[shapes.Count];


        var colors = new List<Color>(shapes.Count * 8);
        for (var i = 0; i < shapes.Count; ++i)
        {
            var shape = shapes[i];
            shape.index = i;

            var prefab = squarePrefab;
            switch (shape.shapeId)
            {
                case "SQUARE":
                    prefab = squarePrefab;
                    break;
                case "CIRCLE":
                    prefab = circlePrefab;
                    break;
                case "TRIANGLE":
                    prefab = trianglePrefab;
                    break;
            }

            if (combineMeshes)
            {
                // TODO combine mesh: no color applied!
                var meshFilter = prefab.GetComponent<MeshFilter>();
                combine[i].mesh = meshFilter.sharedMesh;
                combine[i].transform = shape.transform;

                for (var v = 0; v < combine[i].mesh.vertexCount; ++v)
                {
                    colors.Add(shape.color);
                }
            }
            else
            {
                var position = new Vector3(shape.transform.m03, shape.transform.m13, shape.transform.m23);
                var rotation = shape.transform.rotation;
                var scale = shape.transform.lossyScale;
                var shapeObj = Instantiate(prefab, position, rotation, root.transform);
                shapeObj.transform.localScale = scale;
                shapeObj.SetActive(false);

                var meshRenderer = shapeObj.GetComponent<MeshRenderer>();
                block.SetColor("_Color", shape.color);
                meshRenderer.SetPropertyBlock(block);
            }
        }

        if (combineMeshes)
        {
            var meshFilter = root.AddComponent<MeshFilter>() as MeshFilter;
            meshFilter.mesh = new Mesh();
            meshFilter.mesh.indexFormat = IndexFormat.UInt32;
            meshFilter.mesh.CombineMeshes(combine);
            meshFilter.mesh.SetColors(colors);

            var meshRenderer = root.AddComponent<MeshRenderer>();
            meshRenderer.material = combineMeshMaterial;
        }
    }

    public void Draw(float relativeTime)
    {
        if (combineMeshes)
            return;

        var nbShapesVisible = (int)(m_Shapes.Count * relativeTime);
        var reset = false;
        if (nbShapesVisible < m_NbShapeVisible)
        {
            reset = true;
            m_NbShapeVisible = 0;
        }

        for (; m_NbShapeVisible < nbShapesVisible; ++m_NbShapeVisible)
        {
            var child = root.transform.GetChild(m_NbShapeVisible);
            child.gameObject.SetActive(true);
        }

        if (reset && m_NbShapeVisible < m_Shapes.Count)
        {
            for (var i = m_NbShapeVisible; m_NbShapeVisible < m_Shapes.Count && i < root.transform.childCount; i++)
            {
                var child = root.transform.GetChild(i);
                child.gameObject.SetActive(false);
            }
        }
    }

    public void ShutDown()
    {
        m_NbShapeVisible = 0;
        GameObject.DestroyImmediate(root);
        root = null;
    }
}
