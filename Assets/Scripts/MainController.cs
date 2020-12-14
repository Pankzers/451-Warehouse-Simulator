using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{

    public GameObject firstShelf;
    public GameObject secondShelf;
    public GameObject thirdShelf;
    public GameObject fourthShelf;
    public GameObject fifthShelf;
    public GameObject currShelf;

    public bool onFirst = true;
    public bool onSecond = false;
    public bool onThird = false;
    public bool onFourth = false;
    public bool onFifth = false;

    public GameObject palletPrefab;
    public Transform palletParent;
    private Transform pallet = null;

    public Color originalColor;
    public Material shelfMaterial;

    public Transform arrow = null;
    public Transform forklift = null;
    private DriveForklift forkDrive;

    void Start()
    {
        Debug.Assert(arrow != null);
        Debug.Assert(forklift != null);
        //rt = GameObject.Find("Arrow").GetComponent<RectTransform>();
        forkDrive = forklift.GetComponent<DriveForklift>();
    }

    void Update()
    {
        pallet = forkDrive.selectedPallet;
        if (pallet != null)
        {
            if (onFirst)
            {
                currShelf = firstShelf;
                if (onSecond)
                {
                    currShelf = secondShelf;
                    if (onThird)
                    {
                        currShelf = thirdShelf;
                        if (onFourth)
                        {
                            currShelf = fourthShelf;
                            if (onFifth)
                            {
                                currShelf = fifthShelf;
                            }
                        }
                    }
                }
            }
            displayDropOffLocation();
            newDropOffLocation();
        }
    }

    public void newDropOffLocation()
    {
        currShelf.GetComponent<Renderer>().material = null;
        currShelf.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
        float totalDistance = Vector3.Distance(pallet.position, currShelf.transform.position);
        float heightDifference = pallet.position.y - currShelf.transform.position.y;
        Debug.Log(totalDistance);
        if (totalDistance < 1.5f && heightDifference > 0.2f && heightDifference < 0.3f)
        {
            pallet.position = new Vector3(currShelf.transform.position.x, currShelf.transform.position.y + 0.1f, currShelf.transform.position.z);
            currShelf.GetComponent<Renderer>().material = shelfMaterial;
            if (onFourth)
            {
                onFifth = true;
            }
            if (onThird)
            {
                onFourth = true;
            }
            if (onSecond)
            {
                onThird = true;
            }
            if (onFirst)
            {
                onSecond = true;
            }
            GameObject newPallet = Instantiate(palletPrefab, new Vector3(5.74f, 0.51f, 0), Quaternion.identity);
            newPallet.transform.parent = palletParent;
        }
    }


    public void displayDropOffLocation()
    {
        /*Vector3 objScreenPos = Camera.main.WorldToScreenPoint(currShelf.transform.position);
        Vector3 dir = (objScreenPos - rt.position).normalized;
        float angle = Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x);
        rt.rotation = Quaternion.AngleAxis(angle, Vector3.forward);*/
        Vector3 objectivePos = currShelf.transform.position;
        objectivePos.y = 0;
        Vector3 arrowPos = arrow.position;
        arrowPos.y = 0;
        Vector3 arrowDir = (objectivePos - arrowPos).normalized;
        arrow.rotation = Quaternion.FromToRotation(Vector3.forward, arrowDir);
        arrow.position = forklift.position + new Vector3(0, 0.5f, 0);
    }

}
