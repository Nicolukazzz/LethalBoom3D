using LethalBoom.Core;
using UnityEngine;

namespace LethalBoom.Presentation
{
    /// <summary>
    /// Debug listener: prints everything gameplay raises. Useful while building the
    /// sandbox scenes, and the reference implementation of how presentation binds to
    /// the channel — subscribe on enable, unsubscribe on disable, never poll.
    /// </summary>
    public class GameplayEventLogger : MonoBehaviour
    {
        [SerializeField] GameplayEventChannel channel;
        [SerializeField] bool enableLogging = true;

        void OnEnable()
        {
            if (channel == null) return;
            channel.MatchPhaseChanged += OnMatchPhaseChanged;
            channel.GoalScored += OnGoalScored;
            channel.DiscLaunched += OnDiscLaunched;
            channel.DiscPossessionChanged += OnPossessionChanged;
            channel.DiscExploded += OnDiscExploded;
            channel.PortalTraversed += OnPortalTraversed;
        }

        void OnDisable()
        {
            if (channel == null) return;
            channel.MatchPhaseChanged -= OnMatchPhaseChanged;
            channel.GoalScored -= OnGoalScored;
            channel.DiscLaunched -= OnDiscLaunched;
            channel.DiscPossessionChanged -= OnPossessionChanged;
            channel.DiscExploded -= OnDiscExploded;
            channel.PortalTraversed -= OnPortalTraversed;
        }

        void Log(string message)
        {
            if (enableLogging) Debug.Log($"[Gameplay] {message}");
        }

        void OnMatchPhaseChanged(MatchPhaseChangedEvent e) => Log($"phase {e.Previous} -> {e.Current}");
        void OnGoalScored(GoalScoredEvent e) => Log($"goal by player {e.ScorerId} for {e.Points} pts (bomb held {e.BombTimeHeldAtLaunch:F2}s)");
        void OnDiscLaunched(DiscLaunchedEvent e) => Log($"player {e.LauncherId} launched at {e.Speed:F1} u/s");
        void OnPossessionChanged(DiscPossessionEvent e) => Log($"possession {e.PreviousOwnerId} -> {e.NewOwnerId} ({e.Cause})");
        void OnDiscExploded(DiscExplodedEvent e) => Log($"disc exploded on player {e.VictimId}");
        void OnPortalTraversed(PortalTraversedEvent e) => Log($"portal traversed (disc: {e.TravelerIsDisc})");
    }
}
