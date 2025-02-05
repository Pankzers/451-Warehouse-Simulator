﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour
{

    public GameObject secondPickUpShelf;
    public GameObject thirdPickUpShelf;
    public GameObject fourthPickUpShelf;
    public GameObject fifthPickUpShelf;

    public GameObject currPickUpShelf;
    public GameObject prevPickUpShelf;

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
    public bool done = false;

    public GameObject palletPrefab;
    public Transform palletParent;
    private Transform pallet = null;
    public GameObject newPallet;

    public Color pickUpShelfColor;
    public Material shelfMaterial;

    public Transform arrow = null;
    public Transform forklift = null;
    private DriveForklift forkDrive;

    public Vector3 nextPickUpCoordinates;

    public Text statusText;
    public Text endMessageText;
    public GameObject endMessage;

    public float timeRemaining = 180;
    public bool timerIsRunning = false;
    public Text timerText;

    public Button resetButton;
    public Button exitButton;

    public bool ignoreTimer = false;

    void Start()
    {
        timeRemaining = 180;
        Debug.Assert(arrow != null);
        Debug.Assert(forklift != null);
        //rt = GameObject.Find("Arrow").GetComponent<RectTransform>();
        forkDrive = forklift.GetComponent<DriveForklift>();
        timerIsRunning = true;
        resetButton.onClick.AddListener(resetGame);
        exitButton.onClick.AddListener(exitGame);
        endMessage.SetActive(false);
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                displayTime(timeRemaining);
            } else
            {
                done = true;
                timeRemaining = 0;
                timerIsRunning = false;
            }
        } else
        {
            if (!ignoreTimer)
            {
                endGame();
            }
        }
        pallet = forkDrive.selectedPallet;
        if (pallet != null && !done)
        {
            if (onFirst)
            {
                currDropOffShelf = firstDropOffShelf;
                nextPickUpCoordinates = new Vector3(secondPickUpShelf.transform.position.x, secondPickUpShelf.transform.position.y + 0.2f, secondPickUpShelf.transform.position.z);
                currPickUpShelf = secondPickUpShelf;
                if (onSecond)
                {
                    currDropOffShelf = secondDropOffShelf;
                    nextPickUpCoordinates = new Vector3(thirdPickUpShelf.transform.position.x, thirdPickUpShelf.transform.position.y + 0.2f, thirdPickUpShelf.transform.position.z);
                    currPickUpShelf = thirdPickUpShelf;
                    if (onThird)
                    {
                        currDropOffShelf = thirdDropOffShelf;
                        nextPickUpCoordinates = new Vector3(fourthPickUpShelf.transform.position.x, fourthPickUpShelf.transform.position.y + 0.2f, fourthPickUpShelf.transform.position.z);
                        currPickUpShelf = fourthPickUpShelf;
                        if (onFourth)
                        {
                            currDropOffShelf = fourthDropOffShelf;
                            nextPickUpCoordinates = new Vector3(fifthPickUpShelf.transform.position.x, fifthPickUpShelf.transform.position.y + 0.2f, fifthPickUpShelf.transform.position.z);
                            currPickUpShelf = fifthPickUpShelf;
                            if (onFifth)
                            {
                                currDropOffShelf = fifthDropOffShelf;
                            }
                        }
                    }
                }
            }
            
            newDropOffLocation();
        }
        checkNewPalletLocation();
        if (done)
        {
            arrow.gameObject.SetActive(false);
            endGame();
        }
        displayDropOffLocation();
    }

    public void newDropOffLocation()
    {
        currDropOffShelf.GetComponent<Renderer>().material = null;
        currDropOffShelf.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
        float totalDistance = Vector3.Distance(pallet.position, currDropOffShelf.transform.position);
        float heightDifference = pallet.position.y - currDropOffShelf.transform.position.y;
        if (totalDistance < 1.5f && heightDifference > 0.2f && heightDifference < 0.3f)
        {
            pallet.position = new Vector3(currDropOffShelf.transform.position.x, currDropOffShelf.transform.position.y + 0.2f, currDropOffShelf.transform.position.z);
            currDropOffShelf.GetComponent<Renderer>().material = shelfMaterial;
            forkDrive.selectedPallet.parent = null;
            forkDrive.selectedPallet = null;
            if (onFifth)
            {
                timerIsRunning = false; 
                done = true;
            }
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
            updateDropOffStatus();
            if (!done)
            {
                prevPickUpShelf = currPickUpShelf;
                newPallet = Instantiate(palletPrefab, nextPickUpCoordinates, Quaternion.identity);
                newPallet.transform.eulerAngles = new Vector3(newPallet.transform.eulerAngles.x, 90, newPallet.transform.eulerAngles.z);
                newPallet.transform.SetParent(palletParent);
                //palletParent.
            }
        }
    }


    public void displayDropOffLocation()
    {
        //Vector3 objScreenPos = Camera.main.WorldToScreenPoint(currDropOffShelf.transform.position);
        //Vector3 dir = (objScreenPos - rt.position).normalized;
        //float angle = Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x);
        //rt.rotation = Quaternion.AngleAxis(angle, Vector3.forward);*/
        Vector3 objectivePos = Vector3.zero;
        if(pallet != null)
        {
            objectivePos = currDropOffShelf.transform.position;
        } else
        {
            if(palletParent.childCount != 0)
            {
                objectivePos = palletParent.GetChild(0).position;
            } else
            {
                arrow.gameObject.SetActive(false);
            }
            
        }
        
        objectivePos.y = 0;
        Vector3 arrowPos = arrow.position;
        arrowPos.y = 0;
        Vector3 arrowDir = (objectivePos - arrowPos).normalized;
        arrow.rotation = Quaternion.FromToRotation(Vector3.forward, arrowDir);
        arrow.position = forklift.position + new Vector3(0, 0.5f, 0);
    }

    public void checkNewPalletLocation()
    {
        if (newPallet != null && newPallet.transform.position == nextPickUpCoordinates)
        {
            if (ColorUtility.TryParseHtmlString("#FFA500", out pickUpShelfColor))
            {
                currPickUpShelf.GetComponent<Renderer>().material = null;
                currPickUpShelf.GetComponent<Renderer>().material.color = pickUpShelfColor;
            }
        }
        else
        {
            if (prevPickUpShelf != null)
            {
                prevPickUpShelf.GetComponent<Renderer>().material = shelfMaterial;
            }
        }
    }

    public void endGame()
    {
        if (timeRemaining > 0)
        {
            endMessageText.text = "Congratulations! You delivered all the pallets on time! Click Reset to play again or Quit to exit.";
            statusText.text = "Drop-offs completed: 5 / 5";
        }
        else
        {
            endMessageText.text = "Game Over! You failed to deliver all the pallets on time! Click Reset to try again or Quit to exit.";
        }
        if (!ignoreTimer)
        {
            endMessage.SetActive(true);
        }
    }

    public void displayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        if (minutes == 0 && seconds < 10)
        {
            timerText.color = new Color(1, 0, 0, 1);
        }
    }

    public void updateDropOffStatus()
    {
        if (onSecond)
        {
            statusText.text = "Drop-offs completed: 1 / 5";
            if (onThird)
            {
                statusText.text = "Drop-offs completed: 2 / 5";
                if (onFourth)
                {
                    statusText.text = "Drop-offs completed: 3 / 5";
                    if (onFifth)
                    {
                        statusText.text = "Drop-offs completed: 4 / 5";
                    }
                }
            }
        }
    }

    public void resetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void exitGame()
    {
        Application.Quit();
    }

}
