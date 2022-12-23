using TMPro;
using UnityEngine;
public class ObjPickUp : MonoBehaviour, Ipickable 
{
    public float coin;
    [SerializeField] private TextMeshProUGUI coinTxt;
    private void Start()
    {
        ResetCoin();
    }
    void OnTriggerEnter(Collider Col)
    {
        Ipickable ipickable = Col.gameObject.GetComponent<Ipickable>();
        if (ipickable == null) return;
        ipickable.OnPickUp();
        /*if (Col.gameObject.tag == "Gold")
        {
            coin = coin + 15;
            Col.gameObject.SetActive(false);
            coinTxt.text = coin.ToString();
            Debug.Log("gold");
        }
        if (Col.gameObject.tag == "Silver")
        {
            coin = coin + 10;
            Col.gameObject.SetActive(false);
            coinTxt.text = coin.ToString();
            Debug.Log("silver");
        }
        if (Col.gameObject.tag == "Coper")
        {
            coin = coin + 5;
            Col.gameObject.SetActive(false);
            coinTxt.text = coin.ToString();
            Debug.Log("coper");
        }*/
    }
    private void ResetCoin()
    {
        coin = 0;
    }
    public void OnPickUp()
    {

    }
}