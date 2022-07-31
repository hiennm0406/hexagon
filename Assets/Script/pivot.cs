using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class pivot : MonoBehaviour
{
    public string value;




#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Handles.Label(transform.position, value);
    }
#endif
}
