using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

/*   
 * As long as this isn't an MMO, this Player class should be able to hold most if not all of player information.
 */
public class Player : NPC
{
    // Disable unassigned warnings
#pragma warning disable 414, 219
    #region General Player Info:
    private string playerTitle;//student, apprentice, grunt, practitioner, etc. etc.
    public string PlayerTitle { get { return playerTitle; } set { playerTitle = value; } }
    //Stored in the stats class
    public ushort Level { get { return Stats.Level; } }
    #endregion

    #region Player Stats (For all the Player-only statistics)
    public float playerSpeed = 1.5f;  //temporarily hard-coded
    public float PlayerSpeed
    {
        get
        {
            return playerSpeed;
        }
    }

    float distanceMoved;
    float TurnInterval = BigBoss.Time.TimeInterval;
    #endregion

    #region INVENTORY
    public override void addToInventory(Item item, int count)
    {
        base.addToInventory(item, count);
        BigBoss.Gooey.inventory.Open();
    }

    public override void removeFromInventory(Item item, int count)
    {
        base.removeFromInventory(item, count);
        BigBoss.Gooey.inventory.Open();
    }
    #endregion

    #region Physics

    public override void OnClick()
    {
        if (BigBoss.PlayerInput.InputSetting[InputSettings.DEFAULT_INPUT])
        {
            BigBoss.Gooey.displayInventory = !BigBoss.Gooey.displayInventory;
            BigBoss.Gooey.inventory.Open();
        }
    }
    #endregion

    public override void Init()
    {
        PlayerStats.Load(this, NPCRace.HUMAN);
        this.Flags[NPCFlags.UNKILLABLE] = true;
        CalcStats();
        List<Spell> spells = KnownSpells.Values.ToList();
        for (int i = 0; i < spells.Count; i++)
        {
            BigBoss.Gooey.spellMenu.Set(spells[i], i);
        }

        BigBoss.Gooey.UpdateMaxPower(this.Stats.MaxPower);
        BigBoss.Gooey.UpdatePowerBar(this.Stats.MaxPower);
    }

    // Update is called once per frame
    public override void Update()
    {
        float curTime = Time.time;
        if (!Acting || action.Replaceable())
        {
            BigBoss.PlayerInput.EnableInput();
        }
        else
        {
            BigBoss.PlayerInput.DisableInput();
        }
        if (curTime > turnTime)
        {
            turnTime = curTime + TurnInterval;
            if (Acting)
            {
                DoingAction();
            }
            else
            {
                action = null;
            }
        }
        if (curTime > gridTime)
        {
            gridTime = curTime + .3f;
            UpdateCurrentTileVectors();
        }
        //base.Update();
    }

    #region Actions
    public override void DoingAction()
    {
        if (Acting)
        {
            action.Do();
            BigBoss.Time.PassTurn(1);
        }
    }

    public override void Do(Action action, int cost, bool interuptible, bool actOnStart)
    {
        base.Do(action, cost, interuptible, actOnStart);
        BigBoss.Time.PassTurn(1);
    }

    public override void attack(NPC n)
    {
        base.attack(n);
    }

    public override void eatItem(Item i)
    {
        base.eatItem(i);
        CreateTextMessage(this.Name + " eats the " + i.Name + ".");
    }

    public override void useItem(Item i)
    {
        base.useItem(i);
        CreateTextMessage(this.Name + " uses the " + i.Name + ".");
    }

    public override bool equipItem(Item i)
    {
        if (base.equipItem(i))
        {
            CreateTextMessage("Item " + i.Name + " equipped.");
            animator.SetBool(Equipment.WeaponAnims.Move, true);
            return true;
        }
        CreateTextMessage("Item not equipped.");
        return false;
    }

    public override bool unequipItem(Item i)
    {
        if (base.unequipItem(i))
        {
            CreateTextMessage("Item " + i.Name + " uneqipped.");
            return true;
        }
        return false;
    }

