using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticWorldMesh : MonoBehaviour
{

	public int VertexCount = 8;
	public Vector3[] vertices;
	// Start is called before the first frame update
	void Start()
	{
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		vertices = removeDuplicateVertices(mesh.vertices, transform);
	}

	private Vector3[] removeDuplicateVertices(Vector3[] dupArray, Transform xform)
	{

		for (int j = 0; j < dupArray.Length; j++)
		{
			dupArray[j] = xform.TransformPoint(dupArray[j]);
		}

		Vector3[] newArray = new Vector3[VertexCount];  //change 8 to a variable dependent on shape
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
}
