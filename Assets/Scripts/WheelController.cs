using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    [HideInInspector]
    public List<Transform> tracks;
    private PlayerController player;
    new private Rigidbody rigidbody;
    private float boxSize;
    [SerializeField]
    private Vector3 moveSpeed;
    [HideInInspector]
    public bool fallen = false;
    private Manager manager;
    [HideInInspector]
    public int currenTrack;
    private float playerWidth;
    private Animator animator;
    private bool changingTrack = false;
    private SpriteRenderer renderer;
    private bool jumping = false;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        renderer = this.GetComponent<SpriteRenderer>();
        rigidbody = this.GetComponent<Rigidbody>();
        boxSize = this.GetComponent<BoxCollider>().bounds.size.z;
        playerWidth = player.GetComponent<BoxCollider>().size.x;
        manager = FindObjectOfType<Manager>();
        animator = this.GetComponent<Animator>();
        ChangeAnimatorTrack();
        moveSpeed.z = Random.Range(0.24f, 0.4f);
        animator.SetFloat("Speed", moveSpeed.z);
        StartCoroutine(FadeIn());
    }

    private void FixedUpdate()
    {
        if (!fallen && ! changingTrack)
            MoveForward();
    }

    private void LateUpdate()
    {
        if (!fallen)
        {
            DecisionMaker();
            
        }

        UpdateSortingLayer();
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Jump();
        //}
        
    }

    private IEnumerator FadeIn()
    {
        float timer = 0;
        while (renderer.color != new Color(1, 1, 1, 1))
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime / 20;
            renderer.color = Color.Lerp(renderer.color, new Color(1, 1, 1, 1), timer);
        }
    }

    private void UpdateSortingLayer()
    {
        float dotProduct = Vector3.Dot(this.transform.forward,
            (new Vector3(this.transform.position.x, player.transform.position.y, this.transform.position.z) - player.transform.position).normalized);
        if (dotProduct > 0)
        {
            renderer.sortingOrder = 500;
        }
        else
        {
            renderer.sortingOrder = 200 - Mathf.FloorToInt(Vector3.Distance(this.transform.position, player.transform.position));
        }

        //foreach (var s in manager.stones)
        //{
        //    float dot = Vector3.Dot(this.transform.forward,
        //        (s.transform.position - this.transform.position).normalized);
        //    if (dot < 0)
        //    {
        //        renderer.sortingOrder = s.sortingOrder - 10;
        //    }

        //}
    }
    private void DecisionMaker()
    {

        if (Mathf.Abs(this.transform.position.x - player.transform.position.x) < playerWidth &&
            Mathf.Abs(this.transform.position.z - player.transform.position.z) < boxSize)
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
                StopAllCoroutines();
                StartCoroutine(FallDown());
            }
            else if (!changingTrack)
            {
                var rayDir = new Vector3(tracks[newTrack].position.x, this.transform.position.y, this.transform.position.z) - this.transform.position;
                var b = Physics.Raycast(this.transform.position, rayDir.normalized, boxSize * 3, LayerMask.GetMask("Wheel"));
                if (!b && !jumping)
                {
                    changingTrack = true;
                    rigidbody.velocity = Vector3.zero;
                    StartCoroutine(MoveToTrack(newTrack));
                }
                //else if (Vector3.Distance(this.transform.position, player.transform.position) < 10)
                //{
                //    StartCoroutine(MoveToTrack(newTrack));

                //}
            }


        }
        
        foreach (var t in manager.currentWheels)
        {
            bool shouldbreak = false;
            foreach (var w in t.Value)
            {
                
                if (w.fallen && Mathf.Abs(this.transform.position.x - w.transform.position.x) < 6 &&
                    Mathf.Abs(this.transform.position.z - w.transform.position.z) < 6 && !jumping)
                {
                    shouldbreak = true;
                    Jump();
                    break;
                }
            }
            if (shouldbreak) break;
        }
        if (jumping && rigidbody.velocity.y == 0)
        {
            jumping = false;
        }
        if (Vector3.Dot(this.transform.forward, (new Vector3(this.transform.position.x,player.transform.position.y,this.transform.position.z) - player.transform.position).normalized) > 0 &&
            Vector3.Distance(this.transform.position, player.transform.position) > 50)
        {
            Destroy(this.gameObject);
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
        animator.SetFloat("Speed", 1);
        animator.SetTrigger("Fall");
        rigidbody.Sleep();
        rigidbody.isKinematic = true;
        //float targetY = 0 + this.GetComponent<BoxCollider>().size.x / 1.5f;
        float targetY = this.transform.position.y - 26;
        yield return null;

        while (Mathf.Abs(this.transform.position.y - targetY) > 0.2f)
        {
            yield return new WaitForEndOfFrame();
            this.transform.position = Vector3.MoveTowards(this.transform.position,
                new Vector3(this.transform.position.x, targetY, this.transform.position.z), 0.8f);

        }

        this.GetComponent<BoxCollider>().enabled = false;

      
    }

    private void ChangeAnimatorTrack()
    {
        if (currenTrack >= 0 && currenTrack < 2)
        {
            
            animator.SetFloat("LeftRight", 1);
        }
        else
        {
            
            animator.SetFloat("LeftRight", -1);
        }

    }
    private void Jump()
    {

        jumping = true;
        Vector3 moveVector = this.transform.forward *2 + this.transform.up *2;
        rigidbody.velocity = Vector3.zero;
        rigidbody.AddForce(moveVector, ForceMode.Impulse);
    }
    private IEnumerator MoveToTrack(int track)
    {
        currenTrack = track;
        float x = tracks[track].position.x;
        animator.SetFloat("LeftRight", 0);
        animator.SetFloat("Turning", 1);
        while (Mathf.Abs(this.transform.position.x - x ) > 0.1f)
        {
            yield return new WaitForEndOfFrame();
            this.transform.position = Vector3.MoveTowards(this.transform.position,
                new Vector3(x, this.transform.position.y, this.transform.position.z), 0.1f);
            if (Mathf.Abs(this.transform.position.x - x) < 0.1f)
            {
                break;
            }
        }
        animator.SetFloat("Turning", -1);
        ChangeAnimatorTrack();
        changingTrack = false;
    }
}