    public override void CastSpell(string spell, params IAffectable[] targets)
    {
        if (this.KnownSpells[spell].cost > this.Stats.CurrentPower)
        {
            CreateTextMessage("Not enough power to cast " + spell + "!");
        }
        else
        {
            base.CastSpell(spell, targets);
            CreateTextMessage(this.Name + " casts " + spell + ".");
        }
    }
    #endregion

    #region Movement and Animation
    public override void GetMovement() //this does nothing on player, easier than an is check
    {
    }

    public void RotatePlayer(Quaternion rotation)
    {
        rigidbody.MoveRotation(rotation);
    }

    public void MovePlayer(Vector2 magnitude)
    {
        if (velocity > .1f) distanceMoved += Time.deltaTime;
        if (distanceMoved > BigBoss.Time.TimeInterval)
        {
            distanceMoved = 0f;
            BigBoss.Time.PassTurn(1);
            action = null;
        }
        velocity = magnitude.sqrMagnitude * PlayerSpeed;
        MoveForward(velocity);
    }

    public override bool UpdateCurrentTileVectors()
    {
        if (BigBoss.Levels.Level == null) return false;

        Vector3 pos = GO.transform.position;
        Point current = new Point(pos.x, pos.z);

        GridSpace newGridSpace = BigBoss.Levels.Level[current.x, current.y];
        if (newGridSpace != null && !newGridSpace.IsBlocked() && GridTypeEnum.Walkable(newGridSpace.Type))
        {
            GridSpace = newGridSpace;
            BigBoss.Gooey.CheckChestDistance();
            if (revealer != null)
            {
                revealer.StartCoroutine(revealer.UpdateFogRadius(current.x, current.y));
            }
            FOWSystem.Instance.UpdatePosition(GridSpace, false);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ForceUpdateTiles(GridSpace grid)
    {
        GridSpace = grid;
        BigBoss.Gooey.CheckChestDistance();
        FOWSystem.Instance.UpdatePosition(GridSpace, true);
    }

    public override void FixedUpdate()
    {
        if (!BigBoss.PlayerInput.mouseMovement && !BigBoss.PlayerInput.touchMovement)
        {
            velocity = 0;
        }
        animator.SetFloat("runSpeed", velocity);							// set our animator's float parameter 'runSpeed' to the speed of the NPC
    }
    #endregion

    #region Stats

    public override float AdjustHunger(float amount)
    {
        base.AdjustHunger(amount);

        //Update GUI here

        return Stats.Hunger;
    }

    public override void AddLevel()
    {
        base.AddLevel();

        //Update GUI here
    }

    public override bool AdjustHealth(int amount, bool report = true)
    {
        if (base.AdjustHealth(amount, report))
        {
            BigBoss.Gooey.UpdateHealthBar(0);
            return true; //player died
        }
        else
        {
            //Do all GUI updates here
            BigBoss.Gooey.UpdateHealthBar(this.Stats.CurrentHealth);
            return false; //player still lives!
        }
    }

    public override void AdjustPower(int amount)
    {
        base.AdjustPower(amount);
        BigBoss.Gooey.UpdatePowerBar(this.Stats.CurrentPower);
    }

    public override bool AdjustMaxHealth(int amount)
    {
        if (base.AdjustMaxHealth(amount))
        {
            return true;
        }
        else
        {
            //GUI updates
            BigBoss.Gooey.UpdateMaxHealth(this.Stats.MaxHealth);
            BigBoss.Gooey.UpdateHealthBar(this.Stats.CurrentHealth);
            return false;
        }
    }

    public override void AdjustMaxPower(int amount)
    {
        base.AdjustMaxPower(amount);

        //GUI updates
        BigBoss.Gooey.UpdateMaxPower(this.Stats.MaxPower);
    }

    public override void AdjustXP(float amount)
    {
        base.AdjustXP(amount);

        //GUI updates
        BigBoss.Gooey.UpdateXPBar();
    }

    public override void AdjustAttribute(Attributes attr, int amount)
    {
        base.AdjustAttribute(attr, amount);
        //update gui:
    }
    #endregion

    #region Time Management

    public override void UpdateTurn()
    {
    }

    #endregion

#pragma warning restore 414, 219
}
