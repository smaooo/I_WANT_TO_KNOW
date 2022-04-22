using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class Puddle : MonoBehaviour
{
    private List<Vector3> boundryPoints;
    private List<Vector3> points = new List<Vector3>();
    [SerializeField]
    private Material puddleGround;
    [SerializeField]
    private Material whiteMaterial;

    void Start()
    {
        boundryPoints = this.GetComponent<PathCreator>().path.Points;

        
        foreach(var p in boundryPoints)
        {
            var pos = p;

            pos += Vector3.up * 2;
            points.Add(pos);
            points.Add(pos + Vector3.down * 10);
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
        AssetDatabase.SaveAssets();
        var g = new GameObject();
        g.transform.SetParent(this.transform);
        g.AddComponent<MeshFilter>();
        g.GetComponent<MeshFilter>().mesh = mesh;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        g.layer = 9;
        g.AddComponent<MeshRenderer>().material = puddleGround;
        g.AddComponent<MeshCollider>();
        puddleGround.SetVector("_PlanePosition", this.transform.position);
        puddleGround.SetVector("_PlaneNormal", this.transform.up);
        
    }

    
}
