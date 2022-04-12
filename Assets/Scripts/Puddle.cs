using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Puddle : MonoBehaviour
{
    private List<Vector3> boundryPoints;
    private List<Vector3> points = new List<Vector3>();
    void Start()
    {
        boundryPoints = this.GetComponent<PathCreator>().path.Points;

        print(boundryPoints.Count);
        
        foreach(var p in boundryPoints)
        {
            //Debug.DrawRay(p, Vector3.up, Color.red, 100);
            points.Add(p);
            points.Add(p + Vector3.down * 2);
            //Debug.DrawRay(x, Vector3.down, Color.green, 100);
        }
        //boundryPoints.AddRange(dPoints);
        var triangles = new List<int>();
        for (int i = 2; i < points.Count; i+=2)
        {

            triangles.Add(i);
            triangles.Add(i-2);
            triangles.Add(i-1);
            triangles.Add(i);
            triangles.Add(i-1);
            triangles.Add(i+1);
        }
        var l = points.IndexOf(points.Last());
        //triangles.Add(l);
        //triangles.Add(l);
        triangles.Add(0);
        triangles.Add(l-1);
        triangles.Add(l);
        triangles.Add(0);
        triangles.Add(l);
        triangles.Add(1);
        Mesh mesh = new Mesh();
        mesh.vertices = points.ToArray();
        mesh.triangles = triangles.ToArray();

        var g = new GameObject();

        g.AddComponent<MeshFilter>();
        g.GetComponent<MeshFilter>().mesh = mesh;
        mesh.RecalculateNormals();
        var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        g.AddComponent<MeshRenderer>().material = material ;
        
        //print(boundryPoints.Count);
    }

    

    // Update is called once per frame
    void Update()
    {
        //foreach(var p in boundryPoints)
        //{
        //    Debug.DrawRay(p, Vector3.up, Color.red, 100);
        //}
    }
}
