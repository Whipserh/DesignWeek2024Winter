using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace team16
{
    public class SoundManager : MicrogameInputEvents
    {
        // Public variable for the audio clip
        public AudioClip musicClip;

        // Private variable for the AudioSource component
        public AudioSource audioSource;

        // Awake is called when the script instance is being loaded
        void Awake()
        {
            // Get the AudioSource component attached to this GameObject
            audioSource = GetComponent<AudioSource>();

            // Assign the music clip to the AudioSource component
            audioSource.clip = musicClip;

            // Ensure that the audio clip loops
            //audioSource.loop = true;
        }

        protected override void OnGameStart()
        {
            // Play the background music
            audioSource.Play();
        }

        // Method to play a sound using the audio source
        public void PlaySound(AudioClip clip)
        {
            if (clip != null && audioSource != null)
            {
                // Play the provided audio clip through the audio source
                audioSource.PlayOneShot(clip);
            }
        }
    }
}

