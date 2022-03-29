using UnityEngine.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public List<Sound> sounds;

    private void Awake()
    {
        foreach(Sound s in sounds)
        {
            s.source=gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;

            s.source.loop = s.isInLoop;
        }
    }

    private void Start()
    {
        Play("Theme");
    }

    public void Play (string name)
    {
        Sound s=sounds.Find(x => x.name == name);

        if (s == null)
        {
            Debug.LogWarning($"Nincs ilyen hang: {name}");
            return;
        }

        s.source.Play();
    }
}
