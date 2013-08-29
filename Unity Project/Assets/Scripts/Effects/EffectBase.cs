using System;
using XML;

public class EffectBase
{
    public string effect;
    public int turns;
    public float strength;

    public EffectBase(string effect, int turns, float strength)
    {
        this.effect = effect;
        this.turns = turns;
        this.strength = strength;
    }

    public EffectBase(XMLNode x)
    {
        this.parseXML(x);
    }

    public EffectInstance activate(NPC wo)
    {
        EffectInstance instance = new EffectInstance(wo, effect, strength, turns);
        return instance;
    }

    public void parseXML(XMLNode x)
    {
        effect = XMLNifty.SelectString(x, "type");
        turns = XMLNifty.SelectInt(x, "turns");
        strength = XMLNifty.SelectFloat(x, "strength");
    }
}