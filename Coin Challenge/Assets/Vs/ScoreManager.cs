using TMPro;
using UnityEngine;
public class ScoreManager : MonoBehaviour
{
    public float timeDuration = 3f * 60f, timer;
    [SerializeField] private TextMeshProUGUI min1, min2, sep, sec1, sec2;
    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            UpdateTimerDisplay(timer);
        }
    }
    public void UpdateTimerDisplay(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60), seconds = Mathf.FloorToInt(time % 60);
        string currentTime = string.Format("{00:00}{1:00}", minutes, seconds);
        min1.text = currentTime[0].ToString();
        min2.text = currentTime[1].ToString();
        sec1.text = currentTime[2].ToString();
        sec2.text = currentTime[3].ToString();
    }
    private void ResetTimer()
    {
        timer = timeDuration;
    }
    private void Start()
    {
        ResetTimer();
    }
}