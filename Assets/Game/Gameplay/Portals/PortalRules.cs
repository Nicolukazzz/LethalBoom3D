using UnityEngine;

namespace LethalBoom.Gameplay.Portals
{
    /// <summary>
    /// Every portal rule we expect to iterate on lives here, so experiments are a
    /// second asset instead of a code change. See docs/decisions/ADR-0004.
    /// </summary>
    [CreateAssetMenu(menuName = "LethalBoom/Config/Portal Rules", fileName = "PortalRules")]
    public class PortalRules : ScriptableObject
    {
        [Header("When can portals be placed")]
        public bool allowDuringPreparation = true;
        public bool allowDuringPlay = false;
        [Tooltip("Portals a single player owns at once. A new one past this limit replaces the oldest.")]
        public int portalsPerPlayer = 2;

        [Header("Where")]
        [Tooltip("Surfaces a portal may attach to.")]
        public LayerMask placementSurfaces;
        public float maxPlacementDistance = 20f;
        [Tooltip("Push the portal out of the surface to avoid z-fighting and trigger overlap.")]
        public float surfaceOffset = 0.05f;
        public float minDistanceFromGoal = 2f;
        public float minDistanceFromRivalPortal = 2f;
        [Tooltip("Max angle between surface normal and the placement ray for the surface to count as valid.")]
        [Range(0f, 89f)] public float maxSurfaceAngle = 80f;

        [Header("Who can travel")]
        public bool playersCanTravel = true;
        public bool discCanTravel = true;
        [Tooltip("If true, only the owner may travel through their own portal pair.")]
        public bool ownerOnly = false;

        [Header("Traversal")]
        [Tooltip("Seconds a traveler is ignored by portals after coming out of one. Prevents loops.")]
        public float travelCooldown = 0.15f;
        [Tooltip("Multiplier applied to exit speed. 1 = momentum fully preserved.")]
        public float exitSpeedMultiplier = 1f;
        [Tooltip("Distance pushed along the exit normal when arriving.")]
        public float exitOffset = 0.2f;
    }
}
