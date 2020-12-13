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

    public float dragMod = 1000000f;

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
            if (CheckRayPartIntersection(ray, leftFront))
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
            Debug.Log("Dragging Front!");
            Matrix4x4 nodeMatrix = frontEndSceneNode.getCombinedMatrix();
            //Vector3 frontForward = nodeMatrix.GetColumn(2).normalized;
            Vector3 frontRight = -nodeMatrix.GetColumn(2).normalized;
            //Vector2 screenAxisDir = Vector2.zero;
            //Vector2 screenMouseDir = Vector2.zero;
            float xDist = Input.GetAxis("Mouse X");
            //screenMouseDir.y = Input.GetAxis("Mouse Y");
            //screenAxisDir.x = Vector3.Dot(frontForward, forkliftCams.mSecondaryCamera.transform.forward);
            //screenAxisDir.y = Vector3.Dot(frontForward, forkliftCams.mSecondaryCamera.transform.up);
            //float dist = Vector2.Dot(screenMouseDir, screenAxisDir.normalized) * dragMod;
            Quaternion rot = Quaternion.AngleAxis(xDist, frontRight);
            frontEndSceneNode.transform.localRotation = rot * frontEndSceneNode.transform.localRotation;
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
        Vector3 rayOrigin = ray.origin;
        Vector3 rayDirection = ray.direction;
        NodePrimitive node = obj.GetComponent<NodePrimitive>();
        Matrix4x4 trsMatrix = node.getNodeMatrix();
        Matrix4x4 p = Matrix4x4.TRS(-node.Pivot, Quaternion.identity, Vector3.one);
        trsMatrix = p * trsMatrix;
        /*
         * CODE CREDIT TO CALVIN1602 until end of method
         * https://github.com/opengl-tutorials/ogl/blob/master/misc05_picking/misc05_picking_custom.cpp
         * OpenGL Tutorial Repository and Project
         * This is an adaptation of the code used in MP3 for detection of ray intersection with OBB (Oriented Bounding Box).
         * Citation:
         * Calvin1602. “OpenGL Tutorials Misc05 Picking Custom.” misc05_picking_custom.Cpp, Opengl-Tutorials, 23 Feb. 2018, github.com/opengl-tutorials/ogl/blob/master/misc05_picking/misc05_picking_custom.cpp. 
         */
        float tMin = 0.0f;
        float tMax = 1000.0f;
        Vector3 nodePos = trsMatrix.GetColumn(3);
        Vector3 xAxis = trsMatrix.GetColumn(0).normalized;
        Vector3 yAxis = trsMatrix.GetColumn(1).normalized;
        Vector3 zAxis = trsMatrix.GetColumn(2).normalized;
        Bounds bound = obj.GetComponent<BoxCollider>().bounds;
        Vector3 rayDelta = nodePos - rayOrigin;
        {
            
            float e = Vector3.Dot(xAxis, rayDelta);
            float f = Vector3.Dot(rayDirection, xAxis);
            if (Mathf.Abs(f) > Mathf.Epsilon)
            {
                float t1 = (e + bound.min.x) / f;
                float t2 = (e + bound.max.x) / f;
                if(t1>t2)
                {
                    float w = t1;t1 = t2;t2 = w;
                }
                if (t2 < tMax)
                    tMax = t2;
                if (t1 > tMin)
                    tMin = t1;
                if(tMax < tMin)
                {
                    return false;
                }
            } else
            {
                if (-e + bound.min.x > 0.0f || -e + bound.max.x < 0.0f)
                    return false;
            }
        }
        {

            float e = Vector3.Dot(yAxis, rayDelta);
            float f = Vector3.Dot(rayDirection, yAxis);
            if (Mathf.Abs(f) > Mathf.Epsilon)
            {
                float t1 = (e + bound.min.y) / f;
                float t2 = (e + bound.max.y) / f;
                if (t1 > t2)
                {
                    float w = t1; t1 = t2; t2 = w;
                }
                if (t2 < tMax)
                    tMax = t2;
                if (t1 > tMin)
                    tMin = t1;
                if (tMax < tMin)
                {
                    return false;
                }
            }
            else
            {
                if (-e + bound.min.y > 0.0f || -e + bound.max.y < 0.0f)
                    return false;
            }
        }
        {

            float e = Vector3.Dot(zAxis, rayDelta);
            float f = Vector3.Dot(rayDirection, zAxis);
            if (Mathf.Abs(f) > Mathf.Epsilon)
            {
                float t1 = (e + bound.min.z) / f;
                float t2 = (e + bound.max.z) / f;
                if (t1 > t2)
                {
                    float w = t1; t1 = t2; t2 = w;
                }
                if (t2 < tMax)
                    tMax = t2;
                if (t1 > tMin)
                    tMin = t1;
                if (tMax < tMin)
                {
                    return false;
                }
            }
            else
            {
                if (-e + bound.min.z > 0.0f || -e + bound.max.z < 0.0f)
                    return false;
            }
        }
        return true;
        //END CODE CREDIT TO CALVIN1602
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
