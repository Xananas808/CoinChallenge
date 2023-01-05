using UnityEngine;
public class HourGlassCtrl : MonoBehaviour, Ipickable
{
    public void OnPickUp()
    {
        ScoreManager.AddTime(20);
        gameObject.SetActive(false);
    }
}