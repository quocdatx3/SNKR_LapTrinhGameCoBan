using System;
using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public static AudioManager instance;

	[SerializeField] private Sound[] musics = null;
	[SerializeField] private Sound[] sounds = null;
	[SerializeField] private AudioMixerGroup musicMixerGroup = null;
	[SerializeField] private AudioMixerGroup soundMixerGroup = null;
	private Sound curMusicPlaying;

	void Awake()
	{
		if (instance == null) { instance = this; }

		foreach (Sound s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			
			s.source.clip = s.clip;
			s.source.loop = s.loop;
			s.source.playOnAwake = false;

			s.source.outputAudioMixerGroup = soundMixerGroup;
		}

		foreach(Sound m in musics)
        {
			m.source = gameObject.AddComponent<AudioSource>();

			m.source.clip = m.clip;
			m.source.loop = m.loop;
			m.source.playOnAwake = false;

			m.source.outputAudioMixerGroup = musicMixerGroup;
		}
	}

    private void Start()
    {
		musicMixerGroup.audioMixer.SetFloat("Volume", PlayerPrefs.GetFloat("Volume_music"));
		soundMixerGroup.audioMixer.SetFloat("Volume", PlayerPrefs.GetFloat("Volume_sound"));

		string name = "BGM" + UnityEngine.Random.Range(0, musics.Length);
		Play(name);
	}

	private void Update()
    {
		//auto next song
        if (!curMusicPlaying.source.isPlaying)
		{
			string name = "BGM" + UnityEngine.Random.Range(0, musics.Length);
			Play(name);
        }
    }

    public void Play(string sound)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			s = Array.Find(musics, item => item.name == sound);
			if (s == null)
			{
				Debug.LogWarning("Sound: " + name + " not found!");
				return;
			}
			curMusicPlaying = s;
		}

		s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
		s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

		s.source.Play();
	}

	public void Stop(string sound)
    {
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			s = Array.Find(musics, item => item.name == sound);
			if (s == null)
			{
				Debug.LogWarning("Sound: " + name + " not found!");
				return;
			}
		}

		s.source.Stop();
	}

	public AudioMixerGroup GetMixerGroup(Sound.MixerGroup group)
    {
		return group == Sound.MixerGroup.music ? musicMixerGroup : soundMixerGroup;
    }
}
