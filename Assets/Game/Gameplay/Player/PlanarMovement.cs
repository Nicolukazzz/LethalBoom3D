using LethalBoom.Input;
using UnityEngine;

namespace LethalBoom.Gameplay.Player
{
    /// <summary>
    /// Run and jump on the XY play plane using 3D physics.
    ///
    /// The 2.5D constraint lives here and nowhere else (ADR-0001): the rigidbody is
    /// frozen on Z and rotation, so opening the game up to a full 3D arena later means
    /// replacing this component, not rewriting movement, aiming and the disc.
    ///
    /// Gravity is applied manually because the feel we want (heavy fall, floaty aim)
    /// needs a multiplier that other systems can change through PlayerAbilities.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(GroundCheck))]
    [RequireComponent(typeof(PlayerAbilities))]
    public class PlanarMovement : MonoBehaviour
    {
        [SerializeField] PlayerMovementConfig config;
        [SerializeField] PlayerInputRouter input;

        [Header("Feel")]
        [Tooltip("Grace period after walking off a ledge during which a jump still counts as grounded.")]
        [SerializeField] float coyoteTime = 0.1f;
        [Tooltip("A jump pressed this long before landing still fires on touchdown.")]
        [SerializeField] float jumpBuffer = 0.12f;

        Rigidbody body;
        GroundCheck ground;
        PlayerAbilities abilities;

        int jumpsRemaining;
        float coyoteTimer;
        float jumpBufferTimer;
        float facing = 1f;

        /// <summary>1 = facing right, -1 = facing left. Aiming and visuals read this.</summary>
        public float Facing => facing;

        public bool IsGrounded => ground != null && ground.IsGrounded;

        /// <summary>Horizontal speed, for animation and camera framing.</summary>
        public float HorizontalSpeed => body != null ? Mathf.Abs(body.linearVelocity.x) : 0f;

        void Awake()
        {
            body = GetComponent<Rigidbody>();
            ground = GetComponent<GroundCheck>();
            abilities = GetComponent<PlayerAbilities>();

            body.useGravity = false;
            body.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            if (config == null)
                Debug.LogError($"{name}: PlanarMovement has no PlayerMovementConfig assigned.", this);
        }

        void OnEnable()
        {
            if (input != null)
                input.JumpPressed += OnJumpPressed;
        }

        void OnDisable()
        {
            if (input != null)
                input.JumpPressed -= OnJumpPressed;
        }

        void OnJumpPressed() => jumpBufferTimer = jumpBuffer;

        void Update()
        {
            if (input == null) return;

            float horizontal = input.MoveInput.x;
            if (Mathf.Abs(horizontal) > 0.01f && !abilities.SteeringBlocked)
                facing = Mathf.Sign(horizontal);
        }

        void FixedUpdate()
        {
            if (config == null) return;

            float dt = Time.fixedDeltaTime;
            coyoteTimer = ground.IsGrounded ? coyoteTime : Mathf.Max(0f, coyoteTimer - dt);
            jumpBufferTimer = Mathf.Max(0f, jumpBufferTimer - dt);

            // Jumps refill on touchdown only, not while resting on the floor, so a
            // double jump cannot be regained by grazing the ground mid-jump.
            if (ground.JustLanded)
                jumpsRemaining = config.maxJumps;

            ApplyHorizontal(dt);
            ApplyJump();
            ApplyGravity(dt);
        }

        void ApplyHorizontal(float dt)
        {
            Vector3 velocity = body.linearVelocity;

            // While bumped or stunned the player keeps whatever velocity the impact gave
            // them: overwriting it here is what made the jam bump feel weightless.
            if (abilities.SteeringBlocked)
                return;

            float target = 0f;
            if (input != null)
                target = Mathf.Clamp(input.MoveInput.x, -1f, 1f) * config.moveSpeed * abilities.SpeedMultiplier;

            velocity.x = config.acceleration > 0f
                ? Mathf.MoveTowards(velocity.x, target, config.acceleration * dt)
                : target;

            body.linearVelocity = velocity;
        }

        void ApplyJump()
        {
            if (jumpBufferTimer <= 0f) return;
            if (abilities.JumpBlocked) return;

            bool canGroundJump = coyoteTimer > 0f && jumpsRemaining == config.maxJumps;
            if (!canGroundJump && jumpsRemaining <= 0) return;

            Vector3 velocity = body.linearVelocity;
            velocity.y = config.jumpSpeed;
            body.linearVelocity = velocity;

            jumpsRemaining--;
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }

        void ApplyGravity(float dt)
        {
            float multiplier = config.gravityMultiplier * abilities.GravityMultiplier;
            body.linearVelocity += Physics.gravity * (multiplier * dt);
        }

        /// <summary>Used by respawn and match reset: drop the player somewhere at rest.</summary>
        public void Teleport(Vector3 position)
        {
            body.position = position;
            body.linearVelocity = Vector3.zero;
            jumpsRemaining = config != null ? config.maxJumps : 0;
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }
    }
}
