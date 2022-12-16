using TMPro;
using UnityEngine;
public class Coin : MonoBehaviour
{
    public float coin;
    [SerializeField] private TextMeshProUGUI coinTxt;
    private void Start()
    {
        ResetCoin();
    }
    public void OnTriggerEnter(Collider Col)
    {
        if (Col.gameObject.tag == "Gold")
        {
            coin = coin + 15;
            Col.gameObject.SetActive(false);
            coinTxt.text = coin.ToString();
        }
        if (Col.gameObject.tag == "Silver")
        {
            coin = coin + 10;
            Col.gameObject.SetActive(false);
            coinTxt.text = coin.ToString();
        }
        if (Col.gameObject.tag == "Coper")
        {
            coin = coin + 5;
            Col.gameObject.SetActive(false);
            coinTxt.text = coin.ToString();
        }
    }
    private void ResetCoin()
    {
        coin = 0;
    }
}