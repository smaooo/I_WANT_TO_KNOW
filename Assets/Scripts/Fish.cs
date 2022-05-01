using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    [SerializeField]
    private Transform head;
    new private Rigidbody rigidbody;

    void Start()
    {
        StartCoroutine(Move());
    }


    private void FixedUpdate()
    {
      
        var col = Physics.Raycast(new Ray(this.transform.position, this.transform.forward),1, LayerMask.GetMask("PuddleBound"));

        if (col)
        {
            StopAllCoroutines();
            StartCoroutine(Move());
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("StuckArea"))
        {
            StopAllCoroutines();
            StartCoroutine(Rescue());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        StopAllCoroutines();
        StartCoroutine(Move());
    }

    private IEnumerator Rescue()
    {
        var rotation = Quaternion.AngleAxis(180, this.transform.up);
        while(this.transform.rotation != rotation)
        {
            yield return new WaitForEndOfFrame();
            this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, rotation, 0.1f);

            
        }
        StartCoroutine(Move());
    }
    private Vector3 GenerateWayPoint()
    {
        bool col;
        Vector3 position = Vector3.zero;
        do
        {
            position = this.transform.TransformPoint(Random.insideUnitSphere * Random.Range(1,10));

            var ray = new Ray(this.transform.position, position - this.transform.position);
            RaycastHit hit;
            col = Physics.Linecast(this.transform.position ,position,
                out hit, LayerMask.GetMask("PuddleBound"));
            if (col)
            {
                //Debug.DrawRay(this.transform.position, (position - this.transform.position).normalized * hit.distance, Color.green, 10);
            }
        }
        while (col);
        //Debug.DrawRay(this.transform.position, position - this.transform.position, Color.red, 100);
        
        return position;

    }

    private IEnumerator Move()
    {
        var position = GenerateWayPoint();
        
        while (Vector3.Distance(head.transform.position, position) > 0.2f)
        {

            var rotation = Quaternion.LookRotation((position - this.head.position).normalized, this.transform.up);
            if (this.transform.rotation != rotation)
            {
                this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, rotation, 0.1f);
                this.transform.rotation = Quaternion.Euler(this.transform.rotation.eulerAngles.x, this.transform.rotation.eulerAngles.y, 0);
            }
            //rigidbody.AddForce(this.transform.forward / 10);
            yield return new WaitForEndOfFrame();
            this.transform.position = Vector3.MoveTowards(this.transform.position, this.transform.position + this.transform.forward, 0.005f);

            if (Vector3.Distance(head.transform.position, position) <= 0.2f)
            {
                break;
            }
        }
        StartCoroutine(Move());
    }
    private IEnumerator MoveFish()
    {
        Vector3 newPos = Vector3.zero;
        Vector3 direction = Vector3.zero;

        var velocity = this.transform.right;

       
        rigidbody.AddForce(velocity * 3);
        //newMovement = true;
        yield return new WaitForSeconds(Random.Range(1.0f, 3f));
        StartCoroutine(MoveFish());
    }
}
