using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public MoveCtrl playerMoveCtrl;
    void Start()
    {
        
    }
    void Update()
    {
        
    }
    void Awake()
    {
        instance = this;
    }

}
