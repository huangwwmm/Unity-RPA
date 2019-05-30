using System;
using UnityEditor;
using UnityEngine;

public struct hwmEditorPrefsItem<T>
{
	internal bool _AUTO_SAVE;
	internal string _PREFS_KEY;

	private object m_Value;
	private bool m_Changed;
	private ValueType m_ValueType;

	public T GetValue()
	{
		return (T)m_Value;
	}

	public void SetValue(T value)
	{
		m_Changed |= !m_Value.Equals(value);
		m_Value = value;

		if (_AUTO_SAVE)
		{
			_SaveValue();
		}
	}

	internal void _Initizlize(string prefsKey, bool autoSave = true)
	{
		_AUTO_SAVE = autoSave;
		_PREFS_KEY = prefsKey;
		Type type = typeof(T);
		if (type == typeof(string))
		{
			m_ValueType = ValueType.String;
		}
		else if (type == typeof(float))
		{
			m_ValueType = ValueType.Float;
		}
		else if (type == typeof(bool))
		{
			m_ValueType = ValueType.Bool;
		}
		else if (type == typeof(int))
		{
			m_ValueType = ValueType.Int;
		}
		else
		{
			Debug.LogError(string.Format("ValueItem not support value type({0})", type));
		}

		_LoadValue();
	}

	internal void _LoadValue()
	{
		switch (m_ValueType)
		{
			case ValueType.String:
				m_Value = EditorPrefs.GetString(_PREFS_KEY, "");
				break;
			case ValueType.Float:
				m_Value = EditorPrefs.GetFloat(_PREFS_KEY, 0);
				break;
			case ValueType.Bool:
				m_Value = EditorPrefs.GetBool(_PREFS_KEY, false);
				break;
			case ValueType.Int:
				m_Value = EditorPrefs.GetInt(_PREFS_KEY, 0);
				break;
			default:
				Debug.LogError(string.Format("unresolved _LoadValue ValueType({0})", m_ValueType));
				break;
		}
		m_Changed = false;
	}

	internal void _SaveValue()
	{
		if (!m_Changed)
		{
			return;
		}

		switch (m_ValueType)
		{
			case ValueType.String:
				EditorPrefs.SetString(_PREFS_KEY, (string)m_Value);
				break;
			case ValueType.Float:
				EditorPrefs.SetFloat(_PREFS_KEY, (float)m_Value);
				break;
			case ValueType.Bool:
				EditorPrefs.SetBool(_PREFS_KEY, (bool)m_Value);
				break;
			case ValueType.Int:
				EditorPrefs.SetInt(_PREFS_KEY, (int)m_Value);
				break;
			default:
				Debug.LogError(string.Format("unresolved _SaveValue ValueType({0})", m_ValueType));
				break;
		}
		m_Changed = false;
	}

	private enum ValueType
	{
		String,
		Float,
		Bool,
		Int,
	}
}