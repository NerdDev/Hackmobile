using System;

public class NPCEffect : Effect
{
    protected Properties refEnum; //the enum this effect is linked with
    protected NPC baseNPC; //the NPC the effect links to, simply a reference

    public NPCEffect(Properties e, NPC n)
    {
        baseNPC = n;
        refEnum = e;
    }

    protected void updateFlag()
    {
        baseNPC.properties[refEnum] = this.On;
    }

    public override void apply(int p, bool item)
    {
        base.apply(p, item);
        updateFlag();
    }

    public override void apply(int p, bool item, int turnsToProcess)
    {
        base.apply(p, item, turnsToProcess);
        updateFlag();
    }

    /*
     * For effects that require turn by turn changes
     *  - add them here to the switch and do an update method.
     */
    public override void UpdateTurn()
    {
        base.UpdateTurn();
        if (isActive && On)
        {
            switch (refEnum)
            {
                case Properties.POISONED:
                    updatePoison();
                    break;
                default:
                    break;
            }
        }
    }

    void updatePoison() {
        switch (eff)
        {
            case 1:
                baseNPC.AdjustHealth(-2);
                break;
            case 2:
                baseNPC.AdjustHealth(-4);
                break;
            case 3:
                baseNPC.AdjustHealth(-6);
                break;
            case 4:
                baseNPC.AdjustHealth(-8);
                break;
            case 5:
                baseNPC.AdjustHealth(-10);
                break;
            default:
                baseNPC.AdjustHealth(-10);
                break;
        }
    }
}