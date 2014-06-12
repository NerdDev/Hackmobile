using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Invisibility : EffectInstance
{
    Dictionary<Renderer, Shader[]> originalShaders = new Dictionary<Renderer, Shader[]>();
    Dictionary<Renderer, Color[]> originalColors = new Dictionary<Renderer, Color[]>();
    bool isInvisible;

    public override void Init(NPC n)
    {
        if (n.IsNotAFreaking<Player>())
        {
            //--npc
            //disable rendering on object
            //wispy object displayed in place of NPC?
            ToggleInvisibility(n);
        }
        else
        {
            //--player
            //apply shader to player, NPC's should check for invisibility in their AI functions, not here
            EnableInvisibility(n, .4f);
        }
    }

    private void EnableInvisibility(NPC n, float alpha)
    {
        if (!isInvisible)
        {
            isInvisible = true;
            GameObject npcObj = n.GO;
            foreach (Renderer r in npcObj.GetComponentsInChildren<Renderer>())
            {
                Material[] materials = r.materials;
                originalShaders.Add(r, new Shader[materials.Length]);
                originalColors.Add(r, new Color[materials.Length]);
                Color temp;
                for (int i = 0; i < materials.Length; i++)
                {
                    originalShaders[r][i] = materials[i].shader;
                    materials[i].shader = BigBoss.Gooey.InvisibilityShader;
                    originalColors[r][i] = materials[i].color;
                    temp = originalColors[r][i];
                    temp = materials[i].color;
                    temp.a = alpha;
                    materials[i].color = temp;
                }
            }
        }
    }

    public override void Apply(NPC n)
    {
        //check if Player has SeeInvisible and enable rendering if so, disable if otherwise
        if (n.IsNotAFreaking<Player>())
        {
            ToggleInvisibility(n);
        }
    }

    private void ToggleInvisibility(NPC n)
    {
        if (BigBoss.Player.CanSeeInvisible())
        {
            DisableInvisibility(n);
        }
        else
        {
            EnableInvisibility(n, .1f);
        }
    }

    public override void Remove(NPC n)
    {
        //--player
        //apply shader to player, NPC's should check for invisibility in their AI functions, not here
        //either way disable this dang thing
        DisableInvisibility(n);
    }

    private void DisableInvisibility(NPC n)
    {
        if (isInvisible)
        {
            isInvisible = false;
            GameObject npcObj = n.GO;
            foreach (Renderer r in npcObj.GetComponentsInChildren<Renderer>())
            {
                Material[] materials = r.materials;
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i].shader = originalShaders[r][i];
                    materials[i].color = originalColors[r][i];
                }
            }
        }
    }

    protected override void ParseParams(XMLNode x)
    {
    }
}
