using System.Collections;
using LethalBoom.Core;
using LethalBoom.Gameplay.Player;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LethalBoom.Tests
{
    /// <summary>
    /// Movement is the base every other system stands on, so it gets the only automated
    /// tests for now: gravity, ground detection and the double jump. Feel still has to be
    /// judged by hand in the sandbox; these only guard the mechanics from silent breakage.
    /// </summary>
    public class PlanarMovementTests
    {
        GameObject floor;
        GameObject player;
        PlanarMovement movement;
        GroundCheck groundCheck;
        PlayerAbilities abilities;
        Rigidbody body;
        PlayerMovementConfig config;

        [SetUp]
        public void SetUp()
        {
            int groundLayer = LayerMask.NameToLayer(GameLayers.Ground);

            floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "TestFloor";
            floor.transform.position = new Vector3(0f, -1f, 0f);
            floor.transform.localScale = new Vector3(20f, 1f, 5f);
            floor.layer = groundLayer;

            config = ScriptableObject.CreateInstance<PlayerMovementConfig>();
            config.moveSpeed = 5f;
            config.acceleration = 0f;
            config.jumpSpeed = 8f;
            config.maxJumps = 2;
            config.gravityMultiplier = 3f;

            player = new GameObject("TestPlayer");
            // Built inactive so Awake runs after the serialized fields are injected,
            // the same order Unity gives a prefab whose references are set in the editor.
            player.SetActive(false);
            player.transform.position = new Vector3(0f, 1f, 0f);
            var capsule = player.AddComponent<CapsuleCollider>();
            capsule.height = 1.8f;
            capsule.radius = 0.4f;
            capsule.center = new Vector3(0f, 0.9f, 0f);
            body = player.AddComponent<Rigidbody>();
            groundCheck = player.AddComponent<GroundCheck>();
            abilities = player.AddComponent<PlayerAbilities>();
            movement = player.AddComponent<PlanarMovement>();

            SetPrivate(groundCheck, "groundMask", (LayerMask)(1 << groundLayer));
            SetPrivate(movement, "config", config);

            player.SetActive(true);
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(player);
            Object.Destroy(floor);
            Object.Destroy(config);
        }

        static void SetPrivate(Object target, string field, object value)
        {
            var info = target.GetType().GetField(field,
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            Assert.NotNull(info, $"Field '{field}' not found on {target.GetType().Name}");
            info.SetValue(target, value);
        }

        static IEnumerator Physics(int steps)
        {
            for (int i = 0; i < steps; i++)
                yield return new WaitForFixedUpdate();
        }

        [UnityTest]
        public IEnumerator Player_falls_and_lands_on_ground()
        {
            yield return Physics(120);

            Assert.IsTrue(groundCheck.IsGrounded, "Player should be standing on the floor.");
            Assert.That(body.linearVelocity.y, Is.EqualTo(0f).Within(0.5f), "Vertical velocity should settle on landing.");
        }

        [UnityTest]
        public IEnumerator Player_stays_on_the_play_plane()
        {
            body.AddForce(new Vector3(0f, 0f, 50f), ForceMode.Impulse);
            yield return Physics(60);

            Assert.That(player.transform.position.z, Is.EqualTo(0f).Within(0.001f),
                "Movement is 2.5D: Z must stay frozen even under force.");
        }

        [UnityTest]
        public IEnumerator Double_jump_gives_exactly_two_jumps()
        {
            yield return Physics(120);
            Assert.IsTrue(groundCheck.IsGrounded);

            movement.SendMessage("OnJumpPressed", SendMessageOptions.DontRequireReceiver);
            yield return Physics(2);
            Assert.Greater(body.linearVelocity.y, 0f, "First jump should push the player up.");

            yield return Physics(10);
            movement.SendMessage("OnJumpPressed", SendMessageOptions.DontRequireReceiver);
            yield return Physics(2);
            Assert.Greater(body.linearVelocity.y, 0f, "Second jump should still work in the air.");

            yield return Physics(10);
            float beforeThird = body.linearVelocity.y;
            movement.SendMessage("OnJumpPressed", SendMessageOptions.DontRequireReceiver);
            yield return Physics(2);
            Assert.Less(body.linearVelocity.y, beforeThird + 0.01f, "A third jump must be ignored.");
        }

        [UnityTest]
        public IEnumerator Blocked_jump_ability_prevents_jumping()
        {
            yield return Physics(120);

            var modifier = PlayerAbilities.Modifier.Default;
            modifier.BlocksJump = true;
            abilities.Set(this, modifier);

            movement.SendMessage("OnJumpPressed", SendMessageOptions.DontRequireReceiver);
            yield return Physics(3);

            Assert.That(body.linearVelocity.y, Is.LessThanOrEqualTo(0.01f), "Jump must be blocked while a modifier forbids it.");
        }

        [UnityTest]
        public IEnumerator Abilities_multiply_instead_of_overwriting()
        {
            var half = PlayerAbilities.Modifier.Default;
            half.SpeedMultiplier = 0.5f;
            var quarter = PlayerAbilities.Modifier.Default;
            quarter.SpeedMultiplier = 0.5f;

            abilities.Set("aim", half);
            abilities.Set("stun", quarter);
            yield return null;

            Assert.That(abilities.SpeedMultiplier, Is.EqualTo(0.25f).Within(0.0001f));

            abilities.Clear("stun");
            yield return null;
            Assert.That(abilities.SpeedMultiplier, Is.EqualTo(0.5f).Within(0.0001f));
        }
    }
}
