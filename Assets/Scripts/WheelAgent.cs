using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System.Linq;

public class WheelAgent : Agent
{
    [SerializeField]
    private float moveSpeed = 2f;
    [SerializeField]
    private bool trainingMode;
    private int originalTrack;
    new private Rigidbody rigidbody;
    private bool canFall = false;
    private PlayerController player;


    private List<float> pathTrackX;

    private List<WheelAgent> neighborWheels;


    private bool frozen = false;

    [SerializeField]
    public Transform startPoint;

    [SerializeField]
    public Transform island;

    [SerializeField]
    private MLHelper helper;

    public int currentTrack;

    public bool fallen = false;

    private float boxSize;

    private float currentDistance;

    [SerializeField]
    private Transform destination;
    public override void Initialize()
    {
        //base.Initialize();
        currentDistance = Vector3.Distance(this.transform.position, destination.position);
        neighborWheels = new List<WheelAgent>(helper.NeighborWheels(this, island));
        //helper.SetWheelStartPoint(this, island);
        originalTrack = currentTrack;
        rigidbody = this.GetComponent<Rigidbody>();

        player = helper.IslandPlayer(island);


        boxSize = this.GetComponent<BoxCollider>().size.x;

        if (!trainingMode)
            MaxStep = 0;
       
    }

    public override void OnEpisodeBegin()
    {
        //base.OnEpisodeBegin();

        //if (trainingMode)
        //{

        //}
        this.transform.position = helper.GetTrackLocation(island, originalTrack);
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;

        //helper.SetWheelStartPoint(this, island);
    }

    
    public void SetVars(Transform startPoint, Transform island, MLHelper helper, int currentTrack, Transform destination)
    {
        this.startPoint = startPoint;
        this.island = island;
        this.helper = helper;
        this.currentTrack = currentTrack;
        this.destination = destination;
    }
    public override void OnActionReceived(float[] vectorAction)
    {
        // 0: +1 move forward
        // 1: +1 jump
        // 2: index of track
        // 3: fall
        //base.OnActionReceived(vectorAction);


        if (frozen)
            return;

        Vector3 moveVector = Vector3.zero;

        if (vectorAction[1] > 0 && !fallen)
        {
            foreach (var w in neighborWheels)
            {
                float dist = this.transform.position.z - w.transform.position.z;
                if (w.fallen && w.currentTrack == currentTrack && dist > 0 && dist < boxSize)
                {
                    moveVector = this.transform.forward * boxSize + this.transform.up * boxSize;
                    break;
                }

                if (neighborWheels.IndexOf(w) == neighborWheels.Count - 1)
                {
                    AddReward(-0.2f);
                }
            }
            
        }

        if ((int)vectorAction[2] != currentTrack && !fallen)
        {
            int track = (int)vectorAction[2];
            //moveVector = this.transform.right * helper.DistanceBetweenTwoTracks(island, currentTrack, (int)vectorAction[2]) * helper.TrackLeftRight(island, currentTrack, (int)vectorAction[2]);
            //this.transform.position = Vector3.MoveTowards(this.transform.position, new Vector3(helper.GetTrackLocation(island, track).x, this.transform.position.y, this.transform.position.z),0.1f);
            StartCoroutine(MoveToTrack(track));
            currentTrack = (int)vectorAction[2];
        }
        if (vectorAction[0] > 0 && !fallen)
        {
            moveVector = this.transform.forward * 0.5f;
        }

        if (vectorAction[3] > 0 & !fallen && canFall)
        {
            fallen = true;
            AddReward(-0.5f);
            StartCoroutine(FallDown());
        }
        if (!fallen)
        {
            rigidbody.AddForce(moveVector, ForceMode.Impulse);

        }
        

    }

    private IEnumerator MoveToTrack(int track)
    {
        Vector3 trackLoc = helper.GetTrackLocation(island, track);

        while (this.transform.position.x != trackLoc.x)
        {
            yield return new WaitForEndOfFrame();
            this.transform.position = Vector3.MoveTowards(this.transform.position, new Vector3(trackLoc.x, this.transform.position.y, this.transform.position.z), 0.1f);
        }
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        //base.CollectObservations(sensor);

        sensor.AddObservation(player.transform.position.x);

        sensor.AddObservation(player.transform.position.z);

        foreach (var w in neighborWheels)
        {
            sensor.AddObservation(w.transform.position);
            //sensor.AddObservation(w.fallen);

        }
        sensor.AddObservation(fallen);
    }

    public override void Heuristic(float[] actionsOut)
    {
        //base.Heuristic(actionsOut);
        
        float forward = 0;
        float up = 0;
        //float right = 0;
        int index = currentTrack;

        if (Input.GetKey(KeyCode.W))
        {
            
            forward = 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            if (index > 0)
                index--;

            

        }

        if (Input.GetKey(KeyCode.A))
        {
            if (index < 5)
            {
                print("ASDSA");
                index++;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {

            up = 1;

        }

        

        actionsOut[0] = forward;
        actionsOut[1] = up;
        actionsOut[2] = index;

    }

    public void FreezeAgent()
    {
        Debug.Assert(trainingMode == false, "Freeze/Unforze not supported in training");

        frozen = true;

        rigidbody.Sleep();
    }

    public void UnFreezeAgent()
    {
        Debug.Assert(trainingMode == false, "Freeze/Unforze not supported in training");
        frozen = fallen;

        rigidbody.WakeUp();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (trainingMode && collision.collider.CompareTag("Player"))
        {
            AddReward(-0.5f);
        }

        else if (trainingMode && collision.collider.CompareTag("Wheel"))
        {
            AddReward(-0.2f);
        }
    }

    private IEnumerator FallDown()
    {
        Quaternion rotation = Quaternion.Euler(0, 90, 90);
        while (this.transform.rotation != rotation)
        {
            yield return new WaitForEndOfFrame();
            this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, rotation, 5);
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        AddReward(-0.2f);
    }

    private void OnCollisionExit(Collision collision)
    {
        AddReward(1f);
    }
    private void FixedUpdate()
    {
        //AddReward(0.001f * Time.fixedDeltaTime);
        //if (Vector3.Distance(player.transform.position, this.transform.position) < boxSize)
        //{
        //    var possibleMoves = new List<int>();
        //    foreach (var w in neighborWheels)
        //    {
        //        if (Mathf.Abs(this.transform.position.z - w.transform.position.z) > boxSize)
        //        {
        //            possibleMoves.Add(w.currentTrack);
        //        }
        //    }

        //    possibleMoves.OrderBy(x => Mathf.Abs(x - currentTrack));

        //}

        if (Mathf.Abs(this.transform.position.z - destination.position.z) < 2)
        {
            AddReward(2.0f);
            this.transform.position = helper.GetTrackLocation(island, originalTrack);
        }

        if (Mathf.Abs(this.transform.position.x - helper.GetTrackLocation(island, currentTrack).x) > 0.1f )
        {
            AddReward(-0.2f);
        }

        if (Mathf.Abs(this.transform.position.z - destination.position.z) < currentDistance)
        {
            AddReward(0.2f);
            currentDistance = Vector3.Distance(this.transform.position, destination.position);
        }

        if (Mathf.Abs(this.transform.position.y - island.position.y) > 0.1f)
        {
            AddReward(-0.5f);
        }

    }


}
