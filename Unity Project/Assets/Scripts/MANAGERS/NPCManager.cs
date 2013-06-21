using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCManager : MonoBehaviour {

    NPC nullNPC { get; set; }
    Dictionary<string, NPC> baseNPCs = new Dictionary<string, NPC>();

    void Awake()
    {
        initializeNullData();
    }

    private void initializeNullData()
    {
        GameObject nullGONPC = new GameObject("nullNPC");
        nullNPC = nullGONPC.AddComponent<NPC>();
        nullNPC.setNull();
    }

	public List<NPC> totalNumberOfNPCs;

    public void AddNPCToMasterList(NPC npc)
    {
        totalNumberOfNPCs.Add(npc);
    }

    public void RemoveNPCFromMasterList(NPC npc)
    {
        totalNumberOfNPCs.Remove(npc);//look up this generic method and see if we leave a memory leak
    }

    public NPC getNPC(string npcName)
    {
        if (getNPCs().ContainsKey(npcName))
        {
            return getNPCs()[npcName];
        }
        else
        {
            return nullNPC;
        }
    }

    public Dictionary<string, NPC> getNPCs()
    {
        return baseNPCs;
    }

	public NPC CreateNPC(Vector3 location, string npcName)
	{
        GameObject go = new GameObject(npcName);
        go.transform.position = location;
        NPC npc = go.AddComponent<NPC>();
        npc.setData(npcName);
        MeshFilter mf = go.AddComponent<MeshFilter>();
        mf.mesh = (Resources.Load(npc.Model) as GameObject).GetComponent<MeshFilter>().mesh;
        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        mr.material = Resources.Load(npc.ModelTexture) as Material;
		return npc;
	}
}