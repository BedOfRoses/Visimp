using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class TriangleSurface : MonoBehaviour
{
    // private Vector3 gKraft = Physics.gravity; // dette tilsvarer g-vektor for -9.8m/s^2 
    //
    // public Vector3 barysentriccoord (Vector2 p1, Vector2 p2, Vector2 p3, Vector2 pt)
    // {
    //     Vector2 p12 = p2 - p1;
    //     Vector2 p13 = p3 - p1;
    //     Vector3 n = glm::vec3(glm::cross(glm::vec3(p12, 0.0f), glm::vec3(p13, 0.0f)));
    //     // Vector3 = 
    //     float areal_123 = math.length(n);        // double area
    //     
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

    

   

    private void Update()
    {
        
        
    }
}
