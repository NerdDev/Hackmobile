using System;
using UnityEngine;
using System.Threading;
using System.Collections.Generic;

/// <summary>
/// Fog of War system needs 3 components in order to work:
/// - Fog of War system that will create a height map of your scene and perform all the updates (this class).
/// - Fog of War Image Effect on the camera that will be displaying the fog of war.
/// - Fog of War Revealer on one or more game objects in the world.
/// </summary>

[AddComponentMenu("Fog of War/System")]
public class FOWSystem : MonoBehaviour
{
    public int GridDistance;
    internal int ShaderOffsetX = 512;
    internal int ShaderOffsetY = 256;

    internal int BufferOffsetX = 512; //for second thread only
    internal int BufferOffsetY = 256; //for second thread only

    internal int PositionOffsetX = 512;
    internal int PositionOffsetY = 256;

    public Vector3 lowerRange = new Vector3(-256f, 0f, -256f);
    public int HeightMapSize = 2048;


    public enum LOSChecks
    {
        None,
        OnlyOnce,
        EveryUpdate,
    }

    public enum Buffer
    {
        BAKED,
        DYNAMIC,
    }

    public class Revealer
    {
        public bool isActive = false;
        public LOSChecks los = LOSChecks.None;
        public Vector3 pos = Vector3.zero;
        public float inner = 0f;
        public float outer = 0f;
        public float fade = 0f;
        public bool[] cachedBuffer;
        public int cachedSize = 0;
        public int cachedX = 0;
        public int cachedY = 0;
        public float revDist = 55;
        public float LinearMultiplier = 1f;
        public float ExpMultiplier = .66f;
        public Buffer buffer = Buffer.BAKED;
        public bool Special = false;
    }

    public enum State
    {
        Blending,
        NeedUpdate,
        UpdateTexture0,
        UpdateTexture1,
    }

    static public FOWSystem instance;

    // Height map used for visibility checks. Integers are used instead of floats as integer checks are significantly faster.
    protected int[,] mHeights;
    protected Transform mTrans;
    public Vector3 mOrigin = Vector3.zero;
    protected Vector3 mSize = Vector3.one;

    // Revealers that the thread is currently working with
    static BetterList<Revealer> mRevealers = new BetterList<Revealer>();

    // Revealers that have been added since last update
    static BetterList<Revealer> mAdded = new BetterList<Revealer>();

    // Revealers that have been removed since last update
    static BetterList<Revealer> mRemoved = new BetterList<Revealer>();

    // Revealers that are using the special set of calculations. Not thread-safe (only accessed on secondary thread).
    static BetterList<Revealer> specialRevs = new BetterList<Revealer>();

    // Color buffers -- prepared on the worker thread.
    protected Color32[] mBuffer0; //Main color buffer, the one updated to the screen.
    protected Color32[] mBuffer1; //Main buffer for dynamic objects (which is updated to the 0 buffer).
    protected Color32[] mBuffer2; //Blur buffer.
    protected bool[] mBuffer3; //Boolean buffer to store whether the location is visible
    protected bool[] mBuffer4; //Boolean buffer to store whether the location was checked IsVisible

    // Two textures -- we'll be blending between them in the shader
    protected Texture2D mTexture0;
    protected Texture2D mTexture1;

    // Whether some color buffer is ready to be uploaded to VRAM
    protected float mBlendFactor = 0f;
    protected float mNextUpdate = 0f;
    protected float mPosUpdate = 0f;
    protected int mScreenHeight = 0;
    public State mState = State.Blending;

    Thread mThread;

    /// <summary>
    /// Size of your world in units. For example, if you have a 256x256 terrain, then just leave this at '256'.
    /// </summary>

    public int worldSize = 256;

    /// <summary>
    /// Size of the fog of war texture. Higher resolution will result in more precise fog of war, at the cost of performance.
    /// </summary>

    public int textureSize = 128;

    /// <summary>
    /// How frequently the visibility checks get performed.
    /// </summary>

    public float updateFrequency = 0.1f;

    /// <summary>
    /// How long it takes for textures to blend from one to another.
    /// </summary>

    public float textureBlendTime = 0.5f;

    /// <summary>
    /// How many blur iterations will be performed. More iterations result in smoother edges.
    /// Blurring happens on a separate thread and does not affect performance.
    /// </summary>

    public int blurIterations = 2;

    /// <summary>
    /// What is the lowest and highest height of your world? Revealers below the X will not reveal anything,
    /// while revealers above Y will reveal everything around them.
    /// </summary>

    public Vector2 heightRange = new Vector2(0f, 10f);

    /// <summary>
    /// Mask used for raycasting to determine whether there is an obstruction or not.
    /// </summary>

    public LayerMask raycastMask = -1;

