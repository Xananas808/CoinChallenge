using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayGizmo : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
