using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Numerics;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;


[RequireComponent(typeof(MeshFilter))]
public class Delaunay : MonoBehaviour
{
    
    // SOURCES : MESH GENERATOR IN UNITY - BASICS (BY BRACKEYS) : https://www.youtube.com/watch?v=64NblGkAabk
    
    [SerializeField] public Mesh mesh;
    [SerializeField] public Vector3[] vertices;
    [SerializeField] public int[] triangles;
    [SerializeField] public int[] triangleIndex;
    CultureInfo cult = new CultureInfo(3); 
    CultureInfo cultureInfo = new CultureInfo("en-US");
    [SerializeField] private GameObject centerPrefab;
    [SerializeField] private int Resolution = 50;
    [SerializeField] private bool drawGizmoPointSky = false;
    [SerializeField] private bool drawGizmoCornerBase = false;
    [SerializeField] private bool drawGizmoCenterBase = false;
    
    

    #region MyRegion
    // These are the ones used for xmin and xmax values
    private float smallestx = default;
    private float smallesty = default;
    private float smallestz = default;
 
    private float largestx = default;
    private float largesty = default;
    private float largestz = default;
    

    [SerializeField] private float zmin = 0;
    [SerializeField] private float xmin = 0;
    [SerializeField] private float zmax = 0;
    [SerializeField] private float xmax = 0;
    #endregion

    /*Self-note: if const, then [SerializeField] won't work */
    [SerializeField] public int skipAmount = 100000;
    public string nameOfFile;

    #region Data Storage of Point Sky
    /*Sole purpose of this list is the keep track of all our points from our pointsky data file set */
    [SerializeField] private List<Vector3> mPointSky = new List<Vector3>();
    List<Vector3> CornerBase = new List<Vector3>(); // Corner
    List<Vector3> CenterBase = new List<Vector3>(); // Corner
    #endregion
    

    private void Start()
    {
        CreateGrid();
        UpdateMesh();
        if(!drawGizmoPointSky)
            mPointSky.Clear(); // clear this memory we dont need anymore
        if(!drawGizmoCornerBase)
            CornerBase.Clear();
        if(!drawGizmoCenterBase)
            CenterBase.Clear();
    }
    
    Vector3 GetHeight(Vector3 referance)
    {

        float AverageHeightOfNewPoint = 0;
        int AVG_Counter = 0;
        
        // Søke gjennom punkt sky
        foreach (var bts in mPointSky)
        {
            // Bounds
            var left = referance.x - Resolution / 2;
            var right = referance.x + Resolution / 2;
            var up = referance.z + Resolution / 2;
            var down = referance.z - Resolution / 2;
            
            // Calculating the average point
            if (bts.x >= left && bts.x <= right && bts.z >= down && bts.z <= up)
            {
                AVG_Counter++;
                AverageHeightOfNewPoint += bts.y; 
            }
        }
        if (AverageHeightOfNewPoint > 0)
        {
            AverageHeightOfNewPoint /= AVG_Counter;
            referance.y = AverageHeightOfNewPoint;
        }
        Debug.Log("referance.y : "+ referance.y.ToString("F2"));
        Debug.Log("AverageHeightOfNewPoint: "+ AverageHeightOfNewPoint);
        return referance;
    }
    