    /// <summary>
    /// Radius of the sphere if using SphereCast. If 0, line-based raycasting will be used instead.
    /// </summary>

    public float raycastRadius = 0f;

    /// <summary>
    /// Allows for some height variance when performing line-of-sight checks.
    /// </summary>

    public float margin = 0.4f;

    /// <summary>
    /// If debugging is enabled, the time it takes to calculate the fog of war will be shown in the log window.
    /// </summary>

    public bool debug = false;

    /// <summary>
    /// The fog texture we're blending from.
    /// </summary>

    public Texture2D texture0 { get { return mTexture0; } }

    /// <summary>
    /// The fog texture we're blending to.
    /// </summary>

    public Texture2D texture1 { get { return mTexture1; } }

    /// <summary>
    /// Factor used to blend between the two textures.
    /// </summary>

    public float blendFactor { get { return mBlendFactor; } }

    /// <summary>
    /// Does the grid need re-created for a dynamic level?
    /// </summary>

    public bool reCreateGrid = false;

    private bool reCreateGridSection = false;

    /// <summary>
    /// Create a new fog revealer.
    /// </summary>

    static public Revealer CreateRevealer(bool special)
    {
        Revealer rev = new Revealer();
        rev.isActive = false;
        lock (mAdded) mAdded.Add(rev);
        return rev;
    }

    /// <summary>
    /// Delete the specified revealer.
    /// </summary>

    static public void DeleteRevealer(Revealer rev) { lock (mRemoved) mRemoved.Add(rev); }

    /// <summary>
    /// Set the instance.
    /// </summary>

    void Awake() { instance = this; }

    /// <summary>
    /// Generate the height grid.
    /// </summary>

    void Start()
    {
        mTrans = transform;
        mHeights = new int[HeightMapSize, HeightMapSize];
        mSize = new Vector3(worldSize, heightRange.y - heightRange.x, worldSize);

        mOrigin = new Vector3(0, 0, 0);

        int size = textureSize * textureSize;
        mBuffer0 = new Color32[size];
        mBuffer1 = new Color32[size];
        mBuffer2 = new Color32[size];
        mBuffer3 = new bool[size];
        mBuffer4 = new bool[size];

        // Create the height grid
        //CreateGrid();

        // Update the fog of war's visibility so that it's updated right away
        UpdateBuffer(false);
        UpdateTexture();
        mNextUpdate = Time.time + updateFrequency;

        // Add a thread update function -- all visibility checks will be done on a separate thread
        mThread = new Thread(ThreadUpdate);
        mThread.Start();
    }

    public void TestThis()
    {
        //mOrigin += new Vector3(128f, 0f, 128f);
        BufferOffsetX += 512;
        BufferOffsetY += 512;
    }

    /// <summary>
    /// Ensure that the thread gets terminated.
    /// </summary>

    void OnDestroy()
    {
        if (mThread != null)
        {
            mThread.Abort();
            while (mThread.IsAlive) Thread.Sleep(1);
            mThread = null;
        }
    }

    /// <summary>
    /// Is it time to update the visibility? If so, flip the switch.
    /// </summary>
    public bool UpdatePos = false;
    void Update()
    {
        float time = Time.time;
        if (textureBlendTime > 0f)
        {
            mBlendFactor = Mathf.Clamp01(mBlendFactor + Time.deltaTime / textureBlendTime);
        }
        else mBlendFactor = 1f;

        if (mState == State.Blending)
        {
            if (mNextUpdate < time)
            {
                mNextUpdate = time + updateFrequency;
                mState = State.NeedUpdate;
            }
        }
        else if (mState != State.NeedUpdate)
        {
            UpdateTexture();
        }

        if (UpdatePos)
        {
            UpdatePosition(BigBoss.Player.GridSpace, true);
            UpdatePos = false;
        }
    }

    int priorX;
    int priorY;
    int curX;
    int curY;
    bool firstSet = false;
    bool bufferNeedsSet = false;
    bool useProfiler = false;
    public void UpdatePosition(GridSpace grid, bool force)
    {
        int x = grid.X;
        int y = grid.Y;

        if (!force)
        {
            if (Math.Abs(priorX - x) < GridDistance && Math.Abs(priorY - y) < GridDistance) return;
        }
        curX = priorX;
        curY = priorY;
        SetPosition(grid.X, grid.Y);
        priorX = x;
        priorY = y;
        bufferNeedsSet = true;
        debugThis = true;
        UpdateBuffer(true);
        debugThis = false;
        mState = State.NeedUpdate;
    }

