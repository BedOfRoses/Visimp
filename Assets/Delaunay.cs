using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Numerics;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;


[RequireComponent(typeof(MeshFilter))]
public class Delaunay : MonoBehaviour
{
    private Mesh mesh;

    [SerializeField] private Vector3[] vertices;
    [SerializeField] private int[] triangles;
    
    CultureInfo cult = new CultureInfo(3); 
    CultureInfo cultureInfo = new CultureInfo("en-US");


    
    public struct quad
    {
        public float NE; //NE
        public float NW; //NW
        public float SE; //SE
        public float SW; //SW
        public Vector3 point;
    }
    private float ResolutionQuad = 100;

    public T GetDefaultValue<T>()
    {
        return default(T);
    }

    #region MyRegion

    

    [SerializeField] private float zmin = 0;
    [SerializeField] private float zmax = 0;
    [SerializeField] private float xmin = 0;
    [SerializeField] private float xmax = 0;

    
    [SerializeField] private Vector3 NE = Vector3.zero;
    [SerializeField] private Vector3 NW = Vector3.zero;
    [SerializeField] private Vector3 SW = Vector3.zero;
    [SerializeField] private Vector3 SE = Vector3.zero;

    [SerializeField] private List<Vector3> points = new List<Vector3>();
    #endregion


    public const int skipAmount = 10000000;
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

        // New points that we will create mesh of.
        var _vertices = new List<Vector3>();//[(int) ((xmax + 1) * (zmax + 1))];

        for (int i = 0, z = (int)zmin; i <= zmax; i++)
        {
            for (int x = (int)xmin; x <= xmax; x++)
            {

                //float y = GetSurfaceHeight(new Vector2(x,z));
                 _vertices.Add(new Vector3(x,0,z));
                i++;
            }
            
        }

        vertices = _vertices.ToArray();

        
        int vert = 0;
        int tris = 0;

        triangles = new int[(int)zmax * (int)xmax * 6];

        for (int z = (int)zmin; z < (int)zmax; z++)
        {
            for (int x = (int) xmin; x < (int) xmax; x++)
            {

                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + (int)xmax + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + (int)xmax + 1;
                triangles[tris + 5] = vert + (int)xmax + 2;

                vert++;
                tris += 6;
            }

            vert++;

        }
        
        


    }

    #region Update, Awake and Gizmos

    

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
    #endregion


    void newFunc()
    {
        
        
        
        //quad mainQuad = new quad();
        //mainQuad.NE = xmax + zmax;
        //mainQuad.NW = xmin + zmax;
        //mainQuad.SW = xmin + zmin;
        //mainQuad.SE = xmax + zmin;

        NE = new Vector3(xmax, 0, zmax); // just storing data
        NW = new Vector3(xmin, 0, zmax); // just storing data
        SW = new Vector3(xmin, 0, zmin); // just storing data
        SE = new Vector3(xmax, 0, zmin); // just storing data

        // 1. Hittil har vi lest inn alle punktene
        // 2. Bestemt xmin,zmin, xmax, zmax
        // 3. valgt at ResolutionQuad er vår størrelse
        // 4. for alle datapunkter

        // points er container for alle punktene fra punktsky.
                
        foreach (var vtx in points)
        {
            
            
            
            // (a) bestem hvilken rute de tilhører 
            // (b) bruk midtpunktet i ruta som xz-verdi (altså y verdi)
            // (c) registrer høyden og oppdater høyden for midtpunktet i aktuall rute
            //      (for eksempel gjennomsnitt, vi kan få flere punkter i en rute)
                    
                    
                    
                    
        }
    }


    public float GetSurfaceHeight(Vector2 point)
    {
        // source https://github.com/haldorj/3Dprog22/blob/main/triangulation.cpp
        
        
        for (var i = 0; i < mesh.triangles.Length; i++)
        {
            Vector3 v0, v1, v2 = default;
            
            var index_0 = triangles[i];
            var index_1 = triangles[i+1];
            var index_2 = triangles[i+2];
            
            v0 = vertices[index_0];
            v1 = vertices[index_1];
            v2 = vertices[index_2];

          

            var baryCords = BarycentricFunc(
                new Vector2(v0.x,v0.z), 
                new Vector2(v1.x,v1.z), 
                new Vector2(v2.x,v2.z),
                point);
            
            // var baryCords = ballRef.Barcentry(v0, v1, v2, veccc);
            if (baryCords is not {x: >= 0, y: >= 0, z: >= 0}) continue;
            var height = (baryCords.x * v0.y + baryCords.y * v1.y + baryCords.z * v2.y);

            return height;
        }

        return 0f;
    }
    
    
    private static Vector3 BarycentricFunc(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 pt)
    {
        Vector2 p12 = p2 - p1;
        Vector2 p13 = p3 - p1;
        Vector3 nn = Vector3.Cross(new Vector3(p12.x, 0, p12.y), new Vector3(p13.x, 0, p13.y));
        float areal_123 = nn.magnitude;
        Vector3 baryc = default; 
       
        // u
        Vector2 p = p2 - pt;
        Vector2 q = p3 - pt;
        nn = Vector3.Cross(new Vector3(p.x,0,p.y), new Vector3(q.x,0,q.y));
        baryc.x = nn.y / areal_123;
       
        // v
        p = p3 - pt;
        q = p1 - pt;
        nn = Vector3.Cross(new Vector3(p.x,0,p.y), new Vector3(q.x,0,q.y));
        baryc.y = nn.y / areal_123;
        
        // w
        p = p1 - pt;
        q = p2 - pt;
        nn = Vector3.Cross(new Vector3(p.x,0,p.y), new Vector3(q.x,0,q.y));
        baryc.z = nn.y / areal_123;
        
        // Debug.Log("Baryc"+ baryc.ToString("F2"));
        return baryc;
    }

    
    
    void ReadData(string nameOfFile)
    {
        string filepath = Path.Combine(Application.streamingAssetsPath, nameOfFile);
       
        
        if (File.Exists(filepath))
        {
            var tempText = File.ReadAllLines(filepath);
            if (tempText.Length > 0)
            {
                int howManyPoints = int.Parse(tempText[0]);
                points.Capacity = howManyPoints;

                List<float> tempX = new List<float>();
                List<float> tempY = new List<float>();
                // List<float> tempZ = new List<float>();

                for (var i = 1; i <= howManyPoints; i += skipAmount)
                {
                    var iterator = tempText[i].Split(' ');

                    // both for saving all points in a container and also finding xmin,ymax and that stuff
                    float parse0 = float.Parse(iterator[0], cultureInfo); //x
                    float parse1 = float.Parse(iterator[1], cultureInfo); //y
                    float parse2 = float.Parse(iterator[2], cultureInfo); //z


                    var newPoint = new Vector3(
                        parse0,  //x
                        parse2,  //z
                        parse1); //y
                    points.Add(newPoint);
                    
                    // Y is now Z

                    tempX.Add(parse0); //x
                    // tempZ.Add(parse1); //y
                    tempY.Add(parse2); //z


                }
                
                // Bestem xmin,xmax,ymin,ymax
                // Mainly for storing them somewhere
                xmin = tempX.Min();
                zmin = tempY.Min();
                zmax = tempY.Max();
                xmax = tempX.Max();

                tempX = null;
                // tempZ = null;
                tempY = null;

               
                
                
                
            }

            
            
            

        }
    }
    
    
    
}
