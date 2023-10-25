using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;
using System.Linq;

public class ReadMeshFile : MonoBehaviour
{
    public Vector3[] vertices;
    [SerializeField] private string StinkyString;
    [SerializeField] public int[] triangles;

    [SerializeField] private float smallestx;
    [SerializeField] private float smallesty;
    [SerializeField] private float smallestz;
    
    public void Awake()
    {
        //GetSmallestXYZvalues(StinkyString);
        ReadVerticesData(StinkyString);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;

        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i],0.1f);
        }
        
    }
    
    private void ReadVerticesData(string nameOfFile)
    {
        
        string filepath = Path.Combine(Application.streamingAssetsPath, nameOfFile);
        
        List<Vector3> mVertices = new List<Vector3>();
        
        if (File.Exists(filepath))
        {
            
            var tempText = File.ReadAllLines(filepath);

            CultureInfo cult = new CultureInfo(3); // tror ikke denne blir brukt
            
            if (tempText.Length > 0)
            {
                
                int howManyVertices = int.Parse(tempText[0]);
            
                mVertices.Capacity = howManyVertices;
                
                CultureInfo cultureInfo = new CultureInfo("en-US");

                const int skipAmount = 100000;
                
                ///
                ///
                ///
                
                List<float> tempX = new List<float>();
                List<float> tempY= new List<float>();
                List<float> tempZ= new List<float>();
                
                for (var i = 1; i <= howManyVertices; i += skipAmount)
                {
                    var iterator = tempText[i].Split(' '); 
                    
                    float parse0 = float.Parse(iterator[0], cultureInfo); //x
                    float parse1 = float.Parse(iterator[1], cultureInfo); //y
                    float parse2 = float.Parse(iterator[2], cultureInfo); //z
                    
                    // Y is now Z
                    
                    tempX.Add(parse0); //x
                    tempZ.Add(parse1); //z
                    tempY.Add(parse2); //y
                   

                }
                // Gives us the smallest values for x,y,z so that we can subtract it with the other intervals of vertices data (or point cloud data)
                smallestx = tempX.Min();
                smallesty = tempY.Min();
                smallestz = tempZ.Min();

                tempX = null;
                tempZ = null;
                tempY = null;
                
                /// 
                ///
                /// 
                
                
                
                
                
                for (var i = 1; i <= howManyVertices; i += skipAmount)
                {
                    var iterator = tempText[i].Split(' '); // splitter opp mellom mellomrommet
                    
                    float parse0 = float.Parse(iterator[0], cultureInfo);
                    float parse1 = float.Parse(iterator[1], cultureInfo);
                    float parse2 = float.Parse(iterator[2], cultureInfo);

                    var otherX = parse0 - smallestx;
                    var otherZ = parse1 - smallestz;
                    var otherY = parse2 - smallesty;
                    
                    //Change spot of y and z
                    var _newVertex = new Vector3(otherX, otherY, otherZ);
                    
                    mVertices.Add(_newVertex);
                }

                vertices = mVertices.ToArray();

            }
            
        }
        
        else
        {
            Debug.Log("Fant ikke fil");
        }
        

    }

    private void ReadTriangleData(string nameOfFile)
    {
        string filepath = Path.Combine(Application.streamingAssetsPath, nameOfFile);
        
        List<int> Triangles = new List<int>();
        
        if (File.Exists(filepath))
        {
            var tempText = File.ReadAllLines(filepath);
            
            if (tempText.Length > 0)
            {
                int howManyTriangles = int.Parse(tempText[0]); // 4
                
                Triangles.Capacity = howManyTriangles * 3;

                for (var i = 1; i <= howManyTriangles; i++)
                {
                    var iterator = tempText[i].Split(' '); // splitter opp mellom mellomrommet
                    
                    Triangles.Add(int.Parse(iterator[0]));
                    Triangles.Add(int.Parse(iterator[1]));
                    Triangles.Add(int.Parse(iterator[2]));
                }
                triangles = Triangles.ToArray();
            }
        }
        
        else
        {
            Debug.Log("Fant ikke fil");
        }


    }


    
}
