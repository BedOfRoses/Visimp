using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class CreateMap : MonoBehaviour
{
    // SOURCES : MESH GENERATOR IN UNITY - BASICS (BY BRACKEYS)

    public float dynFrick;
    public float statFrick;
    
    public Mesh mesh;
    public Vector3[] vertices;
    public int[] triangles;
    
    

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateShape();
        UpdateMesh();
        MeshCollider meshji = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        meshji.material.dynamicFriction = dynFrick;
        meshji.material.staticFriction = statFrick;
    }

    void CreateShape()
    {
        vertices = new Vector3[]
        {
            new Vector3(0     ,0.097f  ,0      )       * 10f  ,     // 0
            new Vector3(0.4f  ,0.005f  ,0      )       * 10f  ,     // 1
            new Vector3(0     ,0.005f  ,0.4f   )       * 10f  ,     // 2
            new Vector3(0.4f  ,0.075f  ,0.4f   )       * 10f  ,     // 3
            new Vector3(0.8f  ,0.007f  ,0.4f   )       * 10f  ,     // 4
            new Vector3(0.8f  ,0.039f  ,0      )       * 10f        // 5
        };

        triangles = new int[]
        {
          4,5,1,    
          1,3,4,    
          3,1,2,    
          2,1,0     
        };
    }


    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }

}













//DRITT
// 0,1,2, // A
// 2, 4, 1, // B
// 5,4,2, // C




//TODO SJEKKE DENNE VERTEX DATAEN
// //A 
// new Vector3(0.0f, 0.0f, 0.097f),
// new Vector3(0.4f, 0.0f, 0.005f),
// new Vector3(0.0f, 0.4f, 0.005f),
// //B 
// new Vector3(0.0f, 0.4f, 0.005f),
// new Vector3(0.4f, 0.0f, 0.005f),
// new Vector3(0.4f, 0.4f, 0.075f),
// //C 
// new Vector3(0.4f, 0.4f, 0.075f),
// new Vector3(0.0f, 0.4f, 0.005f),
// new Vector3(0.8f, 0.4f, 0.007f),
// //D 
// new Vector3(0.4f, 0.0f, 0.005f),
// new Vector3(0.8f, 0.4f, 0.007f),
// new Vector3(0.8f, 0.8f, 0.039f)


