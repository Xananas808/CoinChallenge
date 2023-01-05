using TMPro;
using UnityEngine;
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public int score;
    public float timeDuration, remaningTime;
    public float elapsedTime { get {return timeDuration - remaningTime;} }
    [SerializeField] public TextMeshProUGUI min1, min2, sep, sec1, sec2;
    void Update()
    {
        if (remaningTime > 0)
        {
            remaningTime -= Time.deltaTime;
            UpdateTimerDisplay(remaningTime);
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
    public static void AddTime(float timeToAdd)
    {
        Instance.remaningTime+= timeToAdd;
        if (Instance.remaningTime > Instance.timeDuration) Instance.remaningTime= Instance.timeDuration;
    }
    public static void ReverseTime()
    {
        Instance.remaningTime = Instance.elapsedTime;
    }
    private void ResetTimer()
    {
        remaningTime = timeDuration;
    }
    void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        ResetTimer();
    }

}