using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TheWorld : MonoBehaviour
{
    public Transform Aisles = null;
    public SceneNode TheRoot;
    public SeparatingAxisTest SAT = null;

    private void Start()
    {
        Debug.Assert(Aisles != null);
        Debug.Assert(SAT != null);
    }

    private void Update()
    {
        Matrix4x4 i = Matrix4x4.identity;
        TheRoot.CompositeXform(ref i);
    }

    public ArrayList testCollision(Transform liftTransform)
    {
        ArrayList toTest = new ArrayList();
        BoxCollider liftCollider = liftTransform.GetComponent<BoxCollider>();

        //Test Aisles
        foreach (Transform Aisle in Aisles)
        {
            //Vector3 childPos = child.position;
            BoxCollider aisleCollider = Aisle.GetComponent<BoxCollider>();
            if(intersectColliders(liftCollider, aisleCollider))
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

    private bool intersectColliders(Collider one, Collider two)
    {
        return (one.bounds.min.x <= two.bounds.max.x && one.bounds.max.x >= two.bounds.min.x) &&
         (one.bounds.min.y <= two.bounds.max.y && one.bounds.max.y >= two.bounds.min.y) &&
         (one.bounds.min.z <= two.bounds.max.z && one.bounds.max.z >= two.bounds.min.z);
    }
}