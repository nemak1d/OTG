//-------------------------------------------
//
//  ScriptableObjectWizard
//
//-------------------------------------------
using UnityEngine;
using UnityEditor;

namespace N1D.Framework.Editor
{
    public class ScriptableObjectWizard : ScriptableWizard
    {
        //-----------------------------------
        // MonoBehaviour
        //-----------------------------------
		private void OnWizardCreate()
		{
			if (m_CreateType == ScriptableObjectType.None)
			{
				Debug.LogWarning("生成対象のScriptableObjectTypeを指定してください.");
				return;
			}

			var path = EditorUtility.SaveFilePanelInProject("Save", m_CreateType.ToString(), "asset", "ファイル名を入力して保存先を選択してください");
			if (string.IsNullOrEmpty(path))
			{
				return;
			}

			switch (m_CreateType)
			{
				case ScriptableObjectType.TrackSheet:
				{
					CreateAsset<App.TrackSheet>(path);
					break;
				}

				default:
				{
					break;
				}
			}
		}
		//-----------------------------------
		// Method (private)
		//-----------------------------------
		[MenuItem("Tools/Create Scriptable Object")]
		private static void Open()
		{
			GetWindow<ScriptableObjectWizard>("Create Scriptable Object Wizard");
		}

		private static void CreateAsset<T>(string path) where T : ScriptableObject
		{
			Debug.Assert(!string.IsNullOrEmpty(path));

			T instance = CreateInstance<T>();

			AssetDatabase.CreateAsset(instance, path);
			AssetDatabase.SaveAssets();
		}
		//-----------------------------------
		// Property
		//-----------------------------------

		//-----------------------------------
		// Define
		//-----------------------------------
		public enum ScriptableObjectType : int
		{
			None = 0,
			TrackSheet,
		}

		//-----------------------------------
		// Field
		//-----------------------------------
		[SerializeField]
		private ScriptableObjectType m_CreateType = ScriptableObjectType.None;

        //-----------------------------------
        // Internal Class / Struct
        //-----------------------------------
    }
} // N1D.Framework.Editor