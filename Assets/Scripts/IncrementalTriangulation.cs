using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Structures;


//Sort the points along one axis. The first 3 points form a triangle. Consider the next point and connect it with all
//previously connected points which are visible to the point. An edge is visible if the center of the edge is visible to the point.
public static class IncrementalTriangulationAlgorithm
{
    public static List<Triangle> TriangulatePoints(List<Vertex> points)
    {
        List<Triangle> triangles = new List<Triangle>();

        //Sort the points along x-axis
        //OrderBy is always soring in ascending order - use OrderByDescending to get in the other order
        points = points.OrderBy(n => n.position.x).ToList();

        //The first 3 vertices are always forming a triangle
        Triangle newTriangle = new Triangle(points[0].position, points[1].position, points[2].position);

        triangles.Add(newTriangle);

        //All edges that form the triangles, so we have something to test against
        List<Edge> edges = new List<Edge>();

        edges.Add(new Edge(newTriangle.v1, newTriangle.v2));
        edges.Add(new Edge(newTriangle.v2, newTriangle.v3));
        edges.Add(new Edge(newTriangle.v3, newTriangle.v1));

        //Add the other triangles one by one
        //Starts at 3 because we have already added 0,1,2
        for (int i = 3; i < points.Count; i++)
        {
            Vector3 currentPoint = points[i].position;

            //The edges we add this loop or we will get stuck in an endless loop
            List<Edge> newEdges = new List<Edge>();

            //Is this edge visible? We only need to check if the midpoint of the edge is visible 
            for (int j = 0; j < edges.Count; j++)
            {
                Edge currentEdge = edges[j];

                Vector3 midPoint = (currentEdge.v1.position + currentEdge.v2.position) / 2f;

                Edge edgeToMidpoint = new Edge(currentPoint, midPoint);

                //Check if this line is intersecting
                bool canSeeEdge = true;

                for (int k = 0; k < edges.Count; k++)
                {
                    //Dont compare the edge with itself
                    if (k == j)
                    {
                        continue;
                    }

                    if (AreEdgesIntersecting(edgeToMidpoint, edges[k]))
                    {
                        canSeeEdge = false;

                        break;
                    }
                }

                //This is a valid triangle
                if (canSeeEdge)
                {
                    Edge edgeToPoint1 = new Edge(currentEdge.v1, new Vertex(currentPoint));
                    Edge edgeToPoint2 = new Edge(currentEdge.v2, new Vertex(currentPoint));

                    newEdges.Add(edgeToPoint1);
                    newEdges.Add(edgeToPoint2);

                    Triangle newTri = new Triangle(edgeToPoint1.v1, edgeToPoint1.v2, edgeToPoint2.v1);

                    triangles.Add(newTri);
                }
            }


            for (int j = 0; j < newEdges.Count; j++)
            {
                edges.Add(newEdges[j]);
            }
        }

        OrientTrianglesClockwise(triangles);
        return triangles;
    }



    private static bool AreEdgesIntersecting(Edge edge1, Edge edge2)
    {
        Vector2 l1_p1 = new Vector2(edge1.v1.position.x, edge1.v1.position.z);
        Vector2 l1_p2 = new Vector2(edge1.v2.position.x, edge1.v2.position.z);

        Vector2 l2_p1 = new Vector2(edge2.v1.position.x, edge2.v1.position.z);
        Vector2 l2_p2 = new Vector2(edge2.v2.position.x, edge2.v2.position.z);

        bool isIntersecting = AreLinesIntersecting(l1_p1, l1_p2, l2_p1, l2_p2, true);

        return isIntersecting;
    }
    public static void OrientTrianglesClockwise(List<Triangle> triangles)
    {
        for (int i = 0; i < triangles.Count; i++)
        {
            Triangle tri = triangles[i];

            Vector2 v1 = new Vector2(tri.v1.position.x, tri.v1.position.z);
            Vector2 v2 = new Vector2(tri.v2.position.x, tri.v2.position.z);
            Vector2 v3 = new Vector2(tri.v3.position.x, tri.v3.position.z);

            if (!IsTriangleOrientedClockwise(v1, v2, v3))
            {
                tri.ChangeOrientation();
            }
        }
    }

