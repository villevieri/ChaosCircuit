using UnityEngine;

public class ChaosCameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Camera Position")]
    [SerializeField]
    private Vector3 offset =
        new Vector3(0f, 12f, -14f);

    [Header("Following")]
    [SerializeField, Min(0f)] private float smoothTime = 0.12f;

    private Vector3 followVelocity;

    private void Start()
    {
        if (target == null)
            return;

        // Start at the correct position immediately.
        transform.position = target.position + offset;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 desiredPosition = target.position + offset;

        if (smoothTime <= 0f)
        {
            transform.position = desiredPosition;
            followVelocity = Vector3.zero;
            return;
        }

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref followVelocity,
            smoothTime
        );
    }
}