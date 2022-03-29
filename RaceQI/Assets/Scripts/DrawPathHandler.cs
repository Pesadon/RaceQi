using UnityEngine;

public class DrawPathHandler : MonoBehaviour
{
    #region Fields in inspector

    public Transform transformRootObject;

    WaypointNode[] waypointNodes;

    #endregion

    #region Methods

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        if (transformRootObject == null)
            return;

        waypointNodes = transformRootObject.GetComponentsInChildren<WaypointNode>();

        foreach (WaypointNode waypoint in waypointNodes)
        {
            foreach (WaypointNode nextWayPoint in waypoint.nextWaypointNode)
            {
                if (nextWayPoint != null)
                    Gizmos.DrawLine(waypoint.transform.position, nextWayPoint.transform.position);
            }
        }
    }

    #endregion
}
