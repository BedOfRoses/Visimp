using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPower : MonoBehaviour
{
    
   private Vector2 previousPosition;

   public GameObject trekantReferanse;
    
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
        //do you believe in life after love?
        trekantReferanse.GetComponent<MeshCollider>();

    }


    public void barysentric(Vector3 bruh)
    {
        
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

    
    
    public void FixedUpdate()
    {
        /*
         * Hvert tiddsteg:
         * 1. hvilken trekant vi er på (referanse til trekant mesh)
         * 2. regne ut:
         * - akselerasjon
         * - hastighet/fart
         * - ny posisjon.
         */
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
    
}
