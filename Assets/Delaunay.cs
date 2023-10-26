using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;


[RequireComponent(typeof(MeshFilter))]
public class Delaunay : MonoBehaviour
{
    private Mesh mesh;

    [SerializeField] private Vector3[] vertices;
    [SerializeField] private int[] triangles;
    

    [SerializeField] private float zmin = 0;
    [SerializeField] private float zmax = 0;
    [SerializeField] private float xmin = 0;
    [SerializeField] private float xmax = 0;

    [SerializeField] private Vector3 NE = Vector3.zero;
    [SerializeField] private Vector3 NW = Vector3.zero;
    [SerializeField] private Vector3 SW = Vector3.zero;
    [SerializeField] private Vector3 SE = Vector3.zero;

    [SerializeField] private List<Vector3> points = new List<Vector3>();
    

    public const int skipAmount = 1;
    public string nameee;


    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateMesh();
        UpdateMesh();
    }


    void CreateMesh()
    {

        var _vertices = new List<Vector3>();//[(int) ((xmax + 1) * (zmax + 1))];

        for (int i = 0, z = (int)zmin; i < zmax; i++)
        {
            for (int x = (int)xmin; x < xmax; x++)
            {
                _vertices.Add(new Vector3(x,0,z));
                // _vertices[i] = new Vector3(x, 0, z);
                i++;
            }
            
        }

        vertices = _vertices.ToArray();

    }

    private void OnDrawGizmos()
    {
        if (vertices == null) return;

        foreach (var vertx in vertices)
        {
            Gizmos.DrawCube(vertx, Vector3.one * 2f);
        }
    
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        
        mesh.RecalculateNormals();
    }
    
    private void Awake()
    {
        ReadData(nameee);
    }
    
    
    
    

    void ReadData(string nameOfFile)
    {
        string filepath = Path.Combine(Application.streamingAssetsPath, nameOfFile);

        //List<Vector3> mVertices = new List<Vector3>();

        if (File.Exists(filepath))
        {

            var tempText = File.ReadAllLines(filepath);

            CultureInfo cult = new CultureInfo(3); // tror ikke denne blir brukt

            if (tempText.Length > 0)
            {

                int howManyPoints = int.Parse(tempText[0]);
                
                points.Capacity = howManyPoints;
                
                CultureInfo cultureInfo = new CultureInfo("en-US");

                List<float> tempX = new List<float>();
                List<float> tempY = new List<float>();
                // List<float> tempZ = new List<float>();

                for (var i = 1; i <= howManyPoints; i += skipAmount)
                {
                    var iterator = tempText[i].Split(' '); //TODO spesifikt denne linja

                    // both for saving all points in a container and also finding xmin,ymax and that stuff
                    float parse0 = float.Parse(iterator[0], cultureInfo); //x
                    float parse1 = float.Parse(iterator[1], cultureInfo); //y
                    float parse2 = float.Parse(iterator[2], cultureInfo); //z


                    var newPoint = new Vector3(parse0,parse2,parse1);
                    points.Add(newPoint);
                    
                    // Y is now Z

                    tempX.Add(parse0); //x
                    // tempZ.Add(parse1); //z
                    tempY.Add(parse2); //y


                }
                
                // Bestem xmin,xmax,ymin,ymax
                xmin = tempX.Min();
                zmin = tempY.Min();
                zmax = tempY.Max();
                xmax = tempX.Max();

                tempX = null;
                // tempZ = null;
                tempY = null;

                NE = new Vector3(xmax, 0, zmax);
                NW = new Vector3(xmin, 0, zmax);
                SW = new Vector3(xmin, 0, zmin);
                SE = new Vector3(xmax, 0, zmin);

            }


        }
    }
    
    
    
}
