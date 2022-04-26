using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Structures;
public static class Intersections
{
	//Rectangle-rectangle in 2d space with orientation
	public static bool IsIntersectingOBBRectangleRectangle(
		Vector3 r1_FL, Vector3 r1_FR, Vector3 r1_BL, Vector3 r1_BR,
		Vector3 r2_FL, Vector3 r2_FR, Vector3 r2_BL, Vector3 r2_BR)
	{
		bool isIntersecting = false;

		//Create the rectangles
		Rectangle r1 = new Rectangle(r1_FL, r1_FR, r1_BL, r1_BR);
		Rectangle r2 = new Rectangle(r2_FL, r2_FR, r2_BL, r2_BR);

		//Find out if the rectangles are intersecting by approximating them with rectangles 
		//with no rotation and then use AABB intersection
		//Will make it faster if the probability that the rectangles are intersecting is low
		if (!IsIntersectingAABB_OBB(r1, r2))
		{
			return isIntersecting;
		}

		//Find out if the rectangles are intersecting by using the Separating Axis Theorem (SAT)
		isIntersecting = SATRectangleRectangle(r1, r2);

		return isIntersecting;
	}

	//Find out if the rectangles are intersecting by using AABB
	private static bool IsIntersectingAABB_OBB(Rectangle r1, Rectangle r2)
	{
		bool isIntersecting = false;

		//Find the min/max values for the AABB algorithm
		float r1_minX = Mathf.Min(r1.FL.x, Mathf.Min(r1.FR.x, Mathf.Min(r1.BL.x, r1.BR.x)));
		float r1_maxX = Mathf.Max(r1.FL.x, Mathf.Max(r1.FR.x, Mathf.Max(r1.BL.x, r1.BR.x)));

		float r2_minX = Mathf.Min(r2.FL.x, Mathf.Min(r2.FR.x, Mathf.Min(r2.BL.x, r2.BR.x)));
		float r2_maxX = Mathf.Max(r2.FL.x, Mathf.Max(r2.FR.x, Mathf.Max(r2.BL.x, r2.BR.x)));

		float r1_minZ = Mathf.Min(r1.FL.z, Mathf.Min(r1.FR.z, Mathf.Min(r1.BL.z, r1.BR.z)));
		float r1_maxZ = Mathf.Max(r1.FL.z, Mathf.Max(r1.FR.z, Mathf.Max(r1.BL.z, r1.BR.z)));

		float r2_minZ = Mathf.Min(r2.FL.z, Mathf.Min(r2.FR.z, Mathf.Min(r2.BL.z, r2.BR.z)));
		float r2_maxZ = Mathf.Max(r2.FL.z, Mathf.Max(r2.FR.z, Mathf.Max(r2.BL.z, r2.BR.z)));

		if (IsIntersectingAABB(r1_minX, r1_maxX, r1_minZ, r1_maxZ, r2_minX, r2_maxX, r2_minZ, r2_maxZ))
		{
			isIntersecting = true;
		}

		return isIntersecting;
	}

	//Intersection: AABB-AABB (Axis-Aligned Bounding Box) - rectangle-rectangle in 2d space with no orientation
	public static bool IsIntersectingAABB(
		float r1_minX, float r1_maxX, float r1_minZ, float r1_maxZ,
		float r2_minX, float r2_maxX, float r2_minZ, float r2_maxZ)
	{
		//If the min of one box in one dimension is greater than the max of another box then the boxes are not intersecting
		//They have to intersect in 2 dimensions. We have to test if box 1 is to the left or box 2 and vice versa
		bool isIntersecting = true;

		//X axis
		if (r1_minX > r2_maxX)
		{
			isIntersecting = false;
		}
		else if (r2_minX > r1_maxX)
		{
			isIntersecting = false;
		}
		//Z axis
		else if (r1_minZ > r2_maxZ)
		{
			isIntersecting = false;
		}
		else if (r2_minZ > r1_maxZ)
		{
			isIntersecting = false;
		}


		return isIntersecting;
	}

	//Find out if 2 rectangles with orientation are intersecting by using the SAT algorithm
	private static bool SATRectangleRectangle(Rectangle r1, Rectangle r2)
	{
		bool isIntersecting = false;

		//We have just 4 normals because the other 4 normals are the same but in another direction
		//So we only need a maximum of 4 tests if we have rectangles
		//It is enough if one side is not overlapping, if so we know the rectangles are not intersecting

		//Test 1
		Vector3 normal1 = GetNormal(r1.BL, r1.FL);

		if (!IsOverlapping(normal1, r1, r2))
		{
			//No intersection is possible!
			return isIntersecting;
		}

		//Test 2
		Vector3 normal2 = GetNormal(r1.FL, r1.FR);

		if (!IsOverlapping(normal2, r1, r2))
		{
			return isIntersecting;
		}

		//Test 3
		Vector3 normal3 = GetNormal(r2.BL, r2.FL);

		if (!IsOverlapping(normal3, r1, r2))
		{
			return isIntersecting;
		}

		//Test 4
		Vector3 normal4 = GetNormal(r2.FL, r2.FR);

		if (!IsOverlapping(normal4, r1, r2))
		{
			return isIntersecting;
		}

		//If we have come this far, then we know all sides are overlapping
		//So the rectangles are intersecting!
		isIntersecting = true;

		return isIntersecting;
	}

