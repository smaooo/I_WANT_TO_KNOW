using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using Structures;

public class Puddle : MonoBehaviour
{
    private List<Vector3> boundryPoints;
    private List<Vector3> points = new List<Vector3>();
    [SerializeField]
    private Material puddleGround;
    [SerializeField]
    private Material whiteMaterial;
    private List<Vector3> bounds = new List<Vector3>();
    void Start()
    {
        boundryPoints = this.GetComponent<PathCreator>().path.Points;

        bounds.Add(new Vector3(this.transform.position.x + 100, boundryPoints[0].y, this.transform.position.z + 100));
        bounds.Add(new Vector3(this.transform.position.x + 100, boundryPoints[0].y, this.transform.position.z - 100));
        bounds.Add(new Vector3(this.transform.position.x - 100, boundryPoints[0].y, this.transform.position.z + 100));
        bounds.Add(new Vector3(this.transform.position.x - 100, boundryPoints[0].y, this.transform.position.z - 100));

        var vertecies = new List<Vertex>();
        
        foreach(var p in boundryPoints)
        {
            vertecies.Add(new Vertex(p));
            var pos = p;

            pos += Vector3.up * 2;
            points.Add(pos);
            points.Add(pos + Vector3.down * 10);
        }
        //foreach(var bo in bounds)
        //{
        //    vertecies.Add(new Vertex(bo));
        //}
        var upperFaceTris = IncrementalTriangulationAlgorithm.TriangulatePoints(vertecies);
        var upMesh = new Mesh();
        upMesh.vertices = boundryPoints.ToArray();
        var upMeshtIndex = new List<int>();
        foreach (var t in upperFaceTris)
        {

            upMeshtIndex.Add(boundryPoints.IndexOf(t.v1.position));
            upMeshtIndex.Add(boundryPoints.IndexOf(t.v2.position));
            upMeshtIndex.Add(boundryPoints.IndexOf(t.v3.position));

            Debug.DrawRay(t.v1.position, t.v2.position, Color.red, 100);
            Debug.DrawRay(t.v2.position, t.v3.position, Color.red, 100);
            Debug.DrawRay(t.v3.position, t.v1.position, Color.red,100);
        }
        //upMesh.triangles = upMeshtIndex.ToArray();
        //upMesh.RecalculateNormals();
        //var upG = new GameObject();
        //upG.AddComponent<MeshFilter>().mesh = upMesh;
        //upG.AddComponent<MeshRenderer>();


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
