using UnityEngine;

public class CarInputHandler : MonoBehaviour
{
    #region Components

    CarController carController;

    #endregion

    #region Methods
    private void Awake()
    {
        carController = GetComponent<CarController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 inputVector = Vector2.zero;

        inputVector.x = Input.GetAxis("Horizontal");
        inputVector.y = Input.GetAxis("Vertical");

        carController.SetInputVector(inputVector);
    }

    #endregion
}
