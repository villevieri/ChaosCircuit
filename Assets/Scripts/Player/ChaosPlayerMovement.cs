using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class ChaosPlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform movementCamera;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float rotationSpeed = 12f;

    [Header("Jump and Gravity")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -25f;
    [SerializeField] private float maxFallSpeed = 30f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 18f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    [Header("Jetpack")]
    [SerializeField] private float jetpackAcceleration = 40f;
    [SerializeField] private float maxRiseSpeed = 8f;
    [SerializeField] private float maxFuel = 3f;
    [SerializeField] private float fuelRechargeRate = 1.5f;

    private CharacterController controller;

    private float verticalSpeed;
    private float dashTimeRemaining;
    private float dashCooldownRemaining;
    private float fuel;

    private Vector3 dashDirection;

    // Platform supporting the player.
    private Transform currentPlatform;
    private Collider platformCollider;
    private Vector3 platformLocalPoint;
    private Vector3 previousPlatformWorldPoint;

    // Platform detected during CharacterController.Move().
    private Transform detectedPlatform;
    private Collider detectedPlatformCollider;

    public float FuelNormalized =>
        maxFuel > 0f ? fuel / maxFuel : 0f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        fuel = maxFuel;
    }

    private void OnDisable()
    {
        currentPlatform = null;
        platformCollider = null;
        detectedPlatform = null;
        detectedPlatformCollider = null;
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        float deltaTime = Time.deltaTime;

        dashCooldownRemaining =
            Mathf.Max(0f, dashCooldownRemaining - deltaTime);

        // Read movement input.
        Vector2 input = Vector2.zero;

        if (keyboard != null)
        {
            if (keyboard.wKey.isPressed ||
                keyboard.upArrowKey.isPressed)
            {
                input.y += 1f;
            }

            if (keyboard.sKey.isPressed ||
                keyboard.downArrowKey.isPressed)
            {
                input.y -= 1f;
            }

            if (keyboard.dKey.isPressed ||
                keyboard.rightArrowKey.isPressed)
            {
                input.x += 1f;
            }

            if (keyboard.aKey.isPressed ||
                keyboard.leftArrowKey.isPressed)
            {
                input.x -= 1f;
            }
        }

        input = Vector2.ClampMagnitude(input, 1f);

        // Convert input into camera-relative horizontal movement.
        Vector3 forward = movementCamera != null
            ? Vector3.ProjectOnPlane(
                movementCamera.forward,
                Vector3.up
            )
            : Vector3.forward;

        if (forward.sqrMagnitude < 0.001f)
        {
            forward = Vector3.forward;
        }

        forward.Normalize();

        Vector3 right = Vector3.Cross(Vector3.up, forward);

        Vector3 moveDirection =
            right * input.x + forward * input.y;

        bool grounded = controller.isGrounded;

        if (grounded && verticalSpeed < 0f)
        {
            verticalSpeed = -2f;
        }

        // Jump.
        bool jumpPressed =
            keyboard != null &&
            keyboard.spaceKey.wasPressedThisFrame;

        bool jumpedThisFrame = grounded && jumpPressed;

        if (jumpedThisFrame)
        {
            verticalSpeed =
                Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Dash.
        bool dashPressed =
            keyboard != null &&
            keyboard.leftShiftKey.wasPressedThisFrame;

        if (dashPressed &&
            dashCooldownRemaining <= 0f &&
            dashTimeRemaining <= 0f)
        {
            dashDirection = moveDirection.sqrMagnitude > 0.001f
                ? moveDirection.normalized
                : transform.forward;

            dashTimeRemaining = dashDuration;
            dashCooldownRemaining = dashCooldown;
        }

        // Jetpack.
        bool usingJetpack =
            keyboard != null &&
            keyboard.eKey.isPressed &&
            fuel > 0f;

        verticalSpeed += gravity * deltaTime;

        if (usingJetpack)
        {
            float poweredTime = Mathf.Min(deltaTime, fuel);

            verticalSpeed += jetpackAcceleration * poweredTime;
            verticalSpeed = Mathf.Min(verticalSpeed, maxRiseSpeed);

            fuel = Mathf.Max(0f, fuel - poweredTime);
        }
        else if (grounded)
        {
            fuel = Mathf.Min(
                maxFuel,
                fuel + fuelRechargeRate * deltaTime
            );
        }

        verticalSpeed = Mathf.Max(
            verticalSpeed,
            -maxFallSpeed
        );

        // Calculate walking or dashing displacement.
        Vector3 horizontalDisplacement;

        bool dashingThisFrame = dashTimeRemaining > 0f;

        if (dashingThisFrame)
        {
            float dashStep =
                Mathf.Min(deltaTime, dashTimeRemaining);

            horizontalDisplacement =
                dashDirection * dashSpeed * dashStep;

            horizontalDisplacement +=
                moveDirection *
                moveSpeed *
                (deltaTime - dashStep);

            dashTimeRemaining =
                Mathf.Max(0f, dashTimeRemaining - deltaTime);
        }
        else
        {
            horizontalDisplacement =
                moveDirection * moveSpeed * deltaTime;
        }

        // Turn toward movement.
        Vector3 facingDirection = dashingThisFrame
            ? dashDirection
            : moveDirection;

        if (facingDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(facingDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                1f - Mathf.Exp(-rotationSpeed * deltaTime)
            );
        }

        // Add the movement of the supporting platform.
        Vector3 platformDisplacement = Vector3.zero;

        bool canFollowPlatform =
            grounded &&
            !jumpedThisFrame &&
            !usingJetpack &&
            verticalSpeed <= 0f &&
            currentPlatform != null &&
            platformCollider != null;

        if (canFollowPlatform &&
            platformCollider.enabled &&
            platformCollider.gameObject.activeInHierarchy)
        {
            Vector3 currentPlatformWorldPoint =
                currentPlatform.TransformPoint(platformLocalPoint);

            platformDisplacement =
                currentPlatformWorldPoint -
                previousPlatformWorldPoint;
        }

        // Find fresh platform contact during this movement.
        detectedPlatform = null;
        detectedPlatformCollider = null;

        Vector3 displacement =
            horizontalDisplacement +
            Vector3.up * verticalSpeed * deltaTime +
            platformDisplacement;

        CollisionFlags collisions =
            controller.Move(displacement);

        // Stop rising when hitting a ceiling.
        if ((collisions & CollisionFlags.Above) != 0 &&
            verticalSpeed > 0f)
        {
            verticalSpeed = 0f;
        }

        bool supported =
            (collisions & CollisionFlags.Below) != 0;

        if (supported && verticalSpeed < 0f)
        {
            verticalSpeed = -2f;
        }

        // Store the platform contact for the next frame.
        if (supported &&
            !jumpedThisFrame &&
            !usingJetpack &&
            verticalSpeed <= 0f &&
            detectedPlatform != null)
        {
            currentPlatform = detectedPlatform;
            platformCollider = detectedPlatformCollider;

            platformLocalPoint =
                currentPlatform.InverseTransformPoint(
                    transform.position
                );

            previousPlatformWorldPoint =
                currentPlatform.TransformPoint(
                    platformLocalPoint
                );
        }
        else
        {
            currentPlatform = null;
            platformCollider = null;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Side collisions should not attach the player.
        if (hit.normal.y < 0.5f)
        {
            return;
        }

        MovingPlatform platform =
            hit.collider.GetComponentInParent<MovingPlatform>();

        if (platform == null)
        {
            return;
        }

        detectedPlatform = platform.transform;
        detectedPlatformCollider = hit.collider;
    }
}