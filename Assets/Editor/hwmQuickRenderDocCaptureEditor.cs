using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.Collections;

public class hwmQuickRenderDocCaptureEditor : EditorWindow
{
	private Setting m_Setting;
	private SceneAsset m_SceneAsset;
	private Camera m_Camera;
	private hwmEditorCoroutine m_AnalyzeCoroutine;

	[MenuItem("Custom/RPA/Quick RenderDoc Capture")]
	public static void ShowWindow()
	{
		GetWindow<hwmQuickRenderDocCaptureEditor>("Quick RenderDoc Capture", true);
	}

	protected void OnEnable()
	{
		m_Setting.LoadSetting();
		m_SceneAsset = AssetDatabase.LoadAssetAtPath(m_Setting.AnalyzeScenePath.GetValue(), typeof(SceneAsset)) as SceneAsset;
	}

	protected void OnDisable()
	{
		m_Setting.SaveSetting();
	}

	protected void OnGUI()
	{
		if (!hwmRenderDocUtility.IsInstalled())
		{
			EditorGUILayout.HelpBox("Not installed RenderDoc", MessageType.Error);
			return;
		}
		else if (!hwmRenderDocUtility.IsSupported())
		{
			EditorGUILayout.HelpBox("Not supported RenderDoc", MessageType.Error);
			return;
		}
		else if (!hwmRenderDocUtility.IsLoaded())
		{
			EditorGUILayout.HelpBox("Not loaded RenderDoc", MessageType.Error);
			hwmRenderDocUtility.Load();
			return;
		}

		#region Scene
		EditorGUILayout.BeginHorizontal();
		SceneAsset sceneAsset = EditorGUILayout.ObjectField("Analyze Scene", m_SceneAsset, typeof(SceneAsset), true) as SceneAsset;
		if (sceneAsset != m_SceneAsset)
		{
			m_SceneAsset = sceneAsset;
			m_Setting.AnalyzeScenePath.SetValue(AssetDatabase.GetAssetPath(m_SceneAsset));
		}
		EditorGUILayout.EndHorizontal();

		if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().path != m_Setting.AnalyzeScenePath.GetValue())
		{
			if (GUILayout.Button("Open Analyze Scene"))
			{
				EditorSceneManager.OpenScene(m_Setting.AnalyzeScenePath.GetValue(), OpenSceneMode.Single);
			}
			return;
		}
		#endregion

		m_Camera = EditorGUILayout.ObjectField("Camera", m_Camera, typeof(Camera), false) as Camera;
		if (m_Camera == null)
		{
			m_Camera = FindObjectOfType<Camera>();
		}
		if (m_Camera == null)
		{
			EditorGUILayout.HelpBox("Must have a camera", MessageType.Error);
			return;
		}

		EditorGUILayout.Space();
		m_Setting.ParticleSimulateTime.SetValue(EditorGUILayout.FloatField("Particle Simulate Time", m_Setting.ParticleSimulateTime.GetValue()));
		m_Setting.RelativeSize.SetValue(EditorGUILayout.Slider("Relative Size", m_Setting.RelativeSize.GetValue(), 0.0f, 5.0f));
		m_Setting.DestroyAfterAnalyze.SetValue(EditorGUILayout.Toggle("Destroy After Analyze", m_Setting.DestroyAfterAnalyze.GetValue()));

		if (m_AnalyzeCoroutine != null
			&& m_AnalyzeCoroutine.IsRuning())
		{
			EditorGUILayout.HelpBox("Analyzing", MessageType.Info);
			return;
		}

		GameObject gameObject = Selection.activeObject as GameObject;
		if (!gameObject)
		{
			EditorGUILayout.HelpBox("Must section a Prefab or GameObject", MessageType.Error);
			return;
		}

		if (GUILayout.Button("Analyze"))
		{
			bool isPrefab = !string.IsNullOrEmpty(AssetDatabase.GetAssetPath(gameObject));
			m_AnalyzeCoroutine = hwmEditorCoroutine.Start(Analyze(gameObject, isPrefab));
		}
	}

	private IEnumerator Analyze(GameObject origin, bool isPrefab)
	{
		GameObject target = isPrefab ? Instantiate(origin) : origin;
		ParticleSystem[] particleSystems = target.GetComponentsInChildren<ParticleSystem>();
		hwmObjectUtility.SelectionComponent(particleSystems);

		// 选中后下一帧才会有Renderer
		yield return null;
		for (int iParticle = 0; iParticle < particleSystems.Length; iParticle++)
		{
			particleSystems[iParticle].Simulate(m_Setting.ParticleSimulateTime.GetValue());
		}

		Bounds aabb = new Bounds();
		bool aabbInitialized = false;
		Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
		for (int iRenderer = 0; iRenderer < renderers.Length; iRenderer++)
		{
			Bounds iterAABB = renderers[iRenderer].bounds;
			if (aabbInitialized)
			{
				aabb.Encapsulate(iterAABB);
			}
			else
			{
				aabb = iterAABB;
			}
		}

		float diameter = hwmMathUtility.CaculateDiagonal(aabb);
		float distance = hwmRendererUtility.CacluateToCameraDistance(diameter
			, m_Setting.RelativeSize.GetValue()
			, hwmRendererUtility.CaculateHalfTanCameraFov(m_Camera.fieldOfView));
		target.transform.position = m_Camera.transform.position
			+ m_Camera.transform.forward * distance;
		yield return null;
		hwmRenderDocUtility.CaptureScene(hwmEditorReflectionUtility.GetGameWindow());

		yield return null;
		if (m_Setting.DestroyAfterAnalyze.GetValue())
		{
			DestroyImmediate(target);
		}
	}

	private struct Setting
	{
		private const string PREFS_KEY_STARTWITHS = "RPA_QRDC_";

		public hwmEditorPrefsItem<string> AnalyzeScenePath;
		public hwmEditorPrefsItem<float> ParticleSimulateTime;
		public hwmEditorPrefsItem<float> RelativeSize;
		public hwmEditorPrefsItem<bool> DestroyAfterAnalyze;

		public void LoadSetting()
		{
			AnalyzeScenePath._Initizlize(PREFS_KEY_STARTWITHS + "AnalyzeScenePath");
			ParticleSimulateTime._Initizlize(PREFS_KEY_STARTWITHS + "ParticleSimulateTime");
			RelativeSize._Initizlize(PREFS_KEY_STARTWITHS + "RelativeSize");
			DestroyAfterAnalyze._Initizlize(PREFS_KEY_STARTWITHS + "DestroyAfterAnalyze");
		}

		public void SaveSetting()
		{
			AnalyzeScenePath._SaveValue();
			ParticleSimulateTime._SaveValue();
			RelativeSize._SaveValue();
			DestroyAfterAnalyze._SaveValue();
		}
	}
}