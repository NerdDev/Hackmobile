using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Petrification : EffectInstance
{
    int timer = 4;

    public override void Apply(NPC n)
    {
        switch (timer)
        {
            case (4):
                //messages
                //lose speed bonuses
                break;
            case 3:
                //messages
                //slow NPC down
                break;
            case 2:
                //messages
                //lose limb functions (actions requiring limbs, limbs are stone)
                break;
            case 1:
                //messages
                //NPC turned to stnoe
                break;
            case 0:
                //messages
                //NPC is statue
                break;
        }
        timer--;
    }

    protected override void ParseParams(XML.XMLNode x)
    {
    }
}
