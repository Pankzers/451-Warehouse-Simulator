using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriveForklift : MonoBehaviour
{
    public CameraManipulation forkliftCams = null;
    public float direction;

    public SceneNode frameSceneNode;
    public GameObject frame;

    public SceneNode forksSceneNode;
    public GameObject leftFork;
    public GameObject rightFork;

    public SceneNode frontEndSceneNode;
    public GameObject leftFront;
    public GameObject rightFront;


    public TheWorld world = null;

    public Vector3 lastPosition;
    public bool collision;
    public bool draggingFront;
    public bool draggingForks;

    void Start()
    {
        Debug.Assert(forkliftCams != null);
        Debug.Assert(world != null);
        draggingFront = false;
        draggingForks = false;
    }

    void Update()
    {
        if (!checkCollision())
        {
            lastPosition = frameSceneNode.transform.localPosition;
        }
        if (frameSceneNode.transform.right.x < 0)
        {
            direction = -1;
        } else
        {
            direction = 1;
        }
        if (Input.GetKey(KeyCode.W))
        {
            Quaternion q = Quaternion.AngleAxis(-0.1f, Vector3.up);
            frameSceneNode.transform.localRotation = q * frameSceneNode.transform.localRotation;
        } else if (Input.GetKey(KeyCode.A))
        {
            frameSceneNode.transform.localPosition += frameSceneNode.transform.right * 0.01f * -direction;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Quaternion q = Quaternion.AngleAxis(0.1f, Vector3.up);
            frameSceneNode.transform.localRotation = q * frameSceneNode.transform.localRotation;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            frameSceneNode.transform.localPosition += frameSceneNode.transform.right * 0.01f * direction;
        }
        bool isColliding = checkCollision();
        if (isColliding)
        {
            frameSceneNode.transform.localPosition = lastPosition;
        }
        if(Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftAlt))
        {
            Ray ray = forkliftCams.getSecondaryCamRay();
            if(CheckRayPartIntersection(ray, leftFront))
            {
                draggingFront = true;
            } else if(CheckRayPartIntersection(ray, rightFront))
            {
                draggingFront = true;
            } else if (CheckRayPartIntersection(ray, leftFork))
            {
                draggingForks = true;
            } else if (CheckRayPartIntersection(ray, rightFork))
            {
                draggingForks = true;
            }
        }
        if(draggingFront)
        {

        }
        if(draggingForks)
        {

        }
        if(Input.GetMouseButtonUp(0))
        {
            draggingFront = false;
            draggingForks = false;
        }
        forkliftCams.UpdateCameras();
    }

    public bool checkCollision()
    {
        ArrayList toTest = world.testCollision(transform);
        if(toTest.Count != 0)
        {
            Debug.Log("Rough Colliding!");
        }
        //bool fineCollision = world.SAT.CheckCollision()
        foreach (Transform xform in toTest)
        {
            Debug.Log("With: " + xform.name);
            foreach (Transform childform in xform)
            {
                bool fineCollisionBody = world.SAT.CheckCollision(frame.transform, frame.GetComponent<MeshFilter>().mesh, childform, childform.GetComponent<MeshFilter>().mesh);
                bool fineCollisionLeftFork = world.SAT.CheckCollision(leftFork.transform, leftFork.GetComponent<MeshFilter>().mesh, childform, childform.GetComponent<MeshFilter>().mesh);
                //bool fineCollisionLeftFork = world.SAT.CheckCollision(leftFork.transform, xform);
                //bool fineCollisionRightFork = world.SAT.CheckCollision(rightFork.transform, xform);
                bool fineCollisionRightFork = world.SAT.CheckCollision(rightFork.transform, leftFork.GetComponent<MeshFilter>().mesh, childform, childform.GetComponent<MeshFilter>().mesh);
                bool fineCollisionLeftFront = world.SAT.CheckCollision(leftFront.transform, leftFront.GetComponent<MeshFilter>().mesh, childform, childform.GetComponent<MeshFilter>().mesh);
                bool fineCollisionRightFront = world.SAT.CheckCollision(rightFront.transform, rightFront.GetComponent<MeshFilter>().mesh, childform, childform.GetComponent<MeshFilter>().mesh);
                if (fineCollisionBody || fineCollisionLeftFork || fineCollisionRightFork || fineCollisionLeftFront || fineCollisionRightFront)
                {
                    Debug.Log("Fine Collision!");
                    return true;
                }
            }
            
        }
        return false;
    }
    bool CheckRayPartIntersection(Ray ray, GameObject obj)
    {
        Vector3 test = new Vector3(1, 1, 1);
        
        Vector3 rayOrigin = ray.origin;
        Vector3 rayDirection = ray.direction;
        Debug.Log(rayOrigin);
        Debug.Log(rayDirection);
        Debug.DrawRay(rayOrigin, rayDirection, Color.blue, 10);
        NodePrimitive node = obj.GetComponent<NodePrimitive>();
        Matrix4x4 trsMatrix = node.getNodeMatrix();
        Matrix4x4 invtrsMatrix = trsMatrix.inverse;
        rayOrigin = invtrsMatrix * rayOrigin;
        rayDirection = (Vector3)(invtrsMatrix * rayDirection).normalized;
        test = invtrsMatrix * test;
        Debug.Log("Test Point: " + test);
        Debug.Log("Reverse Test Point: " + trsMatrix * test);
        Debug.Log(rayOrigin);
        Debug.Log(rayDirection);
        Debug.DrawRay(rayOrigin, rayDirection, Color.red, 10);
        return false;
    }

}
