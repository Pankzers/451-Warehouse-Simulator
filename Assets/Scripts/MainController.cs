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
    private static Transform pallet = null;

    public Color originalColor;
    public Material shelfMaterial;

    void Start()
    {

    }

    void Update()
    {
        pallet = DriveForklift.selectedPallet;
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
}
