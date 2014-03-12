using System.Collections.Generic;

public class PlayerStats
{
    //
    // All this will need refactoring later
    //

    public static void Load(Player player, Role role, Race race)
    {
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

        GenericFlags<NPCFlags> flags = new GenericFlags<NPCFlags>();
        flags[NPCFlags.NOPOLY] = true;
        flags[NPCFlags.NO_RANDOM_SPAWN] = true;
        player.Flags = flags;

        BodyParts bp = new BodyParts();
        bp.Arms = 2;
        bp.Legs = 2;
        bp.Heads = 1;
        player.Bodyparts = bp;

        player.PlayerTitle = BigBoss.Objects.PlayerProfessions.getTitle(player.PlayerChosenProfession, player.Stats.Level);
        player.IsActive = true;

        Equipment equipment = new Equipment();
        player.Equipment = equipment;
        equipment.AddSlot(EquipType.BODY);
        equipment.AddSlot(EquipType.FEET);
        equipment.AddSlot(EquipType.HAND, 2);
        equipment.AddSlot(EquipType.HEAD);
        equipment.AddSlot(EquipType.LEGS);
        equipment.AddSlot(EquipType.NECK);
        equipment.AddSlot(EquipType.RING, 2);
        equipment.AddSlot(EquipType.SHIRT);

        player.Name = "Kurtis";

        foreach (KeyValuePair<string, Spell> kvp in BigBoss.Objects.PlayerSpells)
        {
            player.KnownSpells.Add(kvp.Key, kvp.Value);
        }

    }
}
