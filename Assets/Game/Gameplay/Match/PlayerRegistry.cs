using System;
using System.Collections.Generic;
using LethalBoom.Gameplay.Player;
using UnityEngine;

namespace LethalBoom.Gameplay.Match
{
    /// <summary>
    /// The players taking part in the current match. Every system that needs "the other
    /// player", "all players" or "player by id" asks here instead of holding a
    /// player1/player2 pair (ADR-0003).
    /// </summary>
    public class PlayerRegistry : MonoBehaviour
    {
        readonly List<PlayerIdentity> players = new();

        public event Action<PlayerIdentity> PlayerRegistered;

        public IReadOnlyList<PlayerIdentity> Players => players;
        public int Count => players.Count;

        public void Register(PlayerIdentity player)
        {
            if (player == null || players.Contains(player)) return;
            players.Add(player);
            PlayerRegistered?.Invoke(player);
        }

        public void Unregister(PlayerIdentity player)
        {
            if (player == null) return;
            players.Remove(player);
        }

        public PlayerIdentity GetById(int id)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i] != null && players[i].Id == id)
                    return players[i];
            }
            return null;
        }

        /// <summary>
        /// The opponent of the given player. Returns the next registered player, so a
        /// three-player mode would need a real rule here instead of a silent assumption.
        /// </summary>
        public PlayerIdentity GetOpponentOf(PlayerIdentity player)
        {
            if (players.Count < 2 || player == null) return null;
            int index = players.IndexOf(player);
            if (index < 0) return null;
            return players[(index + 1) % players.Count];
        }
    }
}
