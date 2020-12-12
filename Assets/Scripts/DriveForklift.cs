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
    public NodePrimitive leftFork;
    public NodePrimitive rightFork;

    public SceneNode frontEndSceneNode;
    public GameObject leftFront;
    public GameObject rightFront;

    public TheWorld world = null;

    public Vector3 lastPosition;
    public bool collision;
    public bool draggingFront;
    public bool draggingForks;
    public Transform selectedPallet;

    void Start()
    {
        Debug.Assert(forkliftCams != null);
        Debug.Assert(world != null);
        draggingFront = false;
        draggingForks = false;
    }

    void Update()
    {
        if (!checkShelfCollision())
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
        bool isColliding = checkShelfCollision();
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
            } else if (CheckRayPartIntersection(ray, leftFork.gameObject))
            {
                draggingForks = true;
            } else if (CheckRayPartIntersection(ray, rightFork.gameObject))
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
        bool canPickUp = checkPalletCollision();
        if (canPickUp)
        {
            pickUpPallet();
        }

        forkliftCams.UpdateCameras();
    }

    public bool checkShelfCollision()
    {
        ArrayList toTest = world.testShelfCollision(transform);
        if(toTest.Count != 0)
        {
            Debug.Log("Rough Colliding!");
        }
        foreach (Transform xform in toTest)
        {
            Debug.Log("With: " + xform.name);
            foreach (Transform childform in xform)
            {
                bool fineCollisionBody = world.SAT.CheckCollision(frame.transform, frame.GetComponent<MeshFilter>().mesh, childform, childform.GetComponent<MeshFilter>().mesh);
                bool fineCollisionLeftFork = world.SAT.CheckCollision(leftFork.transform, leftFork.GetComponent<MeshFilter>().mesh, childform, childform.GetComponent<MeshFilter>().mesh);
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
    public bool checkPalletCollision()
    {
        ArrayList toTest = world.testPalletCollision(transform);
        foreach (Transform palletXForm in toTest)
        {
            foreach (Transform partGroup in palletXForm)
            {
                foreach (Transform part in partGroup)
                {
                    bool fineCollisionLeftFork = world.SAT.CheckCollision(leftFork.transform, leftFork.GetComponent<MeshFilter>().mesh, part, part.GetComponent<MeshFilter>().mesh);
                    bool fineCollisionRightFork = world.SAT.CheckCollision(rightFork.transform, rightFork.GetComponent<MeshFilter>().mesh, part, part.GetComponent<MeshFilter>().mesh);
                    if (fineCollisionLeftFork || fineCollisionRightFork)
                    {
                        Debug.Log("Fine Collison");
                        selectedPallet = palletXForm;
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void pickUpPallet()
    {
        Matrix4x4 leftMatrix = leftFork.getNodeMatrix();
        Vector4 leftPosition = leftMatrix.GetColumn(3); 
        Matrix4x4 rightMatrix = rightFork.getNodeMatrix();
        Vector4 rightPosition = rightMatrix.GetColumn(3);
        float newX = (leftPosition.x + rightPosition.x) / 2;
        float newY = leftPosition.y;
        float newZ = (leftPosition.z + rightPosition.z) / 2;
        selectedPallet.localPosition = new Vector3(newX, newY, newZ);
    }

}
