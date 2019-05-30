#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;

public static class hwmRenderDocUtility
{
	private static bool ms_IsInitialize = false;
	private static MethodInfo CAPTURE_SCENE_METHOD_INFO;

	private static MethodInfo IS_INSTALLED_METHOD_INFO;
	private static MethodInfo IS_LOADED_METHOD_INFO;
	private static MethodInfo IS_SUPPORTED_METHOD_INFO;
	private static MethodInfo LOAD_METHOD_INFO;

	public static bool IsInstalled()
	{
		Initialize();
		return (bool)IS_INSTALLED_METHOD_INFO.Invoke(null, null);
	}

	public static bool IsLoaded()
	{
		Initialize();
		return (bool)IS_LOADED_METHOD_INFO.Invoke(null, null);
	}

	public static bool IsSupported()
	{
		Initialize();
		return (bool)IS_SUPPORTED_METHOD_INFO.Invoke(null, null);
	}

	public static void Load()
	{
		Initialize();
		LOAD_METHOD_INFO.Invoke(null, null);
	}

	public static void CaptureScene(EditorWindow editorWindow)
	{
		Initialize();
		CAPTURE_SCENE_METHOD_INFO.Invoke(hwmEditorReflectionUtility.GetEditorWindowParent(editorWindow), null);
	}

	private static void Initialize()
	{
		if (!ms_IsInitialize)
		{
			Type guiViewType = typeof(EditorGUILayout).Assembly.GetType("UnityEditor.GUIView");
			CAPTURE_SCENE_METHOD_INFO = guiViewType.GetMethod("CaptureRenderDocScene", BindingFlags.Instance | BindingFlags.Public);

			Type renderDocType = typeof(EditorGUILayout).Assembly.GetType("UnityEditorInternal.RenderDoc");
			IS_INSTALLED_METHOD_INFO = renderDocType.GetMethod("IsInstalled", BindingFlags.Static | BindingFlags.Public);
			IS_LOADED_METHOD_INFO = renderDocType.GetMethod("IsLoaded", BindingFlags.Static | BindingFlags.Public);
			IS_SUPPORTED_METHOD_INFO = renderDocType.GetMethod("IsSupported", BindingFlags.Static | BindingFlags.Public);
			LOAD_METHOD_INFO = renderDocType.GetMethod("Load", BindingFlags.Static | BindingFlags.Public);
			ms_IsInitialize = true;
		}
	}

}
#endif