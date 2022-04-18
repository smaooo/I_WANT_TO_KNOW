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
        MoveForward();
    }

    private void LateUpdate()
    {
        DecisionMaker();
        
        
    }

    private void DecisionMaker()
    {
        //Debug.DrawRay(this.transform.position, this.transform.forward * boxSize * 2, Color.red);
        //var playerRay = Physics.Raycast(this.transform.position, this.transform.forward, boxSize * 2, LayerMask.GetMask("Player"));
        if (Mathf.Abs(this.transform.position.x - player.transform.position.x) < 0.8f &&
            Mathf.Abs(this.transform.position.z - player.transform.position.z) < boxSize * 1.5f)
        {
            print("DETECTED");
            int newTrack = -1;
            foreach (var t in tracks)
            {
                if (tracks.IndexOf(t) != currenTrack)
                {
                    var newLoc = new Vector3(t.transform.position.x, this.transform.position.y, this.transform.position.z);
                    var dir = newLoc - this.transform.position;
                    var r = Physics.Raycast(this.transform.position, dir, Vector3.Distance(this.transform.position, newLoc) * 1.5f, LayerMask.GetMask("Wheels"));
                    if (!r)
                    {
                        newTrack = tracks.IndexOf(t);
                        break;
                    }
                }
            }
            if (newTrack == -1)
            {

            }
            else
            {
                var rayDir = new Vector3(tracks[newTrack].position.x, this.transform.position.y, this.transform.position.z) - this.transform.position;
                var b = Physics.Raycast(this.transform.position, rayDir.normalized, 10, LayerMask.GetMask("Wheel"));
                if (!b)
                    StartCoroutine(MoveToTrack(newTrack));
                //else
            }


        }
        foreach (var w in GameObject.FindGameObjectsWithTag("Wheel"))
        {

        }
        if (Vector3.Dot(this.transform.forward, (this.transform.position - player.transform.position).normalized) > 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void MoveForward()
    {
        var moveVector = this.transform.forward * moveSpeed.z;

        rigidbody.AddForce(moveVector, ForceMode.Impulse);
    }

    private IEnumerator MoveToTrack(int track)
    {
        float x = tracks[track].position.x;
        var newLoc = new Vector3(x, this.transform.position.y, this.transform.position.z);

        while (Vector3.Distance(this.transform.position, newLoc) > 0.1f)
        {
            yield return new WaitForEndOfFrame();
            this.transform.position = Vector3.MoveTowards(this.transform.position, newLoc, 0.03f);
            if (Mathf.Abs(this.transform.position.x - newLoc.x) < 0.1f)
            {
                break;
            }
        }
        currenTrack = track;
    }
}
