using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
	public class AudioManager : MonoBehaviour
	{
		public Sound[] sounds;

		public bool muted;

		public bool muteMusic;

		private float musicVolume;
		private float soundVolume;

		public static AudioManager Instance { get; set; }

		void Awake()
		{
			Instance = this;

			//musicVolume = PlayerPrefs.GetFloat("music");
			//soundVolume = PlayerPrefs.GetFloat("sound");

			foreach (Sound s in sounds)
			{
				s.source = gameObject.AddComponent<AudioSource>();
				s.source.clip = s.clip;

				s.source.loop = s.loop;
				if (s.volume == 0)
					s.source.volume = 0.5f;
				else s.source.volume = s.volume;
				if (s.pitch == 0)
					s.source.pitch = 1f;
				else s.source.pitch = s.pitch;
			}

			Play("Song");
			SetVolume(0.5f);
			SetAllVolume(0.5f);
		}

		public void MuteSounds()
		{
			muted = !muted;
		}

		public void MuteMusic()
		{
			muteMusic = !muteMusic;
			float v;
			if (muteMusic) v = 0f;
			else v = 1f;
			foreach (Sound s in sounds)
			{
				if (s.name == "Song")
				{
					s.source.volume = v;
					return;
				}
			}
		}

		public void SetVolume(float v)
		{
			foreach (Sound s in sounds)
			{
				if (s.name == "Song")
				{
					s.source.volume = v;
					return;
				}
			}
		}

		public void SetAllVolume(float v)
		{
			foreach (Sound s in sounds)
			{
				if (s.name != "Song")
					s.source.volume = v;
			}
		}

		public void UnmuteMusic()
		{
			foreach (Sound s in sounds)
			{
				if (s.name == "Song")
				{
					s.source.volume = 1.15f;
					return;
				}
			}
		}

		public void Play(string n)
		{
			if (muted && n != "Song") return;
			foreach (Sound s in sounds)
			{
				if (s.name == n)
				{
					s.source.Play();
					return;
				}
			}
		}

		public void Stop(string n)
		{
			foreach (Sound s in sounds)
			{
				if (s.name == n)
				{
					s.source.Stop();
					return;
				}
			}
		}

		public void StopAll()
		{
			foreach (Sound s in sounds)
			{
				s.source.Stop();
			}
		}
	}
}