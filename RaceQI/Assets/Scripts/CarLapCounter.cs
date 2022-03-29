using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class CarLapCounter : MonoBehaviour
{
    public TMP_Text carPositionText;

    public GameObject winMenuUI;
    public GameObject loseMenuUI;
    public GameObject lapsAndPosition;

    public int lapsToComplete;

    public TMP_Text lapsToCompleteText;
    public TMP_Text completedLaps;

    public float delayHidingUntil;

    public bool showPosition = true;

    int passedCheckpointNumber = 0;
    float timeAtLastCheckpoint = 0;

    int numberOfPassedCheckpoints=0;

    int lapsCompleted = 0;

    bool isRaceCompleted = false;

    int carPosition = 0;

    bool isHideRoutineRunning = false;
    float hideUIDelayTime;

    public event Action<CarLapCounter> OnPassCheckpoint;

    public void SetCarPosition(int position)
    {
        carPosition = position;
    }

    public int GetNumberOfPassedCheckedpoints()
    {
        return numberOfPassedCheckpoints;
    }

    public float GetTimeAtLastCheckpoint()
    {
        return timeAtLastCheckpoint;
    }

    IEnumerator ShowPosition(float delayUntilHidePosition)
    {
        hideUIDelayTime += delayUntilHidePosition;

        carPositionText.text = carPosition.ToString();
        lapsToCompleteText.text = lapsToComplete.ToString();

        carPositionText.gameObject.SetActive(true);

        if (!isHideRoutineRunning)
        {
            isHideRoutineRunning = true;

            yield return new WaitForSeconds(hideUIDelayTime);
            carPositionText.gameObject.SetActive(false);

            isHideRoutineRunning = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Checkpoint"))
        {
            if (isRaceCompleted)
            {                
                return;
            }

            Checkpoint checkpoint = collision.GetComponent<Checkpoint>();

            if (passedCheckpointNumber + 1 == checkpoint.checkpointNumber)
            {
                passedCheckpointNumber++;
                numberOfPassedCheckpoints++;

                completedLaps.text = (lapsCompleted + 1).ToString();

                timeAtLastCheckpoint = Time.time;

                if (checkpoint.isFinishLine)
                {
                    passedCheckpointNumber = 0;
                    lapsCompleted++;

                    if (lapsCompleted >= lapsToComplete)
                        if (transform.name == "Car")
                        {
                            isRaceCompleted = true;
                            lapsAndPosition.SetActive(false);
                            if (carPosition == 1)
                            {
                                winMenuUI.SetActive(true);
                                Time.timeScale = 0.05f;
                            }
                            else
                            {
                                loseMenuUI.SetActive(true);
                                Time.timeScale = 0.05f;
                            }
                        }
                }

                OnPassCheckpoint?.Invoke(this);

                if (showPosition)
                {
                    if (isRaceCompleted)
                        StartCoroutine(ShowPosition(0));
                    else StartCoroutine(ShowPosition(delayHidingUntil));
                }
            }
        }
    }
}