    public void SetPosition(int x, int y)
    {
        Vector3 pos = new Vector3(x, 0f, y);
        mOrigin = pos;
        mOrigin.x -= worldSize * 0.5f;
        mOrigin.z -= worldSize * 0.5f;

        float worldToTex = (float)textureSize / worldSize;
        int pOffsetX = PositionOffsetX;
        int pOffsetY = PositionOffsetY;

        PositionOffsetX = Mathf.RoundToInt((pos.x - lowerRange.x) * worldToTex - textureSize / 2);
        PositionOffsetY = Mathf.RoundToInt((pos.z - lowerRange.z) * worldToTex - textureSize / 2);

        PositionOffsetX = Mathf.Clamp(PositionOffsetX, 0, HeightMapSize);
        PositionOffsetY = Mathf.Clamp(PositionOffsetY, 0, HeightMapSize);
    }

    float mElapsed = 0f;
    /// <summary>
    /// If it's time to update, do so now.
    /// </summary>

    void ThreadUpdate()
    {
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

        for (; ; )
        {
            if (mState == State.NeedUpdate)
            {
                sw.Reset();
                sw.Start();
                UpdateBuffer(false);
                sw.Stop();
                if (debug) Debug.Log(sw.ElapsedMilliseconds);
                mElapsed = 0.001f * (float)sw.ElapsedMilliseconds;
                if (bufferNeedsSet)
                {
                    bufferNeedsSet = false;
                    UpdateBuffer(true);
                }
                else
                {
                    mState = State.UpdateTexture0;
                }
            }
            Thread.Sleep(1);
        }
    }

    /// <summary>
    /// Show the area covered by the fog of war.
    /// </summary>

