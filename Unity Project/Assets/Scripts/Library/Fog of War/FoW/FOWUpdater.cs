using UnityEngine;

public class FOWUpdater : MonoBehaviour
{
    public Color unexploredColor = new Color(0.05f, 0.05f, 0.05f, 1f);

    public Color exploredColor = new Color(0.2f, 0.2f, 0.2f, 1f);

    Matrix4x4 mInverseMVP;

    FOWSystem mFog;

    void LateUpdate()
    {
        if (mFog == null)
        {
            mFog = FOWSystem.Instance;
            if (mFog == null) mFog = FindObjectOfType(typeof(FOWSystem)) as FOWSystem;
        }

        if (mFog == null || !mFog.enabled)
        {
            enabled = false;
            return;
        }

        float invScale = 1f / mFog.worldSize;
        float worldToTex = (float)mFog.worldSize / mFog.textureSize;

        float x = mFog.lowerRange.x + mFog.ShaderOffsetX * worldToTex;
        float z = mFog.lowerRange.z + mFog.ShaderOffsetY * worldToTex;
        Vector4 p = new Vector4(-x * invScale, -z * invScale, invScale, mFog.blendFactor);

        mInverseMVP = (Camera.main.projectionMatrix * Camera.main.worldToCameraMatrix).inverse;

        Vector4 camPos = Camera.main.transform.position;

        // This accounts for Anti-aliasing on Windows flipping the depth UV coordinates.
        // Despite the official documentation, the following approach simply doesn't work:
        // http://docs.unity3d.com/Documentation/Components/SL-PlatformDifferences.html

        if (QualitySettings.antiAliasing > 0)
        {
            RuntimePlatform pl = Application.platform;

            if (pl == RuntimePlatform.WindowsEditor ||
                pl == RuntimePlatform.WindowsPlayer ||
                pl == RuntimePlatform.WindowsWebPlayer)
            {
                camPos.w = 1f;
            }
        }

        Shader.SetGlobalVector("_CamPos", camPos);
        Shader.SetGlobalMatrix("_InverseMVP", mInverseMVP);
        Shader.SetGlobalColor("_Unexplored", unexploredColor);
        Shader.SetGlobalColor("_Explored", exploredColor);
        Shader.SetGlobalVector("_Params", p);
        Shader.SetGlobalTexture("_FogTex0", mFog.texture0);
        Shader.SetGlobalTexture("_FogTex1", mFog.texture1);
    }
}