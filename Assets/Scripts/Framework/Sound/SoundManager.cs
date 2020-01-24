using UnityEngine;
using UnityEngine.Audio;
using N1D.Framework.Core;

namespace N1D.Framework.Sound
{
	public class AudioHandler
	{
		public AudioHandler(AudioSource source)
		{
			Debug.Assert(source != null);
			m_Source = source;
		}

		public bool IsPlaying()
		{
			return m_Source.isPlaying;
		}

		private AudioSource m_Source = null;
	}

	public class SoundManager : Singleton<SoundManager>
	{

		void Awake()
		{
			m_Bgm = gameObject.AddComponent<AudioSource>();
			m_Bgm.outputAudioMixerGroup = m_BgmMixerGroup;
			m_Se = new AudioSource[SeCount];
			for (var i = 0; i < m_Se.Length; ++i)
			{
				m_Se[i] = gameObject.AddComponent<AudioSource>();
				m_Se[i].outputAudioMixerGroup = m_SeMixerGroup;
			}
			m_NextSePlayIndex = 0;

		}

		public void PlayBGM(string bgmClipName)
		{
			var clip = SoundTable.instance.FindBGM(bgmClipName);
			if (clip == null)
			{
				Debug.LogWarningFormat("{0} is not found.", bgmClipName);
				return;
			}

			m_Bgm.clip = clip;
			m_Bgm.loop = true;
			m_Bgm.Play();
		}
		public void StopBGM()
		{
			m_Bgm.Stop();
		}

		public AudioHandler PlaySE(string seClipName)
		{
			var clip = SoundTable.instance.FindSE(seClipName);
			if (clip == null)
			{
				Debug.LogWarningFormat("{0} is not found.", seClipName);
				return null;
			}

			var slot = m_Se[m_NextSePlayIndex];
			slot.PlayOneShot(clip);
			Debug.LogFormat("Play SE -> {0}", seClipName);

			m_NextSePlayIndex = (m_NextSePlayIndex + 1) % m_Se.Length;

			return new AudioHandler(slot);
		}
		public void StopSEAll()
		{
			foreach (var se in m_Se)
			{
				se.Stop();
			}
		}


		public float volumeBgm
		{
			get
			{
				return (m_Bgm == null) ? 0.0f : m_Bgm.volume;
			}
			set
			{
				if (m_Bgm == null)
				{
					return;
				}
				m_Bgm.volume = value;
			}
		}

		public float volumeSe
		{
			get
			{
				return (m_Se == null || m_Se[0] == null) ? 0.0f : m_Se[0].volume;
			}
			set
			{
				if (m_Se == null)
				{
					return;
				}
				foreach (var se in m_Se)
				{
					se.volume = value;
				}
			}
		}


		private const int SeCount = 4;
		private AudioSource m_Bgm = null;
		private AudioSource[] m_Se = null;
		private int m_NextSePlayIndex = 0;

		[SerializeField]
		private AudioMixer m_Mixer = null;
		[SerializeField]
		private AudioMixerGroup m_MasterMixerGroup = null;
		[SerializeField]
		private AudioMixerGroup m_BgmMixerGroup = null;
		[SerializeField]
		private AudioMixerGroup m_SeMixerGroup = null;
	}
} // N1D.Framework.Sound