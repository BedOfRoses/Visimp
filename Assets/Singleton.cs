using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton : MonoBehaviour
{
    //static betyr at den blir allokert 1 gang. De kommer heller ikke opp i IDE når vi programmerer
    
    public static Singleton Instance;

    public int diff;

    private void Awake()
    {
        Instance = this;
    }

    public int GetDiff()
    {
        return diff;
    }


}



public static class Stinkelton
{

}