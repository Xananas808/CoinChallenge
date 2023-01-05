using UnityEngine;
public class CoinCtrl : MonoBehaviour, Ipickable
{
    public int coinValue;
    public void OnPickUp()
    {
        ScoreManager.Instance.score += coinValue;
        gameObject.SetActive(false);
    }
}