using System.Collections.Generic;
using LethalBoom.Gameplay.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace LethalBoom.Gameplay.Match
{
    /// <summary>
    /// Spawns the local players of a match from a list of slots and registers them.
    ///
    /// Each slot names a control scheme instead of a device, so "player 2 uses the right
    /// half of the keyboard" and "player 2 uses a gamepad" are the same code path. Two
    /// players can share one keyboard because the pairing is explicit and non-exclusive.
    /// </summary>
    public class LocalPlayerSpawner : MonoBehaviour
    {
        [System.Serializable]
        public class Slot
        {
            public PlayerProfile profile;
            [Tooltip("Control scheme name from the .inputactions asset, e.g. 'Keyboard Left'.")]
            public string controlScheme = "Keyboard Left";
            [Tooltip("Prefer a gamepad for this slot when one is connected.")]
            public bool preferGamepad;
            public Transform spawnPoint;
        }

        [SerializeField] GameObject playerPrefab;
        [SerializeField] PlayerRegistry registry;
        [SerializeField] List<Slot> slots = new();
        [SerializeField] bool spawnOnStart = true;

        readonly List<GameObject> spawned = new();

        void Start()
        {
            if (spawnOnStart)
                SpawnAll();
        }

        public void SpawnAll()
        {
            if (playerPrefab == null || registry == null)
            {
                Debug.LogError($"{name}: LocalPlayerSpawner needs a player prefab and a registry.", this);
                return;
            }

            var gamepads = new List<Gamepad>(Gamepad.all);
            int gamepadCursor = 0;

            for (int i = 0; i < slots.Count; i++)
            {
                Slot slot = slots[i];
                InputDevice device = null;
                string scheme = slot.controlScheme;

                if (slot.preferGamepad && gamepadCursor < gamepads.Count)
                {
                    device = gamepads[gamepadCursor++];
                    scheme = "Gamepad";
                }
                else if (Keyboard.current != null)
                {
                    device = Keyboard.current;
                }

                var player = PlayerInput.Instantiate(playerPrefab, i, scheme, -1, device);
                if (player == null)
                {
                    Debug.LogError($"{name}: could not instantiate player {i} with scheme '{scheme}'.", this);
                    continue;
                }

                player.gameObject.name = slot.profile != null ? slot.profile.displayName : $"Player {i}";
                spawned.Add(player.gameObject);

                var identity = player.GetComponent<PlayerIdentity>();
                if (identity != null)
                {
                    identity.Assign(i, slot.profile);
                    registry.Register(identity);
                }

                if (slot.spawnPoint != null)
                {
                    var movement = player.GetComponent<PlanarMovement>();
                    if (movement != null)
                        movement.Teleport(slot.spawnPoint.position);
                    else
                        player.transform.position = slot.spawnPoint.position;
                }

                ApplyProfileColor(player.gameObject, slot.profile);
            }
        }

        /// <summary>
        /// Placeholder tinting so the two capsules are told apart before there is art.
        /// Real visuals will move to Presentation once the player has a model.
        /// </summary>
        static void ApplyProfileColor(GameObject player, PlayerProfile profile)
        {
            if (profile == null) return;
            var renderers = player.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
                renderer.material.color = profile.color;
        }

        void OnDisable()
        {
            // Unpair so a domain reload or scene reload does not leak input users.
            foreach (var player in spawned)
            {
                if (player == null) continue;
                var input = player.GetComponent<PlayerInput>();
                if (input != null && input.user.valid)
                    input.user.UnpairDevices();
            }
            spawned.Clear();
        }
    }
}
