using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriveForklift : MonoBehaviour
{
    public GameObject forkliftBase;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Quaternion q = Quaternion.AngleAxis(-0.1f, Vector3.up);
            forkliftBase.transform.localRotation = q * forkliftBase.transform.localRotation;
        } else if (Input.GetKey(KeyCode.A))
        {
            forkliftBase.transform.localPosition += forkliftBase.transform.right * -0.01f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Quaternion q = Quaternion.AngleAxis(0.1f, Vector3.up);
            forkliftBase.transform.localRotation = q * forkliftBase.transform.localRotation;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            forkliftBase.transform.localPosition += forkliftBase.transform.right * 0.01f;
        }
    }
}
