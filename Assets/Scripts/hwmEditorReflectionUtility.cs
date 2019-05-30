#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;

public static class hwmEditorReflectionUtility
{
	/// <summary>
	/// <see cref="EditorWindow.m_Parent"/>
	/// </summary>
	private static FieldInfo EDITOR_WINDOW_PARENT_FIELDINFO;
	/// <summary>
	/// 用于计算FPS
	/// </summary>
	private static FieldInfo MAX_FRAME_TIME_FIELDINFO;
	/// <summary>
	/// 当前帧CPU时间
	/// </summary>
	private static FieldInfo CLIENT_FRAME_TIME_FIELDINFO;
	/// <summary>
	/// 当前帧渲染时间
	/// </summary>
	private static FieldInfo RENDER_FRAME_TIME_FIELDINFO;
	/// <summary>
	/// 当前渲染的三角形数量
	/// </summary>
	private static MethodInfo TRIANGLE_COUNT_METHODINFO;
	/// <summary>
	/// 当前渲染的顶点数量
	/// </summary>
	private static MethodInfo VERTICE_COUNT_METHODINFO;

	public static EditorWindow GetGameWindow()
	{
		return EditorWindow.GetWindow(typeof(EditorGUILayout).Assembly.GetType("UnityEditor.GameView"));
	}

	public static object GetEditorWindowParent(EditorWindow editorWindow)
	{
		if (EDITOR_WINDOW_PARENT_FIELDINFO == null)
		{
			EDITOR_WINDOW_PARENT_FIELDINFO = typeof(EditorWindow).GetField("m_Parent", BindingFlags.Instance | BindingFlags.NonPublic);
		}
		return EDITOR_WINDOW_PARENT_FIELDINFO.GetValue(editorWindow);
	}

	public static float GetMaxFrameTime()
	{
		if (MAX_FRAME_TIME_FIELDINFO == null)
		{
			MAX_FRAME_TIME_FIELDINFO = typeof(EditorGUILayout).Assembly
				.GetType("UnityEditor.GameViewGUI")
				.GetField("m_MaxFrameTime", BindingFlags.Static | BindingFlags.NonPublic);
		}

		return (float)MAX_FRAME_TIME_FIELDINFO.GetValue(null);
	}

	public static float GetClientFrameTime()
	{
		if (CLIENT_FRAME_TIME_FIELDINFO == null)
		{
			CLIENT_FRAME_TIME_FIELDINFO = typeof(EditorGUILayout).Assembly
				.GetType("UnityEditor.GameViewGUI")
				.GetField("m_ClientFrameTime", BindingFlags.Static | BindingFlags.NonPublic);
		}

		return (float)CLIENT_FRAME_TIME_FIELDINFO.GetValue(null);
	}

	public static float GetRenderFrameTime()
	{
		if (RENDER_FRAME_TIME_FIELDINFO == null)
		{
			RENDER_FRAME_TIME_FIELDINFO = typeof(EditorGUILayout).Assembly
				.GetType("UnityEditor.GameViewGUI")
				.GetField("m_RenderFrameTime", BindingFlags.Static | BindingFlags.NonPublic);
		}

		return (float)RENDER_FRAME_TIME_FIELDINFO.GetValue(null);
	}

	public static int GetTriangleCount()
	{
		if (TRIANGLE_COUNT_METHODINFO == null)
		{
			TRIANGLE_COUNT_METHODINFO = typeof(EditorGUILayout).Assembly
				.GetType("UnityEditor.UnityStats")
				.GetMethod("get_triangles", BindingFlags.Static | BindingFlags.Public);
		}

		return (int)TRIANGLE_COUNT_METHODINFO.Invoke(null, null);
	}

	public static int GetVerticeCount()
	{
		if (VERTICE_COUNT_METHODINFO == null)
		{
			VERTICE_COUNT_METHODINFO = typeof(EditorGUILayout).Assembly
				.GetType("UnityEditor.UnityStats")
				.GetMethod("get_vertices", BindingFlags.Static | BindingFlags.Public);
		}

		return (int)VERTICE_COUNT_METHODINFO.Invoke(null, null);
	}
}
#endif