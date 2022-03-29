using UnityEngine;
using System.Linq;

public class OpponentHandler : MonoBehaviour
{
    #region Fields in inspector

    [Header("Opponent settings")]
    public float maxSpeed = 17;
    public bool isAvoidingCars = true;
    public float turnAngle = 45;
    [Range(0.0f, 1.0f)]
    public float skillLevel = 1.0f;

    #endregion

    #region Local variables

    Vector3 targetPosition = Vector3.zero;
    float originalMaximumSpeed = 0;

    #endregion

    #region Avoidance

    Vector2 avoidanceVectorLerped = Vector2.zero;

    #endregion

    #region Waypoints

    WaypointNode currentWaypoint = null;
    WaypointNode previousWaypoint = null;
    WaypointNode[] allWaypoints;

    #endregion

    #region Colliders

    PolygonCollider2D polygonCollider2D;

    #endregion

    #region Components

    CarController carController;

    #endregion

    #region Methods

    private void Awake()
    {
        carController = GetComponent<CarController>();
        allWaypoints = FindObjectsOfType<WaypointNode>();

        polygonCollider2D = GetComponentInChildren<PolygonCollider2D>();

        originalMaximumSpeed = maxSpeed;
    }

    void Start()
    {
        SetMaxSpeed(maxSpeed);
    }

    void FixedUpdate()
    {
        Vector2 inputVector = Vector2.zero;

        FollowWaypoints();

        inputVector.x = TurnToTarget();
        inputVector.y = ApplyThrottleOrBrake(inputVector.x);

        //Send the input to the car controller
        carController.SetInputVector(inputVector);
    }

    void FollowWaypoints()
    {
        //Pick the closest waypoint if we don't have a waypoint set
        if (currentWaypoint == null)
        {
            currentWaypoint = FindClosestWaypoint();
            previousWaypoint = currentWaypoint;
        }

        if (currentWaypoint != null)
        {
            targetPosition = currentWaypoint.transform.position;

            float distanceToWaypoint = (targetPosition - transform.position).magnitude;

            if (distanceToWaypoint > 20)
            {
                Vector3 backToLineHere = FindNearestPointOnLine(previousWaypoint.transform.position, currentWaypoint.transform.position, transform.position);

                float segments = distanceToWaypoint / 10.0f;

                targetPosition = (targetPosition + backToLineHere * segments) / (segments + 1);

                Debug.DrawLine(transform.position, targetPosition, Color.cyan);
            }

            //checks if we are close enough to consider that we have reached the waypoint
            if (distanceToWaypoint <= currentWaypoint.minDistanceToReachWaypoint)
            {
                if (currentWaypoint.maxSpeed > 0)
                    SetMaxSpeed(currentWaypoint.maxSpeed);
                else SetMaxSpeed(20);

                previousWaypoint = currentWaypoint;
                currentWaypoint = currentWaypoint.nextWaypointNode[0];
            }
        }
    }

    WaypointNode FindClosestWaypoint()
    {
        return allWaypoints.OrderBy(t => Vector3.Distance(transform.position, t.transform.position)).FirstOrDefault();
    }

    float TurnToTarget()
    {
        Vector2 vectorToTarget = targetPosition - transform.position;
        vectorToTarget.Normalize();

        if (isAvoidingCars)
            AvoidCars(vectorToTarget, out vectorToTarget);

        //calculates an angle towards the target
        float angleToTarget = Vector2.SignedAngle(transform.up, vectorToTarget);
        angleToTarget *= -1;

        float steerAmount = angleToTarget / turnAngle;

        steerAmount = Mathf.Clamp(steerAmount, -1.0f, 1.0f);

        return steerAmount;
    }

    float ApplyThrottleOrBrake(float inputX)
    {
        if (carController.GetVelocityMagnitude() > maxSpeed)
            return 0;

        float reduceSpeedDueToCornering = Mathf.Abs(inputX) / 1.0f;

        return 1.05f - reduceSpeedDueToCornering * skillLevel;
    }

    void SetMaxSpeed(float newSpeed)
    {
        maxSpeed = Mathf.Clamp(newSpeed, 0, originalMaximumSpeed);

        float skillBasedMaxSpeed = Mathf.Clamp(skillLevel, 0.3f, 1.0f);
        maxSpeed = maxSpeed * skillBasedMaxSpeed * Random.Range(0.9f,1.1f);
    }

    Vector2 FindNearestPointOnLine(Vector2 lineStartPosition, Vector2 lineEndPosition, Vector2 point)
    {
        Vector2 lineHeadingVector = (lineEndPosition - lineStartPosition);

        float maxDistance = lineHeadingVector.magnitude;
        lineHeadingVector.Normalize();

        Vector2 lineVectorStartToPoint = point - lineStartPosition;
        float dotProduct = Vector2.Dot(lineVectorStartToPoint, lineHeadingVector);
        dotProduct = Mathf.Clamp(dotProduct, 0f, maxDistance);

        return lineStartPosition + lineHeadingVector * dotProduct;
    }

    bool IsCarInFrontOfOpponent(out Vector3 position, out Vector3 otherCarRightVector)
    {
        polygonCollider2D.enabled = false;

        RaycastHit2D raycastHit2D = Physics2D.CircleCast(transform.position + transform.up * 0.5f, 0.8f, transform.up, 5, 1 << LayerMask.NameToLayer("Car"));

        polygonCollider2D.enabled = true;

        if (raycastHit2D.collider != null && carController.GetVelocityMagnitude()>5)
        {
            Debug.DrawRay(transform.position, transform.up * 5, Color.red);

            position = raycastHit2D.collider.transform.position;
            otherCarRightVector = raycastHit2D.collider.transform.right;

            return true;
        }
        else
        {
            Debug.DrawRay(transform.position, transform.up * 5, Color.black);
        }

        position = Vector3.zero;
        otherCarRightVector = Vector3.zero;

        return false;
    }

    void AvoidCars(Vector2 vectorToTarget, out Vector2 newVectorToTarget)
    {
        if( IsCarInFrontOfOpponent(out Vector3 otherCarPosition, out Vector3 otherCarRightVector))
        {
            Vector2 avoidanceVector = Vector2.zero;

            avoidanceVector = Vector2.Reflect((otherCarPosition - transform.position).normalized, otherCarRightVector);

            float distanceToTarget = (targetPosition - transform.position).magnitude;

            float driveToTargetInfluence = 6.0f / distanceToTarget;
            driveToTargetInfluence = Mathf.Clamp(driveToTargetInfluence, 0.3f, 1.0f);

            float avoidanceInfluence = 1.0f - driveToTargetInfluence;

            avoidanceVectorLerped = Vector2.Lerp(avoidanceVectorLerped, avoidanceVector, Time.fixedDeltaTime * 4);

            newVectorToTarget = vectorToTarget*driveToTargetInfluence + avoidanceVector*avoidanceInfluence;
            newVectorToTarget.Normalize();

            Debug.DrawRay(transform.position, avoidanceVector * 10, Color.green);
            Debug.DrawRay(transform.position, newVectorToTarget * 10, Color.yellow);

            return;
        }

        newVectorToTarget = vectorToTarget;
    }

    #endregion
}
