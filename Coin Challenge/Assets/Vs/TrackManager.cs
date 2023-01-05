using System.Collections.Generic;
using UnityEngine;
public class TrackManager : MonoBehaviour
{   
    public static TrackManager instance;
    [SerializeField] List<GameObject> trackPrefabListe;
    [SerializeField] Track startTrack;
    [SerializeField] public Track activeTrack;
    [SerializeField] List<Track> tracksInstListe = new List<Track>();
    public Transform PlayerPos;
    Track lastTrack
    {
        get
        {
            return tracksInstListe[tracksInstListe.Count - 1];
        }
    }
    private void Awake()
    {
        instance= this;
    }
    void Start()
    {
        activeTrack = startTrack;
        startTrack.transform.position = transform.position;
        tracksInstListe.Add(startTrack);
    }
    void Update()
    {
        CheckActiveTrack();
    }
    public void SpawnTrack()
    {
        GameObject trackPrefab = GetRndTrack();
        GameObject trackInst = Instantiate(trackPrefab);
        Track trackInstCtrl = trackInst.GetComponent<Track>();
        trackInstCtrl.startConnexionPoint.SetParent(null);
        trackInst.transform.SetParent(trackInstCtrl.startConnexionPoint);
        trackInstCtrl.startConnexionPoint.position = lastTrack.endConnexionPoint.position;
        trackInstCtrl.startConnexionPoint.rotation = lastTrack.endConnexionPoint.rotation;
        tracksInstListe.Add(trackInstCtrl);
    }
    void CheckActiveTrack()
    {
        foreach(Track track in tracksInstListe)
        {
            float dist = Vector3.Distance(PlayerPos.position, track.playerCheckPoint.position);
            if (dist < 1.6f)
            {
                activeTrack = track;
                break;
            }
        }
    }
    private GameObject GetRndTrack()
    {
        int rndIndex = Random.Range(0, trackPrefabListe.Count);

        return trackPrefabListe[rndIndex];
    }
    public static void SetPlayerToLastCheckPoint()
    {
        GameManager.instance.playerMoveCtrl.transform.position = instance.activeTrack.playerCheckPoint.transform.position;
    }
}