using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossSection : MonoBehaviour
{

    public GameObject cube;
    private Material material;
    void Start()
    {
        material = cube.GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        material.SetVector("_PlanePosition", this.transform.position);
        material.SetVector("_PlaneNormal", this.transform.up);
    }
}
