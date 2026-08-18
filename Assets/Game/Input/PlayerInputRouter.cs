using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LethalBoom.Input
{
    /// <summary>
    /// The only component in the project allowed to talk to the Input System.
    /// Gameplay reads MoveInput and subscribes to the events below; it never learns
    /// whether the action came from W, the left stick or R2, which is what makes
    /// gamepads, per-player schemes and remapping a configuration problem instead
    /// of a code change.
    ///
    /// Requires a PlayerInput component on the same object, set to "Send Messages"
    /// off and "Invoke Unity Events" off: we bind the actions ourselves so the
    /// wiring is visible in code rather than in the inspector.
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputRouter : MonoBehaviour
    {
        public event Action JumpPressed;
        public event Action AimThrowPressed;
        public event Action PlacePortalPressed;
        public event Action ReadyPressed;
        public event Action PausePressed;

        /// <summary>Current movement stick/keys, already normalised by the Input System.</summary>
        public Vector2 MoveInput { get; private set; }

        /// <summary>Free-aim stick direction, for the aiming experiments in M2.</summary>
        public Vector2 AimDirection { get; private set; }

        PlayerInput playerInput;
        InputAction move, jump, aimThrow, placePortal, ready, pause, aimDirection;

        /// <summary>Device-independent id of the player this router belongs to.</summary>
        public int PlayerIndex => playerInput != null ? playerInput.playerIndex : -1;

        void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            var map = playerInput.actions.FindActionMap("Player", throwIfNotFound: true);
            move = map.FindAction("Move", true);
            jump = map.FindAction("Jump", true);
            aimThrow = map.FindAction("AimThrow", true);
            placePortal = map.FindAction("PlacePortal", true);
            ready = map.FindAction("Ready", true);
            pause = map.FindAction("Pause", true);
            aimDirection = map.FindAction("AimDirection", true);
        }

        void OnEnable()
        {
            jump.performed += OnJump;
            aimThrow.performed += OnAimThrow;
            placePortal.performed += OnPlacePortal;
            ready.performed += OnReady;
            pause.performed += OnPause;
        }

        void OnDisable()
        {
            jump.performed -= OnJump;
            aimThrow.performed -= OnAimThrow;
            placePortal.performed -= OnPlacePortal;
            ready.performed -= OnReady;
            pause.performed -= OnPause;
        }

        void Update()
        {
            MoveInput = move.ReadValue<Vector2>();
            AimDirection = aimDirection.ReadValue<Vector2>();
        }

        void OnJump(InputAction.CallbackContext _) => JumpPressed?.Invoke();
        void OnAimThrow(InputAction.CallbackContext _) => AimThrowPressed?.Invoke();
        void OnPlacePortal(InputAction.CallbackContext _) => PlacePortalPressed?.Invoke();
        void OnReady(InputAction.CallbackContext _) => ReadyPressed?.Invoke();
        void OnPause(InputAction.CallbackContext _) => PausePressed?.Invoke();
    }
}
