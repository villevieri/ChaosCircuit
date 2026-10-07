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

    [Header("Randomization")]
    [SerializeField] private bool randomizeStart = true;

    [SerializeField] private bool randomizeSpeed = false;

    [SerializeField]
    private Vector2 speedRange =
        new Vector2(1.5f, 3f);

    private Rigidbody platformBody;

    private Vector3 startPosition;
    private Vector3 endPosition;

    private float travelDuration;
    private float cycleDuration;
    private float cycleTime;

    private void Awake()
    {
        platformBody = GetComponent<Rigidbody>();

        startPosition = platformBody.position;
        endPosition = startPosition + movementOffset;

        float actualSpeed = Mathf.Max(0.1f, moveSpeed);

        if (randomizeSpeed)
        {
            float minimumSpeed = Mathf.Max(
                0.1f,
                Mathf.Min(speedRange.x, speedRange.y)
            );

            float maximumSpeed = Mathf.Max(
                minimumSpeed,
                Mathf.Max(speedRange.x, speedRange.y)
            );

            actualSpeed = Random.Range(
                minimumSpeed,
                maximumSpeed
            );
        }

        float distance = movementOffset.magnitude;

        if (distance < 0.001f)
        {
            enabled = false;
            return;
        }

        travelDuration = distance / actualSpeed;

        cycleDuration =
            travelDuration * 2f + pauseAtEnds * 2f;

        cycleTime = randomizeStart
            ? Random.Range(0f, cycleDuration)
            : 0f;

        // Position the platform before gameplay begins.
        platformBody.position = GetCyclePosition(cycleTime);
    }

    private void FixedUpdate()
    {
        cycleTime = Mathf.Repeat(
            cycleTime + Time.fixedDeltaTime,
            cycleDuration
        );

        platformBody.MovePosition(
            GetCyclePosition(cycleTime)
        );
    }

    private Vector3 GetCyclePosition(float time)
    {
        // Travel from start to end.
        if (time < travelDuration)
        {
            return Vector3.Lerp(
                startPosition,
                endPosition,
                time / travelDuration
            );
        }

        time -= travelDuration;

        // Wait at the end.
        if (time < pauseAtEnds)
        {
            return endPosition;
        }

        time -= pauseAtEnds;

        // Travel from end to start.
        if (time < travelDuration)
        {
            return Vector3.Lerp(
                endPosition,
                startPosition,
                time / travelDuration
            );
        }

        // Wait at the start.
        return startPosition;
    }
}