using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneNode : MonoBehaviour
{

    protected Matrix4x4 mCombinedParentXform;

    public Vector3 NodeOrigin = Vector3.zero;
    public List<NodePrimitive> PrimitiveList;

    public Camera primaryCamera;
    public Transform body;

    // Use this for initialization
    protected void Start()
    {
        InitializeSceneNode();
        // Debug.Log("PrimitiveList:" + PrimitiveList.Count);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void InitializeSceneNode()
    {
        mCombinedParentXform = Matrix4x4.identity;
    }

    // This must be called _BEFORE_ each draw!! 
    public void CompositeXform(ref Matrix4x4 parentXform)
    {
        Matrix4x4 orgT = Matrix4x4.Translate(NodeOrigin);
        Matrix4x4 trs = Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);
        
        mCombinedParentXform = parentXform * orgT * trs;
        //Quaternion rot = mCombinedParentXform
        //Debug.DrawRay(mCombinedParentXform.GetColumn(3), mCombinedParentXform.GetColumn(1).normalized * 5.0f, Color.green);
        //Debug.DrawRay(mCombinedParentXform.GetColumn(3), mCombinedParentXform.GetColumn(0).normalized * 5.0f, Color.red);
        //Debug.DrawRay(mCombinedParentXform.GetColumn(3), mCombinedParentXform.GetColumn(2).normalized * 5.0f, Color.blue);

        // propagate to all children
        foreach (Transform child in transform)
        {
            SceneNode cn = child.GetComponent<SceneNode>();
            if (cn != null)
            {
                cn.CompositeXform(ref mCombinedParentXform);
            }
        }

        // disenminate to primitives
        foreach (NodePrimitive p in PrimitiveList)
        {
            p.LoadShaderMatrix(ref mCombinedParentXform);
        }

        if (primaryCamera != null)
        {
            primaryCamera.gameObject.transform.localPosition = mCombinedParentXform.MultiplyPoint(new Vector3(-4, 4, 0));
            primaryCamera.gameObject.transform.localRotation = getQuaternion(body.transform.right, Vector3.right);
            float yRotation = primaryCamera.gameObject.transform.localRotation.eulerAngles.y;
            if (yRotation > 90 && yRotation < 270)
            {
                //primaryCamera.gameObject.transform.localRotation *= Quaternion.Euler(0, 90, 0);
            }
        }

    }

    public Quaternion getQuaternion(Vector3 dir, Vector3 up)
    {
        if (dir == Vector3.zero)
        {
            return Quaternion.identity;
        }
        if (up != dir)
        {
            up.Normalize();
            var from = dir + up * -Vector3.Dot(up, dir);
            var to = Quaternion.FromToRotation(Vector3.forward, from);
            return Quaternion.FromToRotation(from, dir) * to;
        } else
        {
            return Quaternion.FromToRotation(Vector3.forward, dir);
        }
    }

    public Matrix4x4 getCombinedMatrix()
    {
        return mCombinedParentXform;
    }

}