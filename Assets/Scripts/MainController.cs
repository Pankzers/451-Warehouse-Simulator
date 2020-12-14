using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{

    public GameObject secondPickUpShelf;
    public GameObject thirdPickUpShelf;
    public GameObject fourthPickUpShelf;
    public GameObject fifthPickUpShelf;

    public GameObject firstDropOffShelf;
    public GameObject secondDropOffShelf;
    public GameObject thirdDropOffShelf;
    public GameObject fourthDropOffShelf;
    public GameObject fifthDropOffShelf;

    public GameObject currDropOffShelf;

    public bool onFirst = true;
    public bool onSecond = false;
    public bool onThird = false;
    public bool onFourth = false;
    public bool onFifth = false;

    public GameObject palletPrefab;
    public Transform palletParent;
    private static Transform pallet = null;

    public Color originalColor;
    public Material shelfMaterial;

    public RectTransform rt;

    public Vector3 nextPickUpCoordinates;

    void Start()
    {
        rt = GameObject.Find("Arrow").GetComponent<RectTransform>();
    }

    void Update()
    {
        pallet = DriveForklift.selectedPallet;
        if (pallet != null)
        {
            if (onFirst)
            {
                currDropOffShelf = firstDropOffShelf;
                nextPickUpCoordinates = new Vector3(secondPickUpShelf.transform.position.x, secondPickUpShelf.transform.position.y, secondPickUpShelf.transform.position.z);
                if (onSecond)
                {
                    currDropOffShelf = secondDropOffShelf;
                    if (onThird)
                    {
                        currDropOffShelf = thirdDropOffShelf;
                        if (onFourth)
                        {
                            currDropOffShelf = fourthDropOffShelf;
                            if (onFifth)
                            {
                                currDropOffShelf = fifthDropOffShelf;
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
        currDropOffShelf.GetComponent<Renderer>().material = null;
        currDropOffShelf.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
        float totalDistance = Vector3.Distance(pallet.position, currDropOffShelf.transform.position);
        float heightDifference = pallet.position.y - currDropOffShelf.transform.position.y;
        Debug.Log(totalDistance);
        if (totalDistance < 1.5f && heightDifference > 0.2f && heightDifference < 0.3f)
        {
            pallet.position = new Vector3(currDropOffShelf.transform.position.x, currDropOffShelf.transform.position.y + 0.2f, currDropOffShelf.transform.position.z);
            currDropOffShelf.GetComponent<Renderer>().material = shelfMaterial;
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
            GameObject newPallet = Instantiate(palletPrefab, nextPickUpCoordinates, Quaternion.identity);
            newPallet.transform.parent = palletParent;
        }
    }


    public void displayDropOffLocation()
    {
        Vector3 objScreenPos = Camera.main.WorldToScreenPoint(currDropOffShelf.transform.position);
        Vector3 dir = (objScreenPos - rt.position).normalized;
        float angle = Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x);
        rt.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

}
