using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class BallPower : MonoBehaviour
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
   [SerializeField] private Vector2 spawnPosition = Vector2.zero;

   public void CollisionCorrection()
   {
       var k = barysentricCoordinateToBall; // baryCoordForBall
       var center = transform.position;
       var dVec = k - center;
       var s = center - currentPosition;
       var bVec = currentNormal * Vector3.Dot(dVec, k);
       
   }

   

   public void Start()
   { 
       
       // transform.position = new Vector3(0, 0.097f, 0);

     // transform.position = new Vector3(
     //     spawnPosition.x, 
     //     myTrekant.GetSurfaceHeight(new Vector2(spawnPosition.x,spawnPosition.y)), 
     //     spawnPosition.y);
     
       var boi = myTrekant.GetSurfaceHeight(new Vector2(spawnPosition.x, spawnPosition.y));
       Debug.Log("Height: " + boi);

       var newSpawnpos = new Vector3(spawnPosition.x, boi, spawnPosition.y);

       currentPosition = newSpawnpos;

       transform.position = currentPosition;
       
       previousPosition = currentPosition;



       // transform.position = new Vector3(spawnlocation.x, 0, spawnlocation.y);
       // currentPosition = transform.position;
   }

  

   private void FixedUpdate()
    {
        
        
        move();
        //CollisionCorrection();

        deltaPosition = currentPosition - previousPosition;

    }
    
    void move()
    {
        // myTrekant.mesh.triangles.Length = 12
        for (int i = 0; i < myTrekant.mesh.triangles.Length;  i+=3 )
        {
            current_Index = i / 3;
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
                new Vector2(v1.x,v1.z) ,
                new Vector2(v2.x,v2.z), 
                new Vector2(transform.position.x, transform.position.z));
            
            
            
            if (barysentricCoordinateToBall is{x:>= 0, y:>= 0, z:>= 0})
            {
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
                

                // STEG 2 if (current_Index != previous_Index)
                // STEG 2 {
                // STEG 2     // Ballen har rullet over på et nytt triangel, og beregner normalen til kollisjonsplanet, se ligning (8.17)
                // STEG 2     // Korrigere posisjon oppover i normalens retning
                // STEG 2     // Oppdater hastighetsvektoren, se ligning (8.16)
                // STEG 2     // Oppdatere posisjon i retning den nye hastighets vektoren
                // STEG 2     var xvec = (previousNormal + currentNormal).normalized;
                // STEG 2     //var _collisionPlane = normalVector + 
                // STEG 2     CollisionCorrection();
                // STEG 2 }

            }

            //  previousPosition = currentPosition;
            //  previousVelocity = currentVelocity;
            previousNormal = currentNormal;
            previous_Index = current_Index;
            
            // Oppdater gammel normal og indeks
        }
    }


    private void OnDrawGizmos()
    {
        var position = transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(position, currentNormal);
        
        Gizmos.color = Color.green;
        Gizmos.DrawRay(position, currentVelocity);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(position, accelerationVector);

        // Gizmos.draw

    }


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


// Debug.Log("x: "+ ballBarysentrisk.x.ToString() + "y: "+ ballBarysentrisk.y.ToString() + "z:"+ ballBarysentrisk.z.ToString());



// private void Update()
// {
    // Thomas DS kode
    //  Vector2 position = new Vector2(transform.position.x, transform.position.y);
    //  
    //  //calculates the object's movement direction and speed between frames
    //  Vector2 speed = position - previousPosition;
    //  
    //  //used to determine the rotation axis for an object based on its movement direction.
    //  Vector2 rotationAxis = Vector2.Perpendicular(speed);
    //  
    //  //This determines the axis around which the object will rotate. rotating in the opposite direction of its movement
    //  transform.Rotate(new Vector3(rotationAxis.x, rotationAxis.y,0),-speed.magnitude * 35, Space.World);
    //  
    //  //line updates the previousPosition variable to match the current position. This is important for calculating the speed in the next frame.
    //  previousPosition = position;
// }













































// Vector3 barycentricCoordinates(const Vector2 p1, const Vector2 p2, const Vector2 p3, const Vector2 pt)
// {
//     Vector2 p12 = p2 - p1;
//     Vector2 p13 = p3 - p1;
//     Vector3 n = glm::vec3(glm::cross(glm::vec3(p12, 0.0f), glm::vec3(p13, 0.0f)));
//     float areal_123 = glm::length(n); // double area
//     Vector3 baryc;
//     // u
//     Vector2 p = p2 - pt;
//     Vector2 q = p3 - pt;
//     n = glm::vec3(glm::cross(glm::vec3(p, 0.0f), glm::vec3(q, 0.0f)));
//     baryc.x = n.z / areal_123;
//     // v
//     p = p3 - pt;
//     q = p1 - pt;
//     n = glm::vec3(glm::cross(glm::vec3(p, 0.0f), glm::vec3(q, 0.0f)));
//     baryc.y = n.z / areal_123;
//     // w
//     p = p1 - pt;
//     q = p2 - pt;
//     n = glm::vec3(glm::cross(glm::vec3(p, 0.0f), glm::vec3(q, 0.0f)));
//     baryc.z = n.z / areal_123;
//
//     return baryc;
// }

    
    
// public void FixedUpdate()
// {
//     /*
//      * Hvert tiddsteg:
//      * 1. hvilken trekant vi er på (referanse til trekant mesh)
//      * 2. regne ut:
//      * - akselerasjon
//      * - hastighet/fart
//      * - ny posisjon.
//      */
// }