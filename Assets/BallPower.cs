using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPower : MonoBehaviour
{
    
   private Vector2 previousPosition;

   public GameObject trekantReferanse;
   
   private Vector3 deltaPos = Vector3.zero;
   private Vector3 currentPos = Vector3.zero;
   private Vector3 currentVelocity = Vector3.zero;
   
   public CreateMap myTrekant;

   private Vector3 _prevPos;
   
    public void Start()
    {
        _prevPos = transform.position;
        // myTrekant = GetComponent<mytrekant>()
        // trekantReferanse.GetComponent<MeshCollider>();
    }

    Vector2 p1, p2, p3 = new Vector2();
    
    private void FixedUpdate()
    {
        currentPos = transform.position;
        deltaPos = currentPos - _prevPos;
        
        
        
        
        /*  Algoritme, som skal skje i hvert tidssteg
         *
         * 1. Identifiser hvilken trekant ballen er på (med barysentriske koordinater)
         *
         * 2. Beregn normalvektoren i kontaktpunktet med underlaget:
         *  N = v0v1 x v0v2 = (v1 - v0) x (v2 - v0)
         *
         * 3. Beregn akselerasjonsvektoren til kula etter ligning 8.12
         *
         * 4. Oppdater ballens hastighet (ligning (8.14))
         * 5. Oppdater ballens posisjon (ligning(8.15))
         *
         * ---- Tror dette er valgfritt? ----
         * 6.(Beregn ballens rotasjonsvektor)
         * 7.(Beregn ballens rotasjon)
         *
         */
        
        // Steg 1
        // Barcentry(myTrekant.)


        // this.transform.position += Physics.gravity; // this simply adds the force of gravity
        
        move(Time.fixedDeltaTime);
        
        // Beregn askelerasjonensvektoren til kula etter ligning 8.12
    }
    
    void move(float deltatime)
    {
        /*myTrekant.triangles.Length;*/ 
        /* indekser til flaten * /*/
        
        // myTrekant.mesh.triangles.Length = 12
        for (int i = 0; i < myTrekant.mesh.triangles.Length;  i+=3 )
        {
            Vector3 v0, v1, v2; // Finn trekantens vertices v0, v1, v2
            
            // Iterate through the 
            int index_0 = myTrekant.mesh.triangles[i];
            int index_1 = myTrekant.mesh.triangles[i+1];
            int index_2 = myTrekant.mesh.triangles[i+2];
            
            v0 = myTrekant.mesh.vertices[index_0];
            v1 = myTrekant.mesh.vertices[index_1];
            v2 = myTrekant.mesh.vertices[index_2];
            
            Vector3 _tempBallPos = transform.position;   // Finn ballens posisjon i xy-planet
            Vector2 ballpos = new Vector2(_tempBallPos.x, _tempBallPos.z);
            
            // Søk etter triangel som ballen er på nå med barysentriske koordinater
            Vector3 ballBarysentrisk = Barcentry(v0, v1, v2, ballpos); 

            // Debug.Log("x: "+ ballBarysentrisk.x.ToString() + "y: "+ ballBarysentrisk.y.ToString() + "z:"+ ballBarysentrisk.z.ToString());
            
            /* barysentriske koordinater mellom 0 og 1. "blir på en måte ""lokalt"" "*/
            if (ballBarysentrisk.x >= 0 && ballBarysentrisk.y >= 0 && ballBarysentrisk.z >= 0)
            {
                // N = (_v1 - _v0) crossproduct (_v2 - _v0) // Formel for normalvektor i planet.
                Vector3 normalVector = Vector3.Cross((v1 - v0), (v2 - v0)); // Normalvektor i planet

                Vector3 _NormalAccelVector = new Vector3(
                    (normalVector.x * normalVector.z),
                    (normalVector.y * normalVector.y) - 1f,
                    (normalVector.z * normalVector.x)
                    );
                // beregn akselasjonsvektor - ligning (8.12)
                Vector3 _newNormalAccelVector = Physics.gravity.y * _NormalAccelVector;

                // Update Velocity
                var prevVelocity = deltaPos * Time.fixedDeltaTime;
                currentVelocity = prevVelocity + _newNormalAccelVector * Time.fixedDeltaTime; // ligning (8.14)

                // Update Position
                _prevPos = currentPos + currentVelocity * Time.fixedDeltaTime; // ligning (8.15)


                var currentIndex = i;
                if (currentIndex != i / 3)
                {
                    /*
                     * xvec = normalvektor til planet. Da er
                     *      
                     * ->   ->   ->
                     * x =   m + n
                     *      -------
                     *      ->   ->
                     *      |m + n|
                     */
                    //var _collisionPlane = 
                    
                }
                // STEG 2 if ( /* ny indeks != forrige */)
                // STEG 2 {
                // STEG 2     // Ballen har rullet over på et nytt triangel 
                // STEG 2     // beregner normalen til kollisjonsplanet,
                // STEG 2     // se ligning (8.17)
                // STEG 2     
                // STEG 2     // Korrigere posisjon oppover i normalens retning
                // STEG 2     
                // STEG 2     // Oppdater hastighetsvektoren, se ligning (8.16)
                // STEG 2     
                // STEG 2     // Oppdatere posisjon i retning den nye 
                // STEG 2     // hastighets vektoren
                // STEG 2 }
                // STEG 2     // Oppdater gammel normal og indeks

            }
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
       Vector3 nn = Vector3.Cross(p12, p13);

        float areal_123 = nn.magnitude;
        Vector3 baryc;
       
        // u
        Vector2 p = p2 - pt;
        Vector2 q = p3 - pt;
        nn = Vector3.Cross(p, q);
        baryc.x = nn.z / areal_123;
       
        // v
        p = p3 - pt;
        q = p1 - pt;
        nn = Vector3.Cross(p, q);
        baryc.y = nn.z / areal_123;
        
        // w
        p = p1 - pt;
        q = p2 - pt;
        nn = Vector3.Cross(p, q);
        baryc.z = nn.z / areal_123;

        var temp = baryc;
        baryc = new Vector3(baryc.x, baryc.z, baryc.y);
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