using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManipulation : MonoBehaviour
{
    public Camera mMainCamera = null;
    public Camera mSecondaryCamera = null;
    public Transform defaultMainLookPoint = null;
    public Transform defaultSecondaryLookPoint = null;

    private void Start()
    {
        Debug.Assert(mMainCamera != null);
        Debug.Assert(mSecondaryCamera != null);
        Debug.Assert(defaultMainLookPoint != null);
        Debug.Assert(defaultSecondaryLookPoint != null);
    }
    public void UpdateCameras()
    {
        ProcessMouseEvents();
        AimCamera(mMainCamera, defaultMainLookPoint);
        AimCamera(mSecondaryCamera, defaultSecondaryLookPoint);
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
                Quaternion q = Quaternion.AngleAxis(thetax, mMainCamera.transform.up);
                Quaternion q2 = Quaternion.AngleAxis(thetay, mMainCamera.transform.right);
                q = q2 * q;
                Matrix4x4 r = Matrix4x4.TRS(Vector3.zero, q, Vector3.one);
                Matrix4x4 invP = Matrix4x4.TRS(-defaultMainLookPoint.localPosition, Quaternion.identity, Vector3.one);
                r = invP.inverse * r * invP;
                Vector3 newCameraPos = r.MultiplyPoint(mMainCamera.transform.localPosition);
                mMainCamera.transform.localPosition = newCameraPos;

            }
            else if (Input.GetMouseButton(1))
            {
                float thetax = Input.GetAxis("Mouse X");
                float thetay = Input.GetAxis("Mouse Y");
                Vector3 LookPos = defaultSecondaryLookPoint.localPosition;
                Vector3 CamPos = mSecondaryCamera.transform.localPosition;
                defaultSecondaryLookPoint.localPosition = LookPos + thetax * mSecondaryCamera.transform.right + thetay * mSecondaryCamera.transform.up;
                mSecondaryCamera.transform.localPosition = CamPos + thetax * mSecondaryCamera.transform.right + thetay * mSecondaryCamera.transform.up;

            }
            mMainCamera.transform.localPosition = mMainCamera.transform.localPosition + Input.mouseScrollDelta.y * mMainCamera.transform.forward;
        }
    }

    void AimCamera(Camera cam, Transform LP)
    {
        Vector3 V = LP.localPosition - cam.transform.localPosition;
        V = V.normalized;
        Vector3 W = Vector3.Cross(-V, Vector3.up);
        Vector3 U = Vector3.Cross(W, -V);
        cam.transform.localRotation = Quaternion.FromToRotation(Vector3.up, U);
        Quaternion alignU = Quaternion.FromToRotation(cam.transform.forward, V);
        cam.transform.localRotation = alignU * cam.transform.localRotation;

    }
}
