using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPower : MonoBehaviour
{
    
  [SerializeField]  private Vector2 previousPosition;

  [SerializeField]  public GameObject trekantReferanse;
 
  [SerializeField]  private Vector3 deltaPos = Vector3.zero;
  [SerializeField]  private Vector3 currentPos = Vector3.zero;
  [SerializeField]  private Vector3 currentVelocity = Vector3.zero;

  [SerializeField]  private int current_Index;
  [SerializeField]  private int previous_Index;
  
  [SerializeField]  private Vector3 currentNormal = Vector3.zero;  // n - normal-vektor
  [SerializeField]  private Vector3 previousNormal = Vector3.zero; // m - normal-vektor
   
   
   
   public CreateMap myTrekant;

   private Vector3 _prevPos;

   private float _mass = 0.004f;
   
    public void Start()
    {
        _prevPos = transform.position;
        
        
        
        
        
    }

    Vector2 p1, p2, p3 = new Vector2();
    
    private void FixedUpdate()
    {
        currentPos = transform.position;
        deltaPos = currentPos - _prevPos;

        move();
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
           
            
            Vector3 _tempBallPos = transform.position;   // Finn ballens posisjon i xy-planet
            Vector2 ballpos = new Vector2(_tempBallPos.x, _tempBallPos.z);
            
            Vector3 ballBarysentrisk = Barcentry(v0, v1, v2, ballpos); // Søk etter triangel som ballen er på nå med barysentriske koordinater
            
            // Debug.Log("x: "+ ballBarysentrisk.x.ToString() + "y: "+ ballBarysentrisk.y.ToString() + "z:"+ ballBarysentrisk.z.ToString());
            
            if (ballBarysentrisk.x >= 0 && ballBarysentrisk.y >= 0 && ballBarysentrisk.z >= 0) /* barysentriske koordinater mellom 0 og 1. "blir på en måte ""lokalt"" "*/
            {
                previousNormal  = Vector3.Cross((v1 - v0), (v2 - v0)); // Normalvektor i planet N = (_v1 - _v0) crossproduct (_v2 - _v0)
                
                // beregn akselasjonsvektor - ligning (8.12)
                var accelVector = new Vector3(
                    (previousNormal.x * previousNormal.z),
                    (previousNormal.y * previousNormal.y) - 1f,
                    (previousNormal.z * previousNormal.x) ) * Physics.gravity.y;

                // Update Velocity
                var prevVelocity = deltaPos * Time.fixedDeltaTime;
                currentVelocity = prevVelocity + accelVector * Time.fixedDeltaTime; // ligning (8.14)

                // Update Position
                currentPos = _prevPos + currentVelocity * Time.fixedDeltaTime; // ligning (8.15)
                
                if (current_Index != previous_Index)
                {
                    var xvec = (previousNormal + currentNormal).normalized;
                    //var _collisionPlane = normalVector + 

                }
                // STEG 2 if ( /* ny indeks != forrige */)
                // STEG 2 {
                // Ballen har rullet over på et nytt triangel, og beregner normalen til kollisjonsplanet, se ligning (8.17)
                // Korrigere posisjon oppover i normalens retning
                // Oppdater hastighetsvektoren, se ligning (8.16)
                // Oppdatere posisjon i retning den nye hastighets vektoren
            }
            previousNormal = currentNormal;
            previous_Index = current_Index;
            
            // Oppdater gammel normal og indeks
        }
    }
    
    private void Update()
    {
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
        
        
        
    }




    public Vector3 Barcentry(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 pt)
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