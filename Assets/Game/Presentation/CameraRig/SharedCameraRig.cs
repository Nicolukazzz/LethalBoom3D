using LethalBoom.Gameplay.Match;
using UnityEngine;

namespace LethalBoom.Presentation.CameraRig
{
    /// <summary>
    /// One camera for both players (ADR-0001). Follows the midpoint of everyone on the
    /// court and pulls back as they separate, clamped to the arena bounds so the shot
    /// never shows the outside of the box.
    ///
    /// Readability with two fast players is the main risk of the shared-camera choice,
    /// so this exists in M1 rather than being left for polish.
    /// </summary>
    public class SharedCameraRig : MonoBehaviour
    {
        [SerializeField] Camera targetCamera;
        [SerializeField] PlayerRegistry registry;

        [Header("Extra targets")]
        [Tooltip("Anything else worth keeping in frame, such as the disc.")]
        [SerializeField] Transform[] additionalTargets = new Transform[0];

        [Header("Framing")]
        [Tooltip("World-space margin kept around the outermost target.")]
        [SerializeField] float padding = 4f;
        [SerializeField] float minOrthographicSize = 6f;
        [SerializeField] float maxOrthographicSize = 12f;
        [SerializeField] float followSmoothing = 8f;
        [SerializeField] float zoomSmoothing = 5f;

        [Header("Arena bounds (XY)")]
        [SerializeField] Vector2 boundsMin = new Vector2(-11f, -6f);
        [SerializeField] Vector2 boundsMax = new Vector2(11f, 6f);

        [Header("Placement")]
        [Tooltip("Distance from the play plane. The play plane is Z = 0.")]
        [SerializeField] float cameraDistance = 20f;

        Vector3 velocity;
        float currentSize;

        void Awake()
        {
            if (targetCamera == null)
                targetCamera = GetComponent<Camera>();
            currentSize = targetCamera != null ? targetCamera.orthographicSize : minOrthographicSize;
        }

        void LateUpdate()
        {
            if (targetCamera == null) return;
            if (!TryGetTargetBounds(out Vector2 min, out Vector2 max)) return;

            Vector2 center = (min + max) * 0.5f;
            Vector2 extents = (max - min) * 0.5f;

            float aspect = Mathf.Max(0.1f, targetCamera.aspect);
            float sizeForHeight = extents.y + padding;
            float sizeForWidth = (extents.x + padding) / aspect;
            float desiredSize = Mathf.Clamp(Mathf.Max(sizeForHeight, sizeForWidth), minOrthographicSize, maxOrthographicSize);

            currentSize = Mathf.Lerp(currentSize, desiredSize, 1f - Mathf.Exp(-zoomSmoothing * Time.deltaTime));
            targetCamera.orthographicSize = currentSize;

            // Keep the view inside the arena so the camera never reveals the outside.
            float halfHeight = currentSize;
            float halfWidth = currentSize * aspect;
            float clampedX = ClampAxis(center.x, boundsMin.x, boundsMax.x, halfWidth);
            float clampedY = ClampAxis(center.y, boundsMin.y, boundsMax.y, halfHeight);

            Vector3 desiredPosition = new Vector3(clampedX, clampedY, -cameraDistance);
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 1f / Mathf.Max(0.01f, followSmoothing));
        }

        /// <summary>Centres the axis when the arena is narrower than the view.</summary>
        static float ClampAxis(float value, float min, float max, float halfExtent)
        {
            if (max - min <= halfExtent * 2f)
                return (min + max) * 0.5f;
            return Mathf.Clamp(value, min + halfExtent, max - halfExtent);
        }

        bool TryGetTargetBounds(out Vector2 min, out Vector2 max)
        {
            min = new Vector2(float.MaxValue, float.MaxValue);
            max = new Vector2(float.MinValue, float.MinValue);
            bool any = false;

            if (registry != null)
            {
                var players = registry.Players;
                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i] == null) continue;
                    Include(players[i].transform.position, ref min, ref max);
                    any = true;
                }
            }

            for (int i = 0; i < additionalTargets.Length; i++)
            {
                if (additionalTargets[i] == null) continue;
                Include(additionalTargets[i].position, ref min, ref max);
                any = true;
            }

            return any;
        }

        static void Include(Vector3 position, ref Vector2 min, ref Vector2 max)
        {
            min.x = Mathf.Min(min.x, position.x);
            min.y = Mathf.Min(min.y, position.y);
            max.x = Mathf.Max(max.x, position.x);
            max.y = Mathf.Max(max.y, position.y);
        }
    }
}
