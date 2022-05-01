using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuSc : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject adCamera;
    public Animator cameraAnim;
    public Animator textAnim;

    private bool inputActive = false;
    private int inputIndex = 0;

    [SerializeField]
    private GameObject startButton;
    [SerializeField]
    private GameObject quitButton;
    [SerializeField]
    private RawImage fade;
    [SerializeField]
    private GameObject webGlVer;

    void Start()
    {
        StartCoroutine(AnimationPlay());
    }

    
    IEnumerator AnimationPlay()
    {
        textAnim.Play("MenuText");
        yield return new WaitForSeconds(4);
        cameraAnim.Play("MenuCameraMov");
        yield return new WaitForSeconds(3);
        mainCamera.SetActive(true);
        adCamera.SetActive(false);
        inputActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (inputActive)
        {
            ChangeInput();  

            if (Input.GetKeyDown(KeyCode.Return))
            {
                inputActive = false;
                if (inputIndex == 0)
                {
                    StartCoroutine(ChangeScene());

                }
                else if (inputIndex == 1)
                {
                    if (Application.platform == RuntimePlatform.WebGLPlayer)
                    {
                        inputActive = false;
                        webGlVer.SetActive(true);
                    }
                    else
                    {
                        Application.Quit();

                    }
                }
            }

        }
    }

    private void ChangeInput()
    {

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            inputIndex++;
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            inputIndex--;   
        }


        if (inputIndex < 0)
        {
            inputIndex = 1;
        }

        else if (inputIndex > 1)
        {
            inputIndex = 0;
        }

        if (inputIndex == 0)
        {
            startButton.SetActive(true);
            quitButton.SetActive(false);
        }
        else if (inputIndex == 1)
        {
            startButton.SetActive(false);
            quitButton.SetActive(true);
        }
    }

    private IEnumerator ChangeScene()
    {
        float timer = 0;
        while (fade.color.a != 1)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime / 500;
            fade.color = Color.Lerp(fade.color, new Color(1, 1, 1, 1), timer);
            if (fade.color.a > 0.95)
            {
                fade.color = new Color(1,1,1,1);
                SceneManager.LoadScene(2);

            }
        }

    }
}
