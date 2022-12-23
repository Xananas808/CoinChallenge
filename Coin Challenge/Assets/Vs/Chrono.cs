using UnityEngine;
public class Chrono : MonoBehaviour
{
   void OnTriggerEnter(Collider Col)
   {
       if (Col.gameObject.tag == "Chrono")
       {
         Col.gameObject.SetActive(false);
            
         Debug.Log("chrono");
       }
   }
}
