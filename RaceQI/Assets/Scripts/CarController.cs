using UnityEngine;

public class CarController : MonoBehaviour
{
    #region Fields in inspector
    public enum MaxSpeedType { fast, almostAsFastAsFastButSlower, medium, slow };
    public enum TurnType { oversteer, understeer, disabled };
    public enum DriftType { screechTires, likeOnTrails };
    public enum AccType { ludicrous, turtleMode };

    [Header("Car settings")]
    public MaxSpeedType maxSpeedType;
    public TurnType turnType;
    public DriftType driftType;
    public AccType accType;

    #endregion

    #region Local variables

    float maxSpeed = 20;
    float turningAmount = 2;
    float driftingAmount = 0.97f;
    float accAmount = 7;

    float accInput = 0;
    float steeringInput = 0;

    float rotationAngle = 0;

    float velocityVsUp = 0;

    #endregion

    #region Components

    Rigidbody2D carRigidboy2D;

    #endregion

    #region Methods
    private void Awake()
    {
        carRigidboy2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        switch (maxSpeedType)
        {
            case MaxSpeedType.fast:
                maxSpeed = 22;
                break;
            case MaxSpeedType.almostAsFastAsFastButSlower:
                maxSpeed = 19;
                break;
            case MaxSpeedType.medium:
                maxSpeed = 17;
                break;
            case MaxSpeedType.slow:
                maxSpeed = 14;
                break;
        }

        switch (turnType)
        {
            case TurnType.oversteer:
                turningAmount = 2;
                break;
            case TurnType.understeer:
                turningAmount = 1.7f;
                break;
            case TurnType.disabled:
                turningAmount = 0f;
                break;
        }

        switch (driftType)
        {
            case DriftType.likeOnTrails:
                driftingAmount = 0.9f;
                break;
            case DriftType.screechTires:
                driftingAmount = 0.97f;
                break;
        }

        switch (accType)
        {
            case AccType.ludicrous:
                accAmount = 7;
                break;
            case AccType.turtleMode:
                accAmount = 5;
                break;
        }

        ApplyEngineForce();

        KillOrthogonalVelocity();

        ApplySteering();
    }

    void ApplyEngineForce()
    {
        //elõrehaladás "mennyisége"
        velocityVsUp = Vector2.Dot(transform.up, carRigidboy2D.velocity);

        //nem enged gyorsabban menni, mint a maxSpeed
        if (velocityVsUp > maxSpeed && accInput > 0) return;

        //nem enged gyorsabban menni hátrafelé, mint a maxSpeed fele
        if (velocityVsUp < -maxSpeed * 0.5f && accInput < 0) return;

        //limit so we cannot go faster in any direction while accelerating
        if (carRigidboy2D.velocity.sqrMagnitude > maxSpeed * maxSpeed && accInput > 0) return;

        //lassít, ha nem nyomjuk a gázt
        if (accInput <= 0)
        {
            carRigidboy2D.drag = Mathf.Lerp(carRigidboy2D.drag, 3.0f, Time.fixedDeltaTime * 3);
        }
        else carRigidboy2D.drag = 0;

        //motorerõ létrehozása
        Vector2 engineForceVector = transform.up * accInput * accAmount;

        //motorerõ alkalmazása
        carRigidboy2D.AddForce(engineForceVector, ForceMode2D.Force);
    }

    void ApplySteering()
    {
        //nem enged kanyarodni, ha lassan megyünk
        float minSpeedBeforeAllowTurningFactor = (carRigidboy2D.velocity.magnitude / 8);
        minSpeedBeforeAllowTurningFactor = Mathf.Clamp01(minSpeedBeforeAllowTurningFactor);

        //rotationAngle változtatása az inputnak megfelelõen
        rotationAngle -= steeringInput * turningAmount * minSpeedBeforeAllowTurningFactor;

        //autó elfordítása
        carRigidboy2D.MoveRotation(rotationAngle);
    }

    void KillOrthogonalVelocity()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(carRigidboy2D.velocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(carRigidboy2D.velocity, transform.right);

        carRigidboy2D.velocity = forwardVelocity + rightVelocity * driftingAmount;
    }

    float GetLateralVelocity()
    {
        //oldalirányú sebesség
        return Vector2.Dot(transform.right, carRigidboy2D.velocity);
    }

    public bool IsTireScreeching(out float lateralVelocity, out bool isBraking)
    {
        lateralVelocity = GetLateralVelocity();
        isBraking = false;

        //elõre megyünk-e és a fék be van-e nyomva
        if(accInput < 0 && velocityVsUp > 0)
        {
            isBraking = true;
            return true;
        }

        //elég nagy-e az oldalirányú sebesség
        if (Mathf.Abs(GetLateralVelocity()) > 4.0f) 
            return true;

        return false;
    }

    public void SetInputVector(Vector2 inputVector)
    {
        steeringInput = inputVector.x;
        accInput = inputVector.y;
    }

    public float GetVelocityMagnitude()
    {
        return carRigidboy2D.velocity.magnitude;
    }

    #endregion
}
