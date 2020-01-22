using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using N1D.Core;

namespace N1D.Sound
{
	public class SoundTable : Singleton<SoundTable>
	{
		public AudioClip FindBGM(string clipName)
		{
			if (m_BgmDictionary == null)
			{
				m_BgmDictionary = BuildTable(m_BgmTable);
			}

			if (m_BgmDictionary == null)
			{
				return null;
			}

			AudioClip result = null;
			if (m_BgmDictionary.TryGetValue(clipName, out result))
			{
				return result;
			}
			return null;
		}
		public AudioClip FindSE(string clipName)
		{
			if (m_SeDictionary == null)
			{
				m_SeDictionary = BuildTable(m_SeTable);
			}

			if (m_SeDictionary == null)
			{
				return null;
			}

			AudioClip result = null;
			if (m_SeDictionary.TryGetValue(clipName, out result))
			{
				return result;
			}
			return null;
		}

		private Dictionary<string, AudioClip> BuildTable(AudioClip[] clips)
		{
			var dictionary = new Dictionary<string, AudioClip>(clips.Length);
			foreach (var clip in clips)
			{
				dictionary.Add(clip.name, clip);
			}
			return dictionary;
		}

		private Dictionary<string, AudioClip> m_BgmDictionary = null;
		private Dictionary<string, AudioClip> m_SeDictionary = null;

		[SerializeField]
		public AudioClip[] m_BgmTable = null;
		[SerializeField]
		public AudioClip[] m_SeTable = null;
	}
} // N1D.Sound