    public static void OrientTrianglesCounterClockwise(List<Triangle> triangles)
    {
        for (int i = 0; i < triangles.Count; i++)
        {
            var tri = triangles[i];

            var v1 = new Vector2(tri.v1.position.x, tri.v1.position.z);
            var v2 = new Vector2(tri.v2.position.x, tri.v2.position.z);
            var v3 = new Vector2(tri.v3.position.x, tri.v3.position.z);

            if (IsTriangleOrientedClockwise(v1,v2,v3))
            {
                tri.ChangeOrientation();
            }
        }
    }

    public static bool IsTriangleOrientedClockwise(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        bool isClockWise = true;

        float determinant = p1.x * p2.y + p3.x * p1.y + p2.x * p3.y - p1.x * p3.y - p3.x * p2.y - p2.x * p1.y;

        if (determinant > 0f)
        {
            isClockWise = false;
        }

        return isClockWise;
    }

    public static bool AreLinesIntersecting(Vector2 l1_p1, Vector2 l1_p2, Vector2 l2_p1, Vector2 l2_p2, bool shouldIncludeEndPoints)
    {
        //To avoid floating point precision issues we can add a small value
        float epsilon = 0.00001f;

        bool isIntersecting = false;

        float denominator = (l2_p2.y - l2_p1.y) * (l1_p2.x - l1_p1.x) - (l2_p2.x - l2_p1.x) * (l1_p2.y - l1_p1.y);

        //Make sure the denominator is > 0, if not the lines are parallel
        if (denominator != 0f)
        {
            float u_a = ((l2_p2.x - l2_p1.x) * (l1_p1.y - l2_p1.y) - (l2_p2.y - l2_p1.y) * (l1_p1.x - l2_p1.x)) / denominator;
            float u_b = ((l1_p2.x - l1_p1.x) * (l1_p1.y - l2_p1.y) - (l1_p2.y - l1_p1.y) * (l1_p1.x - l2_p1.x)) / denominator;

            //Are the line segments intersecting if the end points are the same
            if (shouldIncludeEndPoints)
            {
                //Is intersecting if u_a and u_b are between 0 and 1 or exactly 0 or 1
                if (u_a >= 0f + epsilon && u_a <= 1f - epsilon && u_b >= 0f + epsilon && u_b <= 1f - epsilon)
                {
                    isIntersecting = true;
                }
            }
            else
            {
                //Is intersecting if u_a and u_b are between 0 and 1
                if (u_a > 0f + epsilon && u_a < 1f - epsilon && u_b > 0f + epsilon && u_b < 1f - epsilon)
                {
                    isIntersecting = true;
                }
            }
        }

        return isIntersecting;
    }

