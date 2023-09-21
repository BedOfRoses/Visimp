using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingBall : MonoBehaviour
{
    
    #region hastighet og vector 3 for hastighet og posisjon
    [SerializeField] private Vector2 ballpos = Vector2.zero;
    [SerializeField]  private Vector3 deltaPos = Vector3.zero;
    [SerializeField]  private Vector3 currentPos = Vector3.zero;
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



    public void Move()
    {
        
    }
    
    
    

    private void FixedUpdate()
    {
        
        
        
        
        
        
        
        
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



