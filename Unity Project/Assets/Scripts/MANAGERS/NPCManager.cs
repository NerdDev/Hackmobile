using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCManager : MonoBehaviour {

	public List<NPC> totalNumberOfNPCs;

    public void AddNPCToMasterList(NPC npc)
    {
        totalNumberOfNPCs.Add(npc);
    }

    public void RemoveNPCFromMasterList(NPC npc)
    {
        totalNumberOfNPCs.Remove(npc);//look up this generic method and see if we leave a memory leak
    }

	public NPC CreateNPC(Vector3 location, string npcName)
	{
        GameObject go = new GameObject();
        go.transform.position = location;
        NPC npc = go.AddComponent<NPC>();
        npc.setData(npc);
        MeshFilter mf = go.AddComponent<MeshFilter>();
        mf.mesh = (Resources.Load(npc.Model) as GameObject).GetComponent<MeshFilter>().mesh;
        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        mr.material = Resources.Load(npc.ModelTexture) as Material;
		return npc;
	}

}//end mono