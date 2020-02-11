//-------------------------------------------
//
//  GameDefinitions
//
//-------------------------------------------
using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace N1D.App
{
	public enum NoteType : int
	{
		Default = 0,
		Long = 1,
	}

	[Serializable]
	public struct NoteSetting
	{
		public NoteType type;
		public int time;
	}
	[Serializable]
	public class NoteSettingArray : Malee.ReorderableArray<NoteSetting> { }

#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(NoteSetting))]
	public class NoteSettingInspector : PropertyDrawer
	{
		private struct NoteSettingProperty
		{
			public SerializedProperty type;
			public SerializedProperty time;
		}

		private NoteSettingProperty Capture(SerializedProperty property)
		{
			NoteSettingProperty capture;
			capture.type = property.FindPropertyRelative("type");
			capture.time = property.FindPropertyRelative("time");

			return capture;
		}
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			label = EditorGUI.BeginProperty(position, label, property);

			var capture = Capture(property);

			var rect = position;
			EditorGUI.LabelField(rect, "note");
			rect.x = EditorGUIUtility.labelWidth;
			rect = ToRight(rect, Mathf.Clamp(rect.width * 0.3f, 100.0f, 150.0f));
			EditorGUI.PropertyField(rect, capture.type, GUIContent.none);
			rect.x += rect.width;
			rect = ToRight(rect, Mathf.Clamp(rect.width * 0.3f, 100.0f, 150.0f), 0.0f);
			EditorGUI.PropertyField(rect, capture.time, GUIContent.none);

			EditorGUI.EndProperty();
		}
		
		private Rect ToRight(Rect position, float width, float margin = 5.0f)
		{
			position.x += margin;
			position.width = width;
			return position;
		}
	}


#endif


} // N1D.App