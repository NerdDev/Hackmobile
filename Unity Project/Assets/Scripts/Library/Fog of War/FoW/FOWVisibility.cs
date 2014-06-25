using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Adding a Fog of War Renderer to any game object will hide that object's renderers if they are not visible according to the fog of war.
/// </summary>

[AddComponentMenu("Fog of War/Renderers")]
public class FOWVisibility : MonoBehaviour
{
    public enum Visibility
    {
        Distance,
        FogVisibility,
    }

    public Visibility visibilityType = Visibility.Distance;
    Transform mTrans;
    Light[] mLights;
    FOWRevealer[] mRevealers;
    float updateTime = 0f;
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
        mLights = GetComponentsInChildren<Light>();
        mRevealers = GetComponentsInChildren<FOWRevealer>();
        mUpdate = true;
    }

    void Start()
    {
        mTrans = transform;
        updateTime = 0.2f + (UnityEngine.Random.value + UnityEngine.Random.value) * .05f;
        mLights = GetComponentsInChildren<Light>();
        mRevealers = GetComponentsInChildren<FOWRevealer>();
    }

    void Update()
    {
        float curTime = Time.time;
        if (curTime > mNextUpdate)
        {
            UpdateNow();
            mNextUpdate = curTime + updateTime;
        }
    }

    void OnEnable()
    {
        //float mFirstUpdate = UnityEngine.Random.value * .1f;
        //InvokeRepeating("UpdateNow", mFirstUpdate, updateTime);
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    void UpdateNow()
    {
        if (FOWSystem.Instance == null)
        {
            enabled = false;
            return;
        }

        bool visible = false;
        switch (visibilityType)
        {
            case Visibility.Distance:
                visible = FOWSystem.Instance.DistanceToPlayer(mTrans.position);
                break;
            case Visibility.FogVisibility:
                visible = FOWSystem.Instance.IsVisible(mTrans.position);
                break;
        }

        if (mUpdate || mIsVisible != visible)
        {
            mUpdate = false;
            mIsVisible = visible;

            for (int i = 0, imax = mLights.Length; i < imax; ++i)
            {
                Light ren = mLights[i];

                if (ren)
                {
                    ren.enabled = mIsVisible;
                }
            }

            for (int i = 0, imax = mRevealers.Length; i < imax; ++i)
            {
                FOWRevealer ren = mRevealers[i];

                if (ren != null)
                {
                    ren.isActive = mIsVisible;
                }
            }
        }
    }

    //IEnumerator UpdateRendering()
    //{
    //    UpdateNow();
    //}

    bool IsVisible()
    {
        return FOWSystem.Instance.IsInsideDestructionRadius(gameObject.transform.position);
    }
}