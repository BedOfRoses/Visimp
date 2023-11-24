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
    private Mesh mesh;

    [SerializeField] private Vector3[] vertices;
    [SerializeField] private int[] triangles;
    
    CultureInfo cult = new CultureInfo(3); 
    CultureInfo cultureInfo = new CultureInfo("en-US");

    [SerializeField] private GameObject centerPrefab;

    [SerializeField] private bool bDrawDebugGizmos = true;
    
    [SerializeField] List<Vector3> bucket = new List<Vector3>();  // Store vtx of points within space

    public struct quad
    {
        public float NE; //NE
        public float NW; //NW
        public float SE; //SE
        public float SW; //SW
        public Vector3 point;
    }
    [SerializeField] private int ResolutionQuad = 20;
    

    #region MyRegion

    
    // These values are before we change the minimum and maximum sizes          // maybe dont need to overwrite them    
    // [SerializeField] private float BeforeConversionsmallestx = default;      // maybe dont need to overwrite them
    // [SerializeField] private float BeforeConversionsmallesty = default;      // maybe dont need to overwrite them
    // [SerializeField] private float BeforeConversionsmallestz = default;      // maybe dont need to overwrite them
    // [SerializeField] private float BeforeConversionlargestx = default;       // maybe dont need to overwrite them
    // [SerializeField] private float BeforeConversionlargesty = default;       // maybe dont need to overwrite them
    // [SerializeField] private float BeforeConversionlargestz = default;       // maybe dont need to overwrite them
    
    // These are the ones used for xmin and xmax values
    [SerializeField] private float smallestx = default;
    [SerializeField] private float smallesty = default;
    [SerializeField] private float smallestz = default;
 
    [SerializeField] private float largestx = default;
    [SerializeField] private float largesty = default;
    [SerializeField] private float largestz = default;
    

    [SerializeField] private float zmin = 0;
    [SerializeField] private float xmin = 0;
    [SerializeField] private float zmax = 0;
    [SerializeField] private float xmax = 0;

    
    [SerializeField] private Vector3 NE = Vector3.zero;
    [SerializeField] private Vector3 NW = Vector3.zero;
    [SerializeField] private Vector3 SW = Vector3.zero;
    [SerializeField] private Vector3 SE = Vector3.zero;

    [SerializeField] private List<Vector3> points = new List<Vector3>();
    #endregion

    /*Self-note: if const, then [SerializeField] won't work */
    [SerializeField] public int skipAmount = 100000;
    public string nameee;

    #region Data Storage of Point Sky
    
    /*Sole purpose of this list is the keep track of all our points from our pointsky data file set */
    [SerializeField] private List<Vector3> mPointSky = new List<Vector3>();
    [SerializeField] private List<Vector3> tempCenter = new List<Vector3>();

    #endregion
    

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        //TODO: CREATE MESH FUNKER IKKE
        // CreateMesh();
        CreateMesh2();
        UpdateMesh();
    }


    void CreateMesh2()
    {


        Vector3 avgPoint = new Vector3(); 
        float avgHeight = 0;
        
        int c = 0;


        float zzzmin=0, zzzmax=0, xxxmax = 0, xxxmin = 0;
        int pointCounter = 0;

        
        for (int i = 0, z = (int)zmin; i < zmax; i+=ResolutionQuad)
        {
            for (int x = (int)xmin; x < xmax; x+=ResolutionQuad)
            {
                
                // Reset or Flush the bucket
                avgHeight = 0;
                pointCounter = 0;
                c = 0;
                avgPoint = new Vector3(); // clear it
                
                
                //Traverse within a space of the x and z axis. Here we will add them into a bucket and create new "center" dots
                foreach (var vtx in mPointSky)
                {
                    // our point is within the square size
                    if (vtx.x >= x && vtx.x < x+ResolutionQuad &&  vtx.z >= z && vtx.z < z+ResolutionQuad) //TODO: MISSING LOGIC OF THE BOUNDS WE ARE ITERATING. RIGHT NOW WE GET 2000000 OF THE SAME POINTS LOL!
                    {
                     // Add to bucket  
                     bucket.Add(vtx);
                     avgPoint.x += vtx.x;
                     avgPoint.z += vtx.z;
                     avgHeight += avgPoint.y;
                     c++;
                    }
                }
                
                if (pointCounter)
                
                // Divide the point by the counter 'c'
                // All points divided by c

               
                
                Instantiate(centerPrefab, avgPoint, Quaternion.identity);
              


            } ///// Going back up / moving further
            
        }

        
        //Instantiate the new center points from the 
        
        
        
        Debug.Log("Finished looping");
        
        
        
        // THIS "vertices" IS SUPPOSED TO BE THE "PLANE"
        // vertices = _vertices.ToArray();
    }

    
    
    void CreateMesh()
    {

        // New points that we will create mesh of.
        var _vertices = new List<Vector3>();//[(int) ((xmax + 1) * (zmax + 1))];

        for (int i = 0, z = (int)zmin; i <= zmax; i+=ResolutionQuad)
        {
            for (int x = (int)xmin; x <= xmax; x+=ResolutionQuad)
            {

                // Variables that are going to give us the greatest values within here
                float avg_height = default;
                float avg_NW = default;
                float avg_NE = default;
                float avg_SW = default;
                float avg_SE = default;
                //Create new plane
                quad flate = new quad();

                
                // Give them the new List to "clear" themselves
                List<float> tempX = new List<float>();
                List<float> tempZ = new List<float>();
                List<Vector3> tempVertex = new List<Vector3>(); // In order to get the average amount of surface area of all
                int countHowManyWithinArea = 0;
                // Loop through our points that are within the x and z to x+resolution and z+resolution
                foreach (var vtx in mPointSky)
                {
                    // our point is within the square size
                    if (vtx.x <= x && vtx.z <= z)
                    {
                        //TODO: Work with implementation of points into a new plane. Check 
                        // This is inheriently the correct implemenatation.
                        // Because now we iterate/move along an "axis" where all these existing points are
                        
                        //Add this vtx that has been 
                        tempX.Add(vtx.x);   // used to get the corners within this respective quad that we are searching
                        tempZ.Add(vtx.z);   // used to get the corners within this respective quad that we are searching
                        avg_NW += z;
                        avg_NE += z + ResolutionQuad;
                        avg_SW += x;
                        avg_SE += x + ResolutionQuad;
                        avg_height += vtx.y;
                        
                        countHowManyWithinArea++;
                    }
                }
                ///////////////////////////////////////////////////////////////////////////////////////////////////////
                // After calculating average height from looping all points that exists
                
                // float _xmax = default;
                // float _xmin = default;
                // float _zmin = default;
                // float _zmax = default;
                // 
                // _xmax = tempX.Max();
                // _xmin = tempX.Min();
                // _zmin = tempZ.Max();
                // _zmax = tempZ.Min();
                // 
                // 
                // Debug.Log("xmax"+_xmax+"xmin"+_zmax+"zmin"+_zmin+"zmax"+_zmax);

                // float avgX = (avg_SW + avg_SE) / 2 * countHowManyWithinArea;
                // float avgY = avg_height / countHowManyWithinArea;
                // float avgZ = (avg_NW + avg_NE) / 2 * countHowManyWithinArea;
 
                // flate.point = new Vector3(avgX, avgY, avgZ);
                // 
                // Debug.Log(flate.point);
                // tempCenter.Add(flate.point);
                // 
                // tempX.Clear();
                // tempZ.Clear();
                
                // flate.NW = _zmax + _zmin;
                // flate.NE = _zmax + _xmax;
                // flate.SW = _zmin + _xmin;
                // flate.SE = _zmin + _xmax;
                
                
                ////////////////////////////////////////////////////////////////////////////////////////////////////////
                // quad surf = new quad();
                // surf.NE     
                
                //float y = GetSurfaceHeight(new Vector2(x,z));
                
                //TODO: MAYBE NOT ADD THESE TO A VERTICES OF ANY SORT
                _vertices.Add(new Vector3(x,0,z));
                i++;
            }
            
        }

        // THIS "vertices" IS SUPPOSED TO BE THE "PLANE"
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

       // foreach (var vertx in vertices)
       // {
       //     Gizmos.color = Color.green;
       //     Gizmos.DrawCube(vertx, Vector3.one * 60f);
       // }

       if (bDrawDebugGizmos)
       {
           foreach (var vtxx in mPointSky)
           {
               Gizmos.color = Color.blue;
               Gizmos.DrawCube(vtxx, Vector3.one * 50f);
           } 
       }
       
       
        
        // foreach (var vtx in tempCenter)
        // {
        //     Gizmos.color = Color.blue;
        //     Gizmos.DrawCube(vtx,Vector3.one * 60f);
        // }
    
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
        /*Firstly just read the data and give the information to different variables, like mPointSky etc*/
        ReadData(nameee);
        
        /*Secondly, set the xmin,xmax,zmin and zmax*/
        SetExtremetiesValue();
        
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
        
        // List<Vector3> mPointSky = new List<Vector3>();
        
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
                for (var i = 1; i <= HowManyPoints; i += skipAmount)
                {
                    var iterator = tempText[i].Split(' '); 
                    
                    float parse0 = float.Parse(iterator[0], cultureInfo); //x
                    float parse1 = float.Parse(iterator[1], cultureInfo); //y
                    float parse2 = float.Parse(iterator[2], cultureInfo); //z
                    ///
                    ///
                    /// Y is now Z !!!!!!!!!!!!!!!!!!!!!
                    ///
                    /// 
                    tempX.Add(parse0);
                    tempY.Add(parse2);
                    tempZ.Add(parse1);
                
                    
                }
                
                
                ////////////////////////////////////////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////////////////////////////////////////
                ///
                ///     IMPORTANT FOR SMALLEST VALUES (float values)
                ///     Y AND Z HAS NOW CHANGED PLACES, WHICH MEANS:
                ///     X(use), Y(height), Z(use)
                ///     
                /// 
                ////////////////////////////////////////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////////////////////////////////////////
                
                smallestx = tempX.Min();
                smallesty = tempY.Min();
                smallestz = tempZ.Min();
                largestx = tempX.Max();
                largesty = tempY.Max();
                largestz = tempZ.Max();
                
                tempX.Clear();
                tempY.Clear();
                tempZ.Clear();
                //tempX = null;
                //tempY = null;
                //tempZ = null;
                
              
              
                // Now we read in again, but this time we add these points and also
                // subtract their smallest value from each point.
                // that way we keep the respective relation between all the points
                for (var i = 1; i <= HowManyPoints; i += skipAmount)
                {
                    var iterator = tempText[i].Split(' '); // splitter opp mellom mellomrommet
                    
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

                //TODO: ADD THESE to a temp vertices
                vertices = mPointSky.ToArray();

            }
            
        }
        
        else
        {
            Debug.Log("Fant ikke fil");
        }
        

    }
    
    
    
}