    void CreateGrid()
    {


        // Highest Floor for the x and z. This will be the size of steps for x and z.
        int zStepsHighestFloor = (int)zmax / Resolution;
        int xStepsHighestFloor = (int)xmax / Resolution;
        
        
        // Loop for setting the height of the center points
        // Bounds of Z, z begins at 0
        for (int z = (int) zmin; z <= zStepsHighestFloor; z++)
        {
            // Bounds of X, x begins at 0
            for (int x = (int) xmin; x <= xStepsHighestFloor; x++)
            {
                // Here we add the new corners of the grid from algoritme 10.6
                CornerBase.Add(new Vector3(x*Resolution,0,z*Resolution));
                
                // This is because the center points will end outside
                if (z < zStepsHighestFloor - 1 && x < xStepsHighestFloor - 1)
                {
                    var boa = new Vector3(x * Resolution + Resolution / 2, 0, z * Resolution + Resolution / 2);
                    boa = GetHeight(boa); // update the y value
                    CenterBase.Add(boa);
                }
                
            }
        }

        // Here vertices are overriden and we have the correct data for the vertices.
        vertices = CenterBase.ToArray(); 
        
        int vert = 0;
        int tris = 0;

        var tempList = new List<int>();
        
        int index = 0;
        int step1 = default; // first triangle
        int step2 = default; // first triangle
        int step3 = default; // first triangle
        int step4 = default; // second triangle
        int step5 = default; // second triangle
        int step6 = default; // second triangle

        
        for (int i = 0; i < zStepsHighestFloor - 2; i++)
        {
            for (int x = 0; x < xStepsHighestFloor - 2; x++)
            {
                step1 = index; 
                step2 = index + xStepsHighestFloor - 1;
                step3 = index + 1;
           
                step4 = index + 1;
                step5 = index + xStepsHighestFloor - 1;
                step6 = index + xStepsHighestFloor;
                index++;
                
                tempList.Add(step1);
                tempList.Add(step2);
                tempList.Add(step3);
                tempList.Add(step4);
                tempList.Add(step5);
                tempList.Add(step6);
            } 
            index++;
        }

        foreach (var tri in tempList)
        {
            Debug.Log(tri.ToString());
        }
        triangles = tempList.ToArray();
    
        Debug.Log("Resoltion: " + Resolution);
        Debug.Log("xStep: " + xStepsHighestFloor);
        Debug.Log("zStep: " + zStepsHighestFloor);
    }
    
    
    #region Update, Awake and Gizmos
    
    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        
        mesh.RecalculateNormals();
    }
    
    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        
        /*Firstly just read the data and give the information to different variables, like mPointSky etc*/
        ReadData(nameOfFile);
        
        /*Secondly, set the xmin,xmax,zmin and zmax*/
        SetExtremetiesValue();
    }
    #endregion


    private void OnDrawGizmos()
    {
        if (drawGizmoPointSky)
        {
            Gizmos.color = Color.cyan;
            foreach (var point in mPointSky)
            {
                Gizmos.DrawCube(point,Vector3.one * 15f);
            }
        }

        if (drawGizmoCornerBase)
        {
            Gizmos.color = Color.red;
            foreach (var corner in CornerBase)
            {
                Gizmos.DrawCube(corner,Vector3.one * 15f);
            }
        }

        if (drawGizmoCenterBase)
        {
            Gizmos.color = Color.green;
            foreach (var center in CenterBase)
            {
                Gizmos.DrawCube(center,Vector3.one * 15f);
            }
        }
        
    }

    #region Height functions
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
    #endregion


    void SetExtremetiesValue()
    {

        ///
        /// ONLY FINDING FOR X AND Z
        /// ALSO
        /// THIS IS AFTER DATA CONVERSION WHERE Y AND Z HAS CHANGED POSITIONS
        ///
        
        var tempX = new List<float>();
        var tempZ = new List<float>();
        
        foreach (var vtx in mPointSky)
        {
            tempX.Add(vtx.x);
            tempZ.Add(vtx.z);
        }

        xmax = tempX.Max();
        xmin = tempX.Min();
        zmax = tempZ.Max();
        zmin = tempZ.Min();

        // Clear temporaries
        tempZ.Clear();
        tempX.Clear();

    }
    
    private void ReadData(string nameOfFile)
    {
        
        string filepath = Path.Combine(Application.streamingAssetsPath, nameOfFile);
        
        if (File.Exists(filepath))
        {
            
            var tempText = File.ReadAllLines(filepath);
            
            if (tempText.Length > 0)
            {
                // Get the total amount of points
                int HowManyPoints = int.Parse(tempText[0]);
                
                // Set the capacity of PointSky 
                mPointSky.Capacity = HowManyPoints;
                
                // Temporary storage of all float values from respective X,Y,Z
                // That will be used to find extreme values
                List<float> tempX = new List<float>();
                List<float> tempY= new List<float>();
                List<float> tempZ= new List<float>();
                
                // Iterate to add in respective temporary storage.
                // Primarily just to get the extremeties within this loop.
                for (var i = 1; i <= HowManyPoints; i += skipAmount)
                {
                    var iterator = tempText[i].Split(' '); 
                    
                    float parse0 = float.Parse(iterator[0], cultureInfo); //x
                    float parse1 = float.Parse(iterator[1], cultureInfo); //y
                    float parse2 = float.Parse(iterator[2], cultureInfo); //z
                    tempX.Add(parse0);
                    tempY.Add(parse2);
                    tempZ.Add(parse1);
                
                    
                }
                
                
                
                smallestx = tempX.Min();
                smallesty = tempY.Min();
                smallestz = tempZ.Min();
                largestx = tempX.Max();
                largesty = tempY.Max();
                largestz = tempZ.Max();
                
                tempX.Clear();
                tempY.Clear();
                tempZ.Clear();
               
              
                // Now we read in again, but this time we add these points and also
                // subtract their smallest value from each point.
                // that way we keep the respective relation between all the points
                for (var i = 1; i <= HowManyPoints; i += skipAmount)
                {
                    var iterator = tempText[i].Split(' ');
                    
                    float parse0 = float.Parse(iterator[0], cultureInfo); // x but in unity it would be X  
                    float parse1 = float.Parse(iterator[1], cultureInfo); // y but in unity it would be Z
                    float parse2 = float.Parse(iterator[2], cultureInfo); // z but in unity it would be Y // SINCE ORIGINAL DATA THIS IS HEIGHT
                    
                    // Since y and set has changed in the previous loop 
                    // We have to subtract the incoming y(parse1) with the smallest z value(in this case 
                    var otherX = parse0 - smallestx; // x-x
                    var otherY = parse2 - smallesty; // y-y
                    var otherZ = parse1 - smallestz; // z-z
                    
                    var _newVertex = new Vector3(otherX, otherY, otherZ);
                    
                    mPointSky.Add(_newVertex);
                }

                vertices = mPointSky.ToArray(); // Later overriden with the correct vertices data within this pointsky data.

            }
            
        }
        
        else
        {
            Debug.Log("Fant ikke fil");
        }
        

    }
    
    
    
} // end
