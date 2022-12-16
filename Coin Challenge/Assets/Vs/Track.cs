using UnityEngine;
public class Track : MonoBehaviour
{
    public Transform startConnexionPoint, endConnexionPoint, playerCheckPoint, killZone;
    void Start()
    {

    }
    void Update()
    {
        CheckKillZone();
    }
    void CheckKillZone()
    {
        if (GameManager.instance.playerMoveCtrl.foot.transform.position.y < killZone.transform.position.y)
        {

        }
    }
}
