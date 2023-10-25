using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;

public class ReadMeshFile : MonoBehaviour
{
    public Vector3[] vertices;

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
                
                for (var i = 1; i <= howManyVertices; i++)
                {
                    var iterator = tempText[i].Split(' '); // splitter opp mellom mellomrommet
                    
                    
                    float parse0 = float.Parse(iterator[0], cultureInfo);
                    float parse1 = float.Parse(iterator[1], cultureInfo);
                    float parse2 = float.Parse(iterator[2], cultureInfo);

                    //Change spot of y and z
                    var _newVertex = new Vector3(parse0, parse2, parse1);
                    
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

}
