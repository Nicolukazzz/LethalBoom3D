using System.Collections.Generic;
using UnityEngine;

namespace LethalBoom.Gameplay.Player
{
    /// <summary>
    /// Stackable modifiers to how a player moves. Aiming slows you down, a bump locks
    /// your steering, a future stun could do both — each system registers its own
    /// modifier under its own key and removes it when done.
    ///
    /// In the jam project the disc component wrote straight into the movement
    /// component's properties, so whoever cleared them last won and a bug in one
    /// system left the player permanently frozen.
    /// </summary>
    public class PlayerAbilities : MonoBehaviour
    {
        public struct Modifier
        {
            public float SpeedMultiplier;
            public float GravityMultiplier;
            public bool BlocksJump;
            public bool BlocksSteering;

            public static Modifier Default => new Modifier
            {
                SpeedMultiplier = 1f,
                GravityMultiplier = 1f,
                BlocksJump = false,
                BlocksSteering = false
            };
        }

        readonly Dictionary<object, Modifier> modifiers = new();

        /// <summary>Product of every registered speed multiplier.</summary>
        public float SpeedMultiplier { get; private set; } = 1f;

        /// <summary>Product of every registered gravity multiplier.</summary>
        public float GravityMultiplier { get; private set; } = 1f;

        /// <summary>True if any system currently forbids jumping.</summary>
        public bool JumpBlocked { get; private set; }

        /// <summary>True if any system currently takes horizontal control away (bump, stun).</summary>
        public bool SteeringBlocked { get; private set; }

        /// <summary>Registers or replaces the modifier owned by <paramref name="source"/>.</summary>
        public void Set(object source, Modifier modifier)
        {
            if (source == null) return;
            modifiers[source] = modifier;
            Recalculate();
        }

        public void Clear(object source)
        {
            if (source == null) return;
            if (modifiers.Remove(source))
                Recalculate();
        }

        void Recalculate()
        {
            float speed = 1f;
            float gravity = 1f;
            bool jumpBlocked = false;
            bool steeringBlocked = false;

            foreach (var modifier in modifiers.Values)
            {
                speed *= modifier.SpeedMultiplier;
                gravity *= modifier.GravityMultiplier;
                jumpBlocked |= modifier.BlocksJump;
                steeringBlocked |= modifier.BlocksSteering;
            }

            SpeedMultiplier = speed;
            GravityMultiplier = gravity;
            JumpBlocked = jumpBlocked;
            SteeringBlocked = steeringBlocked;
        }
    }
}
