using UnityEditor;
using UnityEngine;
using LethalBoom.Core;

namespace LethalBoom.EditorTools
{
    /// <summary>
    /// Builds the placeholder arena from primitives so the greybox is reproducible and
    /// reviewable as code instead of an unmergeable scene diff. Regenerate it after
    /// changing dimensions; hand-tweak only once the layout is final.
    /// Arena is a 2.5D box: play happens on the XY plane at Z = 0, depth is cosmetic.
    /// </summary>
    public static class GreyboxArenaBuilder
    {
        const string RootName = "GreyboxArena";

        // Playfield bounds, matching GameJamEA's court (left -10, right 10, top 5, bottom -5).
        const float HalfWidth = 10f;
        const float HalfHeight = 5f;
        const float Depth = 6f;
        const float Thickness = 1f;

        [MenuItem("Tools/LethalBoom/Build Greybox Arena")]
        public static void Build()
        {
            var existing = GameObject.Find(RootName);
            if (existing != null)
                Object.DestroyImmediate(existing);

            var root = new GameObject(RootName);

            int ground = LayerMask.NameToLayer(GameLayers.Ground);
            int wall = LayerMask.NameToLayer(GameLayers.Wall);
            int portalSurface = LayerMask.NameToLayer(GameLayers.PortalSurface);

            // Floor is Ground (jump landing); walls and ceiling double as portal surfaces.
            Box(root, "Floor", new Vector3(0, -HalfHeight - Thickness * 0.5f, 0),
                new Vector3(HalfWidth * 2f, Thickness, Depth), ground);
            Box(root, "Ceiling", new Vector3(0, HalfHeight + Thickness * 0.5f, 0),
                new Vector3(HalfWidth * 2f, Thickness, Depth), portalSurface);
            Box(root, "WallLeft", new Vector3(-HalfWidth - Thickness * 0.5f, 0, 0),
                new Vector3(Thickness, HalfHeight * 2f, Depth), portalSurface);
            Box(root, "WallRight", new Vector3(HalfWidth + Thickness * 0.5f, 0, 0),
                new Vector3(Thickness, HalfHeight * 2f, Depth), portalSurface);
            // Back wall keeps the camera framing readable; front is left open.
            Box(root, "WallBack", new Vector3(0, 0, Depth * 0.5f + Thickness * 0.5f),
                new Vector3(HalfWidth * 2f, HalfHeight * 2f, Thickness), wall);

            // A few platforms to test jumping and disc bounces.
            Box(root, "Platform_L", new Vector3(-5.5f, -1.5f, 0), new Vector3(4f, 0.5f, Depth * 0.6f), ground);
            Box(root, "Platform_R", new Vector3(5.5f, -1.5f, 0), new Vector3(4f, 0.5f, Depth * 0.6f), ground);
            Box(root, "Platform_Center", new Vector3(0f, 1.5f, 0), new Vector3(4f, 0.5f, Depth * 0.6f), ground);

            Undo.RegisterCreatedObjectUndo(root, "Build Greybox Arena");
            Debug.Log("[LethalBoom] Greybox arena rebuilt.");
        }

        static void Box(GameObject parent, string name, Vector3 position, Vector3 size, int layer)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent.transform);
            go.transform.position = position;
            go.transform.localScale = size;
            go.isStatic = true;
            if (layer >= 0)
                go.layer = layer;
        }
    }
}
