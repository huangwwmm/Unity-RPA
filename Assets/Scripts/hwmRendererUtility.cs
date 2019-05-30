using UnityEngine;

public static class hwmRendererUtility
{
	/// <summary>
	/// 应用天空盒
	/// </summary>
	public static void ApplySkybox(Material skybox)
	{
		RenderSettings.skybox = skybox;
	}

	/// <summary>
	/// TODO 这个有问题，先不要用
	/// 应用Lightmaps
	/// </summary>
	/// <param name="lightmapDirs"><see cref="LightmapData.lightmapDir"/></param>
	/// <param name="lightmapColors"><see cref="LightmapData.lightmapColor"/></param>
	/// <param name="shadowMasks"><see cref="LightmapData.shadowMask"/></param>
	public static void ApplyLightmaps(Texture2D[] lightmapDirs
		, Texture2D[] lightmapColors
		, Texture2D[] shadowMasks)
	{
		hwmDebugUtility.Assert(false, "这个有问题，先不要用");
		LightmapData[] lightmaps = new LightmapData[hwmMathUtility.Max(lightmapDirs.Length
			, lightmapDirs.Length
			, lightmapDirs.Length)];

		for (int iLightmap = 0; iLightmap < lightmaps.Length; iLightmap++)
		{
			LightmapData iterLightmap = new LightmapData();
			iterLightmap.lightmapDir = lightmapDirs.Length > iLightmap ? lightmapDirs[iLightmap] : null;
			iterLightmap.lightmapColor = lightmapColors.Length > iLightmap ? lightmapColors[iLightmap] : null;
			iterLightmap.shadowMask = shadowMasks.Length > iLightmap ? shadowMasks[iLightmap] : null;
			lightmaps[iLightmap] = iterLightmap;
		}
		LightmapSettings.lightmaps = lightmaps;
	}

	/// <summary>
	/// 计算一个物体距离相机多远时，会在屏幕上占relativeSizeInScreen
	/// </summary>
	/// <returns></returns>
	public static float CacluateToCameraDistance(float diameter, float relativeSizeInScreen, float halfTanFov)
	{
		return diameter / relativeSizeInScreen * 0.5f / halfTanFov;
	}

	/// <summary>
	/// 计算一个物体占屏幕的比例
	/// </summary>
	public static float CaculateRelativeSizeInScreen(float diameter, float toCameraDistance, float halfTanFov)
	{
		return diameter / (halfTanFov * toCameraDistance * 2.0f);
	}

	/// <summary>
	/// <see cref="CaculateRelativeSizeInScreen(float, float, float)"/>
	/// </summary>
	public static float CaculateRelativeSizeInScreen(float diameter, float toCameraDistance, Camera camera)
	{
		float halfTanFov = CaculateHalfTanCameraFov(camera.fieldOfView);
		return CaculateRelativeSizeInScreen(diameter, toCameraDistance, halfTanFov);
	}

	/// <summary>
	/// 计算tan(fov * 0.5f)
	/// </summary>
	public static float CaculateHalfTanCameraFov(float fov)
	{
		return Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
	}
}