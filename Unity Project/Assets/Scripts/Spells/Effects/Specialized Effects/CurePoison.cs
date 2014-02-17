using System;
using XML;

public class CurePoison : EffectInstance
{
    public override void Init(NPC n)
    {
        n.RemoveAnEffect<PoisonEffect>();
    }

    protected override void ParseParams(XMLNode x)
    {
    }

    public override string ToString()
    {
        return "CurePoison";
    }
}
