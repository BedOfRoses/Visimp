using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Source Bracekeys https://www.youtube.com/watch?v=64NblGkAabk

[RequireComponent(typeof(MeshFilter))]
public class Brackeys : MonoBehaviour
{
    private Mesh mesh;


    private Vector3[] vertices; 
    private int[] triangles;
    
    public int xSize = 20;
    public int zSize = 20;


    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateShape();
        UpdateMesh();
    }


    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int z = 0, i = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                // looping all the vertices
                // need an index
                vertices[i] = new Vector3(x, 0, z);
                i++;

            }
        }

        triangles = new int[6];
        triangles[0] = 0;
        triangles[1] = xSize + 1;
        triangles[2] = 1;
        triangles[3] = 1;
        triangles[4] = xSize + 1;
        triangles[5] = xSize + 2;
//


    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    private void OnDrawGizmos()
    {
        if(vertices == null) return;
        foreach (var vtx in vertices)
        {
            Gizmos.DrawCube(vtx, Vector3.one * 0.1f);
        }
    }
}
