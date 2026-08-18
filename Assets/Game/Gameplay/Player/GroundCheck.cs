using UnityEngine;

namespace LethalBoom.Gameplay.Player
{
    /// <summary>
    /// Ground detection isolated from movement so it can be tuned, visualised and
    /// replaced on its own. Uses a sphere overlap under the collider instead of a
    /// raycast so edges and slopes do not produce false negatives.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class GroundCheck : MonoBehaviour
    {
        [SerializeField] LayerMask groundMask;
        [SerializeField] float radius = 0.25f;
        [SerializeField] float distance = 0.1f;
        [SerializeField] bool drawGizmo = true;

        Collider ownCollider;
        readonly Collider[] hits = new Collider[4];

        /// <summary>True while standing on something in the ground mask.</summary>
        public bool IsGrounded { get; private set; }

        /// <summary>True on the frame the player touched down. Movement uses it to refill jumps.</summary>
        public bool JustLanded { get; private set; }

        void Awake() => ownCollider = GetComponent<Collider>();

        void FixedUpdate()
        {
            bool wasGrounded = IsGrounded;
            IsGrounded = Physics.OverlapSphereNonAlloc(CheckPosition(), radius, hits, groundMask, QueryTriggerInteraction.Ignore) > 0;
            JustLanded = IsGrounded && !wasGrounded;
        }

        Vector3 CheckPosition()
        {
            Vector3 position = transform.position;
            float bottom = ownCollider != null ? ownCollider.bounds.min.y : position.y;
            return new Vector3(position.x, bottom - distance, position.z);
        }

        void OnDrawGizmosSelected()
        {
            if (!drawGizmo) return;
            if (ownCollider == null) ownCollider = GetComponent<Collider>();
            Gizmos.color = Application.isPlaying && IsGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(CheckPosition(), radius);
        }
    }
}
