#if UNITY_EDITOR
using System.Collections;
using System.Text;
using UnityEditor;
using UnityEngine;

public class hwmEffectStressTesting : MonoBehaviour
{
	private const float WAIT_SECOND = 0.3f;

	private bool m_IsExecuting;
	private bool m_IsSampling;
	private string m_AnalysisResult;
	private hwmDoubleSampleHelper m_MaxFrameTimeSample;
	private hwmDoubleSampleHelper m_ClientFrameTimeSample;
	private hwmDoubleSampleHelper m_RenderFrameTimeSample;
	private hwmDoubleSampleHelper m_TriangleCountSample;
	private hwmDoubleSampleHelper m_VerticeCountSample;
	private hwmLongSampleHelper m_DynamicBatchedDrawCallCountSample;
	private hwmLongSampleHelper m_StaticBatchedDrawCallCountSample;
	private hwmLongSampleHelper m_InstancedBatchedDrawCallCountSample;
	private hwmLongSampleHelper m_DynamicBatcheCountSample;
	private hwmLongSampleHelper m_StaticBatcheCountSample;
	private hwmLongSampleHelper m_InstancedBatcheCountSample;

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
		m_IsSampling = false;
		Application.targetFrameRate = int.MaxValue;
		Time.maximumDeltaTime = 0;
		QualitySettings.vSyncCount = 0;
		// 预先加载反射数据
		hwmEditorReflectionUtility.GetMaxFrameTime();
		hwmEditorReflectionUtility.GetClientFrameTime();
		hwmEditorReflectionUtility.GetRenderFrameTime();
		hwmEditorReflectionUtility.GetTriangleCount();
		hwmEditorReflectionUtility.GetVerticeCount();
		hwmEditorReflectionUtility.GetDynamicBatchedDrawCallCount();
		hwmEditorReflectionUtility.GetStaticBatchedDrawCallCount();
		hwmEditorReflectionUtility.GetInstancedBatchedDrawCallCount();
		hwmEditorReflectionUtility.GetDynamicBatcheCount();
		hwmEditorReflectionUtility.GetStaticBatcheCount();
		hwmEditorReflectionUtility.GetInstancedBatcheCount();
		yield return null;

