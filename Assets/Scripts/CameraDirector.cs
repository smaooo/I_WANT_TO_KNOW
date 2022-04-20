using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CameraDirector : MonoBehaviour
{
    [SerializeField]
    private GameObject face;
    [SerializeField]
    private RawImage fade;
    [SerializeField]
    private Texture colorPicker;

    void Start()
    {
        StartCoroutine(ZoomToFace());
    }

    private IEnumerator FadeOut()
    {
        ConstData.Act1LastColor = new Texture2D(Screen.width,Screen.height);
        float timer = 0;
        bool sceneLoading = false;  
        while (fade.color != new Color(1,1,1,1))
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime / 2000;
            fade.color = Color.Lerp(fade.color, new Color(1, 1, 1, 1), timer);

            if (Mathf.Abs(fade.color.a - 0.3f) < 0.05f && !sceneLoading)
            {
                SceneManager.LoadScene(2);
                sceneLoading = true;

            }
            ConstData.Act1LastColor.ReadPixels(new Rect(0,0,colorPicker.width, colorPicker.height),0,0);
            ConstData.Act1LastColor.Apply();
        }

    }

    private IEnumerator ZoomToFace()
    {
        float z = -1.5f;
        Vector3 dest = new Vector3(this.transform.position.x, this.transform.position.y, z);
        float timer = 0;
        float divider = 2000;
        bool changed = false;
        while (this.transform.position.z != z)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime / divider;
            
            this.transform.position = Vector3.Lerp(this.transform.position, dest, timer);
            if (Mathf.Abs(this.transform.position.z - z) < 15 && !changed)
            {

                changed = true;
                divider = 2.5f;
            }

            if (Mathf.Abs(this.transform.position.z - z) < 7.5f && !face.activeSelf)
            {

                face.SetActive(true);
            }
        }


        dest = new Vector3(0.317999989f, 3.22099996f, -0.314074099f);

        timer = 0;
        divider = 3000;
        Vector3 checkPoint = new Vector3(0.27059567f, 3.14410448f, -0.503555f);
        changed = false;
        bool fadeSarted = false;
        
        while (this.transform.position != dest)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime / divider;

            this.transform.position = Vector3.Lerp(this.transform.position, dest, timer);

            if (Vector3.Distance(this.transform.position, checkPoint) < 0.1f && !changed)
            {
                changed = true;
                divider = 500;
            }

            if (Vector3.Distance(this.transform.position, dest) < 0.05f && !fadeSarted)
            {
                fadeSarted = true;
                StartCoroutine(FadeOut());
            }
        }

      
    }
}
