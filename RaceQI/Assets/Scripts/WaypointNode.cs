using UnityEngine;

public class WaypointNode : MonoBehaviour
{
    [Header("Speed at waypoint")]
    public float maxSpeed = 0;

    [Header("Distance to next waypoint")]
    public float minDistanceToReachWaypoint = 5;

    public WaypointNode[] nextWaypointNode;
}
