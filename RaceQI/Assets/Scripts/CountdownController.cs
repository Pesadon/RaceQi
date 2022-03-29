using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountdownController : MonoBehaviour
{
    #region Fields in inspector

    public int countdownTime;
    public TMP_Text countdownDisplay;

    public GameObject car;
    public List<GameObject> opponents;

    #endregion

    #region Methods

    private void Start()
    {
        car = GameObject.Find("Car");

        foreach (var item in GameObject.FindGameObjectsWithTag("AI"))
        {
            opponents.Add(item);
        }

        StartCoroutine(CountdownToStart());
    }

    private void FixedUpdate()
    {
        if (!countdownDisplay.gameObject.activeInHierarchy)
        {
            car.GetComponent<CarInputHandler>().enabled = true;

            foreach (var item in opponents)
            {
                item.GetComponent<OpponentHandler>().enabled = true;
                item.GetComponent<CarController>().enabled = true;
            }
        }
        else
        {
            car.GetComponent<CarInputHandler>().enabled = false;

            foreach (var item in opponents)
            {
                item.GetComponent<OpponentHandler>().enabled = false;
                item.GetComponent<CarController>().enabled = false;
            }
        }
    }

    IEnumerator CountdownToStart()
    {
        while (countdownTime > 0)
        {
            countdownDisplay.text = countdownTime.ToString();

            yield return new WaitForSeconds(1f);

            countdownTime--;
        }

        countdownDisplay.text = "start";

        yield return new WaitForSeconds(0.5f);

        countdownDisplay.gameObject.SetActive(false);
    }

    #endregion
}
