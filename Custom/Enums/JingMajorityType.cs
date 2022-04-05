namespace TerraBend.Custom.Enums {
    /// <summary>
    /// Slightly specific type of enum that represents, at a given type,
    /// what the status of majority is for a player's Jing. Contains all
    /// of the Jing Types + "Balanced".
    /// </summary>
    public enum JingMajorityType {
        Positive,
        Neutral,
        Negative,
        Unaligned,
        Balanced
    }
}