using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public static class hwmObjectUtility
{
	/// <summary>
	/// 获取一个transform相对rootTransform的缩放
	/// 如果rootTransform为null，则获得transform相对世界空间缩放
	/// </summary>
	public static Vector3 CalculateLossyScale(Transform transform, Transform rootTransform = null)
	{
		if (rootTransform == null)
		{
			return transform.lossyScale;
		}
		else
		{
#if UNITY_EDITOR
			const int MAX_DEEP = 1000;
			int deep = 0;
#endif

			Transform iterTransform = transform;
			Vector3 scale = Vector3.one;
			while (iterTransform != rootTransform
					&& iterTransform != null)
			{
				scale = hwmMathUtility.EachMulti(scale, iterTransform.localScale);
				iterTransform = iterTransform.parent;
#if UNITY_EDITOR
				if (deep++ >= MAX_DEEP)
				{
					hwmDebugUtility.Assert(false, "CalculateLossyScale Deep超出限制，是不是代码逻辑有问题");
					break;
				}
#endif
			}
			return scale;
		}
	}

	/// <summary>
	/// 计算一个Transform的name Path
	/// 例，一个Transform结构：
	///		A
	///		|-B
	///		  |-C
	///		    |-D
	///		  |-E
	///	D的Path = /A/B/C/D
	///	E的Path = /A/E
	/// </summary>
	public static string CalculateTransformPath(Transform transform, Transform rootTransform = null)
	{
#if UNITY_EDITOR
		const int MAX_DEEP = 1000;
		int deep = 0;
#endif

		StringBuilder stringBuilder = hwmStringUtility.AllocStringBuilderCache();
		Transform iterTransform = transform;
		while (iterTransform != rootTransform
				&& iterTransform != null)
		{
			stringBuilder.Insert(0, "/" + iterTransform.name);
			iterTransform = iterTransform.parent;

#if UNITY_EDITOR
			if (deep++ >= MAX_DEEP)
			{
				hwmDebugUtility.Assert(false, "CalculateTransformPath Deep超出限制，是不是代码逻辑有问题");
				break;
			}
#endif
		}
		return hwmStringUtility.ReleaseStringBuilderCacheAndReturnString();
	}

	/// <summary>
	/// 计算一个Transform的Index Path，并转换为string
	/// 结果是倒序的
	/// 例，一个Transform结构：
	///		A
	///		|-B
	///		|-C
	///		  |-D
	///		    |-E
	///		  |-F
	///	E的Path = 0/0/1/
	///	F的Path = 1/1/
	/// </summary>
	public static string CalculateTransformIndexPathStringReverseOrder(Transform transform, Transform rootTransform = null)
	{
#if UNITY_EDITOR
		const int MAX_DEEP = 1000;
		int deep = 0;
#endif

		StringBuilder stringBuilder = hwmStringUtility.AllocStringBuilderCache();
		Transform iterTransform = transform;
		Transform iterTransformParent = iterTransform.parent;
		while (iterTransformParent != rootTransform
				&& iterTransformParent != null)
		{
			int index = int.MinValue;
			for (int iChild = 0; iChild < iterTransformParent.childCount; iChild++)
			{
				if (iterTransformParent.GetChild(iChild) == iterTransform)
				{
					index = iChild;
					break;
				}
			}
			stringBuilder.Append(index).Append("/");

			iterTransform = iterTransformParent;
			iterTransformParent = iterTransformParent.parent;

#if UNITY_EDITOR
			if (deep++ >= MAX_DEEP)
			{
				hwmDebugUtility.Assert(false, "CalculateTransformIndexPathStringReverseOrder Deep超出限制，是不是代码逻辑有问题");
				break;
			}
#endif
		}
		return hwmStringUtility.ReleaseStringBuilderCacheAndReturnString();
	}

	/// <summary>
	/// 移除节点下的所有子节点
	/// </summary>
	public static void DestroyAllChildern(Transform transform)
	{
		for (int iChild = transform.childCount - 1; iChild >= 0; iChild--)
		{
			Object.DestroyImmediate(transform.GetChild(iChild).gameObject);
		}
	}

	/// <summary>
	/// 寻找obj上的transfrom
	/// </summary>
	public static Transform FindTransform(Object obj)
	{
		Transform transform;

		if (obj is Transform)
		{
			transform = obj as Transform;
		}
		else if (obj is Component)
		{
			transform = (obj as Component).transform;
		}
		else if (obj is GameObject)
		{
			transform = (obj as GameObject).transform;
		}
		else
		{
			transform = null;
			hwmDebugUtility.Assert(false, "FindTransform()");
		}
		return transform;
	}

#if UNITY_EDITOR
	public static bool HasStaticEditorFlag(GameObject gameObject, StaticEditorFlags flag)
	{
		return (GameObjectUtility.GetStaticEditorFlags(gameObject) & flag) != 0;
	}

	public static void SelectionComponent<T>(T[] components) where T : Component
	{
		Object[] objects = new Object[components.Length];
		for (int iComponent = 0; iComponent < components.Length; iComponent++)
		{
			objects[iComponent] = components[iComponent].gameObject;
		}
		Selection.objects = objects;
	}

	public static void SelectionGameObjects(GameObject[] gameObjects)
	{
		Object[] objects = new Object[gameObjects.Length];
		for (int iGameObject = 0; iGameObject < gameObjects.Length; iGameObject++)
		{
			objects[iGameObject] = gameObjects[iGameObject];
		}
		Selection.objects = objects;
	}
#endif
}