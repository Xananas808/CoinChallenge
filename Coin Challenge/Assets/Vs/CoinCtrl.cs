using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCtrl : MonoBehaviour, Ipickable
{
    public int coinValue;
    public void OnPickUp()
    {
        Debug.Log("Piece collect�" + coinValue);
        ScoreManager.Instance.score += coinValue;
        gameObject.SetActive(false);
    }
}
