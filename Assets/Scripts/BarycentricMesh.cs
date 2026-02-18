using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
public class BarycentricMesh : MonoBehaviour
{
    void Awake()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        List<Vector3> newVerts = new List<Vector3>();
        List<int> newTris = new List<int>();
        List<Vector3> bary = new List<Vector3>();

        for (int i = 0; i < triangles.Length; i += 3)
        {
            int i0 = triangles[i];
            int i1 = triangles[i + 1];
            int i2 = triangles[i + 2];

            int baseIndex = newVerts.Count;

            newVerts.Add(vertices[i0]);
            newVerts.Add(vertices[i1]);
            newVerts.Add(vertices[i2]);

            bary.Add(new Vector3(1, 0, 0));
            bary.Add(new Vector3(0, 1, 0));
            bary.Add(new Vector3(0, 0, 1));

            newTris.Add(baseIndex);
            newTris.Add(baseIndex + 1);
            newTris.Add(baseIndex + 2);
        }

        Mesh newMesh = new Mesh();
        newMesh.SetVertices(newVerts);
        newMesh.SetTriangles(newTris, 0);
        newMesh.SetUVs(0, bary);
        newMesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = newMesh;
    }
}