		Vector3 effectPosition = camera.transform.position
			+ camera.transform.forward * toCameraDistance;
		GameObject[] instantiates = new GameObject[instantiateCount];
		StringBuilder stringBuilder = new StringBuilder()
			.Append("Asset Path").Append(',')
			.Append("Asset").Append(',')
			.Append("Average FPS").Append(',')
			.Append("Max FPS").Append(',')
			.Append("Average CPU Time").Append(',')
			.Append("Max CPU Time").Append(',')
			.Append("Average GPU Time").Append(',')
			.Append("Max GPU Time").Append(',')
			.Append("Average Triangle").Append(',')
			.Append("Max Triangle").Append(',')
			.Append("Average Vertice").Append(',')
			.Append("Max Vertice").Append(',')
			.Append("Average Dynamic DrawCall").Append(',')
			.Append("Max Dynamic DrawCall").Append(',')
			.Append("Average Dynamic Batche").Append(',')
			.Append("Max Dynamic Batche").Append(',')
			.Append("Average Static DrawCall").Append(',')
			.Append("Max Static DrawCall").Append(',')
			.Append("Average Static Batche").Append(',')
			.Append("Max Static Batche").Append(',')
			.Append("Average Instanced DrawCall").Append(',')
			.Append("Max Instanced DrawCall").Append(',')
			.Append("Average Instanced Batche").Append(',')
			.Append("Max Instanced Batche").Append(',')
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
			m_DynamicBatchedDrawCallCountSample.Reset();
			m_StaticBatchedDrawCallCountSample.Reset();
			m_InstancedBatchedDrawCallCountSample.Reset();
			m_DynamicBatcheCountSample.Reset();
			m_StaticBatcheCountSample.Reset();
			m_InstancedBatcheCountSample.Reset();
			m_IsSampling = true;
			yield return new WaitForSeconds(simulateTime);
			m_IsSampling = false;
			string assetPath = AssetDatabase.GetAssetPath(iterPrefab);
			stringBuilder.Append(assetPath).Append(',')
				.Append(hwmStringUtility.SubFileNameFromPath(assetPath)).Append(',')
				.Append(string.Format("{0:F0} FPS", 1.0 / m_MaxFrameTimeSample.GetAverage())).Append(',')
				.Append(string.Format("{0:F0} FPS", 1.0 / m_MaxFrameTimeSample.GetMaximum())).Append(',')
				.Append(string.Format("{0:F1} ms", m_ClientFrameTimeSample.GetAverage())).Append(',')
				.Append(string.Format("{0:F1} ms", m_ClientFrameTimeSample.GetMaximum())).Append(',')
				.Append(string.Format("{0:F1} ms", m_RenderFrameTimeSample.GetAverage())).Append(',')
				.Append(string.Format("{0:F1} ms", m_RenderFrameTimeSample.GetMaximum())).Append(',')
				.Append(string.Format("{0:F2} k", m_TriangleCountSample.GetAverage())).Append(',')
				.Append(string.Format("{0:F2} k", m_TriangleCountSample.GetMaximum())).Append(',')
				.Append(string.Format("{0:F2} k", m_VerticeCountSample.GetAverage())).Append(',')
				.Append(string.Format("{0:F2} k", m_VerticeCountSample.GetMaximum())).Append(',')
				.Append(m_DynamicBatchedDrawCallCountSample.GetAverage()).Append(',')
				.Append(m_DynamicBatchedDrawCallCountSample.GetMaximum()).Append(',')
				.Append(m_DynamicBatcheCountSample.GetAverage()).Append(',')
				.Append(m_DynamicBatcheCountSample.GetMaximum()).Append(',')
				.Append(m_StaticBatchedDrawCallCountSample.GetAverage()).Append(',')
				.Append(m_StaticBatchedDrawCallCountSample.GetMaximum()).Append(',')
				.Append(m_StaticBatcheCountSample.GetAverage()).Append(',')
				.Append(m_StaticBatcheCountSample.GetMaximum()).Append(',')
				.Append(m_InstancedBatchedDrawCallCountSample.GetAverage()).Append(',')
				.Append(m_InstancedBatchedDrawCallCountSample.GetMaximum()).Append(',')
				.Append(m_InstancedBatcheCountSample.GetMaximum()).Append(',')
				.Append(m_InstancedBatcheCountSample.GetAverage()).Append(',')
				.Append('\n');

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
		if (m_IsSampling)
		{
			m_MaxFrameTimeSample.Sample(hwmEditorReflectionUtility.GetMaxFrameTime());
			m_ClientFrameTimeSample.Sample(hwmEditorReflectionUtility.GetClientFrameTime() * 1000.0f);
			m_RenderFrameTimeSample.Sample(hwmEditorReflectionUtility.GetRenderFrameTime() * 1000.0f);
			m_TriangleCountSample.Sample(hwmEditorReflectionUtility.GetTriangleCount() * 0.001);
			m_VerticeCountSample.Sample(hwmEditorReflectionUtility.GetVerticeCount() * 0.001);
			m_DynamicBatchedDrawCallCountSample.Sample(hwmEditorReflectionUtility.GetDynamicBatchedDrawCallCount());
			m_StaticBatchedDrawCallCountSample.Sample(hwmEditorReflectionUtility.GetStaticBatchedDrawCallCount());
			m_InstancedBatchedDrawCallCountSample.Sample(hwmEditorReflectionUtility.GetInstancedBatchedDrawCallCount());
			m_DynamicBatcheCountSample.Sample(hwmEditorReflectionUtility.GetDynamicBatcheCount());
			m_StaticBatcheCountSample.Sample(hwmEditorReflectionUtility.GetStaticBatcheCount());
			m_InstancedBatcheCountSample.Sample(hwmEditorReflectionUtility.GetInstancedBatcheCount());
		}
	}
}
#endif