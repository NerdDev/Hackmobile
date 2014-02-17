using System.Collections.Generic;
public class PlayerStats
{
    //
    // All this will need refactoring later
    //

    public static void Load(Player player, Role role, Race race)
    {
        player.Role = role;
        player.Race = race;
        LoadBasicStats(player);
        LoadStatModRole(player, role);
        LoadStatModRace(player, race);
    }

    private static void LoadStatModRace(Player player, Race race)
    {
        //do nothing atm
    }

    private static void LoadStatModRole(Player player, Role role)
    {
        //do nothing atm
    }

    private static void LoadBasicStats(Player player)
    {
        AttributesData attr = new AttributesData();
        attr.Constitution = 14;
        attr.Charisma = 12;
        attr.Wisdom = 9;
        attr.Dexterity = 17;
        attr.Intelligence = 16;
        attr.Strength = 16;
        attr.Difficulty = 1;
        attr.Size = Size.MEDIUM;
        player.Attributes = attr;

        Stats stats = new Stats();
        stats.MaxHealth = 100;
        stats.MaxPower = 50;
        stats.Level = 1;
        stats.initialize();
        stats.Hunger = 900;
        player.Stats = stats;

        ESFlags<NPCFlags> flags = new ESFlags<NPCFlags>();
        flags.Set(true, NPCFlags.NOPOLY);
        flags.Set(true, NPCFlags.NO_RANDOM_SPAWN);
        player.Flags = flags;

        BodyParts bp = new BodyParts();
        bp.Arms = 2;
        bp.Legs = 2;
        bp.Heads = 1;
        player.Bodyparts = bp;

        player.PlayerTitle = BigBoss.Objects.PlayerProfessions.getTitle(player.PlayerChosenProfession, player.Stats.Level);
        player.IsActive = true;
        player.Equipment = new Equipment(player.Bodyparts);
        player.Name = "Kurtis";

        foreach (KeyValuePair<string, Spell> kvp in BigBoss.Objects.PlayerSpells)
        {
            player.KnownSpells.Add(kvp.Key, kvp.Value);
        }

    }
}
