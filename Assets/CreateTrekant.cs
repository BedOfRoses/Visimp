using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class CreateTrekant : MonoBehaviour
{

    struct ExampleVertex
    {
        public Vector3 pos;
        public short normalX, normalY;
        public Color32 tanget;
    }
    
    // Start is called before the first frame update
    void Start()
    {

        var mesh = new Mesh();
        var layout = new[]
        {
            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
            new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float16, 2),
            new VertexAttributeDescriptor(VertexAttribute.Tangent, VertexAttributeFormat.UNorm8, 4)
        };

        var vertexCount = 10;
        mesh.SetVertexBufferParams(vertexCount,layout);

        // set vertex data
        var verts = new NativeArray<ExampleVertex>(vertexCount, Allocator.Temp);


        for (int i = 0; i < vertexCount; i++)
        {
            ExampleVertex vertex = new ExampleVertex();
            vertex.pos = new Vector3(i, 0, 0);
            vertex.normalX = 32767;
            vertex.normalY = 32767;
            vertex.tanget = new Color32(255, 0, 0, 255);

            verts[i] = vertex;
        }
        
        
        mesh.SetVertexBufferData(verts, 0,0, vertexCount);
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
