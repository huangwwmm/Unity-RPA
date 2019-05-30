#if UNITY_EDITOR
using System.Collections;
using System.Text;
using UnityEditor;
using UnityEngine;

public class hwmEffectStressTesting : MonoBehaviour
{
	private const float WAIT_SECOND = 0.3f;

	private bool m_IsExecuting;
	private string m_AnalysisResult;
	private hwmDoubleSampleHelper m_MaxFrameTimeSample;
	private hwmDoubleSampleHelper m_ClientFrameTimeSample;
	private hwmDoubleSampleHelper m_RenderFrameTimeSample;
	private LongSampleHelper m_TriangleCountSample;
	private LongSampleHelper m_VerticeCountSample;

	public string GetAnalysisResult()
	{
		return m_AnalysisResult;
	}

	public bool IsExecuting()
	{
		return m_IsExecuting;
	}

	public IEnumerator Execute(Camera camera
		, GameObject[] effectPrefabs
		, int instantiateCount
		, float simulateTime
		, float toCameraDistance)
	{
		m_IsExecuting = true;
		Application.targetFrameRate = int.MaxValue;
		Time.maximumDeltaTime = 0;
		QualitySettings.vSyncCount = 0;
		// 预先加载反射数据
		hwmEditorReflectionUtility.GetMaxFrameTime();
		hwmEditorReflectionUtility.GetClientFrameTime();
		hwmEditorReflectionUtility.GetRenderFrameTime();
		hwmEditorReflectionUtility.GetTriangleCount();
		hwmEditorReflectionUtility.GetVerticeCount();
		yield return null;

		Vector3 effectPosition = camera.transform.position
			+ camera.transform.forward * toCameraDistance;
		GameObject[] instantiates = new GameObject[instantiateCount];
		StringBuilder stringBuilder = new StringBuilder()
			.Append("Asset,Average FPS,Max FPS,Average CPU Time, Max CPU Time,Average GPU Time, Max GPU Time,Max Triangle,Max Vertice")
			.Append('\n');
		yield return null;

		for (int iPrefab = 0; iPrefab < effectPrefabs.Length; iPrefab++)
		{
			GameObject iterPrefab = effectPrefabs[iPrefab];
			if (!iterPrefab)
			{
				continue;
			}

			for (int iInstantiate = 0; iInstantiate < instantiateCount; iInstantiate++)
			{
				instantiates[iInstantiate] = Instantiate(iterPrefab, effectPosition, Quaternion.identity);
			}
			// 不知道为什么刚创建出来很卡，所以多等几帧
			yield return new WaitForSeconds(WAIT_SECOND);
			m_MaxFrameTimeSample.Reset();
			m_ClientFrameTimeSample.Reset();
			m_RenderFrameTimeSample.Reset();
			m_TriangleCountSample.Reset();
			m_VerticeCountSample.Reset();
			yield return new WaitForSeconds(simulateTime);
			stringBuilder.Append(AssetDatabase.GetAssetPath(iterPrefab)).Append(',')
				.Append(string.Format("{0:F0} FPS", 1.0 / m_MaxFrameTimeSample.GetAverage())).Append(',')
				.Append(string.Format("{0:F0} FPS", 1.0 / m_MaxFrameTimeSample.GetMaximum())).Append(',')
				.Append(string.Format("{0:F1} ms", m_ClientFrameTimeSample.GetAverage())).Append(',')
				.Append(string.Format("{0:F1} ms", m_ClientFrameTimeSample.GetMaximum())).Append(',')
				.Append(string.Format("{0:F1} ms", m_RenderFrameTimeSample.GetAverage())).Append(',')
				.Append(string.Format("{0:F1} ms", m_RenderFrameTimeSample.GetMaximum())).Append(',')
				.Append(string.Format("{0} k", m_TriangleCountSample.GetMaximum() * 0.001)).Append(',')
				.Append(string.Format("{0} k", m_VerticeCountSample.GetMaximum() * 0.001)).Append('\n');
			yield return null;

			for (int iInstantiate = 0; iInstantiate < instantiateCount; iInstantiate++)
			{
				DestroyImmediate(instantiates[iInstantiate]);
			}
			yield return new WaitForSeconds(WAIT_SECOND);
		}

		yield return null;
		m_AnalysisResult = stringBuilder.ToString();
		m_IsExecuting = false;
	}

	public void Update()
	{
		if (m_IsExecuting)
		{
			m_MaxFrameTimeSample.Sample(hwmEditorReflectionUtility.GetMaxFrameTime());
			m_ClientFrameTimeSample.Sample(hwmEditorReflectionUtility.GetClientFrameTime() * 1000.0f);
			m_RenderFrameTimeSample.Sample(hwmEditorReflectionUtility.GetRenderFrameTime() * 1000.0f);
			m_TriangleCountSample.Sample(hwmEditorReflectionUtility.GetTriangleCount());
			m_VerticeCountSample.Sample(hwmEditorReflectionUtility.GetVerticeCount());
		}
	}
}
#endif