using UnityEngine;
public class HourReverseCtrl : MonoBehaviour, Ipickable
{
    public void OnPickUp()
    {
        ScoreManager.ReverseTime();
        gameObject.SetActive(false);
    }
}