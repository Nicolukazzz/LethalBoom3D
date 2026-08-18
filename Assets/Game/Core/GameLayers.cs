namespace LethalBoom.Core
{
    /// <summary>
    /// Physics layers are the project's collision contract; the collision matrix in
    /// Project Settings does the filtering, not runtime tag string comparisons.
    /// Keep in sync with ProjectSettings/TagManager.asset.
    /// </summary>
    public static class GameLayers
    {
        public const string Player = "Player";
        public const string Disc = "Disc";
        public const string Ground = "Ground";
        public const string Wall = "Wall";
        public const string PortalSurface = "PortalSurface";
        public const string Portal = "Portal";
        public const string Goal = "Goal";
    }
}
