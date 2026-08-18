using System.Collections;
using LethalBoom.Gameplay.Match;
using LethalBoom.Gameplay.Player;
using LethalBoom.Input;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace LethalBoom.Tests
{
    /// <summary>
    /// Loads the real sandbox scene and checks it comes up clean. The unit tests build
    /// players by hand, so they never touched PlayerInput and missed a malformed input
    /// asset that made every spawn throw. This is the test that would have caught it.
    /// </summary>
    public class SandboxSceneTests
    {
        const string SceneName = "Sandbox_Movement";

        [UnityTest]
        public IEnumerator Sandbox_scene_spawns_both_players_without_errors()
        {
            SceneManager.LoadScene(SceneName, LoadSceneMode.Single);
            yield return null;
            yield return null;

            var registry = Object.FindFirstObjectByType<PlayerRegistry>();
            Assert.NotNull(registry, $"{SceneName} must contain a PlayerRegistry.");
            Assert.AreEqual(2, registry.Count, "Both local players should be registered.");

            for (int i = 0; i < registry.Count; i++)
            {
                var player = registry.Players[i];
                Assert.AreEqual(i, player.Id, "Players are registered in slot order.");
                Assert.NotNull(player.GetComponent<PlanarMovement>(), "Spawned player is missing PlanarMovement.");
                Assert.NotNull(player.GetComponent<PlayerInputRouter>(), "Spawned player is missing PlayerInputRouter.");
                Assert.NotNull(player.Profile, $"Player {i} has no profile assigned.");
            }

            // Any exception or error logged during spawn fails the test.
            LogAssert.NoUnexpectedReceived();
        }

        [UnityTest]
        public IEnumerator Spawned_players_land_on_the_arena()
        {
            SceneManager.LoadScene(SceneName, LoadSceneMode.Single);
            yield return null;

            for (int i = 0; i < 200; i++)
                yield return new WaitForFixedUpdate();

            var registry = Object.FindFirstObjectByType<PlayerRegistry>();
            Assert.NotNull(registry);

            foreach (var player in registry.Players)
            {
                var ground = player.GetComponent<GroundCheck>();
                Assert.NotNull(ground);
                Assert.IsTrue(ground.IsGrounded, $"{player.name} never landed; check the arena geometry or the ground mask.");
                Assert.That(player.transform.position.z, Is.EqualTo(0f).Within(0.001f), "Players must stay on the play plane.");
            }
        }

        [UnityTest]
        public IEnumerator Both_players_get_their_own_input_devices()
        {
            SceneManager.LoadScene(SceneName, LoadSceneMode.Single);
            yield return null;
            yield return null;

            var routers = Object.FindObjectsByType<PlayerInputRouter>(FindObjectsSortMode.None);
            Assert.AreEqual(2, routers.Length, "Each player owns one input router.");

            var indices = new System.Collections.Generic.HashSet<int>();
            foreach (var router in routers)
            {
                Assert.GreaterOrEqual(router.PlayerIndex, 0, "Router has no paired input user.");
                Assert.IsTrue(indices.Add(router.PlayerIndex), "Two players ended up sharing a player index.");
            }
        }
    }
}
