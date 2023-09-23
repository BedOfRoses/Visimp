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

    private void Awake()
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
            new Vector3(0     ,0.097f  ,0   ),     // 0
            new Vector3(0.4f  ,0.005f  ,0   ),     // 1
            new Vector3(0     ,0.005f  ,0.4f),     // 2
            new Vector3(0.4f  ,0.075f  ,0.4f),     // 3
            new Vector3(0.8f  ,0.007f  ,0.4f),     // 4
            new Vector3(0.8f  ,0.039f  ,0   )      // 5
        };

        triangles = new int[]
        {
            2,1,0,    
          3,1,2,    
           1,3,4,    
          // 4,5,1 
        };
    }
    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }
    
    public float GetSurfaceHeight(Vector2 point)
    {
        // source https://github.com/haldorj/3Dprog22/blob/main/triangulation.cpp
        
        Debug.Log("triangles.Length: " + triangles.Length);
        Debug.Log("mesh.triangles.Length: " + mesh.triangles.Length);
        
        // for (var i = 0; i < triangles.Length; i++)
        for (var i = 0; i < mesh.triangles.Length; i++)
        {
            Vector3 v0, v1, v2 = default;
            
            var index_0 = triangles[i];
            var index_1 = triangles[i+1];
            var index_2 = triangles[i+2];
            
            v0 = vertices[index_0];
            v1 = vertices[index_1];
            v2 = vertices[index_2];

            //var baryCords = GetComponent<BallPower>().Barcentry(v0, v1, v2, veccc);

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


