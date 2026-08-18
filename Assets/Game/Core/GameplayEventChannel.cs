using System;
using UnityEngine;

namespace LethalBoom.Core
{
    /// <summary>
    /// The single seam between gameplay and everything that only reacts to it:
    /// HUD, audio, VFX, camera shake and (later) playtest metrics.
    ///
    /// Gameplay code raises events here and never references a ParticleSystem,
    /// AudioSource, Animator or Canvas. Presentation subscribes and translates.
    /// Being a ScriptableObject, it is wired through the inspector instead of a
    /// singleton, so a sandbox scene can use its own channel in isolation.
    /// </summary>
    [CreateAssetMenu(menuName = "LethalBoom/Gameplay Event Channel", fileName = "GameplayEventChannel")]
    public class GameplayEventChannel : ScriptableObject
    {
        public event Action<MatchPhaseChangedEvent> MatchPhaseChanged;
        public event Action<GoalScoredEvent> GoalScored;
        public event Action<DiscLaunchedEvent> DiscLaunched;
        public event Action<DiscPossessionEvent> DiscPossessionChanged;
        public event Action<DiscBounceEvent> DiscBounced;
        public event Action<DiscExplodedEvent> DiscExploded;
        public event Action<PortalPlacedEvent> PortalPlaced;
        public event Action<PortalTraversedEvent> PortalTraversed;

        public void Raise(MatchPhaseChangedEvent e) => MatchPhaseChanged?.Invoke(e);
        public void Raise(GoalScoredEvent e) => GoalScored?.Invoke(e);
        public void Raise(DiscLaunchedEvent e) => DiscLaunched?.Invoke(e);
        public void Raise(DiscPossessionEvent e) => DiscPossessionChanged?.Invoke(e);
        public void Raise(DiscBounceEvent e) => DiscBounced?.Invoke(e);
        public void Raise(DiscExplodedEvent e) => DiscExploded?.Invoke(e);
        public void Raise(PortalPlacedEvent e) => PortalPlaced?.Invoke(e);
        public void Raise(PortalTraversedEvent e) => PortalTraversed?.Invoke(e);

        /// <summary>
        /// ScriptableObjects survive play-mode reloads in the editor; stale subscribers
        /// from a previous session would leak. Cleared on enable to keep sessions clean.
        /// </summary>
        void OnEnable()
        {
            MatchPhaseChanged = null;
            GoalScored = null;
            DiscLaunched = null;
            DiscPossessionChanged = null;
            DiscBounced = null;
            DiscExploded = null;
            PortalPlaced = null;
            PortalTraversed = null;
        }
    }
}
