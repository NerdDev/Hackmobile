using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class NPCDictionary<W, T> : WODictionary<W, T> where W : NPC, new() where T : NPCInstance
{
}