	//Is this side overlapping?
	private static bool IsOverlapping(Vector3 normal, Rectangle r1, Rectangle r2)
	{
		bool isOverlapping = false;

		//Project the corners of rectangle 1 onto the normal
		float dot1 = DotProduct(normal, r1.FL);
		float dot2 = DotProduct(normal, r1.FR);
		float dot3 = DotProduct(normal, r1.BL);
		float dot4 = DotProduct(normal, r1.BR);

		//Find the range
		float min1 = Mathf.Min(dot1, Mathf.Min(dot2, Mathf.Min(dot3, dot4)));
		float max1 = Mathf.Max(dot1, Mathf.Max(dot2, Mathf.Max(dot3, dot4)));


		//Project the corners of rectangle 2 onto the normal
		float dot5 = DotProduct(normal, r2.FL);
		float dot6 = DotProduct(normal, r2.FR);
		float dot7 = DotProduct(normal, r2.BL);
		float dot8 = DotProduct(normal, r2.BR);

		//Find the range
		float min2 = Mathf.Min(dot5, Mathf.Min(dot6, Mathf.Min(dot7, dot8)));
		float max2 = Mathf.Max(dot5, Mathf.Max(dot6, Mathf.Max(dot7, dot8)));


		//Are the ranges overlapping?
		if (min1 <= max2 && min2 <= max1)
		{
			isOverlapping = true;
		}

		return isOverlapping;
	}

	//Get the normal from 2 points. This normal is pointing left in the direction start -> end
	//But it doesn't matter in which direction the normal is pointing as long as you have the same
	//algorithm for all edges
	private static Vector3 GetNormal(Vector3 startPos, Vector3 endPos)
	{
		//The direction
		Vector3 dir = endPos - startPos;

		//The normal, just flip x and z and make one negative (don't need to normalize it)
		Vector3 normal = new Vector3(-dir.z, dir.y, dir.x);

		//Draw the normal from the center of the rectangle's side
		Debug.DrawRay(startPos + (dir * 0.5f), normal.normalized * 2f, Color.red);

		return normal;
	}

	//Get the dot product
	//p - the vector we want to project
	//u - the unit vector p is being projected on
	//proj_p_on_u = Vector3.Dot(p, u) * u;
	//But we only need to project a point, so just Vector3.Dot(p, u)
	private static float DotProduct(Vector3 v1, Vector3 v2)
	{
		//2d space
		float dotProduct = v1.x * v2.x + v1.z * v2.z;

		return dotProduct;
	}

	//To create a rectangle
	private struct Rectangle
	{
		//Corners
		public Vector3 FL, FR, BL, BR;

		public Rectangle(Vector3 FL, Vector3 FR, Vector3 BL, Vector3 BR)
		{
			this.FL = FL;
			this.FR = FR;
			this.BL = BL;
			this.BR = BR;
		}
	}

	//
	// Intersection: Point in triangle
	//

	//There's a possibility that one of the triangles is smaller than the other
	//So we have to check if any of the triangle's corners is inside the other triangle
	public static bool AreCornersIntersecting(Triangle t1, Triangle t2)
	{
		bool isIntersecting = false;

		//We only have to test one corner from each triangle
		//Triangle 1 in triangle 2
		if (IsPointInTriangle(t1.v1.position, t2.v1.position, t2.v2.position, t2.v3.position))
		{
			isIntersecting = true;
		}
		//Triangle 2 in triangle 1
		else if (IsPointInTriangle(t2.v1.position, t1.v1.position, t1.v2.position, t1.v3.position))
		{
			isIntersecting = true;
		}

		return isIntersecting;
	}



	//Is a point p inside a triangle p1-p2-p3?
	//From http://totologic.blogspot.se/2014/01/accurate-point-in-triangle-test.html
	public static bool IsPointInTriangle(Vector3 p, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		bool isWithinTriangle = false;

		float denominator = ((p2.z - p3.z) * (p1.x - p3.x) + (p3.x - p2.x) * (p1.z - p3.z));

		float a = ((p2.z - p3.z) * (p.x - p3.x) + (p3.x - p2.x) * (p.z - p3.z)) / denominator;
		float b = ((p3.z - p1.z) * (p.x - p3.x) + (p1.x - p3.x) * (p.z - p3.z)) / denominator;
		float c = 1 - a - b;

		//The point is within the triangle if 0 <= a <= 1 and 0 <= b <= 1 and 0 <= c <= 1
		if (a >= 0f && a <= 1f && b >= 0f && b <= 1f && c >= 0f && c <= 1f)
		{
			isWithinTriangle = true;
		}

		return isWithinTriangle;
	}
}
