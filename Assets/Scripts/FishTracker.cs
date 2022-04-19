using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishTracker : MonoBehaviour
{
    [SerializeField]
    private GameObject puddle;
    private Material puddleMaterial;
    private bool canRipple = true;
    void Start()
    {
        puddleMaterial = puddle.GetComponent<MeshRenderer>().material;
        puddleMaterial.SetFloat("TrackerRadius", this.GetComponent<SphereCollider>().radius * 2);
        //if (this.transform.parent.gameObject.name == "FISH1")
        
    }

    // Update is called once per frame
    void Update()
    {
        
        var dist = this.transform.position.y - puddle.transform.position.y;
        
        if (dist <= 0 && dist > -0.05f && canRipple)
        {
            canRipple = false;
            puddleMaterial.SetVector("_RefPos", this.transform.position);
            puddleMaterial.SetFloat("_CurrentTime", Time.realtimeSinceStartup);
            puddleMaterial.SetInt("_Ripple", 0);
            Invoke("ResetRipple", 2);
        }
    }

    private void ResetRipple()
    {
        canRipple = true;
    }
}
