using UnityEngine;

namespace LethalBoom.Gameplay.Player
{
    /// <summary>
    /// Per-player identity data. Replaces the hardcoded Player1/Player2 enum of GameJamEA:
    /// systems work with whatever profiles the match registered.
    /// </summary>
    [CreateAssetMenu(menuName = "LethalBoom/Config/Player Profile", fileName = "PlayerProfile")]
    public class PlayerProfile : ScriptableObject
    {
        [Tooltip("Shown in HUD and end screen.")]
        public string displayName = "Player";
        [Tooltip("Team colour used by HUD, portals, goals and VFX.")]
        public Color color = Color.cyan;
    }
}
