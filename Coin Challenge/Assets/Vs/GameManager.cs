using UnityEngine;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public MoveCtrl playerMoveCtrl;
    void Awake()
    {
        instance = this;
    }
}