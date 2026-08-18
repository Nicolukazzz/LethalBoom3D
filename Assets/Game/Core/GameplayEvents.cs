using UnityEngine;

namespace LethalBoom.Core
{
    /// <summary>Payload structs for everything gameplay reports to the outside world.</summary>
    public readonly struct GoalScoredEvent
    {
        public readonly int ScorerId;
        public readonly int ConcedingPlayerId;
        public readonly int Points;
        public readonly float BombTimeHeldAtLaunch;
        public readonly Vector3 Position;

        public GoalScoredEvent(int scorerId, int concedingPlayerId, int points, float bombTimeHeldAtLaunch, Vector3 position)
        {
            ScorerId = scorerId;
            ConcedingPlayerId = concedingPlayerId;
            Points = points;
            BombTimeHeldAtLaunch = bombTimeHeldAtLaunch;
            Position = position;
        }
    }

    public readonly struct DiscLaunchedEvent
    {
        public readonly int LauncherId;
        public readonly Vector3 Position;
        public readonly Vector3 Direction;
        public readonly float Speed;
        public readonly float BombTimeHeld;

        public DiscLaunchedEvent(int launcherId, Vector3 position, Vector3 direction, float speed, float bombTimeHeld)
        {
            LauncherId = launcherId;
            Position = position;
            Direction = direction;
            Speed = speed;
            BombTimeHeld = bombTimeHeld;
        }
    }

    public readonly struct DiscPossessionEvent
    {
        /// <summary>-1 when the disc became loose (nobody holds it).</summary>
        public readonly int NewOwnerId;
        public readonly int PreviousOwnerId;
        public readonly PossessionCause Cause;
        public readonly Vector3 Position;

        public DiscPossessionEvent(int newOwnerId, int previousOwnerId, PossessionCause cause, Vector3 position)
        {
            NewOwnerId = newOwnerId;
            PreviousOwnerId = previousOwnerId;
            Cause = cause;
            Position = position;
        }
    }

    public enum PossessionCause
    {
        MatchStart,
        Pickup,
        Interception,
        Steal,
        Explosion,
        BounceLimit,
        Reset
    }

    public readonly struct DiscBounceEvent
    {
        public readonly Vector3 Position;
        public readonly Vector3 Normal;
        public readonly float Speed;
        public readonly int BounceCount;

        public DiscBounceEvent(Vector3 position, Vector3 normal, float speed, int bounceCount)
        {
            Position = position;
            Normal = normal;
            Speed = speed;
            BounceCount = bounceCount;
        }
    }

    public readonly struct DiscExplodedEvent
    {
        public readonly int VictimId;
        public readonly Vector3 Position;

        public DiscExplodedEvent(int victimId, Vector3 position)
        {
            VictimId = victimId;
            Position = position;
        }
    }

    public readonly struct PortalTraversedEvent
    {
        public readonly int TravelerPlayerId;
        public readonly bool TravelerIsDisc;
        public readonly int PortalOwnerId;
        public readonly Vector3 EntryPosition;
        public readonly Vector3 ExitPosition;

        public PortalTraversedEvent(int travelerPlayerId, bool travelerIsDisc, int portalOwnerId, Vector3 entryPosition, Vector3 exitPosition)
        {
            TravelerPlayerId = travelerPlayerId;
            TravelerIsDisc = travelerIsDisc;
            PortalOwnerId = portalOwnerId;
            EntryPosition = entryPosition;
            ExitPosition = exitPosition;
        }
    }

    public readonly struct PortalPlacedEvent
    {
        public readonly int OwnerId;
        public readonly Vector3 Position;
        public readonly Vector3 Normal;

        public PortalPlacedEvent(int ownerId, Vector3 position, Vector3 normal)
        {
            OwnerId = ownerId;
            Position = position;
            Normal = normal;
        }
    }

    public enum MatchPhase
    {
        Preparation,
        Countdown,
        Playing,
        GoalScored,
        Ended
    }

    public readonly struct MatchPhaseChangedEvent
    {
        public readonly MatchPhase Previous;
        public readonly MatchPhase Current;

        public MatchPhaseChangedEvent(MatchPhase previous, MatchPhase current)
        {
            Previous = previous;
            Current = current;
        }
    }
}
