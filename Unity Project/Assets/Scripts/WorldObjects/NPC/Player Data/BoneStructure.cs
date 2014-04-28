using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BoneStructure : MonoBehaviour
{
    public Dictionary<string, Transform> transforms = new Dictionary<string, Transform>();

    void Start()
    {
        FindChildByName(transform);
    }

    public List<GameObject> AddEquipment(GameObject original)
    {
        // Here, boneObj must be instatiated and active (at least the one with the renderer),
        // or else GetComponentsInChildren won't work.
        List<GameObject> objects = new List<GameObject>();
        SkinnedMeshRenderer[] BonedObjects = original.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer renderer in BonedObjects)
        {
            objects.Add(ProcessBonedObject(renderer));
        }
        return objects;
    }

    private GameObject ProcessBonedObject(SkinnedMeshRenderer mesh)
    {
        // Create the SubObject
        GameObject newMesh = new GameObject(mesh.gameObject.name);
        newMesh.transform.parent = transform;

        // Add the renderer
        SkinnedMeshRenderer newSkinnedMesh = newMesh.AddComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;

        // Assemble Bone Structure	
        Transform[] MyBones = new Transform[mesh.bones.Length];

        // As clips are using bones by their names, we find them that way.
        for (int i = 0; i < mesh.bones.Length; i++)
        {
            MyBones[i] = transforms[mesh.bones[i].name];
        }

        // Assemble Renderer	
        newSkinnedMesh.bones = MyBones;
        newSkinnedMesh.sharedMesh = mesh.sharedMesh;
        newSkinnedMesh.materials = mesh.materials;

        return newMesh;
    }

    // Recursive search of the child by name.
    private void FindChildByName(Transform obj)
    {
        // If the name match, we're return it

        transforms.Add(obj.transform.name, obj.transform);

        // Else, we go continue the search horizontaly and verticaly
        foreach (Transform child in obj)
        {
            FindChildByName(child);
        }
    }
}
