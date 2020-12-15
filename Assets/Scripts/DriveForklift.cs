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

    public bool draggingFront;
    public bool draggingForks;

    private int collisionCount = 0;

    public Transform selectedPallet;

    public float dragMod = 50f;

    public MainController controller;
    public float acceleration = 10f;
    public float friction = 0.02f;
    private float velocity = 0;

    void Start()
    {
        Debug.Assert(forkliftCams != null);
        Debug.Assert(world != null);
        draggingFront = false;
        draggingForks = false;
    }

    void Update()
    {
        if (Mathf.Abs(velocity) < 0.01f)
        {
            velocity = 0;
        }
        bool movedForward = false;
        bool movedBackward = false;
        bool rotatedLeft = false;
        bool rotatedRight = false;
        bool frontMoved = false;
        bool forksMoved = false;
        bool rolledForward = false;
        
        
        Quaternion lastFrontRotation = Quaternion.identity;
        Vector3 lastForksPosition = Vector3.zero;
        if (controller.timeRemaining > 0 || controller.ignoreTimer)
        {
            if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
            {
                if (velocity < 8)
                    velocity += Mathf.Max(acceleration * Time.deltaTime);
                //frameSceneNode.transform.position += frameSceneNode.transform.right * movementMod;
                movedForward = true;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                if (velocity > -8)
                    velocity -= (acceleration * Time.deltaTime);
                //frameSceneNode.transform.position -= frameSceneNode.transform.right * movementMod;
                movedBackward = true;
            }
            Quaternion rotateLeft = Quaternion.identity;
            Quaternion rotateRight = Quaternion.identity;
            if (velocity != 0)
            {
                rotateLeft = Quaternion.AngleAxis(-(1 - Mathf.Log(Mathf.Abs(velocity), 12)) * velocity * 30 * Time.deltaTime, Vector3.up);
                rotateRight = Quaternion.AngleAxis((1 - Mathf.Log(Mathf.Abs(velocity), 12)) * velocity * 30 * Time.deltaTime, Vector3.up);
            } 
            
            if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            {
                rotatedLeft = true;
                frameSceneNode.transform.localRotation = rotateLeft * frameSceneNode.transform.localRotation;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                //Debug.Log("RotatingRight");
                rotatedRight = true;
                frameSceneNode.transform.localRotation = rotateRight * frameSceneNode.transform.localRotation;
            }

            /*if(Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
            {
                frameSceneNode.transform.position += frameSceneNode.transform.right * movementMod;
                movedForward = true;
            } else if(Input.GetKey(KeyCode.S))
            {
                frameSceneNode.transform.position -= frameSceneNode.transform.right * movementMod;
                movedBackward = true;
            }
            if(Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            {
                rotatedLeft = true;
                frameSceneNode.transform.localRotation = rotateLeft * frameSceneNode.transform.localRotation;
            } else if (Input.GetKey(KeyCode.D))
            {
                //Debug.Log("RotatingRight");
                rotatedRight = true;
                frameSceneNode.transform.localRotation = rotateRight * frameSceneNode.transform.localRotation;
            }*/
            if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftAlt))
            {
                Ray ray = forkliftCams.getSecondaryCamRay();
                if (CheckRayPartIntersection(ray, leftFront))
                {
                    draggingFront = true;
                }
                else if (CheckRayPartIntersection(ray, rightFront))
                {
                    draggingFront = true;
                }
                else if (CheckRayPartIntersection(ray, leftFork.gameObject))
                {
                    draggingForks = true;
                }
                else if (CheckRayPartIntersection(ray, rightFork.gameObject))
                {
                    draggingForks = true;
                }
            }
            if (draggingFront)
            {
                frontMoved = true;
                lastFrontRotation = frontEndSceneNode.transform.localRotation;
                Matrix4x4 nodeMatrix = frontEndSceneNode.getCombinedMatrix();
                Vector3 frontForward = nodeMatrix.GetColumn(0).normalized;
                Vector3 frontRight = -Vector3.forward;

                float xDist = Input.GetAxis("Mouse X");
                float yAngle = Mathf.Acos(Vector3.Dot(Vector3.up, frontForward)) * Mathf.Rad2Deg;
                if (xDist < 0 && yAngle < 70)
                {
                    xDist = 0;
                }
                if (xDist > 0 && yAngle > 102)
                {
                    xDist = 0;
                }
                Quaternion rot = Quaternion.AngleAxis(xDist, -Vector3.forward);
                frontEndSceneNode.transform.localRotation *= rot;
            }
            if (draggingForks)
            {
                forksMoved = true;
                lastForksPosition = forksSceneNode.transform.localPosition;
                Matrix4x4 forkMatrix = forksSceneNode.getCombinedMatrix();
                Vector3 forwardDir = forkMatrix.GetColumn(0).normalized;
                Vector2 screenMouseDir = Vector2.zero;
                float yAngle = Mathf.Acos(Vector3.Dot(Vector3.up, forwardDir)) * Mathf.Rad2Deg;
                screenMouseDir.x = Input.GetAxis("Mouse X");
                screenMouseDir.y = Input.GetAxis("Mouse Y");
                float dist = (screenMouseDir.y + (screenMouseDir.x * ((yAngle - 90) / 90))) * dragMod * 15;
                if (dist > 0 && forksSceneNode.transform.localPosition.y > 5.6)
                {
                    dist = 0;
                }
                if (dist < 0 && forksSceneNode.transform.localPosition.y < -0.4f)
                {
                    dist = 0;
                }
                forksSceneNode.transform.localPosition += Vector3.up * dist;
                //selected.localPosition += dir * dist;
                //AxisFrame.localPosition = selected.position;
            }
            if (Input.GetMouseButtonUp(0))
            {
                draggingFront = false;
                draggingForks = false;
            }
            Debug.Log(velocity);
            float movementMod = velocity * Time.deltaTime;
            if (velocity != 0)
            {
                frameSceneNode.transform.position += frameSceneNode.transform.right * movementMod;
                rolledForward = true;
            }
            //UPDATE THE FORKLIFT SCENE HIERARCHY!!
            Matrix4x4 i = Matrix4x4.identity;
            frameSceneNode.CompositeXform(ref i);
            //SERIOUSLY IF THIS IS NOT UPDATED COLLISION DOES NOT WORK
            if (velocity != 0)
            {
                Transform palletCollision = checkPalletCollision();
                bool shelfCollision = checkShelfCollision();
                if (selectedPallet != null)
                {
                    pickUpPallet();
                }
                if ((palletCollision != null && palletCollision != selectedPallet) || shelfCollision)
                {
                    if (movedForward || movedBackward || rolledForward)
                    {
                        frameSceneNode.transform.position -= (frameSceneNode.transform.right * movementMod);
                    }
                    if (rotatedLeft)
                    {
                        frameSceneNode.transform.localRotation = rotateRight * frameSceneNode.transform.localRotation;
                    }
                    if (rotatedRight)
                    {
                        frameSceneNode.transform.localRotation = rotateLeft * frameSceneNode.transform.localRotation;
                    }
                    if (frontMoved)
                    {
                        frontEndSceneNode.transform.localRotation = lastFrontRotation;
                    }
                    if (forksMoved)
                    {
                        forksSceneNode.transform.localPosition = lastForksPosition;
                    }
                    velocity = -velocity / 2;
                }

            }
            if(velocity != 0 && !movedForward && !movedBackward )
            {
                //Debug.Log("Decelerating");
                velocity -= ((friction * velocity) + (Mathf.Cos(velocity/8)* (friction * velocity))) * Time.deltaTime;
            }
                
        }
        
        forkliftCams.UpdateCameras();
    }

    public bool checkShelfCollision()
    {
        ArrayList toTest = world.testShelfCollision(transform);
        if(toTest.Count != 0)
        {
            //Debug.Log("Rough Colliding!");
        }
        foreach (Transform xform in toTest)
        {
            //Debug.Log("With: " + xform.name);
            foreach (Transform childform in xform)
            {
                if (world.SAT.CheckCollision(frame.transform, frame.GetComponent<MeshFilter>().mesh, childform, childform.GetComponent<MeshFilter>().mesh))
                    return true;
                if (world.SAT.CheckCollision(leftFork.transform, leftFork.GetComponent<MeshFilter>().mesh, childform, childform.GetComponent<MeshFilter>().mesh))
                    return true;
                if (world.SAT.CheckCollision(rightFork.transform, leftFork.GetComponent<MeshFilter>().mesh, childform, childform.GetComponent<MeshFilter>().mesh))
                    return true;
                if (world.SAT.CheckCollision(leftFront.transform, leftFront.GetComponent<MeshFilter>().mesh, childform, childform.GetComponent<MeshFilter>().mesh))
                    return true;
                if (world.SAT.CheckCollision(rightFront.transform, rightFront.GetComponent<MeshFilter>().mesh, childform, childform.GetComponent<MeshFilter>().mesh))
                    return true;
            }
            
        }
        return false;
    }
    bool CheckRayPartIntersection(Ray ray, GameObject obj)
    {
        Vector3 rayOrigin = ray.origin;
        Vector3 rayDirection = ray.direction;
        //Debug.DrawRay(rayOrigin, rayDirection * 10, Color.yellow, 10);
        NodePrimitive node = obj.GetComponent<NodePrimitive>();
        Matrix4x4 trsMatrix = node.getNodeMatrix();
        Vector3 nodePos = trsMatrix.GetColumn(3);
        

        Vector3 xAxis = trsMatrix.GetColumn(0).normalized;
        Vector3 yAxis = trsMatrix.GetColumn(1).normalized;
        Vector3 zAxis = trsMatrix.GetColumn(2).normalized;

        

        //Debug.DrawRay(nodePos, xAxis, Color.red, 5);
        //Debug.DrawRay(nodePos, yAxis, Color.green, 5);
        //Debug.DrawRay(nodePos, zAxis, Color.blue, 5);

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
        nodePos = trsMatrix.GetColumn(3);
        xAxis = trsMatrix.GetColumn(0).normalized;
        yAxis = trsMatrix.GetColumn(1).normalized;
        zAxis = trsMatrix.GetColumn(2).normalized;
        //Debug.DrawRay(nodePos, xAxis, Color.red,5);
        //Debug.DrawRay(nodePos, yAxis, Color.green,5);
        //Debug.DrawRay(nodePos, zAxis, Color.blue,5);
        Bounds bound = obj.GetComponent<BoxCollider>().bounds;
        //Debug.DrawLine(nodePos, nodePos + xAxis * bound.max.x + yAxis * bound.max.y + zAxis * bound.max.z, Color.white, 10);
        //Debug.DrawLine(nodePos, nodePos + xAxis * bound.max.x + yAxis * bound.max.y + zAxis * bound.min.z, Color.white, 10);
        //Debug.DrawLine(nodePos, nodePos + xAxis * bound.max.x + yAxis * bound.min.y + zAxis * bound.max.z, Color.white, 10);
        //Debug.DrawLine(nodePos, nodePos + xAxis * bound.max.x + yAxis * bound.min.y + zAxis * bound.min.z, Color.white, 10);
        //Debug.DrawLine(nodePos, nodePos + xAxis * bound.min.x + yAxis * bound.max.y + zAxis * bound.max.z, Color.white, 10);
        //Debug.DrawLine(nodePos, nodePos + xAxis * bound.min.x + yAxis * bound.max.y + zAxis * bound.min.z, Color.white, 10);
        //Debug.DrawLine(nodePos, nodePos + xAxis * bound.min.x + yAxis * bound.min.y + zAxis * bound.max.z, Color.white, 10);
        //Debug.DrawLine(nodePos, nodePos + xAxis * bound.min.x + yAxis * bound.min.y + zAxis * bound.min.z, Color.white, 10);
        //Vector3 minBound = bound.min;
        //Vector3 maxBound = bound.max;
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
    
    public Transform checkPalletCollision()
    {
        ArrayList toTest = world.testPalletCollision(transform);
        Vector3 forksPos = (leftFork.getNodeMatrix().GetColumn(3) + rightFork.getNodeMatrix().GetColumn(3)) / 2;
        //Debug.Log(toTest.Count);
        
        foreach (Transform palletXForm in toTest)
        {
            if(palletXForm == selectedPallet)
            {
                return selectedPallet;
            }
            Vector3 deltaVec = Vector3.zero;
            float dist = 10;
            float dirVal = -1000;
            //Debug.Log(palletXForm.name);

            foreach (Transform partGroup in palletXForm)
            {
                deltaVec = Vector3.zero;
                if(partGroup.name == "Top")
                {
                    deltaVec = partGroup.position - forksPos;
                    dist = deltaVec.magnitude;
                    dirVal = Vector3.Dot(deltaVec, Vector3.up);
                }
                foreach (Transform part in partGroup)
                {
                    bool fineCollisionLeftFork = world.SAT.CheckCollision(leftFork.transform, leftFork.GetComponent<MeshFilter>().mesh, part, part.GetComponent<MeshFilter>().mesh);
                    bool fineCollisionRightFork = world.SAT.CheckCollision(rightFork.transform, rightFork.GetComponent<MeshFilter>().mesh, part, part.GetComponent<MeshFilter>().mesh);
                    if(fineCollisionLeftFork || fineCollisionRightFork)
                    {
                        if(dist < 1 && dirVal > 0)
                        {
                            selectedPallet = palletXForm;
                            return selectedPallet;
                        }
                        return palletXForm;
                    }
                    //if (fineCollisionLeftFork && fineCollisionRightFork)
                    //{
                    //    //Debug.Log("Pick up pallet");
                    //    selectedPallet = palletXForm;
                    //    return true;
                    //} else if (fineCollisionLeftFork || fineCollisionRightFork)
                    //{
                    //    return true;
                    //}
                }
            }
        }
        return null;
    }

    /*public void pickUpPallet()
    {
        Matrix4x4 forksSceneMatrix = forksSceneNode.getCombinedMatrix();
        Matrix4x4 leftMatrix = leftFork.getNodeMatrix();
        Vector3 leftPosition = leftMatrix.GetColumn(3); 
        Matrix4x4 rightMatrix = rightFork.getNodeMatrix();
        Vector3 rightPosition = rightMatrix.GetColumn(3);
        //float newX = (leftPosition.x + rightPosition.x) / 2;
        float newX = leftPosition.x + 0.5f;
        float newY = leftPosition.y - 0.1f;
        float newZ = (leftPosition.z + rightPosition.z) / 2;
        selectedPallet.localPosition = new Vector3(newX, newY, newZ);
        Vector3 forkUp = forksSceneMatrix.GetColumn(1).normalized;
        Vector3 forkForward = forksSceneMatrix.GetColumn(2).normalized;
        selectedPallet.localRotation = Quaternion.FromToRotation(Vector3.up, forkUp);
        selectedPallet.localRotation *= Quaternion.FromToRotation(Vector3.forward, forkForward);

    }*/

    public void pickUpPallet()
    {
        Matrix4x4 forksSceneMatrix = forksSceneNode.getCombinedMatrix();
        Matrix4x4 leftMatrix = leftFork.getNodeMatrix();
        Vector3 leftPosition = leftMatrix.GetColumn(3);
        Matrix4x4 rightMatrix = rightFork.getNodeMatrix();
        Vector3 rightPosition = rightMatrix.GetColumn(3);
        //float newX = (leftPosition.x + rightPosition.x) / 2;
        //float newX = leftPosition.x + 0.f;
        //float newY = leftPosition.y - 0.1f;
        //float newZ = (leftPosition.z + rightPosition.z) / 2;
        Vector3 forkup = forksSceneMatrix.GetColumn(1).normalized;
        Vector3 forkforward = forksSceneMatrix.GetColumn(2).normalized;
        Vector3 forkright = forksSceneMatrix.GetColumn(0).normalized;
        leftPosition = (leftPosition + rightPosition) / 2;
        //leftPosition -= forkforward * 0.3f;
        leftPosition += forkright*0.1f;
        leftPosition -= forkup * 0.05f;
        selectedPallet.localPosition = leftPosition;
        Vector4 newRotation = leftMatrix.GetColumn(2);

        selectedPallet.localRotation = Quaternion.FromToRotation(Vector3.up, forkup);
        selectedPallet.localRotation *= Quaternion.FromToRotation(Vector3.forward, forkforward);
    }

}