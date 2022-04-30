using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource source;
    [SerializeField]
    private List<AudioClip> typeWriterSounds;

    private void Start()
    {
        source = this.GetComponent<AudioSource>();
    }
    public void PlaySound()
    {
        source.clip = typeWriterSounds[Random.Range(0, typeWriterSounds.Count)];
        source.Play();
    }
}
