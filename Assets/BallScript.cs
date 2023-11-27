using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
      #region Posisjon Spawning
    [SerializeField] private Vector2 spawnlocation = Vector2.zero;
    #endregion
    
  #region hastighet og vector 3 for hastighet og posisjon
  [SerializeField]  private Vector3 deltaPosition = Vector3.zero;
  [SerializeField]  private Vector3 currentPosition = Vector3.zero;
  [SerializeField] private Vector3 previousPosition = Vector3.zero;
  [SerializeField]  private Vector3 currentVelocity = Vector3.zero;
  [SerializeField] private Vector3 previousVelocity = Vector3.zero;
  [SerializeField] private Vector3 accelerationVector = Vector3.zero;
  [SerializeField] private Vector3 velocityCorrection = Vector3.zero;
  #endregion
  
  
  
  #region index for triangler
  [SerializeField]  private int current_Index;
  [SerializeField]  private int previous_Index;
  #endregion
  
  #region normalen til triangler
  [SerializeField]  private Vector3 currentNormal = Vector3.zero;  // n - normal-vektor
  [SerializeField]  private Vector3 previousNormal = Vector3.zero; // m - normal-vektor
  [SerializeField] private Vector3 collitionNormal = Vector3.zero;
  
  #endregion

  #region referanser
  public Delaunay myTrekant;
  #endregion
  
  #region Vertexer
  [SerializeField] private Vector3 vertex0 = Vector3.zero;
  [SerializeField] private Vector3 vertex1 = Vector3.zero;
  [SerializeField] private Vector3 vertex2 = Vector3.zero;
  #endregion
  
   [SerializeField] private Vector3 barysentricCoordinateToBall = Vector3.zero;
   [SerializeField] private Vector2 spawnPosition = Vector2.zero;
   [SerializeField] private float timeOfFirstTriangle = 0;
   [SerializeField] private float radius = 0.015f;
   [SerializeField] private Vector3 spawnPos3 = Vector3.zero;
   
   
   
   public void CollisionCorrection()
   {
    
    var k = new Vector3(currentPosition.x ,
        myTrekant.GetSurfaceHeight(new Vector2(currentPosition.x, currentPosition.z)), 
        currentPosition.z);

   
    if (!(currentPosition.y < k.y)) return;
    var _newPositionAfterCollition = k + radius * currentNormal;
    transform.position = _newPositionAfterCollition;

   }

   

   public void Start()
   {
       var boi = myTrekant.GetSurfaceHeight(new Vector2(spawnPosition.x, spawnPosition.y));
       Debug.Log("Height: " + boi);
       var newSpawnpos = new Vector3(spawnPosition.x, boi+radius, spawnPosition.y);
       currentPosition = newSpawnpos;
       transform.position = currentPosition;
       previousPosition = currentPosition;
       spawnPos3 = currentPosition;
   }

  

   private void FixedUpdate()
    {
        
        
        // move();
        // CollisionCorrection();

        deltaPosition = currentPosition - previousPosition;

    }
    
    void move()
    {
        // myTrekant.mesh.triangles.Length = 12
        for (int i = 0; i < myTrekant.mesh.triangles.Length;  i+=3 )
        {
            current_Index = i / 3; //Deler på tre siden vi itererer med i+=3
            Vector3 v0, v1, v2; 
            // Iterate through the vertex data
            int index_0 = myTrekant.mesh.triangles[i];
            int index_1 = myTrekant.mesh.triangles[i+1];
            int index_2 = myTrekant.mesh.triangles[i+2];
            
            v0 = myTrekant.mesh.vertices[index_0];
            v1 = myTrekant.mesh.vertices[index_1];
            v2 = myTrekant.mesh.vertices[index_2];
            
            vertex0 = new Vector3(v0.x,v0.y,v0.z);
            vertex1 = new Vector3(v1.x,v1.y,v1.z);
            vertex2 = new Vector3(v2.x, v2.y,v2.z);
           
            
            barysentricCoordinateToBall = BarycentricFunction(
                new Vector2(v0.x, v0.z), 
                new Vector2(v1.x, v1.z) ,
                new Vector2(v2.x, v2.z), 
                new Vector2(transform.position.x, transform.position.z));
            
            
            
            if (barysentricCoordinateToBall is{x:>= 0, y:>= 0, z:>= 0})
            {
                
                timeOfFirstTriangle += Time.fixedDeltaTime;
                
                // Normalvektor i planet N = (_v1 - _v0) crossproduct (_v2 - _v0)
                 currentNormal  = Vector3.Cross((v1 - v0), (v2 - v0)).normalized; 
               
                
                // beregn akselasjonsvektor - ligning (8.12)
                accelerationVector = new Vector3(
                    (currentNormal.x * currentNormal.y),
                    (currentNormal.y * currentNormal.y) - 1f,
                    (currentNormal.z * currentNormal.y) ) * -Physics.gravity.y;
                
            
                
                // Oppdaterer hastighet ligning ( 8 . 1 4 )
                // Update Velocity ( Vk+1 = Vk + a*deltatime )
                currentVelocity = previousVelocity + accelerationVector * Time.fixedDeltaTime; // ligning (8.14)
                previousVelocity = currentVelocity;
                // previousVelocity = deltaPos * Time.fixedDeltaTime;

                // Oppdaterer posisjon ligning  ( 8 . 1 5 )
                // Update Position ( Pk+1 = Pk + Vk*deltaTime )
                currentPosition = previousPosition + previousVelocity * Time.fixedDeltaTime; // ligning (8.15)
                previousPosition = currentPosition;
               
                
                this.transform.position = currentPosition;
                
                if (current_Index != previous_Index)
                { 
                    collitionNormal = (previousNormal + currentNormal).normalized;
                    
                    
                    //velocityCorrection = previousVelocity -
                    //                     2 * Vector3.Dot(previousVelocity, collitionNormal) * collitionNormal;
                    
                    // Collision ball to wall -  ligning (8.16)
                    velocityCorrection = previousVelocity -
                                         2 * Vector3.Project(previousVelocity, collitionNormal); 
                    
                    previousVelocity = velocityCorrection + accelerationVector * Time.fixedDeltaTime;
                    
                    // update pos
                    currentPosition = previousPosition + previousVelocity * Time.fixedDeltaTime; // ligning (8.15)
                    
                    
                    previousPosition = currentPosition;
                    transform.position = currentPosition;
                    
                    // CollisionCorrection();
                }
                
                previousNormal = currentNormal;
                previous_Index = current_Index;
            }

            
            
        }
    }


    // private void OnDrawGizmos()
    // {
    //     var position = transform.position;
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawRay(position, currentNormal);
    //     
    //     Gizmos.color = Color.green;
    //     Gizmos.DrawRay(position, currentVelocity);
    //
    //     Gizmos.color = Color.blue;
    //     Gizmos.DrawRay(position, collitionNormal);
    //     
    // }


    private static Vector3 BarycentricFunction(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 pt)
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
