using UnityEngine;

namespace LethalBoom.Gameplay.Player
{
    /// <summary>Movement feel. Shared by both players unless a profile overrides it.</summary>
    [CreateAssetMenu(menuName = "LethalBoom/Config/Player Movement", fileName = "PlayerMovementConfig")]
    public class PlayerMovementConfig : ScriptableObject
    {
        [Header("Run")]
        public float moveSpeed = 5f;
        [Tooltip("How fast the player reaches target speed. 0 = instant (GameJamEA behaviour).")]
        public float acceleration = 60f;

        [Header("Jump")]
        public float jumpSpeed = 12f;
        public int maxJumps = 2;
        [Tooltip("Extra gravity multiplier; Unity's 3D gravity is -9.81, GameJamEA used gravityScale 3.")]
        public float gravityMultiplier = 3f;
        public float groundCheckRadius = 0.25f;
        public float groundCheckDistance = 0.1f;

        [Header("Aiming")]
        [Tooltip("Movement speed multiplier while aiming. GameJamEA froze the player (0).")]
        [Range(0f, 1f)] public float aimMoveMultiplier = 0f;
        [Tooltip("Gravity multiplier while aiming: the local slow-motion hang time.")]
        [Range(0.1f, 1f)] public float aimGravityMultiplier = 0.4f;
        public bool blockJumpWhileAiming = true;

        [Header("Body check / steal bump")]
        public float bumpSpeed = 10f;
        public float maxBumpSpeed = 6f;
        [Tooltip("Time the bumped player cannot steer.")]
        public float bumpLockTime = 0.15f;
        [Tooltip("Cooldown before another steal can happen.")]
        public float stealCooldown = 0.6f;
    }
}
