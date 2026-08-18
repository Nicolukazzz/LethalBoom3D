using UnityEngine;

namespace LethalBoom.Gameplay.Match
{
    /// <summary>Match rules and scoring. Balance lives here, not in code.</summary>
    [CreateAssetMenu(menuName = "LethalBoom/Config/Match", fileName = "MatchConfig")]
    public class MatchConfig : ScriptableObject
    {
        [Header("Duration")]
        [Tooltip("Match length in seconds. GameJamEA used 60.")]
        public float matchDurationSeconds = 60f;
        [Tooltip("Countdown before the clock starts running.")]
        public float countdownSeconds = 3f;

        [Header("Scoring")]
        [Tooltip("Points awarded for any goal, before the bomb bonus.")]
        public int baseScorePerGoal = 10;
        [Tooltip("Extra points when the disc was launched with the bomb about to blow (scaled 0..1 by held time).")]
        public int maxBombBonusPerGoal = 20;

        [Header("After a goal")]
        [Tooltip("Delay before the disc comes back into play.")]
        public float discRespawnDelaySeconds = 1f;
        [Tooltip("Whether goals get relocated after every goal (GameJamEA did).")]
        public bool relocateGoalsAfterScore = true;

        [Header("Respawn")]
        [Tooltip("Delay before a player killed by the bomb is playable again.")]
        public float respawnDelayAfterExplosion = 1.5f;
    }
}
