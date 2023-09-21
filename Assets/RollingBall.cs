using UnityEngine;

public class RollingBall : MonoBehaviour
{
    
    #region hastighet og vector 3 for hastighet og posisjon
    [SerializeField] private Vector2 ballpos = Vector2.zero;
    [SerializeField]  private Vector3 deltaPos = Vector3.zero;
    [SerializeField]  private Vector3 currentPosition = Vector3.zero;
    [SerializeField]  private Vector3 previousPosition = Vector3.zero;
    [SerializeField] private Vector3 _prevPos;
    [SerializeField]  private Vector3 currentVelocity = Vector3.zero;
    [SerializeField] private Vector3 previousVelocity = Vector3.zero;
    #endregion

    #region index for triangler
    [SerializeField]  private int current_Index;
    [SerializeField]  private int previous_Index;
    #endregion
  
    #region normalen til triangler
    [SerializeField]  private Vector3 currentNormal = Vector3.zero;  // n - normal-vektor
    [SerializeField]  private Vector3 previousNormal = Vector3.zero; // m - normal-vektor
    #endregion

    #region referanser
    public CreateMap myTrekant;
    #endregion

  
    #region Vertexer
    [SerializeField] private Vector3 vertex0 = Vector3.zero;
    [SerializeField] private Vector3 vertex1 = Vector3.zero;
    [SerializeField] private Vector3 vertex2 = Vector3.zero;
    #endregion
  
  
    [SerializeField] private Vector3 barysentricCoordinateToBall = Vector3.zero;
    [SerializeField] private Vector3 centerOfBall = Vector3.zero;
    private float _mass = 0.004f;


    [SerializeField] private Vector3 currentTriangle = Vector3.zero;
    [SerializeField] private Vector3 previousTriangle = Vector3.zero;

    [SerializeField] private Vector3 v0, v1, v2 = Vector3.zero;
    
    [SerializeField] private Vector3 accelerationVector = Vector3.zero;

    public void Move()
    {
        // Finne t r e k a n t
        for(var i = 0; i < myTrekant.mesh.triangles.Length; i+=3)
        {
            current_Index = i / 3;
            int index_0 = myTrekant.mesh.triangles[i];
            int index_1 = myTrekant.mesh.triangles[i+1];
            int index_2 = myTrekant.mesh.triangles[i+2];
            
            // Finn trekantens vertices v0 , v1 , v2
            v0 = new Vector3(myTrekant.mesh.vertices[index_0].x,myTrekant.mesh.vertices[index_0].y, myTrekant.mesh.vertices[index_0].z);
            v1 = new Vector3(myTrekant.mesh.vertices[index_1].x,myTrekant.mesh.vertices[index_1].y, myTrekant.mesh.vertices[index_1].z);
            v2 = new Vector3(myTrekant.mesh.vertices[index_2].x,myTrekant.mesh.vertices[index_2].y, myTrekant.mesh.vertices[index_2].z);

            // Finn ballensposisjon i xy=planet
            // Soek etter triangel som ballen er pa na
            // med barysentriske koordinater
            barysentricCoordinateToBall = Barycentric(new Vector2(v0.x, v0.z), new Vector2(v1.x, v1.z),
                new Vector2(v2.x, v2.z), new Vector2(transform.position.x, transform.position.z));
            
            if (barysentricCoordinateToBall is {x: >= 0, y: >= 0, z: >= 0})
            {
                // beregne normal
                currentNormal = Vector3.Cross(v1 - v0, v2 - v0);
                
                // beregn akselerasjons vektor = ligning 8.12
                accelerationVector = Physics.gravity.y * new Vector3(
                    currentNormal.x * currentNormal.z,
                    currentNormal.y * currentNormal.y - 1f,
                    currentNormal.z * currentNormal.y
                );

                currentVelocity = previousVelocity + accelerationVector * Time.fixedDeltaTime;

                currentPosition = previousPosition + previousVelocity * Time.fixedDeltaTime;

                transform.position += currentPosition; //TODO MÅ BRUKE DEN
                
                // Oppdaterer hastighet og posisjon
                // ligning ( 8 . 1 4 ) og ( 8 . 1 5 )
                
                if (current_Index != previous_Index)
                {
                // Ballen har rullet over pa nytt triangel
                // Beregner normalen til kollisjonsplanet,
                // se ligning ( 8 . 1 7 )
                // Korrigere posisjon oppover i normalens retning
                // Oppdater hastighetsvektoren , se ligning ( 8 . 1 6 )
                // Oppdatere posisjon i retning den nye
                // hastighets vektoren
                }
                
            // Oppdater gammel normal og indeks
            previousNormal = currentNormal;
            previous_Index = current_Index;
            }
        }
        
        
    }
    
    
// transform.position += new Vector3(0f, 0.01f, 0f);
    

    private void FixedUpdate()
    {
        
 
        
        Move();
        
        
        
        
        
        
    }
    
    
    public Vector3 Barycentric(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 pt)
    {
        Vector2 p12 = p2 - p1;
        Vector2 p13 = p3 - p1;
        Vector3 nn = Vector3.Cross(new Vector3(p12.x, 0, p12.y), new Vector3(p13.x, 0, p13.y));
        float areal_123 = nn.magnitude;
        // Debug.Log("areal: " + areal_123);
        Vector3 baryc;
   
        // u
        Vector2 q = p2 - pt;
        Vector2 p = p3 - pt;
        nn = Vector3.Cross(new Vector3(p.x,0,p.y), new Vector3(q.x,0,q.y));
        baryc.x = nn.y / areal_123;
   
        // v
        q = p3 - pt;
        p = p1 - pt;
        nn = Vector3.Cross(new Vector3(p.x,0,p.y), new Vector3(q.x,0,q.y));
        baryc.y = nn.y / areal_123;
    
        // w
        q = p1 - pt;
        p = p2 - pt;
        nn = Vector3.Cross(new Vector3(p.x,0,p.y), new Vector3(q.x,0,q.y));
        baryc.z = nn.y / areal_123;
    
        return baryc;
    }

    
}