    //This assumes that we have a polygon and now we want to triangulate it
    //The points on the polygon should be ordered counter-clockwise
    //This alorithm is called ear clipping and it's O(n*n) Another common algorithm is dividing it into trapezoids and it's O(n log n)
    //One can maybe do it in O(n) time but no such version is known
    //Assumes we have at least 3 points
    public static List<Triangle> TriangulateConcavePolygon(List<Vector3> points)
    {
        //The list with triangles the method returns
        List<Triangle> triangles = new List<Triangle>();

        //If we just have three points, then we dont have to do all calculations
        if (points.Count == 3)
        {
            triangles.Add(new Triangle(points[0], points[1], points[2]));

            return triangles;
        }



        //Step 1. Store the vertices in a list and we also need to know the next and prev vertex
        List<Vertex> vertices = new List<Vertex>();

        for (int i = 0; i < points.Count; i++)
        {
            vertices.Add(new Vertex(points[i]));
        }

        //Find the next and previous vertex
        for (int i = 0; i < vertices.Count; i++)
        {
            int nextPos = ClampListIndex(i + 1, vertices.Count);

            int prevPos = ClampListIndex(i - 1, vertices.Count);

            vertices[i].prevVertex = vertices[prevPos];

            vertices[i].nextVertex = vertices[nextPos];
        }



        //Step 2. Find the reflex (concave) and convex vertices, and ear vertices
        for (int i = 0; i < vertices.Count; i++)
        {
            CheckIfReflexOrConvex(vertices[i]);
        }

        //Have to find the ears after we have found if the vertex is reflex or convex
        List<Vertex> earVertices = new List<Vertex>();

        for (int i = 0; i < vertices.Count; i++)
        {
            IsVertexEar(vertices[i], vertices, earVertices);
        }



        //Step 3. Triangulate!
        while (true)
        {
            //This means we have just one triangle left
            if (vertices.Count == 3)
            {
                //The final triangle
                triangles.Add(new Triangle(vertices[0], vertices[0].prevVertex, vertices[0].nextVertex));

                break;
            }

            //Make a triangle of the first ear
            Vertex earVertex = earVertices[0];

            Vertex earVertexPrev = earVertex.prevVertex;
            Vertex earVertexNext = earVertex.nextVertex;

            Triangle newTriangle = new Triangle(earVertex, earVertexPrev, earVertexNext);

            triangles.Add(newTriangle);

            //Remove the vertex from the lists
            earVertices.Remove(earVertex);

            vertices.Remove(earVertex);

            //Update the previous vertex and next vertex
            earVertexPrev.nextVertex = earVertexNext;
            earVertexNext.prevVertex = earVertexPrev;

            //...see if we have found a new ear by investigating the two vertices that was part of the ear
            CheckIfReflexOrConvex(earVertexPrev);
            CheckIfReflexOrConvex(earVertexNext);

            earVertices.Remove(earVertexPrev);
            earVertices.Remove(earVertexNext);

            IsVertexEar(earVertexPrev, vertices, earVertices);
            IsVertexEar(earVertexNext, vertices, earVertices);
        }

        //Debug.Log(triangles.Count);

        return triangles;
    }



    //Check if a vertex if reflex or convex, and add to appropriate list
    private static void CheckIfReflexOrConvex(Vertex v)
    {
        v.isReflex = false;
        v.isConvex = false;

        //This is a reflex vertex if its triangle is oriented clockwise
        Vector2 a = v.prevVertex.GetPos2D_XZ();
        Vector2 b = v.GetPos2D_XZ();
        Vector2 c = v.nextVertex.GetPos2D_XZ();

        if (IsTriangleOrientedClockwise(a, b, c))
        {
            v.isReflex = true;
        }
        else
        {
            v.isConvex = true;
        }
    }



    //Check if a vertex is an ear
    private static void IsVertexEar(Vertex v, List<Vertex> vertices, List<Vertex> earVertices)
    {
        //A reflex vertex cant be an ear!
        if (v.isReflex)
        {
            return;
        }

        //This triangle to check point in triangle
        Vector2 a = v.prevVertex.GetPos2D_XZ();
        Vector2 b = v.GetPos2D_XZ();
        Vector2 c = v.nextVertex.GetPos2D_XZ();

        bool hasPointInside = false;

        for (int i = 0; i < vertices.Count; i++)
        {
            //We only need to check if a reflex vertex is inside of the triangle
            if (vertices[i].isReflex)
            {
                Vector2 p = vertices[i].GetPos2D_XZ();

                //This means inside and not on the hull
                if (Intersections.IsPointInTriangle(a, b, c, p))
                {
                    hasPointInside = true;

                    break;
                }
            }
        }

        if (!hasPointInside)
        {
            earVertices.Add(v);
        }
    }

    //Clamp list indices
    //Will even work if index is larger/smaller than listSize, so can loop multiple times
    public static int ClampListIndex(int index, int listSize)
    {
        index = ((index % listSize) + listSize) % listSize;

        return index;
    }
}

