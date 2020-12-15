using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManipulation : MonoBehaviour
{
    public SceneNode forks = null;
    public Camera mMainCamera = null;
    public Camera mSecondaryCamera = null;
    public Vector3 defaultMainLookPoint = new Vector3();
    public Vector3 defaultSecondaryLookPoint = new Vector3();

    public Vector3 MainCamPos = new Vector3();
    public Vector3 SecondaryCamPos = new Vector3();

    public SceneNode mainCamNode;
    public SceneNode secondaryCamNode;

    private void Start()
    {
        Debug.Assert(mMainCamera != null);
        Debug.Assert(mSecondaryCamera != null);
        Debug.Assert(defaultMainLookPoint != null);
        Debug.Assert(defaultSecondaryLookPoint != null);
        Debug.Assert(forks != null);
        OnPreCull();
    }
    public void UpdateCameras()
    {
        ProcessMouseEvents();
        if(mainCamNode != null)
        {
            AimCamera(mMainCamera, defaultMainLookPoint, MainCamPos,mainCamNode);
        } else
        {
            AimCamera(mMainCamera, defaultMainLookPoint, MainCamPos);
        }
        if (secondaryCamNode != null)
        {
            AimCamera(mSecondaryCamera, defaultSecondaryLookPoint, SecondaryCamPos, secondaryCamNode);
        } else
        {
            AimCamera(mSecondaryCamera, defaultSecondaryLookPoint, SecondaryCamPos);
        }

            
    }

    void ProcessMouseEvents()
    {
        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
        {
            if (Input.GetMouseButton(0))
            {
                float thetax = Input.GetAxis("Mouse X");
                float thetay = Input.GetAxis("Mouse Y");
                float thetadiff = Mathf.Acos(Vector3.Dot(Vector3.up, mMainCamera.transform.forward)) * Mathf.Rad2Deg;
                if (thetadiff < 20)
                {
                    if (thetay < 0)
                    {
                        thetay = 0;
                    }
                }
                else if (thetadiff > 175)
                {
                    if (thetay > 0)
                    {
                        thetay = 0;
                    }
                }
                Quaternion q = Quaternion.AngleAxis(thetax, Vector3.up);
                Quaternion q2 = Quaternion.AngleAxis(thetay, Vector3.right);
                q = q2 * q;
                Matrix4x4 r = Matrix4x4.TRS(Vector3.zero, q, Vector3.one);
                Matrix4x4 invP = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
                r = invP.inverse * r * invP;
                Vector3 newCameraPos = r.MultiplyPoint(MainCamPos);
                MainCamPos = newCameraPos;

            }
            else if (Input.GetMouseButton(1))
            {
                float thetax = Input.GetAxis("Mouse X");
                float thetay = Input.GetAxis("Mouse Y");
                Vector3 LookPos = defaultSecondaryLookPoint;
                Vector3 CamPos = SecondaryCamPos;
                defaultSecondaryLookPoint.x += thetax;
                SecondaryCamPos.x += thetax;

            }
            MainCamPos = MainCamPos + Input.mouseScrollDelta.y * mMainCamera.transform.forward;
        }
    }

    void AimCamera(Camera cam, Vector3 LookPoint, Vector3 CamPos, SceneNode node = null)
    {
        Vector3 V;
        if(node != null)
        {
            Matrix4x4 parentNodeMatrix = node.getCombinedMatrix();
            cam.transform.localPosition = parentNodeMatrix * CamPos + parentNodeMatrix.GetColumn(3);
            //V = (Vector3)parentNodeMatrix.GetColumn(3) - cam.transform.localPosition;
            V = (Vector3)parentNodeMatrix.GetColumn(3) + (Vector3)(parentNodeMatrix*LookPoint) - cam.transform.localPosition;
            //Debug.Log("LP: " + parentNodeMatrix.GetColumn(3));
        } else
        {
            V = LookPoint - cam.transform.localPosition;
            Debug.Log("LP: " + LookPoint);
        }
        //Debug.Log("Camera Local Pos: " + cam.transform.localPosition);
        
        
        V = V.normalized;
        Vector3 W = Vector3.Cross(-V, Vector3.up);
        Vector3 U = Vector3.Cross(W, -V);
        cam.transform.localRotation = Quaternion.FromToRotation(Vector3.up, U);
        Quaternion alignU = Quaternion.FromToRotation(cam.transform.forward, V);
        cam.transform.localRotation = alignU * cam.transform.localRotation;

    }

    public Ray getSecondaryCamRay()
    {
        return mSecondaryCamera.ScreenPointToRay(Input.mousePosition);
    }

    private void OnPreCull()
    {
        if (mMainCamera != null)
        {
            mMainCamera.cullingMatrix = Matrix4x4.Ortho(-99999, 99999, -99999, 99999, 0.001f, 99999) *
                                Matrix4x4.Translate(Vector3.forward * -99999 / 2f) *
                                mMainCamera.worldToCameraMatrix;
        }

        if (mSecondaryCamera != null)
        {
            mSecondaryCamera.cullingMatrix = Matrix4x4.Ortho(-99999, 99999, -99999, 99999, 0.001f, 99999) *
                                Matrix4x4.Translate(Vector3.forward * -99999 / 2f) *
                                mSecondaryCamera.worldToCameraMatrix;
        }

    }
}
