using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Puddle : MonoBehaviour
{
    private List<Vector3> boundryPoints;
    private List<Vector3> points = new List<Vector3>();
    [SerializeField]
    private Material puddleGround;

    void Start()
    {
        boundryPoints = this.GetComponent<PathCreator>().path.Points;

        
        foreach(var p in boundryPoints)
        {
            points.Add(p);
            points.Add(p + Vector3.down * 2);
        }

        var b = Instantiate(this.transform.GetChild(0).gameObject, this.transform);
        b.transform.position = new Vector3(b.transform.position.x, points[1].y, b.transform.position.z);
        //b.GetComponent<MeshRenderer>().enabled = true;
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
        g.transform.SetParent(this.transform);
        g.AddComponent<MeshFilter>();
        g.GetComponent<MeshFilter>().mesh = mesh;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
        g.AddComponent<MeshRenderer>().material = puddleGround;
        g.AddComponent<MeshCollider>();
        

    }

    
}
