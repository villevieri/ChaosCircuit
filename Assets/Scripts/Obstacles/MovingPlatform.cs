using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovingPlatform : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private Vector3 movementOffset =
        new Vector3(6f, 0f, 0f);

    [SerializeField, Min(0.1f)]
    private float moveSpeed = 2f;

    [SerializeField, Min(0f)]
    private float pauseAtEnds = 0.5f;

    private Rigidbody platformBody;

    private Vector3 startPosition;
    private Vector3 endPosition;

    private bool movingToEnd = true;
    private float pauseRemaining;

    private void Awake()
    {
        platformBody = GetComponent<Rigidbody>();

        startPosition = platformBody.position;
        endPosition = startPosition + movementOffset;
    }

    private void FixedUpdate()
    {
        if (pauseRemaining > 0f)
        {
            pauseRemaining -= Time.fixedDeltaTime;
            return;
        }

        Vector3 targetPosition =
            movingToEnd ? endPosition : startPosition;

        Vector3 nextPosition = Vector3.MoveTowards(
            platformBody.position,
            targetPosition,
            moveSpeed * Time.fixedDeltaTime
        );

        platformBody.MovePosition(nextPosition);

        if ((nextPosition - targetPosition).sqrMagnitude < 0.0001f)
        {
            movingToEnd = !movingToEnd;
            pauseRemaining = pauseAtEnds;
        }
    }
}