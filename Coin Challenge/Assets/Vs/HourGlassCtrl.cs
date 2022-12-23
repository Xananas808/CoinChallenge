using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HourGlassCtrl : MonoBehaviour, Ipickable
{
    public void OnPickUp()
    {
        Debug.Log("sablier collecté");
    }
}
