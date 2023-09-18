using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPower : MonoBehaviour
{
    
   private Vector2 previousPosition;

   public GameObject trekantReferanse;

   private Vector3 currentPos;
   private CreateMap myTrekant;
   
    /*
     *Newtons 2.lov
     * F = m * a
     */

    //Tyngdensakselerasjon
    public double g;
    float liten_m = 1;
  

    Vector3 fallGravity()
    {
        g = (9.81 * liten_m) / (Time.fixedDeltaTime) * (Time.fixedDeltaTime);
        return new Vector3(0,(float)g,0);
    }

    public void Start()
    {
        // trekantReferanse.GetComponent<MeshCollider>();

    }


    
    
   
   
    Vector2 p1, p2, p3 = new Vector2();
    

    private void FixedUpdate()
    {

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


        currentPos = Barcentry(myTrekant.mesh.vertices[0],
            myTrekant.mesh.vertices[2],
            myTrekant.mesh.vertices[3],
            previousPosition);
        
        // currentPos = Barcentry(
        //     Vector2(myTrekant.vertices[0].x, myTrekant.vertices[0].z),
        //     Vector2(myTrekant.vertices[1].x, myTrekant.vertices[1].z),
        //     Vector2(myTrekant.vertices[2].x, myTrekant.vertices[2].z),
        //     previousPosition
        // );
        currentPos = Barcentry(p1,p2 ,p3 , previousPosition);
        
        // Steg 2
        UpdateNormalVectorWithFloor();
        
        // Beregn askelerasjonensvektoren til kula etter ligning 8.12
    }


    Vector3 BallsAcceleration()
    {
        var ballaccel = new Vector3();
        // ballaccel = Physics.gravity * Vector3(1, 1, 1);
        // ballaccel = Vector3.Scale(Physics.gravity, Vector3(1f, 1f, 1f));
        
        return ballaccel;
    }

    private void UpdateNormalVectorWithFloor()
    {
        
    }



    void move()
    {
        // Vector3 v0, v1, v2;
        // for (int i = 0; i < myTrekant.triangles.Length; i+=3)
        // {
        //     v0 = new Vector3(myTrekant.vertices[i].x, myTrekant.vertices[i].y, myTrekant.vertices[i].z);
        //     
        // }
    }
    
    private void Update()
    {
        Vector2 position = new Vector2(transform.position.x, transform.position.y);
        
        //calculates the object's movement direction and speed between frames
        Vector2 speed = position - previousPosition;
        
        //used to determine the rotation axis for an object based on its movement direction.
        Vector2 rotationAxis = Vector2.Perpendicular(speed);
        
        //This determines the axis around which the object will rotate. rotating in the opposite direction of its movement
        transform.Rotate(new Vector3(rotationAxis.x, rotationAxis.y,0),-speed.magnitude * 35, Space.World);
        
        //line updates the previousPosition variable to match the current position. This is important for calculating the speed in the next frame.
        previousPosition = position;
        
        
        
    }




    public Vector3 Barcentry(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 pt)
    {
        // y and z change places idiot mathematica blyat
        //Remember that x and z is the plane, and y is the up and down direction. ( Y AND Z CHANGE PLACE)        

        
        Vector2 p12 = p2 - p1;
        Vector2 p13 = p3 - p1;
       // Vector3 n = glm::vec3(glm::cross(glm::vec3(p12, 0.0f), glm::vec3(p13, 0.0f)));
       Vector3 nn = Vector3.Cross(p12, p13);

        //float areal_123 = glm::length(n); // double area
        float areal_123 = nn.magnitude;
        Vector3 baryc;
       
        // u
        Vector2 p = p2 - pt;
        Vector2 q = p3 - pt;
        // n = glm::vec3(glm::cross(glm::vec3(p, 0.0f), glm::vec3(q, 0.0f)));
        nn = Vector3.Cross(p, q);
        baryc.x = nn.z / areal_123;
       
        // v
        p = p3 - pt;
        q = p1 - pt;
        // n = glm::vec3(glm::cross(glm::vec3(p, 0.0f), glm::vec3(q, 0.0f)));
        nn = Vector3.Cross(p, q);
        baryc.y = nn.z / areal_123;
        
        // w
        p = p1 - pt;
        q = p2 - pt;
        // n = glm::vec3(glm::cross(glm::vec3(p, 0.0f), glm::vec3(q, 0.0f)));
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