using UnityEngine;
public class TrackTrigger : MonoBehaviour
{
    [SerializeField] GameObject porte;
    [SerializeField] Collider trigger; 
    void OnTriggerEnter(Collider col)
    {
        if (!col.transform.root.CompareTag("Player")) return;
        porte.SetActive(true);
        gameObject.SetActive(false);
        TrackManager.instance.SpawnTrack();
    }
}