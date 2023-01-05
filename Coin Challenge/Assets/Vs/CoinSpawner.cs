using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [SerializeField] GameObject coinPrefab;
    [SerializeField] float arcRadius, heightOffSet;
    [SerializeField] int coinCount;
    void Start()
    {
        SpawnCoinArc();
    }
    void SpawnCoinArc()
    {
        float angleStep = 180f / (coinCount -1);
        for (int i = 0; i < coinCount; i++)
        {
            float angle = (i * angleStep) - 90;
            Vector3 dir = DirectionFromAngleXY(angle);
            Vector3 localPos = dir * arcRadius;
            localPos.y += heightOffSet;

            GameObject instance = Instantiate(coinPrefab);
            instance.transform.SetParent(transform);
            instance.transform.localPosition = localPos;
        }
    }
    public static Vector3 DirectionFromAngleXY(float _angleInDegrees)
    {
        return new Vector3(Mathf.Sin(_angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(_angleInDegrees * Mathf.Deg2Rad), 0);
    }
}
