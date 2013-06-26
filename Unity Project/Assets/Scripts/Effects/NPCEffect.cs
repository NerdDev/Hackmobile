using System;

public class NPCEffect : Effect
{
    protected NPCProperties refEnum; //the enum this effect is linked with
    protected NPC baseNPC; //the NPC the effect links to, simply a reference

    public NPCEffect(NPCProperties e, NPC n)
    {
        baseNPC = n;
        refEnum = e;
    }

    protected void updateFlag()
    {
        baseNPC.properties[refEnum] = this.On;
    }

    public virtual void apply(int p, bool item)
    {
        base.apply(p, item);
        updateFlag();
    }

    /*
     * For effects that require turn by turn changes
     *  - add them here to the switch and do an update method.
     */
    public void UpdateTurn()
    {
        switch (refEnum)
        {
            case NPCProperties.POISONED:
                updatePoison();
                break;
            default:
                break;
        }
    }

    void updatePoison() {
        //do poison update her
    }
}