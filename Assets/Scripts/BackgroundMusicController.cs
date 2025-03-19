using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicController : MonoBehaviour
{
  private AudioSource audioSource;
  void Start()
  {
    audioSource = GetComponent<AudioSource>();
    PlayMusic();
  }
  
  public void PlayMusic()
  {
    if (!audioSource.isPlaying)
    {
      audioSource.Play();
    }
  }
  
  public void StopMusic()
  {
    if (audioSource.isPlaying)
    {
      audioSource.Stop();
    }
  }
} 
