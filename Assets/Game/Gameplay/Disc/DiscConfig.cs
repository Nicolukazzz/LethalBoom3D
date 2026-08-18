using UnityEngine;

namespace LethalBoom.Gameplay.Disc
{
    /// <summary>Disc physics, bomb timer and pickup rules. Defaults mirror GameJamEA.</summary>
    [CreateAssetMenu(menuName = "LethalBoom/Config/Disc", fileName = "DiscConfig")]
    public class DiscConfig : ScriptableObject
    {
        [Header("Launch")]
        public float launchSpeed = 10f;

        [Header("Bounces")]
        [Tooltip("Speed multiplier applied on every wall bounce. >1 makes rallies escalate.")]
        public float bounceSpeedMultiplier = 1.15f;
        [Tooltip("Hard cap on speed so the disc stays readable and tunneling stays unlikely.")]
        public float maxSpeed = 40f;
        [Tooltip("Bounces before the bounce-limit rule fires. GameJamEA used 8 and teleported the disc to the opponent.")]
        public int maxBounces = 8;

        [Header("Bomb")]
        [Tooltip("Seconds a player may hold the disc before it explodes. The timer only runs while held.")]
        public float bombTimerDuration = 3f;

        [Header("Pickup / interception")]
        [Tooltip("Radius within which a player catches a flying or loose disc.")]
        public float pickupRadius = 0.8f;
        [Tooltip("Time the launcher cannot re-catch their own throw.")]
        public float launcherImmunityTime = 0.3f;
    }
}
