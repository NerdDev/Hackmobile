using System.Collections;
using UnityEngine;

/// <summary>
/// Fog of War system needs 3 components in order to work:
/// - Fog of War system that will create a height map of your scene and perform all the updates.
/// - Fog of War Image Effect on the camera that will be displaying the fog of war.
/// - Fog of War Revealer on one or more game objects in the world (this class).
/// </summary>

[AddComponentMenu("Fog of War/Revealer")]
public class FOWRevealer : MonoBehaviour
{
    Transform mTrans;

    /// <summary>
    /// Radius of the area being revealed. Everything below X is always revealed. Everything up to Y may or may not be revealed.
    /// </summary>

    public Vector2 range = new Vector2(2f, 30f);

    /// <summary>
    /// Radius of the area being occluded.
    /// </summary>
    public float RevealDistance = 75;

    /// <summary>
    /// What kind of line of sight checks will be performed.
    /// - "None" means no line of sight checks, and the entire area covered by radius.y will be revealed.
    /// - "OnlyOnce" means the line of sight check will be executed only once, and the result will be cached.
    /// - "EveryUpdate" means the line of sight check will be performed every update. Good for moving objects.
    /// </summary>

    public FOWSystem.LOSChecks lineOfSightCheck = FOWSystem.LOSChecks.None;

    /// <summary>
    /// Whether the revealer is actually active or not. If you wanted additional checks such as "is the unit dead?",
    /// then simply derive from this class and change the "isActive" value accordingly.
    /// </summary>

    public bool isActive = true;

    FOWSystem.Revealer mRevealer;

    void Awake()
    {
        mTrans = transform;
        mRevealer = FOWSystem.CreateRevealer();
    }

    void OnDisable()
    {
        mRevealer.isActive = false;
    }

    void OnDestroy()
    {
        FOWSystem.DeleteRevealer(mRevealer);
        mRevealer = null;
    }

    void LateUpdate()
    {
        if (isActive)
        {
            if (lineOfSightCheck != FOWSystem.LOSChecks.OnlyOnce) mRevealer.cachedBuffer = null;

            mRevealer.pos = mTrans.position;
            mRevealer.inner = range.x;
            mRevealer.outer = range.y;
            mRevealer.los = lineOfSightCheck;
            mRevealer.revDist = RevealDistance;
            mRevealer.isActive = true;
        }
        else
        {
            mRevealer.isActive = false;
            mRevealer.cachedBuffer = null;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (lineOfSightCheck != FOWSystem.LOSChecks.None && range.x > 0f)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, range.x);
        }
        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(transform.position, range.y);
    }

    bool running = false;
    WaitForSeconds wfs = new WaitForSeconds(.1f);
    public IEnumerator UpdateFogRadius(int x0, int y0)
    {
        if (running) yield break;
        running = true;
        int x = (int)Mathf.Sqrt(RevealDistance), y = 0;
        int radiusError = 1 - x;

        while (x >= y)
        {
            CreateGridSpace(x + x0, y + y0);
            yield return null;
            CreateGridSpace(y + x0, x + y0);
            yield return null;
            CreateGridSpace(-x + x0, y + y0);
            yield return null;
            CreateGridSpace(-y + x0, x + y0);
            yield return null;
            CreateGridSpace(-x + x0, -y + y0);
            yield return null;
            CreateGridSpace(-y + x0, -x + y0);
            yield return null;
            CreateGridSpace(x + x0, -y + y0);
            yield return null;
            CreateGridSpace(y + x0, -x + y0);
            y++;
            if (radiusError < 0)
            {
                radiusError += 2 * y + 1;
            }
            else
            {
                x--;
                radiusError += 2 * (y - x + 1);
            }
            yield return wfs;
        }
        running = false;
    }

    public void CreateGridSpace(int x, int y)
    {
        GridSpace grid = BigBoss.Levels.Level[x, y];
        if (grid != null)
        {
            grid.Instantiate();
        }
        grid = null;
    }

    /// <summary>
    /// Want to force-rebuild the cached buffer? Just call this function.
    /// </summary>

    public void Rebuild() { mRevealer.cachedBuffer = null; }
}