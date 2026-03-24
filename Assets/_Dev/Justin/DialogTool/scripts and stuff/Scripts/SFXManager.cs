using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/*
 * Script written by: Justin Leyser
 * Last updated: 01.11.2025
 * Summary of functionality: Plays SFX (fade-in, fade-out), Manages List of AudioSources for SFX, public functions should be called from other scripts
 */

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

	private List<AudioSource> sfxSources = new List<AudioSource>();
	[SerializeField] private AudioMixerGroup sfxMixer;

    [SerializeField] private float fadeInDuration = 0f;
    [SerializeField] private float startVolumeOfSFX = 1f;
    [SerializeField] private float StopFadeoutDuration = 0.2f;

    private void Awake()
	{
		// when we use a scene, that's always open, we dont need to instanciate the SFXManager
		if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        AudioSource sfxSource1 = gameObject.AddComponent<AudioSource>();
        sfxSource1.outputAudioMixerGroup = sfxMixer;
        sfxSources.Add(sfxSource1);
	}


    //Call this Function with the SFX that you want to play!
    //How it works: It cycles throug a list of AudioSources to find a unused AudioSource to play the SFX on.
    //If no unused AudioSource is available, it will add one to the list and play the SFX there.
    public void PlaySFX(AudioClip newClip) 
    {
        StopAllCoroutines();
        for (int i = 0; i < sfxSources.Count; i++)
        {
            if (!sfxSources[i].isPlaying)
            {
				sfxSources[i].clip = newClip;
                StartCoroutine(FadeInCoroutine(sfxSources[i]));
                break;
            }
			if (i == sfxSources.Count - 1)
			{
                AudioSource newSource = gameObject.AddComponent<AudioSource>();
				newSource.outputAudioMixerGroup = sfxMixer;
				StartCoroutine(FadeInCoroutine(newSource));
				sfxSources.Add(newSource);
					
			}
		}
    }

	//Call this Function to fade out all currently playing sfx and enjoy some sfx silence afterwards
	public void StopAllSFX()
	{
		StopAllCoroutines();
		StartCoroutine(StopAllSFXCoroutine());
	}

	// Controls the Fade-in of SFX
	IEnumerator FadeInCoroutine(AudioSource source)
    {
        float timer = 0f;
        source.volume = 0f;
		source.Play();

		while (timer < fadeInDuration)
		{
           
		   source.volume = Mathf.Lerp(startVolumeOfSFX, 1f, timer / fadeInDuration);

		   timer += Time.deltaTime;
           yield return null;
		}

		source.volume = 1;
	}

	//Controls the Fade-out from every currently playing SFX, when StopSFX() is called
	IEnumerator StopAllSFXCoroutine()
    {
        float timer = 0f;
		
        while (timer < StopFadeoutDuration)
        {
			foreach(AudioSource source in sfxSources)
            {
				source.volume = Mathf.Lerp(1f, 0f, timer / StopFadeoutDuration);
			} 
           
			timer += Time.deltaTime;
            yield return null;
		}

		foreach (AudioSource source in sfxSources)
		{
			source.volume = 0f;
            source.Stop();
		}
		
        
	}
}
