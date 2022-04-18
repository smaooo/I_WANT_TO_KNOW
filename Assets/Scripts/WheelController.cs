using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    
    public List<Transform> tracks;
    private PlayerController player;
    private List<WheelController> otherWheels;
    private Vector3 destination;
    new private Rigidbody rigidbody;
    private float boxSize;
    [SerializeField]
    private Vector3 moveSpeed;
    private bool fallen = false;

    public int currenTrack;
    private float playerWidth;
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        destination = new Vector3(this.transform.position.x, this.transform.position.y, player.transform.position.z - 10);
        rigidbody = this.GetComponent<Rigidbody>();
        boxSize = this.GetComponent<MeshRenderer>().bounds.size.z;
        playerWidth = player.GetComponent<MeshRenderer>().bounds.size.x;
    }

    private void FixedUpdate()
    {
        if (!fallen)
            MoveForward();
    }

    private void LateUpdate()
    {
        if (!fallen)
            DecisionMaker();
        
        
    }

    private void DecisionMaker()
    {
        //Debug.DrawRay(this.transform.position, this.transform.forward * boxSize * 2, Color.red);
        //var playerRay = Physics.Raycast(this.transform.position, this.transform.forward, boxSize * 2, LayerMask.GetMask("Player"));
        if (Mathf.Abs(this.transform.position.x - player.transform.position.x) < playerWidth &&
            Mathf.Abs(this.transform.position.z - player.transform.position.z) < boxSize * 1.5f)
        {
            int newTrack = -1;
            foreach (var t in tracks)
            {
                if (tracks.IndexOf(t) != currenTrack)
                {
                    var newLoc = new Vector3(t.transform.position.x, this.transform.position.y, this.transform.position.z);
                    var dir = newLoc - this.transform.position;
                    var r1 = Physics.Raycast(this.transform.position, dir,
                        Vector3.Distance(this.transform.position, newLoc) * 1.5f, LayerMask.GetMask("Wheels"));
                    var newLocFront = new Vector3(t.transform.position.x, this.transform.position.y, this.transform.position.z + boxSize);
                    var dirFront = newLocFront - this.transform.position;
                    var r2 = Physics.Raycast(this.transform.position, dirFront,
                        Vector3.Distance(this.transform.position, newLocFront) * 1.5f, LayerMask.GetMask("Wheels"));
                    var newLocBack = new Vector3(t.transform.position.x, this.transform.position.y, this.transform.position.z - boxSize);
                    var dirBack = newLocBack - this.transform.position;
                    var r3 = Physics.Raycast(this.transform.position, newLocBack * 1.5f,
                        Vector3.Distance(this.transform.position, newLocBack) * 1.5f, LayerMask.GetMask("Wheels"));

                    if (!r1 && !r2 && !r3)
                    {
                        newTrack = tracks.IndexOf(t);
                        break;
                    }
                }
            }
            if (newTrack == -1)
            {
                StartCoroutine(FallDown());
            }
            else
            {
                var rayDir = new Vector3(tracks[newTrack].position.x, this.transform.position.y, this.transform.position.z) - this.transform.position;
                var b = Physics.Raycast(this.transform.position, rayDir.normalized, 10, LayerMask.GetMask("Wheel"));
                if (!b)
                    StartCoroutine(MoveToTrack(newTrack));
            }


        }
        foreach (var w in GameObject.FindGameObjectsWithTag("Wheel"))
        {

        }
        if (Vector3.Dot(this.transform.forward, (this.transform.position - player.transform.position).normalized) > 0 &&
            Vector3.Distance(this.transform.position, player.transform.position) > 2)
        {
            //Destroy(this.gameObject);
        }
    }

    private void MoveForward()
    {
        var moveVector = this.transform.forward * moveSpeed.z;

        rigidbody.AddForce(moveVector, ForceMode.Impulse);
    }

    private IEnumerator FallDown()
    {
        fallen = true;
        Quaternion rotation1 = Quaternion.AngleAxis(90, Vector3.up);
        Quaternion rotation2 = Quaternion.AngleAxis(-90, Vector3.forward);
        Quaternion rotation = rotation1 * rotation2;
        while (this.transform.rotation != rotation)
        {
            yield return new WaitForEndOfFrame();
            this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, rotation, 1);
            
        }
        //this.transform.rotation = Quaternion.AngleAxis(90, this.transform.up);
        //this.transform.rotation = Quaternion.AngleAxis(90, this.transform.forward);

    }

    private IEnumerator MoveToTrack(int track)
    {
        float x = tracks[track].position.x;
        
        while (Mathf.Abs(this.transform.position.x - x ) > 0.1f)
        {
            yield return new WaitForEndOfFrame();
            this.transform.position = Vector3.MoveTowards(this.transform.position,
                new Vector3(x, this.transform.position.y, this.transform.position.z), 0.03f);
            if (Mathf.Abs(this.transform.position.x - x) < 0.1f)
            {
                break;
            }
        }
        currenTrack = track;
    }
}
