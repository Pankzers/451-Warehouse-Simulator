using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontEndManipulation : MonoBehaviour
{
    public GameObject leftFork, rightFork;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            for (int i = 0; i < hits.Length; i++)
            {
                Debug.Log(hits[i].transform.name);
            }
        }
    }
}
