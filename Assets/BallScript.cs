using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
     
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


   private void Awake()
   {
       radius = transform.localScale.x / 2;
       Debug.Log("Radius" + radius + "GameObj: "+ gameObject.name.ToString());
   }

   public void Start()
   {
       var height = myTrekant.GetSurfaceHeight(new Vector2(transform.position.x, transform.position.z));
       Debug.Log("Height: " + height);
       var newSpawnpos = new Vector3(transform.position.x, height + radius, transform.position.z);
       currentPosition = newSpawnpos;
       transform.position = currentPosition;
       previousPosition = currentPosition;
       spawnPos3 = currentPosition;
   }

  

   private void FixedUpdate()
    {
        Move();
    }
   
    void Move()
    {
        int triangleLength = myTrekant.triangles.Length;
        
        for (int i = 0; i < triangleLength;  i+=3 )
        { 
            current_Index = i / 3;

           Vector3 v0 = myTrekant.vertices[myTrekant.triangles[i]];
           Vector3 v1 = myTrekant.vertices[myTrekant.triangles[i+1]];
           Vector3 v2 = myTrekant.vertices[myTrekant.triangles[i+2]];
           
           barysentricCoordinateToBall = BarycentricFunction(
               new Vector2(v0.x, v0.z), 
               new Vector2(v1.x, v1.z) ,
               new Vector2(v2.x, v2.z), 
               new Vector2(transform.position.x, transform.position.z));
           

            if (barysentricCoordinateToBall is{x:>= 0, y:>= 0, z:>= 0})
            {
                currentNormal  = CalculateCurrentNormal(v0,v1,v2); 
                // timeOfFirstTriangle += Time.fixedDeltaTime;

                accelerationVector = CalculateAccelerationVector(currentNormal);
                
                currentVelocity = CalculateCurrentVelocity();
                
                previousVelocity = currentVelocity;

                currentPosition = CalculateCurrentPosition();
                
                previousPosition = currentPosition;
                
                this.transform.position = currentPosition;
                
                if (current_Index != previous_Index)
                { 
                    collitionNormal = CalculateCollitionNormal();
                    
                    velocityCorrection = CalculateVelocityCorrection(); 
                    
                    previousVelocity = velocityCorrection + accelerationVector * Time.fixedDeltaTime;

                    currentPosition = CalculateCurrentPosition();
                  
                    previousPosition = currentPosition;
                    transform.position = currentPosition;
                
                }
                
                previousNormal = currentNormal;
                previous_Index = current_Index;
            }

            
            
        }

    }

    Vector3 CalculateCurrentNormal(Vector3 vv0,Vector3 vv1, Vector3 vv2)
    {
        // Ligning fra 8.6 Algoritme punkt 2.
        return (Vector3.Cross((vv1 - vv0), (vv2 - vv0)).normalized);
    }
    Vector3 CalculateVelocityCorrection()
    {
        // Ligning (8.16)
        return (previousVelocity -
                2 * Vector3.Project(previousVelocity, collitionNormal));
    }
    Vector3 CalculateCollitionNormal()
    {
        // Ligning (8.17)
        return ((previousNormal + currentNormal).normalized);
    }
    Vector3 CalculateCurrentPosition()
    {
        // Ligning (8.15)
       return (previousPosition + previousVelocity * Time.fixedDeltaTime);
    }
    Vector3 CalculateCurrentVelocity()
    {
        // Ligning (8.14)
        return (previousVelocity + accelerationVector * Time.fixedDeltaTime);
    }
    Vector3 CalculateAccelerationVector(Vector3 NormalVector)
    {
        // Ligning (8.12)
        return new Vector3(
            (NormalVector.x * NormalVector.y),
            (NormalVector.y * NormalVector.y) - 1f,
            (NormalVector.z * NormalVector.y) ) * -Physics.gravity.y;
    }
    
   
    private static Vector3 BarycentricFunction(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 pt)
    {
        // source/inspiration https://github.com/haldorj/3Dprog22/ from when we did the 3DProg project.
        
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
