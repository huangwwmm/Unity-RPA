using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class hwmEffectStressTestingEditor : EditorWindow
{
	private Setting m_Setting;
	/// <summary>
	/// 用public是为了FindProperty
	/// </summary>
	public GameObject[] _EffectPrefabs;
	private Vector2 m_PrefabsScrollPosition = Vector2.zero;
	private Camera m_Camera;
	private hwmEffectStressTesting m_EffectStressTesting;
	private string m_LastAnalysisResult;

	[MenuItem("Custom/RPA/Effect Stress Testing")]
	public static void ShowWindow()
	{
		GetWindow<hwmEffectStressTestingEditor>("Effect Stress Testing", true);
	}

	protected void OnEnable()
	{
		m_Setting.LoadSetting();

		try
		{
			string[] guids = m_Setting.PrefabGUIDs.GetValue().Split('|');
			List<GameObject> prefabs = new List<GameObject>(guids.Length);
			for (int iGuid = 0; iGuid < guids.Length; iGuid++)
			{
				string iterGuid = guids[iGuid];
				GameObject iterPrefab = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(iterGuid), typeof(GameObject)) as GameObject;
				if (iterPrefab)
				{
					prefabs.Add(iterPrefab);
				}
			}
			_EffectPrefabs = prefabs.ToArray();
		}
		catch (Exception)
		{
			// 这里很可能会失败，也不重要就不处理了
		}
	}

	protected void OnDisable()
	{
		SavePrefabGUIDs();
		m_Setting.SaveSetting();
	}

	protected void OnGUI()
	{
		if (!Application.isPlaying)
		{
			EditorGUILayout.HelpBox("Only runtime", MessageType.Warning);
		}
		else if (m_Camera == null)
		{
			EditorGUILayout.HelpBox("Must have a camera", MessageType.Error);
		}
		else if (m_EffectStressTesting)
		{
			if (m_EffectStressTesting.IsExecuting())
			{
				EditorGUILayout.HelpBox("Executing", MessageType.Info);
			}
			else
			{
				m_LastAnalysisResult = m_EffectStressTesting.GetAnalysisResult();
				DestroyImmediate(m_EffectStressTesting.gameObject);
				m_EffectStressTesting = null;
			}
		}
		else if (GUILayout.Button("Execute"))
		{
			m_EffectStressTesting = new GameObject("EffectStressTesting").AddComponent<hwmEffectStressTesting>();
			m_EffectStressTesting.StartCoroutine(m_EffectStressTesting.Execute(m_Camera
				, _EffectPrefabs
				, m_Setting.InstantiateCount.GetValue()
				, m_Setting.SimulateTime.GetValue()
				, m_Setting.ToCameraDistance.GetValue()));
		}

		EditorGUILayout.Space();
		OnGUI_Setting();
		OnGUI_Camera();
		OnGUI_Prefabs();
		OnGUI_AnalysisResult();
	}

	private void OnGUI_Prefabs()
	{
		SerializedObject serializedObject = new SerializedObject(this);
		SerializedProperty stringsProperty = serializedObject.FindProperty("_EffectPrefabs");
		m_PrefabsScrollPosition = EditorGUILayout.BeginScrollView(m_PrefabsScrollPosition);
		EditorGUILayout.PropertyField(stringsProperty, true);
		EditorGUILayout.EndScrollView();
		serializedObject.ApplyModifiedProperties();
		if (GUI.changed)
		{
			SavePrefabGUIDs();
		}
	}

	private void OnGUI_Setting()
	{
		m_Setting.InstantiateCount.SetValue(EditorGUILayout.IntField("Instantiate Count", m_Setting.InstantiateCount.GetValue()));
		m_Setting.SimulateTime.SetValue(EditorGUILayout.FloatField("Simulate Time", m_Setting.SimulateTime.GetValue()));
		m_Setting.ToCameraDistance.SetValue(EditorGUILayout.FloatField("To Camera Distance", m_Setting.ToCameraDistance.GetValue()));
	}

	private void OnGUI_Camera()
	{
		m_Camera = EditorGUILayout.ObjectField("Camera", m_Camera, typeof(Camera), false) as Camera;
		if (m_Camera == null)
		{
			m_Camera = FindObjectOfType<Camera>();
		}
	}

	private void OnGUI_AnalysisResult()
	{
		if (!string.IsNullOrWhiteSpace(m_LastAnalysisResult))
		{
			EditorGUILayout.Space();
			EditorGUILayout.TextArea(m_LastAnalysisResult, GUILayout.Height(80));
			EditorGUILayout.TextField("Export Result Path", m_Setting.ExportResultPath.GetValue());
			if (GUILayout.Button("Export result to csv file"))
			{
				try
				{
					File.WriteAllText(m_Setting.ExportResultPath.GetValue(), m_LastAnalysisResult);
					UnityEditor.EditorUtility.RevealInFinder(m_Setting.ExportResultPath.GetValue());
				}
				catch (Exception e)
				{
					UnityEditor.EditorUtility.DisplayDialog("Export result to csv file failed", e.ToString(), "ok");
				}
			}
		}
	}

	private void SavePrefabGUIDs()
	{
		StringBuilder stringBuilder = hwmStringUtility.AllocStringBuilderCache();
		for (int iPrefab = 0; iPrefab < _EffectPrefabs.Length; iPrefab++)
		{
			GameObject iterPrefab = _EffectPrefabs[iPrefab];
			if (!iterPrefab)
			{
				continue;
			}

			stringBuilder.Append(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(iterPrefab))).Append('|');
		}
		m_Setting.PrefabGUIDs.SetValue(hwmStringUtility.ReleaseStringBuilderCacheAndReturnString());
	}

	internal struct Setting
	{
		private const string PREFS_KEY_STARTWITHS = "RPA_EST_";

		public hwmEditorPrefsItem<int> InstantiateCount;
		public hwmEditorPrefsItem<float> SimulateTime;
		public hwmEditorPrefsItem<string> PrefabGUIDs;
		public hwmEditorPrefsItem<float> ToCameraDistance;
		public hwmEditorPrefsItem<string> ExportResultPath;

		public void LoadSetting()
		{
			InstantiateCount._Initizlize(PREFS_KEY_STARTWITHS + "InstantiateCount");
			SimulateTime._Initizlize(PREFS_KEY_STARTWITHS + "SimulateTime");
			PrefabGUIDs._Initizlize(PREFS_KEY_STARTWITHS + "PrefabGUIDs");
			ToCameraDistance._Initizlize(PREFS_KEY_STARTWITHS + "ToCameraDistance");
			ExportResultPath._Initizlize(PREFS_KEY_STARTWITHS + "ExportResultPath");
		}

		public void SaveSetting()
		{
			InstantiateCount._SaveValue();
			SimulateTime._SaveValue();
			PrefabGUIDs._SaveValue();
			ToCameraDistance._SaveValue();
			ExportResultPath._SaveValue();
		}
	}
}