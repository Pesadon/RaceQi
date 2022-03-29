using UnityEngine;
public class StopCanvasRotation : MonoBehaviour
{
    public float lockedPosition;

    void Update()
    {
        // Locks the rotation.
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, lockedPosition, lockedPosition);
    }
}