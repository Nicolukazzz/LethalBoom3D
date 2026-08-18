using UnityEngine;

namespace LethalBoom.Gameplay.Player
{
    /// <summary>
    /// Who this player is at runtime. Systems address players by <see cref="Id"/>,
    /// never by a Player1/Player2 enum, so nothing breaks when the match has a
    /// different number of players or a different pairing of devices.
    /// </summary>
    public class PlayerIdentity : MonoBehaviour
    {
        [SerializeField] PlayerProfile profile;

        /// <summary>Assigned when the player joins the match. -1 until then.</summary>
        public int Id { get; private set; } = -1;

        public PlayerProfile Profile => profile;
        public string DisplayName => profile != null ? profile.displayName : $"Player {Id}";
        public Color Color => profile != null ? profile.color : Color.white;

        public void Assign(int id, PlayerProfile playerProfile)
        {
            Id = id;
            if (playerProfile != null)
                profile = playerProfile;
        }

        /// <summary>Used when the player is spawned already configured in a sandbox scene.</summary>
        public void AssignId(int id) => Id = id;
    }
}
