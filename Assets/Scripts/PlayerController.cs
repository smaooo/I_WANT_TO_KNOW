using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using QA;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI textField;
    private int lastKey;
    public bool inputActive = false;
    private Manager manager;
    new private Rigidbody rigidbody;
    private Animator animator;
    [SerializeField]
    private float moveSpeed = 2f;

    [SerializeField]
    private bool trainingMode = false;
    private Vector3 startPoint;
    [SerializeField]
    private Transform endPoint;
    [SerializeField]
    private Transform leftBorder;
    [SerializeField]
    private Transform rightBorder;
    [SerializeField]
    private GameObject playerInputPack;
    private enum State { Walking, Conversation}
    [SerializeField]
    private State state = State.Conversation;
    void Start()
    {
        animator = this.GetComponent<Animator>();
        rigidbody = this.GetComponent<Rigidbody>();
        if (trainingMode)
        {
            startPoint = this.transform.position;
            StartCoroutine(MoveRandom());
        }

        else
        {
            manager = FindObjectOfType<Manager>();
            //Invoke("MakeInputActive", 0.5f);
            textField.text = "";

        }
    }

    void Update()
    {
        if (inputActive && !trainingMode && state == State.Conversation)
        {
            CheckKeyInput();
            
        }
    }

    private void OnGUI()
    {
        Event e = Event.current;
        if (e.isKey && e.type == EventType.KeyUp)
        {
            lastKey = (int)(char)e.keyCode;
        }
    

        
    }

    private void FixedUpdate()
    {
        if (state == State.Walking)
            Move();
    }

    private void Move()
    {
        Vector3 forward = Input.GetAxisRaw("Vertical") > 0.1f ? this.transform.forward : Vector3.zero;
        Vector3 leftRight = Vector3.zero;
        if (Input.GetAxisRaw("Vertical") > 0)
        {
            if (Mathf.Abs(this.transform.position.x - leftBorder.transform.position.x) < 2)
            {
                if (Input.GetAxisRaw("Horizontal") > 0)
                {
                    leftRight = this.transform.right * Input.GetAxisRaw("Horizontal");
                    animator.SetFloat("LeftRight", Input.GetAxisRaw("Horizontal"));

                }
                else
                {

                    animator.SetFloat("LeftRight", 0);
                }

            }

            else if (Mathf.Abs(this.transform.position.x - rightBorder.transform.position.x) < 2)
            {
                if (Input.GetAxisRaw("Horizontal") < 0)
                {
                    leftRight = this.transform.right * Input.GetAxisRaw("Horizontal");
                    animator.SetFloat("LeftRight", Input.GetAxisRaw("Horizontal"));

                }
                else
                {

                    animator.SetFloat("LeftRight", 0);
                }
            }
            else
            {
                leftRight = this.transform.right * Input.GetAxisRaw("Horizontal");
                animator.SetFloat("LeftRight", Input.GetAxisRaw("Horizontal"));

            }
        }
        else
        {
                animator.SetFloat("LeftRight", 0);
            
        }

        //Vector3 leftRight = Input.GetAxisRaw("Vertical") > 0 ? Input.GetAxisRaw("Horizontal") * this.transform.right : Vector3.zero;
        Vector3 moveVector = forward + leftRight;

        animator.SetFloat("Forward", Input.GetAxisRaw("Vertical") > 0.1f ? 1 : 0);


        rigidbody.velocity = moveVector * moveSpeed;
    }
    private void MakeInputActive()
    {
        inputActive = true;
    }
    private void CheckKeyInput()
    {
        if (lastKey != 0)
        {
            if ((lastKey >= 97 && lastKey <=122) || lastKey == 32 || lastKey == 46 || lastKey == 47
                || lastKey == 49)
            {
                textField.text += ((char)lastKey).ToString();
                lastKey = 0;
            }
            
            else if (lastKey == 8 && textField.text.Length > 0)
            {
                textField.text = textField.text.Remove(textField.text.Length - 1);
                lastKey = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            var input = textField.text;
            textField.text = "";
            playerInputPack.SetActive(false);
            inputActive = false;
            manager.SetNextQuestion(input);
        }
    }


    private IEnumerator MoveRandom()
    {

        if (Mathf.Abs(this.transform.position.z - endPoint.position.z) < 5f)
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
                velocity += this.transform.right * Random.Range(1f, 2f);
            }
            else if (Mathf.Abs(this.transform.position.x - rightBorder.position.x) < 0.5f)
            {
                velocity += -this.transform.right * Random.Range(1f, 2f);
            }
            else
            {
                float vRand = Random.value;
                velocity += vRand < 0.5f ? this.transform.right * 2 : -this.transform.right * 2;

            }
        }
        rigidbody.velocity = velocity * 5;

        yield return new WaitForSeconds(Random.Range(0f, 1f));
        StartCoroutine(MoveRandom());

    }
}
