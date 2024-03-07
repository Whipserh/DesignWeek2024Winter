using UnityEngine;

namespace team16
{
    public class GameManager : MicrogameInputEvents
    {
        public GameObject secretEndingPrefab;
        public GameObject goodEndingPrefab;
        public GameObject badEndingPrefab;

        public AudioClip badEnding;
        public AudioClip goodEnding;
        public AudioClip secretEnding;

        private AudioSource audioSource;
        private bool gameEnded = false;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>(); // Assuming the AudioSource is attached to the same GameObject as the GameManager
        }

        public void TriggerHappyOwnerEnding()
        {
            if (!gameEnded)
            {
                ReportGameCompletedEarly();
                gameEnded = true;
                // Display happy owner ending prefab
                Instantiate(secretEndingPrefab);
                // Play sound effect
                audioSource.PlayOneShot(secretEnding);
            }
        }

        public void TriggerChaosEnding()
        {
            if (!gameEnded)
            {
                //ReportGameCompletedEarly();
                gameEnded = true;
                // Display chaos ending prefab
                Instantiate(goodEndingPrefab);
                // Play sound effect
                audioSource.PlayOneShot(goodEnding);
            }
        }

        public void TriggerIWillBeBackEnding()
        {
            if (!gameEnded)
            {
                ReportGameCompletedEarly();
                gameEnded = true;
                // Display I will be back ending prefab
                Instantiate(badEndingPrefab);
                // Play sound effect
                audioSource.PlayOneShot(badEnding);
            }
        }
    }
}
