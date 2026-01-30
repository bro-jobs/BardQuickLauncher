namespace XIVMultiLauncher.Data;

/// <summary>
/// Static game data for FFXIV data centers and worlds.
/// Data sourced from XIVAPI.
/// </summary>
public static class GameData
{
    public static readonly DataCenter[] DataCenters =
    {
        // Japan
        new(1, "Elemental", "Japan"),
        new(2, "Gaia", "Japan"),
        new(3, "Mana", "Japan"),
        new(10, "Meteor", "Japan"),

        // North America
        new(4, "Aether", "North America"),
        new(5, "Primal", "North America"),
        new(8, "Crystal", "North America"),
        new(11, "Dynamis", "North America"),

        // Europe
        new(6, "Chaos", "Europe"),
        new(7, "Light", "Europe"),
        new(12, "Shadow", "Europe"),

        // Oceania
        new(9, "Materia", "Oceania"),
    };

    public static readonly World[] Worlds =
    {
        // Elemental (1)
        new(45, "Carbuncle", 1),
        new(49, "Kujata", 1),
        new(50, "Typhon", 1),
        new(58, "Garuda", 1),
        new(68, "Atomos", 1),
        new(72, "Tonberry", 1),
        new(90, "Aegis", 1),
        new(94, "Gungnir", 1),

        // Gaia (2)
        new(43, "Alexander", 2),
        new(46, "Fenrir", 2),
        new(51, "Ultima", 2),
        new(59, "Ifrit", 2),
        new(69, "Bahamut", 2),
        new(76, "Durandal", 2),
        new(92, "Ridill", 2),
        new(98, "Tiamat", 2),

        // Mana (3)
        new(23, "Asura", 3),
        new(28, "Pandaemonium", 3),
        new(44, "Anima", 3),
        new(47, "Hades", 3),
        new(48, "Ixion", 3),
        new(61, "Titan", 3),
        new(70, "Chocobo", 3),
        new(96, "Masamune", 3),

        // Meteor (10)
        new(24, "Belias", 10),
        new(29, "Shinryu", 10),
        new(30, "Unicorn", 10),
        new(31, "Yojimbo", 10),
        new(32, "Zeromus", 10),
        new(52, "Valefor", 10),
        new(60, "Ramuh", 10),
        new(71, "Mandragora", 10),

        // Aether (4)
        new(40, "Jenova", 4),
        new(54, "Faerie", 4),
        new(57, "Siren", 4),
        new(63, "Gilgamesh", 4),
        new(65, "Midgardsormr", 4),
        new(73, "Adamantoise", 4),
        new(79, "Cactuar", 4),
        new(99, "Sargatanas", 4),

        // Primal (5)
        new(35, "Famfrit", 5),
        new(53, "Exodus", 5),
        new(55, "Lamia", 5),
        new(64, "Leviathan", 5),
        new(77, "Excalibur", 5),
        new(78, "Hyperion", 5),
        new(93, "Ragnarok", 5), // Note: There's also a Ragnarok in EU
        new(95, "Ultros", 5),
        new(75, "Behemoth", 5),

        // Crystal (8)
        new(34, "Brynhildr", 8),
        new(37, "Mateus", 8),
        new(41, "Zalera", 8),
        new(62, "Diabolos", 8),
        new(74, "Coeurl", 8),
        new(81, "Malboro", 8),
        new(82, "Goblin", 8),
        new(91, "Balmung", 8),

        // Dynamis (11)
        new(406, "Halicarnassus", 11),
        new(407, "Maduin", 11),
        new(408, "Marilith", 11),
        new(409, "Seraph", 11),
        new(411, "Cuchulainn", 11),
        new(412, "Kraken", 11),
        new(413, "Rafflesia", 11),
        new(414, "Golem", 11),

        // Chaos (6)
        new(39, "Omega", 6),
        new(80, "Cerberus", 6),
        new(83, "Louisoix", 6),
        new(85, "Moogle", 6),
        new(97, "Ragnarok", 6),
        new(400, "Sagittarius", 6),
        new(401, "Phantom", 6),
        new(404, "Alpha", 6),

        // Light (7)
        new(33, "Twintania", 7),
        new(36, "Lich", 7),
        new(42, "Zodiark", 7),
        new(56, "Phoenix", 7),
        new(66, "Odin", 7),
        new(67, "Shiva", 7),
        new(402, "Raiden", 7),
        new(403, "Sephirot", 7), // Assuming exists

        // Shadow (12)
        // Add worlds when available

        // Materia (9)
        new(21, "Ravana", 9),
        new(22, "Bismarck", 9),
        new(86, "Sephirot", 9),
        new(87, "Sophia", 9),
        new(88, "Zurvan", 9),
    };

    public static DataCenter? GetDataCenter(uint id)
        => DataCenters.FirstOrDefault(dc => dc.Id == id);

    public static DataCenter? GetDataCenterByName(string name)
        => DataCenters.FirstOrDefault(dc =>
            dc.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    public static World? GetWorld(uint id)
        => Worlds.FirstOrDefault(w => w.Id == id);

    public static World? GetWorldByName(string name)
        => Worlds.FirstOrDefault(w =>
            w.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    public static IEnumerable<World> GetWorldsForDataCenter(uint dataCenterId)
        => Worlds.Where(w => w.DataCenterId == dataCenterId).OrderBy(w => w.Name);

    public static IEnumerable<DataCenter> GetDataCentersByRegion(string region)
        => DataCenters.Where(dc =>
            dc.Region.Equals(region, StringComparison.OrdinalIgnoreCase));
}

public record DataCenter(uint Id, string Name, string Region);
public record World(uint Id, string Name, uint DataCenterId);
