using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriveForklift : MonoBehaviour
{
    public CameraManipulation forkliftCams = null;
    public float direction;

    public SceneNode forkliftBase;
    public GameObject frame;

    public SceneNode forksSceneNode;
    public GameObject leftFork;
    public GameObject rightFork;

    public TheWorld world = null;

    public Vector3 startingPosition;
    public bool collision;

    void Start()
    {
        Debug.Assert(forkliftCams != null);
        Debug.Assert(world != null);
    }

    void Update()
    {
        startingPosition = forkliftBase.transform.localPosition;
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
        checkCollision();
        if (collision)
        {
            forkliftBase.transform.localPosition = startingPosition;
            collision = false;
        }
        forkliftCams.UpdateCameras();
    }

    public void checkCollision()
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
                if (fineCollisionBody || fineCollisionLeftFork || fineCollisionRightFork)
                {
                    Debug.Log("Fine Collision!");
                    collision = true;
                } 
            }
            
        }
        
    }


}
