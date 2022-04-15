using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSc : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject adCamera;
    public Animator cameraAnim;
    public Animator textAnim;
    // Start is called before the first frame update
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
