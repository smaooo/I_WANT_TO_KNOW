using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MLHelper : MonoBehaviour
{
    [SerializeField]
    private GameObject wheelPrefab;

    [System.Serializable]
    public struct IslandWheels
    {
        public Transform island;
        public List<WheelAgent> wheels;
        public List<Transform> startPoints;
        public PlayerController player;
        public Transform destination;
        public void SetStartPoint(WheelAgent wheel)
        {

            int index = Random.Range(0, startPoints.Count);
            wheel.transform.position = startPoints[index].position;
            wheel.currentTrack = index;
        }

         
    }

    [SerializeField]
    private List<IslandWheels> islands;

    private void Awake()
    {
        
    }

    private void Start()
    {
        //Invoke("InstNewWheel", 2);
    }


    private void InstNewWheel()
    {
        var i = islands[Random.Range(0, islands.Count)];
        var s = i.startPoints[Random.Range(0, 6)];
        var w = Instantiate(wheelPrefab, s);
        var agent = w.GetComponent<WheelAgent>();
        i.wheels.Add(agent);
        agent.SetVars(s, i.island, this, i.startPoints.IndexOf(s), i.destination);
    }
    public List<WheelAgent> NeighborWheels(WheelAgent wheel, Transform island)
    {
        var wheels = islands.Find(i => i.island == island).wheels;

        wheels.Remove(wheel);

        return wheels;
    }

    public void SetWheelStartPoint(WheelAgent wheel, Transform island)
    {
        islands.Find(i => i.island == island).SetStartPoint(wheel);
    }
    public int NumberOfTracksInIsland(Transform island)
    {
        return islands.Find(i => i.island == island).startPoints.Count;
    }

    public float DistanceBetweenTwoTracks(Transform island, int from, int to)
    {
        var i = islands.Find(i => i.island == island).startPoints;
        return Mathf.Abs(i[to].position.x - i[from].position.x);
    }

    public int TrackLeftRight(Transform island, int from, int to)
    {
        var i = islands.Find(i => i.island == island).startPoints;
        var a = Vector3.Angle(i[from].position, i[to].position);
        return a > 0 ? 1 : -1;
    }

    public PlayerController IslandPlayer(Transform island)
    {
        return islands.Find(i => i.island == island).player;
    }

    public Vector3 GetTrackLocation(Transform island, int index)
    {
        var i = islands.Find(i => i.island == island).startPoints;
        return i[index].position;
    }
}
