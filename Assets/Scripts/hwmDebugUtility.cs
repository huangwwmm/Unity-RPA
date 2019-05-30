#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public static class hwmDebugUtility
{
	private static System.Diagnostics.Stopwatch ms_Stopwatch;

	static hwmDebugUtility()
	{
		ms_Stopwatch = new System.Diagnostics.Stopwatch();
		ms_Stopwatch.Start();
	}

	public static long GetMillisecondsSinceStartup()
	{
		return ms_Stopwatch.ElapsedMilliseconds;
	}

	public static long GetTicksSinceStartup()
	{
		return ms_Stopwatch.ElapsedTicks;
	}

	/// <summary>
	/// Log<see cref="LogLightmapSetting"/>的信息
	/// </summary>
	public static void LogLightmapSetting()
	{
		Debug.Log("lightmaps length " + LightmapSettings.lightmaps.Length);
		for (int iLightmapData = 0; iLightmapData < LightmapSettings.lightmaps.Length; iLightmapData++)
		{
			LightmapData iterLightmapData = LightmapSettings.lightmaps[iLightmapData];
			Debug.Log("lightmapColor", iterLightmapData.lightmapColor);
			Debug.Log("lightmapDir", iterLightmapData.lightmapDir);
			Debug.Log("shadowMask", iterLightmapData.shadowMask);
		}

		Debug.Log("lightmapsMode " + LightmapSettings.lightmapsMode);
		Debug.Log("lightProbes length" + LightmapSettings.lightProbes.bakedProbes.Length);
	}

	/// <summary>
	/// <see cref="hwmDebugUtility.Assert"/>
	/// </summary>
	public static bool Assert(bool condition, string message, bool displayDialog = true)
	{
		if (!condition)
		{
			Debug.Assert(condition, message);
#if UNITY_EDITOR
			if (displayDialog)
			{
				EditorUtility.DisplayDialog("Assert Failed", message, "OK");
			}
			Debug.Break();
#endif
		}
		return condition;
	}

	/// <summary>
	/// <see cref="hwmDebugUtility.Assert"/>
	/// </summary>
	public static bool Assert(bool condition, string message, UnityEngine.Object context, bool displayDialog = true)
	{
		if (!condition)
		{
			Debug.Assert(condition, message, context);
#if UNITY_EDITOR
			if (displayDialog)
			{
				EditorUtility.DisplayDialog("Assert Failed", message, "OK");
			}
			Debug.Break();
#endif
		}
		return condition;
	}
}
