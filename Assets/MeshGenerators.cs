using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MeshGenerators
{
    public static Mesh GenerateSquareMesh()
    {
        var mesh = new Mesh();
        var verticies = new Vector3[3]
        {
            new Vector3(0,0.57735f,0), new Vector3(-0.5f, -0.28828f, 0), new Vector3(0.5f, -0.28828f, 0)
        };

        //Triangles
        var tri = new int[3];

        tri[0] = 0;
        tri[1] = 2;
        tri[2] = 1;

        mesh.vertices = verticies;
        mesh.triangles = tri;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        return mesh;
    }

    public static Mesh GenerateTriangleMesh()
    {
        var mesh = new Mesh();
        // Verticies
        var verticies = new Vector3[3]
        {
            new Vector3(0,0.57735f,0), new Vector3(-0.5f, -0.28828f, 0), new Vector3(0.5f, -0.28828f, 0)
        };

        //Triangles
        var tri = new int[3];

        tri[0] = 0;
        tri[1] = 2;
        tri[2] = 1;

        mesh.vertices = verticies;
        mesh.triangles = tri;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        return mesh;
    }

    public static Mesh GenerateCircleMesh(float radius)
    {
        // We want to make sure that the circle appears to be curved.
        // This can be approximated by drawing a regular polygon with lots of segments.
        // The number of segments can be increased based on the radius so that large circles also appear curved.
        // We use an offset and multiplier to create a tunable linear function.
        const float segmentOffset = 40f;
        const float segmentMultiplier = 2 * Mathf.PI;
        var numSegments = (int)(radius * segmentMultiplier + segmentOffset);

        // Create an array of points arround a circle
        var circleVertices = Enumerable.Range(0, numSegments)
            .Select(i =>
            {
                var theta = 2 * Mathf.PI * i / numSegments;
                return new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)) * radius;
            })
            .ToArray();

        // Find all the triangles in the shape
        var triangles = new Triangulator(circleVertices).Triangulate();

        var mesh = new Mesh
        {
            name = "Circle",
            vertices = circleVertices.ToVector3(),
            triangles = triangles
        };

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        return mesh;
    }

}
