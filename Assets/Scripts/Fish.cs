using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{

    private bool newMovement = true;
    new private Rigidbody rigidbody;

    void Start()
    {
        rigidbody = this.GetComponent<Rigidbody>();
        this.transform.GetChild(0).gameObject.SetActive(true);
        StartCoroutine(MoveFish());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        //if (newMovement)
        //{
        //    StartCoroutine(MoveFish());
        //    newMovement = false;

        //}
    }

    private void OnCollisionEnter(Collision collision)
    {
        rigidbody.velocity = -rigidbody.velocity;
    }
    private IEnumerator MoveFish()
    {
        Vector3 newPos = Vector3.zero;
        Vector3 direction = Vector3.zero;

        float yR = Random.Range(-20f,20f);
        float zR = Random.Range(-20f,20f);
        do
        {
            
            yield return null;

            //newPos = this.transform.TransformPoint(new Vector3());
            newPos = Quaternion.Euler(0, yR, zR) * this.transform.right;
            direction = newPos - this.transform.position;
            Debug.DrawRay(this.transform.position, direction.normalized, Color.green);
        }
        while (Vector3.Angle(this.transform.right, direction) > 90 || Vector3.Angle(this.transform.right, direction) < -90);
        rigidbody.AddForce(direction * 3);
        rigidbody.AddTorque(direction);

        //newMovement = true;
        yield return new WaitForSeconds(Random.Range(1.0f, 3f));
        StartCoroutine(MoveFish());
    }
}
