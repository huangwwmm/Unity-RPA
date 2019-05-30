﻿using UnityEngine;

public static class hwmMathUtility
{
	/// <summary>
	/// transform空间下的一个AABB变换到世界空间后的AABB
	/// TODO 这里应该可以减少运算量，等出现性能问题时再来优化
	/// </summary>
	public static void CalculateAABBTransform(Transform transform
		, Vector3 localAABBSize
		, Vector3 localAABBOffset
		, out Vector3 aabbMin
		, out Vector3 aabbMax)
	{
		Matrix4x4 localToWorldMatrix = transform.localToWorldMatrix;
		Vector3 localAABBHalfSize = localAABBSize * 0.5f;
		Vector3 aabbOffset = localToWorldMatrix * localAABBOffset;

		// 这里的xyzXYZ，小写字母对应轴的负方向，大写字母对应轴正方向
		Vector3 xyz = localToWorldMatrix * (
			-new Vector3(-localAABBHalfSize.x, -localAABBHalfSize.y, -localAABBHalfSize.z));
		Vector3 xyZ = localToWorldMatrix * (
			-new Vector3(-localAABBHalfSize.x, -localAABBHalfSize.y, localAABBHalfSize.z));
		Vector3 Xyz = localToWorldMatrix * (
			-new Vector3(localAABBHalfSize.x, -localAABBHalfSize.y, -localAABBHalfSize.z));
		Vector3 XyZ = localToWorldMatrix * (
			-new Vector3(localAABBHalfSize.x, -localAABBHalfSize.y, localAABBHalfSize.z));
		Vector3 xYz = -XyZ;
		Vector3 xYZ = -Xyz;
		Vector3 XYz = -xyZ;
		Vector3 XYZ = -xyz;

		aabbMin = new Vector3(Min(xyz.x, xyZ.x, xYz.x, xYZ.x, Xyz.x, XyZ.x, XYz.x, XYZ.x)
				, Min(xyz.y, xyZ.y, xYz.y, xYZ.y, Xyz.y, XyZ.y, XYz.y, XYZ.y)
				, Min(xyz.z, xyZ.z, xYz.z, xYZ.z, Xyz.z, XyZ.z, XYz.z, XYZ.z))
			+ aabbOffset;

		aabbMax = new Vector3(Max(xyz.x, xyZ.x, xYz.x, xYZ.x, Xyz.x, XyZ.x, XYz.x, XYZ.x)
				, Max(xyz.y, xyZ.y, xYz.y, xYZ.y, Xyz.y, XyZ.y, XYz.y, XYZ.y)
				, Max(xyz.z, xyZ.z, xYz.z, xYZ.z, Xyz.z, XyZ.z, XYz.z, XYZ.z))
			+ aabbOffset;
	}

	/// <summary>
	/// <see cref="CalculateAABBTransform(Transform, Vector3, Vector3, out Vector3, out Vector3)"/>
	/// </summary>
	public static Bounds CalculateAABBTransform(Transform transform
		, Vector3 localAABBSize
		, Vector3 localAABBOffset)
	{
		Vector3 aabbMin, aabbMax;
		CalculateAABBTransform(transform, localAABBSize, localAABBOffset, out aabbMin, out aabbMax);
		return new Bounds(transform.position + aabbMin + (aabbMax - aabbMin) * 0.5f, aabbMax - aabbMin);
	}

	/// <summary>
	/// 不用<see cref="Mathf.Min(int[])"/>是因为params参数会有GC
	/// </summary>
	public static float Min(float x1, float x2, float x3, float x4, float x5, float x6, float x7, float x8)
	{
		return Mathf.Min(Mathf.Min(Mathf.Min(x1, x2), Mathf.Min(x3, x4))
			, Mathf.Min(Mathf.Min(x5, x6), Mathf.Min(x7, x8)));
	}

	/// <summary>
	/// 不用<see cref="Mathf.Max(int[])"/>是因为params参数会有GC
	/// </summary>
	public static float Max(float x1, float x2, float x3)
	{
		return Mathf.Max(Mathf.Max(x1, x2), x3);
	}

	/// <summary>
	/// <see cref="Max(int, int)"/>
	/// </summary>
	public static int Max(int x1, int x2, int x3)
	{
		return Max(Max(x1, x2), x3);
	}

	/// <summary>
	/// 封装int的Max是为了避免int转float再转int
	/// 转换过程中有内存拷贝有cpu计算，性能比较废
	/// </summary>
	public static int Max(int x1, int x2)
	{
		return x1 > x2 ? x1 : x2;
	}

