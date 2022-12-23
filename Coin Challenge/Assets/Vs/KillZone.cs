using UnityEngine;

public class KillZone : MonoBehaviour
{
    [SerializeField] GameObject killZone;
    void Update()
    {

    }
    public void OnTriggerEnter(Collider Col)
    {
        if (Col.gameObject.tag == "Kill")
        {
            Debug.Log("tombé");
      
        }
    }
}
