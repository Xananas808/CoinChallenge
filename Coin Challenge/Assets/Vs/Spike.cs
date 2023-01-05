using UnityEngine;
public class Spike : MonoBehaviour
{
    [SerializeField] Transform parentSpike;

    [SerializeField] Collider spike;

    bool spikeUp
    {
        get
        {
            return parentSpike.transform.localPosition.y > -4;
        }
    }
    public void OnTriggerEnter(Collider col)
    {
        if (!col.transform.root.CompareTag("Player")) return;
        if (!spikeUp) return;

            TrackManager.SetPlayerToLastCheckPoint();
        
    }
}