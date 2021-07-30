namespace TribesOfDust.Tribes
{
    /// <summary>
    /// The tribe enumeration defines a list of tribes available for players to pick from.
    /// </summary>
    /// <remarks>
    /// This enumeration currently serves too many purposes, as indicated by the combination of
    /// ownership entity type and race. For now, it doesn't make a lot of sense to split this, but
    /// keep in mind when using this enumeration, that it is likely to change.
    /// </remarks>
    public enum TribeType
    {
        Unknown,

        PlayerRomans,
        PlayerDruids,
        PlayerInuits,
        PlayerPirates,

        Monster,
    }
}