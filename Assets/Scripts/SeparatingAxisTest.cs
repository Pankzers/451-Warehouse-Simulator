using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SeparatingAxisTest : MonoBehaviour {

	// References
	// Getting the Right Axes to Test with
	//https://gamedev.stackexchange.com/questions/44500/how-many-and-which-axes-to-use-for-3d-obb-collision-with-sat/

	//Unity Code, that nearly worked, but registered collisions incorrectly in some cases
	//http://thegoldenmule.com/blog/2013/12/supercolliders-in-unity/

	//[SerializeField]
	//private Cube _cubeA;

	//[SerializeField]
	//private Cube _cubeB;

	Vector3[] aAxes;
	Vector3[] bAxes;
	Vector3[] AllAxes;
	Vector3[] aVertices;
	Vector3[] bVertices;

	//float minOverlap = 0;
	//Vector3 minOverlapAxis = Vector3.zero;

	//List<Vector3> penetrationAxes;
	//List<float> penetrationAxesDistance;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

		//if ( CheckCollision(_cubeA, _cubeB))
		//{
		//	_cubeA.Hit = _cubeB.Hit = true;

		//}
		//else
		//{
		//	_cubeA.Hit = _cubeB.Hit = false;
		//}


	}

	public bool CheckCollision( Transform a, Mesh aMesh, Transform b, Mesh bMesh)
	{
		//minOverlap = 0;
		//minOverlapAxis = Vector3.zero;

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
			//Debug.DrawRay(nodePos, aAxes[0] * 2f, Color.red);
			//Debug.DrawRay(nodePos, aAxes[1] * 2f, Color.green);
			//Debug.DrawRay(nodePos, aAxes[2] * 2f, Color.blue);

		}
		//Debug.DrawRay(a.transform.position, aAxes[0] * 2f, Color.red);
		//Debug.DrawRay(a.transform.position, aAxes[1] * 2f, Color.green);
		//Debug.DrawRay(a.transform.position, aAxes[2] * 2f, Color.blue);

		//Debug.DrawRay(b.transform.position, bAxes[0] * 2f, Color.red);
		//Debug.DrawRay(b.transform.position, bAxes[1] * 2f, Color.green);
		//Debug.DrawRay(b.transform.position, bAxes[2] * 2f, Color.blue);

		int aAxesLength = aAxes.Length;
		int bAxesLength = bAxes.Length;

		aVertices = GetVertices(a,aMesh);
		bVertices = GetVertices(b,bMesh);

		int aVertsLength = aVertices.Length;
		int bVertsLength = bVertices.Length;

		//penetrationAxes = new List<Vector3>();
		//penetrationAxesDistance = new List<float>();

		bool hasOverlap = false;

		if ( ProjectionHasOverlap(a, b, AllAxes.Length, AllAxes, bVertsLength, bVertices, aVertsLength, aVertices, Color.red, Color.green) )
		{
			hasOverlap = true;
		}
		else if (ProjectionHasOverlap(b, a, AllAxes.Length, AllAxes, aVertsLength, aVertices, bVertsLength, bVertices, Color.green, Color.red) )
		{
			hasOverlap = true;
		}

		// Penetration can be seen here, but its not reliable 
		//Debug.Log(minOverlap+" : "+minOverlapAxis);

		return hasOverlap;
	}

	/// Detects whether or not there is overlap on all separating axes.
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
		//bool hasOverlap = true;

		//minOverlap = float.PositiveInfinity;

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

			//if ( overlap < minOverlap )
			{
				//minOverlap = overlap;
				//minOverlapAxis = axis;

				//penetrationAxes.Add(axis);
				//penetrationAxesDistance.Add(overlap);

			}

			//Debug.Log(overlap);

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
				//dupArray[j] = parentMatrix * dupArray[j];
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
			//Vector3 x = parentMatrix.GetColumn(0);
			//Vector3 y = parentMatrix.GetColumn(1);
			//Vector3 z = parentMatrix.GetColumn(2);
			axes = new[]
			{
				((Vector3)parentMatrix.GetColumn(0).normalized),
				((Vector3)parentMatrix.GetColumn(1).normalized),
				((Vector3)parentMatrix.GetColumn(2).normalized)
			};
			//Vector3 axisA = Vector3.Cross(Vector3.up, y.normalized);
			//float angleA = Mathf.Acos(Vector3.Dot(Vector3.up, y.normalized)) * Mathf.Rad2Deg;
			//Quaternion rotation = Quaternion.AngleAxis(angleA, axisA);

		}
		
		return axes;

	}
}