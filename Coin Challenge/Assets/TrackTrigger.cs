using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackTrigger : MonoBehaviour
{
    [SerializeField] Collider trigger;
    void Start()
    {
        
    }
    
    void OnTriggerEnter(Collider col)
    {
        if (!col.transform.root.CompareTag("Player")) return;
        Debug.Log("Player Int");
        TrackManager.instance.SpawnTrack();
        trigger.enabled = false;
    }
}
