using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class EffectInstance : PassesTurns
{
    public int turnsToProcess;
    public float strength; //strength of effect
    public NPC wo; //ref to NPC this is on
    public string effect; //string key to effect reference

    public EffectInstance()
    {
    }

    public EffectInstance(NPC wo, string eff, float str, int turns = 0)
    {
        apply(wo, eff, str, turns);
    }

    public void apply(NPC worldObject, string effect, float strength, int turns = 0)
    {
        this.wo = worldObject;
        this.effect = effect;
        this.strength = strength;
        this.turnsToProcess = turns;
        if (turns == 0)
        {
            EffectManager.effects[effect].init(worldObject, strength);
            EffectManager.effects[effect].apply(worldObject, strength);
        }
        else
        {
            EffectManager.effects[effect].init(worldObject, strength);
            IsActive = true;
            BigBoss.TimeKeeper.RegisterToUpdateList(this);
        }
    }

    public EffectInstance merge(EffectBase newEffect)
    {
        return EffectManager.effects[effect].merge(this, newEffect);
    }

    #region Turn Management
    public void UpdateTurn()
    {
        if (checkTurns())
        {
            EffectManager.effects[effect].apply(wo, strength);
        }
    }

    private bool checkTurns()
    {
        if (turnsToProcess > 0)
        {
            turnsToProcess--;
            return true;
        }
        else if (turnsToProcess == 0)
        {
            EffectManager.effects[effect].remove(wo, strength);
            return false;
        }
        else
        {
            return true;
        }
    }

    int currentPoints;
    public int CurrentPoints
    {
        get
        {
            return currentPoints;
        }
        set
        {
            currentPoints = value;
        }
    }

    int basePoints;
    public int BasePoints
    {
        get
        {
            return basePoints;
        }
        set
        {
            basePoints = value;
        }
    }

    bool isActive;
    public bool IsActive
    {
        get
        {
            return isActive;
        }
        set
        {
            isActive = value;
        }
    }
    #endregion
}