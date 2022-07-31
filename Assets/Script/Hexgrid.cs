using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Hexgrid : MonoBehaviour
{
    public int X; 
    public int Y;
    public int Z;

    public HexNumber owner;






#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Handles.Label(transform.position, X + " " + Y + " "+ Z);
    }
#endif
}

