﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TheWorld : MonoBehaviour
{
    public Transform Aisles = null;
    public Transform Pallets = null;
    public Transform Walls = null;
    public SceneNode TheRoot;
    public SeparatingAxisTest SAT = null;

    private void Start()
    {
        Physics.autoSimulation = true;
        Physics.IgnoreLayerCollision(0, 1);
        Debug.Assert(Aisles != null);
        Debug.Assert(Pallets != null);
        Debug.Assert(Walls != null);
        Debug.Assert(SAT != null);
    }

    /*private void Update()
    {
        Matrix4x4 i = Matrix4x4.identity;
        TheRoot.CompositeXform(ref i);
    }*/

    public ArrayList testShelfCollision(Transform liftTransform)
    {
        ArrayList toTest = new ArrayList();
        BoxCollider liftCollider = liftTransform.GetComponent<BoxCollider>();

        //Test Aisles
        foreach (Transform Aisle in Aisles)
        {
            //Vector3 childPos = child.position;
            BoxCollider aisleCollider = Aisle.GetComponent<BoxCollider>();
            if (intersectColliders(liftCollider, aisleCollider))
            {
                foreach (Transform Shelf in Aisle)
                {
                    BoxCollider shelfCollider = Shelf.GetComponent<BoxCollider>();
                    if (intersectColliders(liftCollider, shelfCollider))
                    {
                        toTest.Add(Shelf);
                    }
                }
            }

        }
        return toTest;
    }

    public bool testWallCollision(Transform liftTransform)
    {
        BoxCollider liftCollider = liftTransform.GetComponent<BoxCollider>();
        foreach (Transform wall in Walls)
        {
            BoxCollider wallCollider = wall.GetComponent<BoxCollider>();
            if (intersectColliders(liftCollider, wallCollider))
            {
                return true;
            }
        }
        return false;
    }

    public ArrayList testPalletCollision(Transform forkliftXForm)
    {
        float threshold = 7f;
        ArrayList toTest = new ArrayList();
        DriveForklift forkDrive = forkliftXForm.GetComponent<DriveForklift>();
        Vector3 forksPos = (forkDrive.leftFork.getNodeMatrix().GetColumn(3) + forkDrive.rightFork.getNodeMatrix().GetColumn(3)) / 2;
        foreach (Transform palletXForm in Pallets)
        {
            
            if (Vector3.Distance(forksPos, palletXForm.position) < threshold)
            {
                toTest.Add(palletXForm);
            }
        }
        return toTest;
    }

    public ArrayList testPalletShelfCollision(Transform palletXForm)
    {
        float threshold = 7f;
        ArrayList toTest = new ArrayList();

        Vector3 palletPos = palletXForm.position;
        foreach (Transform Aisle in Aisles)
        {
            foreach (Transform Shelf in Aisle)
            {
                Vector3 ShelfPos = Shelf.position;
                if (Vector3.Distance(ShelfPos, palletPos) < threshold)
                {
                    toTest.Add(Shelf);
                }
            }
        }
        return toTest;
    }

    private bool intersectColliders(Collider one, Collider two)
    {
        //Debug.Log(one.bounds.min.x);

        return (one.bounds.min.x <= two.bounds.max.x && one.bounds.max.x >= two.bounds.min.x) &&
         (one.bounds.min.y <= two.bounds.max.y && one.bounds.max.y >= two.bounds.min.y) &&
         (one.bounds.min.z <= two.bounds.max.z && one.bounds.max.z >= two.bounds.min.z);
    }
}