	/// <summary>
	/// 不用<see cref="Mathf.Max(int[])"/>是因为params参数会有GC
	/// </summary>
	public static float Max(float x1, float x2, float x3, float x4, float x5, float x6, float x7, float x8)
	{
		return Mathf.Max(Mathf.Max(Mathf.Max(x1, x2), Mathf.Max(x3, x4))
			, Mathf.Max(Mathf.Max(x5, x6), Mathf.Max(x7, x8)));
	}

	/// <summary>
	/// 不用<see cref="Mathf.Min(int[])"/>是因为params参数会有GC
	/// </summary>
	public static float Min(float x1, float x2, float x3)
	{
		return Mathf.Min(Mathf.Min(x1, x2), x3);
	}

	/// <summary>
	/// 计算Vector3，值最大的分量
	/// </summary>
	public static float Max(Vector3 vec)
	{
		return Max(vec.x, vec.y, vec.z);
	}

	/// <summary>
	/// 计算Vector3，值最小的分量
	/// </summary>
	public static float Min(Vector3 vec)
	{
		return Min(vec.x, vec.y, vec.z);
	}

	/// <summary>
	/// 对Vector3的三个分量取绝对值
	/// </summary>
	public static Vector3 EachAbs(Vector3 vec)
	{
		return new Vector3(Mathf.Abs(vec.x)
			, Mathf.Abs(vec.y)
			, Mathf.Abs(vec.z));
	}

	/// <summary>
	/// <see cref="Mathf.CeilToInt"/>
	/// </summary>
	public static Vector3 EachCeilToInt(Vector3 vec)
	{
		vec.x = Mathf.CeilToInt(vec.x);
		vec.y = Mathf.CeilToInt(vec.y);
		vec.z = Mathf.CeilToInt(vec.z);
		return vec;
	}

	/// <summary>
	/// <see cref="Mathf.FloorToInt"/>
	/// </summary>
	public static Vector3 EachFloorToInt(Vector3 vec)
	{
		vec.x = Mathf.FloorToInt(vec.x);
		vec.y = Mathf.FloorToInt(vec.y);
		vec.z = Mathf.FloorToInt(vec.z);
		return vec;
	}

	/// <summary>
	/// 求两个向量，每个轴的最大值
	/// </summary>
	public static Vector3 EachMax(Vector3 a, Vector3 b)
	{
		return new Vector3(Mathf.Max(a.x, b.x)
			, Mathf.Max(a.y, b.y)
			, Mathf.Max(a.z, b.z));
	}

	/// <summary>
	/// 求两个向量，每个轴的最小值
	/// </summary>
	public static Vector3 EachMin(Vector3 a, Vector3 b)
	{
		return new Vector3(Mathf.Min(a.x, b.x)
			, Mathf.Min(a.y, b.y)
			, Mathf.Min(a.z, b.z));
	}

	/// <summary>
	/// 对向量的每个分量<see cref="Mathf.Clamp"/>
	/// </summary>
	public static Vector3 EachClamp(Vector3 a, Vector3 min, Vector3 max)
	{
		return new Vector3(Mathf.Clamp(a.x, min.x, max.x)
			, Mathf.Clamp(a.y, min.y, max.y)
			, Mathf.Clamp(a.z, min.z, max.z));
	}

	/// <summary>
	/// 两个向量，每个分量相乘
	/// </summary>
	public static Vector3 EachMulti(Vector3 a, Vector3 b)
	{
		return new Vector3(a.x * b.x
			, a.y * b.y
			, a.z * b.z);
	}

	/// <summary>
	/// <see cref="Mathf.CeilToInt"/>
	/// </summary>
	public static Vector3 Clamp(Vector3 vec, Vector3 min, Vector3 max)
	{
		vec.x = Mathf.Clamp(vec.x, min.x, max.x);
		vec.y = Mathf.Clamp(vec.y, min.y, max.y);
		vec.z = Mathf.Clamp(vec.z, min.z, max.z);
		return vec;
	}

	/// <summary>
	/// Bounds的对角线，等价于：
	///		能把bounds包围起来的最小的球体的直径
	/// </summary>
	public static float CaculateDiagonal(Bounds bounds)
	{
		return (bounds.max - bounds.min).magnitude;
	}

	/// <summary>
	/// 计算Bounds的最长边
	/// </summary>
	public static float CaculateLongestSide(Bounds bounds)
	{
		return Max(EachAbs(bounds.max - bounds.min));
	}

	/// <summary>
	/// 计算Bounds的最短边
	/// </summary>
	public static float CaculateShortestSide(Bounds bounds)
	{
		return Min(EachAbs(bounds.max - bounds.min));
	}
}