using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SeparatingAxisTest : MonoBehaviour {

	/* References
	 * CODE CREDIT TO:
	 * irixapps (2017) 3D Separating Axis Theorem implementation in Unity, (Version 1.07) [Unity Project and Sourcecode]. https://unitylist.com/p/5uc/Unity-Separating-Axis-SAT
	 * 
	 * Getting the Right Axes to Test with
	 * https://gamedev.stackexchange.com/questions/44500/how-many-and-which-axes-to-use-for-3d-obb-collision-with-sat/
	 * 
	 * Unity Code, that nearly worked, but registered collisions incorrectly in some cases
	 * http://thegoldenmule.com/blog/2013/12/supercolliders-in-unity/ */

	Vector3[] aAxes;
	Vector3[] bAxes;
	Vector3[] AllAxes;
	Vector3[] aVertices;
	Vector3[] bVertices;


	public bool CheckCollision( Transform a, Mesh aMesh, Transform b, Mesh bMesh)
	{

		aAxes = GetAxes(a);
		bAxes = GetAxes(b);

		AllAxes = new Vector3[]
		{
			aAxes[0],
			aAxes[1],
			aAxes[2],
			bAxes[0],
			bAxes[1],
			bAxes[2],
			Vector3.Cross(aAxes[0], bAxes[0]),
			Vector3.Cross(aAxes[0], bAxes[1]),
			Vector3.Cross(aAxes[0], bAxes[2]),
			Vector3.Cross(aAxes[1], bAxes[0]),
			Vector3.Cross(aAxes[1], bAxes[1]),
			Vector3.Cross(aAxes[1], bAxes[2]),
			Vector3.Cross(aAxes[2], bAxes[0]),
			Vector3.Cross(aAxes[2], bAxes[1]),
			Vector3.Cross(aAxes[2], bAxes[2])
		};
		NodePrimitive node = a.GetComponent<NodePrimitive>();
		if(node != null)
        {
			Matrix4x4 nodeMatrix = node.getNodeMatrix();
			Vector3 nodePos = nodeMatrix.GetColumn(3);

		}

		int aAxesLength = aAxes.Length;
		int bAxesLength = bAxes.Length;

		aVertices = GetVertices(a,aMesh);
		bVertices = GetVertices(b,bMesh);

		int aVertsLength = aVertices.Length;
		int bVertsLength = bVertices.Length;

		bool hasOverlap = false;

		if ( ProjectionHasOverlap(a, b, AllAxes.Length, AllAxes, bVertsLength, bVertices, aVertsLength, aVertices, Color.red, Color.green) )
		{
			hasOverlap = true;
		}
		else if (ProjectionHasOverlap(b, a, AllAxes.Length, AllAxes, aVertsLength, aVertices, bVertsLength, bVertices, Color.green, Color.red) )
		{
			hasOverlap = true;
		}

		return hasOverlap;
	}

	// Detects whether or not there is overlap on all separating axes.
	private bool ProjectionHasOverlap(
		Transform aTransform,
		Transform bTransform,

		int aAxesLength,
		Vector3[] aAxes,

		int bVertsLength,
		Vector3[] bVertices,

		int aVertsLength,
		Vector3[] aVertices,

		Color aColor,
		Color bColor)
	{

		for (int i = 0; i < aAxesLength; i++)
		{
			
			
			float bProjMin = float.MaxValue, aProjMin = float.MaxValue;
			float bProjMax = float.MinValue, aProjMax = float.MinValue;

			Vector3 axis = aAxes[i];

			// Handles the cross product = {0,0,0} case
			if (aAxes[i] == Vector3.zero ) return true;

			for (int j = 0; j < bVertsLength; j++)
			{
				float val = FindScalarProjection((bVertices[j]), axis);

				if (val < bProjMin)
				{
					bProjMin = val;
				}

				if (val > bProjMax)
				{
					bProjMax = val;
				}
			}

			for (int j = 0; j < aVertsLength; j++)
			{
				float val = FindScalarProjection((aVertices[j]), axis);

				if (val < aProjMin)
				{
					aProjMin = val;
				}

				if (val > aProjMax)
				{
					aProjMax = val;
				}
			}

			float overlap = FindOverlap(aProjMin, aProjMax, bProjMin, bProjMax);

			if (overlap <= 0)
			{
				// Separating Axis Found Early Out
				return false;
			}
		}

		return true; // A penetration has been found
	}


	/// Calculates the scalar projection of one vector onto another, assumes normalised axes
	private static float FindScalarProjection(Vector3 point, Vector3 axis)
	{
		return Vector3.Dot(point, axis);
	}

	/// Calculates the amount of overlap of two intervals.
	private float FindOverlap(float astart, float aend, float bstart, float bend)
	{
		if (astart < bstart)
		{
			if (aend < bstart)
			{
				return 0f;
			}

			return aend - bstart;
		}

		if (bend < astart)
		{
			return 0f;
		}

		return bend - astart;
	}

	public Vector3[] removeDuplicateVertices(Vector3[] dupArray, Transform xform, Matrix4x4 parentMatrix = new Matrix4x4(), int vertCount = 8)
	{
		if(parentMatrix == Matrix4x4.zero)
        {
			for (int j = 0; j < dupArray.Length; j++)
			{
				dupArray[j] = xform.TransformPoint(dupArray[j]);
			}
		} else
        {
			Vector3 posOffset = parentMatrix.GetColumn(3);
			for (int j = 0; j < dupArray.Length; j++)
			{
				dupArray[j] = (Vector3)(parentMatrix * dupArray[j]) + posOffset;
			}
		}
		
		
		Vector3[] newArray = new Vector3[vertCount];  //change 8 to a variable dependent on shape
		bool isDup = false;
		int newArrayIndex = 0;
		for (int i = 0; i < dupArray.Length; i++)
		{
			for (int j = 0; j < newArray.Length; j++)
			{
				if (dupArray[i] == newArray[j])
				{
					isDup = true;
				}
			}
			if (!isDup)
			{
				newArray[newArrayIndex] = dupArray[i];
				newArrayIndex++;
				isDup = false;
			}
		}
		return newArray;
	}

	public Vector3[] GetVertices(Transform otherTransform, Mesh mesh, int correctVertCount = 8)
	{
		StaticWorldMesh worldMesh = otherTransform.GetComponent<StaticWorldMesh>();
		if (worldMesh != null)
        {
			return worldMesh.vertices;
        }
		NodePrimitive node = otherTransform.GetComponent<NodePrimitive>();
		Vector3[] vertices = mesh.vertices;
		Vector3[] verts;
		if (node != null)
        {
			verts = removeDuplicateVertices(
									vertices,
									otherTransform,
									node.getNodeMatrix(),
									correctVertCount);
		} else
        {
			verts = removeDuplicateVertices(vertices, otherTransform, vertCount: correctVertCount);
		}
		
		return verts;
	}

	public Vector3[] GetAxes(Transform otherTransform)
	{
		NodePrimitive node = otherTransform.GetComponent<NodePrimitive>();
		Vector3[] axes = null;
		if (node == null)
        {
			axes = new[]
			{
				(otherTransform.right),
				(otherTransform.up),
				(otherTransform.forward)
			};
		} else
        {
			Matrix4x4 parentMatrix = node.getNodeMatrix();
			axes = new[]
			{
				((Vector3)parentMatrix.GetColumn(0).normalized),
				((Vector3)parentMatrix.GetColumn(1).normalized),
				((Vector3)parentMatrix.GetColumn(2).normalized)
			};

		}
		
		return axes;

	}
}