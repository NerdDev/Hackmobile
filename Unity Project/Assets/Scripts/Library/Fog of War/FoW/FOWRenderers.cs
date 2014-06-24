using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Adding a Fog of War Renderer to any game object will hide that object's renderers if they are not visible according to the fog of war.
/// </summary>

[AddComponentMenu("Fog of War/Renderers")]
public class FOWRenderers : MonoBehaviour
{
    Transform mTrans;
    //Renderer[] mRenderers;
    float mNextUpdate = 0f;
    bool mIsVisible = true;
    bool mUpdate = true;
    public bool instantiated = false;
    private WaitForSeconds wfs;
    GridSpace gridSpace;

    /// <summary>
    /// Whether the renderers are currently visible or not.
    /// </summary>

    public bool isVisible { get { return mIsVisible; } }

    void Start()
    {
        mTrans = transform;
        mNextUpdate = 0.2f + (UnityEngine.Random.value + UnityEngine.Random.value) * .2f;
        //wfs = new WaitForSeconds(mNextUpdate);
        gridSpace = BigBoss.Levels.Level[mTrans.position.x.ToInt(), mTrans.position.z.ToInt()];
    }

    public void OnEnable()
    {
        instantiated = false;
        float mFirstUpdate = UnityEngine.Random.value * .1f;
        InvokeRepeating("UpdateRendering", mFirstUpdate, mNextUpdate);
    }

    void UpdateRendering()
    {
        if (enabled)
        {
            mIsVisible = IsVisible();
            if (!mIsVisible)
            {
                gridSpace.DestroyGridSpace();
            }
            else if (!instantiated)
            {
                Vector3 pos = gameObject.transform.position;
                DrawGridsAround(pos);
                
                instantiated = true;
            }
            // yield return wfs;
        }
        else
        {
            CancelInvoke("UpdateRendering");
        }
    }

    void DrawGridsAround(Vector3 pos)
    {
        StartCoroutine(BigBoss.Levels.Level.Array.DrawAroundCoroutine(pos.x.ToInt(), pos.z.ToInt(), true, (arr, x, y) =>
        {
            GridSpace grid = arr[x, y];
            if (grid != null)
            {
                grid.Instantiate();
            };
            return true;
        }));
    }

    bool IsVisible()
    {
        return FOWSystem.Instance.IsVis(gameObject.transform.position);
    }
}