using System.Collections;
using UnityEngine;

/// <summary>
/// Adding a Fog of War Renderer to any game object will hide that object's renderers if they are not visible according to the fog of war.
/// </summary>

[AddComponentMenu("Fog of War/Renderers")]
public class FOWRenderers : MonoBehaviour
{
    Transform mTrans;
    Renderer[] mRenderers;
    float mNextUpdate = 0f;
    bool mIsVisible = true;
    bool mUpdate = true;
    private bool instantiated = false;
    private WaitForSeconds wfs;

    /// <summary>
    /// Whether the renderers are currently visible or not.
    /// </summary>

    public bool isVisible { get { return mIsVisible; } }

    /// <summary>
    /// Rebuild the list of renderers and immediately update their visibility state.
    /// </summary>

    public void Rebuild()
    {
        mRenderers = GetComponentsInChildren<Renderer>();
        mUpdate = true;
        mNextUpdate = .01f;
    }

    void Start()
    {
        mTrans = transform;
        mNextUpdate = 0.2f + (Random.value + Random.value) * .05f;
        wfs = new WaitForSeconds(mNextUpdate);
        mRenderers = GetComponentsInChildren<Renderer>();
        StartCoroutine(UpdateRendering());
    }

    void OnEnable()
    {
        StartCoroutine(UpdateRendering());
    }

    void UpdateNow()
    {
        //mNextUpdate = Time.time + 0.075f + Random.value * 0.05f;

        if (FOWSystem.instance == null)
        {
            enabled = false;
            return;
        }

        if (mUpdate) mRenderers = GetComponentsInChildren<Renderer>();

        bool visible = FOWSystem.instance.IsVisible(mTrans.position);

        if (mUpdate || mIsVisible != visible)
        {
            mUpdate = false;
            mIsVisible = visible;

            for (int i = 0, imax = mRenderers.Length; i < imax; ++i)
            {
                Renderer ren = mRenderers[i];

                if (ren)
                {
                    ren.enabled = mIsVisible;
                }
                else
                {
                    mUpdate = true;
                    mNextUpdate = Time.time;
                }
            }
        }
    }

    IEnumerator UpdateRendering()
    {
        float mFirstUpdate = Random.value * .1f;
        yield return new WaitForSeconds(mFirstUpdate);
        while (enabled)
        {
            bool visible = IsVisible();
            if (mUpdate || mIsVisible != visible)
            {
                mUpdate = false;
                mIsVisible = visible;

                for (int i = 0, imax = mRenderers.Length; i < imax; ++i)
                {
                    Renderer ren = mRenderers[i];

                    if (ren)
                    {
                        ren.enabled = mIsVisible;
                    }
                    else
                    {
                        Rebuild();
                    }
                }
            }
            if (visible && !instantiated)
            {
                Vector3 pos = gameObject.transform.position;
                BigBoss.Levels.Level.Array.DrawAround(pos.x.ToInt(), pos.z.ToInt(), true, (arr, x, y) =>
                {
                    GridSpace grid = arr[x, y];
                    if (grid != null && grid.Block == null)
                    {
                        BigBoss.Levels.Builder.Instantiate(grid, x, y);
                    };
                    return true;
                });
                instantiated = true;
            }
            yield return wfs;
        }
    }

    bool IsVisible()
    {
        return FOWSystem.instance.IsVis(gameObject.transform.position);
    }
}