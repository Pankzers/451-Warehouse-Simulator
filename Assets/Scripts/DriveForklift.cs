using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriveForklift : MonoBehaviour
{
    public CameraManipulation forkliftCams = null;
    public float direction;
    //public bool collision = true;

    public SceneNode forkliftBase;
    public GameObject frame;

    public SceneNode forksSceneNode;
    public GameObject leftFork;
    public GameObject rightFork;
    public bool leftCollide, rightCollide;

    public GameObject box;

    void Start()
    {
        Debug.Assert(forkliftCams != null);
    }

    void Update()
    {
        checkCollision();
        if (forkliftBase.transform.right.x < 0)
        {
            direction = -1;
        } else
        {
            direction = 1;
        }
        if (Input.GetKey(KeyCode.W))
        {
            Quaternion q = Quaternion.AngleAxis(-0.1f, Vector3.up);
            forkliftBase.transform.localRotation = q * forkliftBase.transform.localRotation;
        } else if (Input.GetKey(KeyCode.A))
        {
            forkliftBase.transform.localPosition += forkliftBase.transform.right * 0.01f * -direction;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Quaternion q = Quaternion.AngleAxis(0.1f, Vector3.up);
            forkliftBase.transform.localRotation = q * forkliftBase.transform.localRotation;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            forkliftBase.transform.localPosition += forkliftBase.transform.right * 0.01f * direction;
        }

        forkliftCams.UpdateCameras();
    }

    /*public void checkCollision()
    {
        Matrix4x4 parentMatrix = forksSceneNode.getCombinedMatrix();
        Vector3 position = parentMatrix.GetColumn(3);
        Mesh leftMesh = leftFork.GetComponent<MeshFilter>().mesh;
        Vector3[] leftVertices = leftMesh.vertices;
        Mesh rightMesh = rightFork.GetComponent<MeshFilter>().mesh;
        Vector3[] rightVertices = rightMesh.vertices;
        Collider collider = box.transform.GetComponent<Collider>();
        for (int i = 0; i < rightVertices.Length; i++)
        {
            if (collider.bounds.Contains(leftVertices[i] + position))
            {
                leftCollide = true;
            } else
            {
                leftCollide = false;
            }
            if (collider.bounds.Contains(rightVertices[i] + position))
            {
                rightCollide = true;
            } else
            {
                rightCollide = false;
            }
        }
    }*/

    public void checkCollision()
    {
        Matrix4x4 parentMatrix = forksSceneNode.getCombinedMatrix();
        Vector3 position = parentMatrix.GetColumn(3);
        Mesh leftMesh = leftFork.GetComponent<MeshFilter>().mesh;
        Vector3[] leftVertices = leftMesh.vertices;
        Mesh rightMesh = rightFork.GetComponent<MeshFilter>().mesh;
        Vector3[] rightVertices = rightMesh.vertices;
        BoxCollider collider = box.transform.GetComponent<BoxCollider>();
        for (int i = 0; i < leftVertices.Length; i++)
        {
            Vector3 leftPoint = leftVertices[i] + position;
            leftPoint = collider.transform.InverseTransformPoint(leftPoint) - collider.center;
            Vector3 rightPoint = rightVertices[i] + position;
            rightPoint = collider.transform.InverseTransformPoint(rightPoint) - collider.center;
            float halfX = (collider.size.x * 0.5f);
            float halfY = (collider.size.y * 0.5f);
            float halfZ = (collider.size.z * 0.5f);
            if (leftPoint.x < halfX && leftPoint.x > -halfX && leftPoint.y < halfY && leftPoint.y > -halfY && leftPoint.z < halfZ && leftPoint.z > -halfZ)
            {
                leftCollide = true;
            } else
            {
                leftCollide = false;
            }
            if (rightPoint.x < halfX && rightPoint.x > -halfX && rightPoint.y < halfY && rightPoint.y > -halfY && rightPoint.z < halfZ && rightPoint.z > -halfZ)
            {
                rightCollide = true;
            }
            else
            {
                rightCollide = false;
            }
        }
    }


}
