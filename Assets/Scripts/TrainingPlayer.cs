using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingPlayer : MonoBehaviour
{

    new private Rigidbody rigidbody;
    [SerializeField]
    private Transform endPoint;
    [SerializeField]
    private Transform leftBorder;
    [SerializeField]
    private Transform rightBorder;

    private Vector3 startPoint;

    void Start()
    {
        startPoint = this.transform.position;
        rigidbody = this.GetComponent<Rigidbody>();
        StartCoroutine(MoveRandom());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    private IEnumerator MoveRandom()
    {
        
        if (Mathf.Abs(this.transform.position.z - endPoint.position.z) < 2f)
        {
            print("Reached");
            rigidbody.velocity = Vector3.zero;
            this.transform.position = startPoint;
        }
        yield return null;
        float rand = Random.value;
        Vector3 velocity = Vector3.zero;
        if (rand > 0.2f)
        {
            velocity += this.transform.forward;
        }
        else
        {
            velocity = Vector3.zero;
        }
        if (rand > 0.45f && rand < 0.65f)
        {
            if (Mathf.Abs(this.transform.position.x - leftBorder.position.x) < 0.5f)
            {
                velocity += this.transform.right * Random.Range(1f,2f);
            }
            else if (Mathf.Abs(this.transform.position.x - rightBorder.position.x) < 0.5f)
            {
                velocity += -this.transform.right * Random.Range(1f, 2f);
            }
            else
            {
                float vRand = Random.value;
                velocity += vRand < 0.5f ? this.transform.right * 2 : -this.transform.right *2;
                
            }
        }
        rigidbody.velocity = velocity * 5;

        yield return new WaitForSeconds(Random.Range(0f,1f));
        StartCoroutine(MoveRandom());

    }
}