    void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(0f, (heightRange.x + heightRange.y) * 0.5f, 0f),
            new Vector3(worldSize, heightRange.y - heightRange.x, worldSize));
    }

    /// <summary>
    /// Determine if the specified point is visible or not using line-of-sight checks.
    /// </summary>

    void IsVisibleMain(Revealer r, int startX, int startY, int finishX, int finishY, int sightHeight, float worldToTex)
    {
        int minRange = Mathf.RoundToInt(r.inner * r.inner * worldToTex * worldToTex);
        int fadeRange = Mathf.RoundToInt(r.fade * r.fade * worldToTex * worldToTex);
        int maxRange = Mathf.RoundToInt(r.outer * r.outer * worldToTex * worldToTex);
        Color32 white = new Color32(255, 255, 255, 255);

        int initialDistX = Mathf.Abs(finishX - startX);
        int initialDistY = Mathf.Abs(finishY - startY);
        int incrementX = startX < finishX ? 1 : -1;
        int incrementY = startY < finishY ? 1 : -1;
        int dir = initialDistX - initialDistY;

        //finishX += BufferOffsetX;
        //finishY += BufferOffsetY;
        //startX += BufferOffsetX;
        //startY += BufferOffsetY;

        int sx = startX;
        int sy = startY;

        bool finished = false;
        while (!finished)
        {
            if (startX > -1 && startX < textureSize &&
                    startY > -1 && startY < textureSize) //if within bounds
            {
                if (startX == finishX && startY == finishY)
                {
                    finished = true;
                }
                // If the sampled height is higher than expected, then the point must be obscured
                if (mHeights[(startX + BufferOffsetX), (startY + BufferOffsetY)] <= sightHeight)
                {
                    int xdist = startX - sx; //distance in X at this point
                    int ydist = startY - sy; //distance in Y at this point
                    int dist = xdist * xdist + ydist * ydist; //squared distance
                    int index = startX + startY * textureSize; //index access for texture

                    if (dist > fadeRange)
                    {
                        int distFromFade = dist - fadeRange;
                        white.r = (byte)Mathf.Clamp((int)(255 - r.LinearMultiplier * Math.Pow(distFromFade, r.ExpMultiplier) + mBuffer1[index].r), 0, 255);
                    }
                    mBuffer1[index] = white;
                    white.r = 255;
                    mBuffer4[index] = true;
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }

            /*
             * Move to next set of directions
             */
            int dir2 = dir << 1;

            if (dir2 > -initialDistY)
            {
                dir -= initialDistY;
                startX += incrementX;
            }

            if (dir2 < initialDistX)
            {
                dir += initialDistX;
                startY += incrementY;
            }
        }
    }

    bool IsVisible(int startX, int startY, int finishX, int finishY, int sightHeight)
    {
        int initialDistX = Mathf.Abs(finishX - startX);
        int initialDistY = Mathf.Abs(finishY - startY);
        int incrementX = startX < finishX ? 1 : -1;
        int incrementY = startY < finishY ? 1 : -1;
        int dir = initialDistX - initialDistY;

        finishX += BufferOffsetX;
        finishY += BufferOffsetY;
        startX += BufferOffsetX;
        startY += BufferOffsetY;
        int finishHeight = mHeights[finishX, finishY];

        while (true)
        {
            if (startX == finishX && startY == finishY) return true;

            // If the sampled height is higher than expected, then the point must be obscured
            if (mHeights[startX, startY] > sightHeight) return false;

            int dir2 = dir << 1;

            if (dir2 > -initialDistY)
            {
                dir -= initialDistY;
                startX += incrementX;
            }

            if (dir2 < initialDistX)
            {
                dir += initialDistX;
                startY += incrementY;
            }
        }
    }

    /// <summary>
    /// Convert the specified height into the internal integer representation. Integer checks are much faster than floats.
    /// </summary>

    public int WorldToGridHeight(float height)
    {
        int val = Mathf.RoundToInt(height / mSize.y * 255f);
        return Mathf.Clamp(val, 0, 255);
    }

    public Color32[] GetBuffer(Buffer buffer)
    {
        switch (buffer)
        {
            case Buffer.BAKED:
                return mBuffer1;
            case Buffer.DYNAMIC:
                return mBuffer1;
        }
        return mBuffer1;
    }

    /// <summary>
    /// Create the heightmap grid using the default technique (raycasting).
    /// </summary>

    protected virtual void CreateGrid()
    {
        Vector3 pos = mOrigin;
        pos.y += mSize.y;
        float texToWorld = (float)worldSize / textureSize;
        bool useSphereCast = raycastRadius > 0f;

        for (int z = 0; z < textureSize; ++z)
        {
            pos.z = mOrigin.z + z * texToWorld;

            for (int x = 0; x < textureSize; ++x)
            {
                pos.x = mOrigin.x + x * texToWorld;

                RaycastHit hit;

                if (useSphereCast)
                {
                    if (Physics.SphereCast(new Ray(pos, Vector3.down), raycastRadius, out hit, mSize.y, raycastMask))
                    {
                        mHeights[x, z] = WorldToGridHeight(pos.y - hit.distance - raycastRadius);
                        continue;
                    }
                }
                else if (Physics.Raycast(new Ray(pos, Vector3.down), out hit, mSize.y, raycastMask))
                {
                    mHeights[x, z] = WorldToGridHeight(pos.y - hit.distance);
                    continue;
                }
                mHeights[x, z] = 0;
            }
        }
    }

    public void ModifyGrid(Vector3 pos, int extraHeight, int steps = 6, float raycastRadius = 0)
    {
        bool useSphereCast = raycastRadius > 0f;

        // Position relative to the fog of war
        pos = pos - lowerRange;
        pos.y += mSize.y;

        // For conversion from world coordinates to texture coordinates
        float texToWorld = (float)worldSize / textureSize;
        float worldToTex = (float)textureSize / worldSize;

        // Coordinates we'll be dealing with
        int xmin = Mathf.RoundToInt((pos.x * worldToTex - steps));
        int ymin = Mathf.RoundToInt((pos.z * worldToTex - steps));
        int xmax = Mathf.RoundToInt((pos.x * worldToTex + steps));
        int ymax = Mathf.RoundToInt((pos.z * worldToTex + steps));

        for (int y = ymin; y < ymax; y++)
        {
            if (y > -1 && y < HeightMapSize)
            {
                pos.z = lowerRange.z + y * texToWorld;

                for (int x = xmin; x < xmax; x++)
                {
                    if (x > -1 && x < HeightMapSize)
                    {
                        pos.x = lowerRange.x + x * texToWorld;

                        RaycastHit hit;
                        if (useSphereCast)
                        {
                            if (Physics.SphereCast(new Ray(pos, Vector3.down), raycastRadius, out hit, mSize.y, raycastMask))
                            {
                                mHeights[x, y] = WorldToGridHeight(pos.y - hit.distance - raycastRadius) + extraHeight;
                                continue;
                            }
                        }
                        else if (Physics.Raycast(new Ray(pos, Vector3.down), out hit, mSize.y, raycastMask))
                        {
                            mHeights[x, y] = WorldToGridHeight(pos.y - hit.distance) + extraHeight;
                            continue;
                        }
                        else
                        {
                            mHeights[x, y] = 0;
                        }
                    }
                }
            }
        }
    }
    bool debugThis;
    /// <summary>
    /// Update the fog of war's visibility.
    /// </summary>
    float factor;
    Revealer Main;
    void UpdateBuffer(bool shifted)
    {
        // Update the buffer offsets for the height map
        BufferOffsetX = PositionOffsetX;
        BufferOffsetY = PositionOffsetY;

        // Add all items scheduled to be added
        if (mAdded.size > 0)
        {
            lock (mAdded)
            {
                while (mAdded.size > 0)
                {
                    int index = mAdded.size - 1;
                    if (mAdded.buffer[index].Special)
                    {
                        specialRevs.Add(mAdded.buffer[index]);
                    }
                    else
                    {
                        mRevealers.Add(mAdded.buffer[index]);
                    }
                    mAdded.RemoveAt(index);
                }
            }
        }

        // Remove all items scheduled for removal
        if (mRemoved.size > 0)
        {
            lock (mRemoved)
            {
                while (mRemoved.size > 0)
                {
                    int index = mRemoved.size - 1;
                    mRevealers.Remove(mRemoved.buffer[index]);
                    mRemoved.RemoveAt(index);
                }
            }
        }

        // Use the texture blend time, thus estimating the time this update will finish
        // Doing so helps avoid visible changes in blending caused by the blended result being X milliseconds behind.
        factor = (textureBlendTime > 0f) ? Mathf.Clamp01(mBlendFactor + mElapsed / textureBlendTime) : 1f;

        // For conversion from world coordinates to texture coordinates
        float worldToTex = (float)textureSize / worldSize;

        // Clear the buffer's red channel (channel used for current visibility -- it's updated right after)
        for (int i = 0, imax = mBuffer0.Length; i < imax; ++i)
        {
            mBuffer0[i] = Color32.Lerp(mBuffer0[i], mBuffer1[i], factor);
            mBuffer1[i].r = 0;
            mBuffer3[i] = true;
            mBuffer4[i] = false;
        }

        for (int i = 0; i < mRevealers.size; ++i)
        {
            Revealer rev = mRevealers[i];
            if (!rev.isActive) continue;
            if (rev.Special)
            {
                specialRevs.Add(rev);
                continue;
            }
        }

        for (int i = 0; i < specialRevs.size; ++i)
        {
            Revealer rev = specialRevs[i];
            if (!rev.isActive) continue;
            Main = rev;
            RevealUsingLOSMain(rev, worldToTex);
        }
        specialRevs.Clear();

        // Update the visibility buffer, one revealer at a time
        if (Main != null)
        {
            for (int i = 0; i < mRevealers.size; ++i)
            {
                Revealer rev = mRevealers[i];
                if (!rev.isActive) continue;
                if (rev.Special) continue;
                RevealUsingLOS(rev, worldToTex);
            }
        }

        // Update the special revealers afterwards


        // Blur the final visibility data
        for (int i = 0; i < blurIterations; ++i) BlurVisibility();

        // Reveal the map based on what's currently visible
        //RevealMap(); //unused now that explored/unexplored areas are not differentiated, that's stored in the green channel
    }

    private void Reveal(float worldToTex, Revealer rev)
    {
        if (rev.los == LOSChecks.None)
        {
            RevealUsingRadius(rev, worldToTex); //doesn't have shadows
        }
        else if (rev.los == LOSChecks.OnlyOnce)
        {
            RevealUsingCache(rev, worldToTex); //currently not accurate to fog shadows
        }
        else
        {
            RevealUsingLOS(rev, worldToTex);
        }
    }

    /// <summary>
    /// The fastest form of visibility updates -- radius-based, no line of sights checks.
    /// </summary>

    void RevealUsingRadius(Revealer r, float worldToTex)
    {
        // Position relative to the fog of war
        Vector3 pos = r.pos - mOrigin;

        // Coordinates we'll be dealing with
        int xmin = Mathf.RoundToInt((pos.x - r.outer) * worldToTex);
        int ymin = Mathf.RoundToInt((pos.z - r.outer) * worldToTex);
        int xmax = Mathf.RoundToInt((pos.x + r.outer) * worldToTex);
        int ymax = Mathf.RoundToInt((pos.z + r.outer) * worldToTex);

        int cx = Mathf.RoundToInt(pos.x * worldToTex);
        int cy = Mathf.RoundToInt(pos.z * worldToTex);

        cx = Mathf.Clamp(cx, 0, textureSize - 1);
        cy = Mathf.Clamp(cy, 0, textureSize - 1);

        int radius = Mathf.RoundToInt(r.outer * r.outer * worldToTex * worldToTex);

        Color32[] mBuffer = GetBuffer(r.buffer);

        for (int y = ymin; y < ymax; ++y)
        {
            if (y > -1 && y < textureSize)
            {
                int yw = y * textureSize;

                for (int x = xmin; x < xmax; ++x)
                {
                    if (x > -1 && x < textureSize)
                    {
                        int xd = x - cx;
                        int yd = y - cy;
                        int dist = xd * xd + yd * yd;

                        // Reveal this pixel
                        if (dist < radius) mBuffer[x + yw].r = 255;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Reveal the map around the revealer performing line-of-sight checks.
    /// </summary>
    void RevealUsingLOS(Revealer r, float worldToTex)
    {
        // Position relative to the fog of war
        Vector3 pos = r.pos - mOrigin;
        Vector3 mainPos = Main.pos - mOrigin;

        // Coordinates we'll be dealing with
        int xmin = Mathf.RoundToInt((pos.x - r.outer) * worldToTex);
        int ymin = Mathf.RoundToInt((pos.z - r.outer) * worldToTex);
        int xmax = Mathf.RoundToInt((pos.x + r.outer) * worldToTex);
        int ymax = Mathf.RoundToInt((pos.z + r.outer) * worldToTex);

        int mCX;
        int mCY;

        int cx;
        int cy;

        mCX = Mathf.RoundToInt(mainPos.x * worldToTex);
        mCY = Mathf.RoundToInt(mainPos.z * worldToTex);

        mCX = Mathf.Clamp(mCX, 0, textureSize - 1);
        mCY = Mathf.Clamp(mCY, 0, textureSize - 1);

        cx = Mathf.RoundToInt(pos.x * worldToTex);
        cy = Mathf.RoundToInt(pos.z * worldToTex);

        cx = Mathf.Clamp(cx, 0, textureSize - 1);
        cy = Mathf.Clamp(cy, 0, textureSize - 1);

        int minRange = Mathf.RoundToInt(r.inner * r.inner * worldToTex * worldToTex);
        int fadeRange = Mathf.RoundToInt(r.fade * r.fade * worldToTex * worldToTex);
        int maxRange = Mathf.RoundToInt(r.outer * r.outer * worldToTex * worldToTex);
        int mainRange = Mathf.RoundToInt(Main.outer * Main.outer * worldToTex * worldToTex);

        int gh = WorldToGridHeight(r.pos.y);
        Color32 white = new Color32(255, 255, 255, 255);

        Color32[] mBuffer = GetBuffer(r.buffer);

        for (int y = ymin; y < ymax; ++y)
        {
            if (y > -1 && y < textureSize)
            {
                for (int x = xmin; x < xmax; ++x)
                {
                    if (x > -1 && x < textureSize)
                    {
                        int index = x + y * textureSize;

                        int xd = x - mCX;
                        int yd = y - mCY;
                        int dist = xd * xd + yd * yd;

                        xd = x - cx;
                        yd = y - cy;
                        int dist2 = xd * xd + yd * yd;

                        if (dist < mainRange && dist2 < maxRange)
                        {
                            if (mBuffer1[index].r == 255) continue;
                            if (!mBuffer4[index])
                            {
                                if (mCX <= -1 || mCX >= textureSize ||
                                    mCY <= -1 || mCY >= textureSize)
                                {
                                    mBuffer3[index] = false;
                                }
                                else if (!IsVisible(mCX, mCY, x, y, gh))
                                {
                                    mBuffer3[index] = false;
                                }
                                mBuffer4[index] = true;
                            }
                            if (!mBuffer3[index]) continue;

                            if (cx > -1 && cx < textureSize &&
                                cy > -1 && cy < textureSize &&
                                IsVisible(cx, cy, x, y, gh))
                            {
                                if (dist > fadeRange)
                                {
                                    int distFromFade = dist - fadeRange;
                                    white.r = (byte)Mathf.Clamp((int)(255 - r.LinearMultiplier * Math.Pow(distFromFade, r.ExpMultiplier) + mBuffer1[index].r), 0, 255);
                                }
                                mBuffer1[index] = white;
                                white.r = 255;
                            }
                        }
                    }
                }
            }
        }
    }

    void RevealUsingLOSMain(Revealer r, float worldToTex)
    {
        // Position relative to the fog of war
        Vector3 pos = r.pos - mOrigin;

        int cx = Mathf.RoundToInt(pos.x * worldToTex);
        int cy = Mathf.RoundToInt(pos.z * worldToTex);

        cx = Mathf.Clamp(cx, 0, textureSize - 1);
        cy = Mathf.Clamp(cy, 0, textureSize - 1);

        int gh = WorldToGridHeight(r.pos.y);

        int x = Mathf.RoundToInt(r.inner * worldToTex), y = 0;
        int radiusError = 1 - x;
        while (x >= y)
        {
            IsVisibleMain(r, cx, cy, x + cx, y + cy, gh, worldToTex);
            IsVisibleMain(r, cx, cy, y + cx, x + cy, gh, worldToTex);
            IsVisibleMain(r, cx, cy, -x + cx, y + cy, gh, worldToTex);
            IsVisibleMain(r, cx, cy, -y + cx, x + cy, gh, worldToTex);
            IsVisibleMain(r, cx, cy, -x + cx, -y + cy, gh, worldToTex);
            IsVisibleMain(r, cx, cy, -y + cx, -x + cy, gh, worldToTex);
            IsVisibleMain(r, cx, cy, x + cx, -y + cy, gh, worldToTex);
            IsVisibleMain(r, cx, cy, y + cx, -x + cy, gh, worldToTex);
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
        }
    }

    /// <summary>
    /// Reveal the map using a cached result.
    /// </summary>

    void RevealUsingCache(Revealer r, float worldToTex)
    {
        if (r.cachedBuffer == null) RevealIntoCache(r, worldToTex);

        Color32 white = new Color32(255, 255, 255, 255);
        Color32[] mBuffer = GetBuffer(r.buffer);

        for (int y = r.cachedY, ymax = r.cachedY + r.cachedSize; y < ymax; ++y)
        {
            if (y > -1 && y < textureSize)
            {
                int by = y * textureSize;
                int cy = (y - r.cachedY) * r.cachedSize;

                for (int x = r.cachedX, xmax = r.cachedX + r.cachedSize; x < xmax; ++x)
                {
                    if (x > -1 && x < textureSize)
                    {
                        int cachedIndex = x - r.cachedX + cy;

                        if (r.cachedBuffer[cachedIndex])
                        {
                            mBuffer[x + by] = white;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Create the cached visibility result.
    /// </summary>

    void RevealIntoCache(Revealer r, float worldToTex)
    {
        // Position relative to the fog of war
        Vector3 pos = r.pos - mOrigin;

        // Coordinates we'll be dealing with
        int xmin = Mathf.RoundToInt((pos.x - r.outer) * worldToTex);
        int ymin = Mathf.RoundToInt((pos.z - r.outer) * worldToTex);
        int xmax = Mathf.RoundToInt((pos.x + r.outer) * worldToTex);
        int ymax = Mathf.RoundToInt((pos.z + r.outer) * worldToTex);

        int cx = Mathf.RoundToInt(pos.x * worldToTex);
        int cy = Mathf.RoundToInt(pos.z * worldToTex);

        cx = Mathf.Clamp(cx, 0, textureSize - 1);
        cy = Mathf.Clamp(cy, 0, textureSize - 1);

        // Create the buffer to reveal into
        int size = Mathf.RoundToInt(xmax - xmin);
        r.cachedBuffer = new bool[size * size];
        r.cachedSize = size;
        r.cachedX = xmin;
        r.cachedY = ymin;

        // The buffer should start off clear
        for (int i = 0, imax = size * size; i < imax; ++i) r.cachedBuffer[i] = false;

        int minRange = Mathf.RoundToInt(r.inner * r.inner * worldToTex * worldToTex);
        int maxRange = Mathf.RoundToInt(r.outer * r.outer * worldToTex * worldToTex);
        int variance = Mathf.RoundToInt(Mathf.Clamp01(margin / (heightRange.y - heightRange.x)) * 255);
        int gh = WorldToGridHeight(r.pos.y);

        for (int y = ymin; y < ymax; ++y)
        {
            if (y > -1 && y < textureSize)
            {
                for (int x = xmin; x < xmax; ++x)
                {
                    if (x > -1 && x < textureSize)
                    {
                        int xd = x - cx;
                        int yd = y - cy;
                        int dist = xd * xd + yd * yd;

                        if (dist < minRange || (cx == x && cy == y))
                        {
                            r.cachedBuffer[(x - xmin) + (y - ymin) * size] = true;
                        }
                        else if (dist < maxRange)
                        {
                            Vector2 v = new Vector2(xd, yd);
                            v.Normalize();
                            v *= r.inner;

                            int sx = cx + Mathf.RoundToInt(v.x);
                            int sy = cy + Mathf.RoundToInt(v.y);

                            if (sx > -1 && sx < textureSize &&
                                sy > -1 && sy < textureSize &&
                                IsVisible(sx, sy, x, y, gh))
                            {
                                r.cachedBuffer[(x - xmin) + (y - ymin) * size] = true;
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Blur the visibility data.
    /// </summary>

    void BlurVisibility()
    {
        Color32 c;

        for (int y = 0; y < textureSize; ++y)
        {
            int yw = y * textureSize;
            int yw0 = (y - 1);
            if (yw0 < 0) yw0 = 0;
            int yw1 = (y + 1);
            if (yw1 == textureSize) yw1 = y;

            yw0 *= textureSize;
            yw1 *= textureSize;

            for (int x = 0; x < textureSize; ++x)
            {
                int x0 = (x - 1);
                if (x0 < 0) x0 = 0;
                int x1 = (x + 1);
                if (x1 == textureSize) x1 = x;

                int index = x + yw;
                int val = mBuffer1[index].r;

                val += mBuffer1[x0 + yw].r;
                val += mBuffer1[x1 + yw].r;
                val += mBuffer1[x + yw0].r;
                val += mBuffer1[x + yw1].r;

                val += mBuffer1[x0 + yw0].r;
                val += mBuffer1[x1 + yw0].r;
                val += mBuffer1[x0 + yw1].r;
                val += mBuffer1[x1 + yw1].r;

                c = mBuffer2[index];
                c.r = (byte)(val / 9);
                mBuffer2[index] = c;
            }
        }

        // Swap the buffer so that the blurred one is used
        Color32[] temp = mBuffer1;
        mBuffer1 = mBuffer2;
        mBuffer2 = temp;
    }

    /// <summary>
    /// Reveal the map by updating the green channel to be the maximum of the red channel.
    /// </summary>

    void RevealMap()
    {
        for (int y = 0; y < textureSize; ++y)
        {
            int yw = y * textureSize;

            for (int x = 0; x < textureSize; ++x)
            {
                int index = x + yw;
                Color32 c = mBuffer1[index];

                if (c.g < c.r)
                {
                    c.g = c.r;
                    mBuffer1[index] = c;
                }
            }
        }
    }

    /// <summary>
    /// Update the specified texture with the new color buffer.
    /// </summary>

    void UpdateTexture()
    {
        if (mScreenHeight != Screen.height || mTexture0 == null)
        {
            mScreenHeight = Screen.height;

            if (mTexture0 != null) Destroy(mTexture0);
            if (mTexture1 != null) Destroy(mTexture1);

            // Native ARGB format is the fastest as it involves no data conversion
            mTexture0 = new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, false);
            mTexture1 = new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, false);

            mTexture0.wrapMode = TextureWrapMode.Clamp;
            mTexture1.wrapMode = TextureWrapMode.Clamp;

            mTexture0.SetPixels32(mBuffer0);
            mTexture0.Apply();
            mTexture1.SetPixels32(mBuffer1);
            mTexture1.Apply();
            mState = State.Blending;
        }
        else if (mState == State.UpdateTexture0)
        {
            // Texture updates are spread between two frames to make it even less noticeable when they get updated
            mTexture0.SetPixels32(mBuffer0);
            mTexture0.Apply();
            mBlendFactor = 0f;
            mState = State.UpdateTexture1;
        }
        else if (mState == State.UpdateTexture1)
        {
            mTexture1.SetPixels32(mBuffer1);
            mTexture1.Apply();
            mState = State.Blending;
        }
        ShaderOffsetX = BufferOffsetX;
        ShaderOffsetY = BufferOffsetY;
    }

    /// <summary>
    /// Checks to see if the specified position is currently visible.
    /// </summary>

    public bool IsVisible(Vector3 pos)
    {
        if (mBuffer0 == null) return false;
        pos -= mOrigin;

        float worldToTex = (float)textureSize / worldSize;
        int cx = Mathf.RoundToInt(pos.x * worldToTex);
        int cy = Mathf.RoundToInt(pos.z * worldToTex);

        cx = Mathf.Clamp(cx, 0, textureSize - 1);
        cy = Mathf.Clamp(cy, 0, textureSize - 1);
        int index = cx + cy * textureSize;
        return mBuffer1[index].r > 0 || mBuffer0[index].r > 0;
    }

    public int[] WorldToTexture(Vector3 input)
    {
        float worldToTex = (float)textureSize / worldSize;
        input -= mOrigin;
        int cx = Mathf.RoundToInt(input.x * worldToTex);
        int cy = Mathf.RoundToInt(input.z * worldToTex);

        cx = Mathf.Clamp(cx, 0, textureSize - 1);
        cy = Mathf.Clamp(cy, 0, textureSize - 1);

        return new int[] { cx, cy };
    }

    public bool IsVis(Vector3 pos)
    {
        return IsVis(pos.x, pos.z);
    }

    public bool IsVis(Point pos)
    {
        return IsVis(pos.x, pos.y);
    }

    public bool DistanceToPlayer(Vector3 pos)
    {
        if (BigBoss.PlayerInfo == null) return false;
        if (Vector3.Distance(pos, BigBoss.PlayerInfo.transform.position) < 20)
        {
            return true;
        }
        return false;
    }

    //these are called often, so I am keeping two versions
    public bool IsVis(int x, int y)
    {
        foreach (Revealer r in mRevealers)
        {
            float xr = x - r.pos.x;
            float yr = y - r.pos.z;
            float circleRadii = xr * xr + yr * yr;
            if (circleRadii < r.revDist)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsVis(float x, float y)
    {
        foreach (Revealer r in mRevealers)
        {
            float xr = x - r.pos.x;
            float yr = y - r.pos.z;
            float circleRadii = xr * xr + yr * yr;
            if (circleRadii < r.revDist)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks to see if the specified position has been explored.
    /// </summary>

    public bool IsExplored(Vector3 pos)
    {
        if (mBuffer0 == null) return false;
        pos -= mOrigin;

        float worldToTex = (float)textureSize / worldSize;
        int cx = Mathf.RoundToInt(pos.x * worldToTex);
        int cy = Mathf.RoundToInt(pos.z * worldToTex);

        cx = Mathf.Clamp(cx, 0, textureSize - 1);
        cy = Mathf.Clamp(cy, 0, textureSize - 1);
        return mBuffer0[cx + cy * textureSize].g > 0;
    }
}