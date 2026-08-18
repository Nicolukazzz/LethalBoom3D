using NUnit.Framework;
using UnityEditor;
using UnityEngine.InputSystem;

namespace LethalBoom.Tests
{
    /// <summary>
    /// Guards the input asset against the class of breakage that made the sandbox
    /// unplayable: a composite binding written without its composite type in `path`
    /// resolves to null and throws deep inside InputBindingResolver, long after the
    /// asset itself imported without complaint.
    ///
    /// Also pins the action and control scheme names that PlayerInputRouter and
    /// LocalPlayerSpawner look up by string.
    /// </summary>
    public class InputActionsAssetTests
    {
        const string AssetPath = "Assets/Game/Input/LethalBoomControls.inputactions";

        InputActionAsset asset;

        [SetUp]
        public void SetUp()
        {
            asset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(AssetPath);
            Assert.NotNull(asset, $"Input asset not found at {AssetPath}");
        }

        [TearDown]
        public void TearDown()
        {
            if (asset != null)
                asset.Disable();
        }

        [Test]
        public void Player_map_resolves_its_bindings()
        {
            var map = asset.FindActionMap("Player", throwIfNotFound: true);

            // Enabling forces binding resolution; a malformed composite throws here.
            Assert.DoesNotThrow(() => map.Enable());
        }

        [TestCase("Move")]
        [TestCase("Jump")]
        [TestCase("AimThrow")]
        [TestCase("PlacePortal")]
        [TestCase("Ready")]
        [TestCase("Pause")]
        [TestCase("AimDirection")]
        public void Action_consumed_by_the_router_exists(string actionName)
        {
            var map = asset.FindActionMap("Player", throwIfNotFound: true);
            Assert.NotNull(map.FindAction(actionName), $"PlayerInputRouter expects an action named '{actionName}'.");
        }

        [TestCase("Keyboard Left")]
        [TestCase("Keyboard Right")]
        [TestCase("Gamepad")]
        public void Control_scheme_named_by_the_spawner_exists(string schemeName)
        {
            Assert.GreaterOrEqual(asset.FindControlSchemeIndex(schemeName), 0,
                $"LocalPlayerSpawner slots reference the '{schemeName}' control scheme.");
        }

        [Test]
        public void Every_composite_binding_declares_its_type()
        {
            var map = asset.FindActionMap("Player", throwIfNotFound: true);
            foreach (var binding in map.bindings)
            {
                if (!binding.isComposite) continue;
                Assert.IsNotEmpty(binding.path,
                    $"Composite binding '{binding.name}' has an empty path. The composite type (e.g. '2DVector') goes in path, not name.");
            }
        }

        [Test]
        public void Move_is_bound_in_every_control_scheme()
        {
            var map = asset.FindActionMap("Player", throwIfNotFound: true);
            var move = map.FindAction("Move", throwIfNotFound: true);

            foreach (string scheme in new[] { "Keyboard Left", "Keyboard Right", "Gamepad" })
            {
                bool bound = false;
                foreach (var binding in move.bindings)
                {
                    if (binding.groups != null && binding.groups.Contains(scheme))
                    {
                        bound = true;
                        break;
                    }
                }
                Assert.IsTrue(bound, $"Move has no binding in the '{scheme}' scheme.");
            }
        }
    }
}
