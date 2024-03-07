using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

namespace team16
{
    public class CatMovement : MicrogameInputEvents
    {
        public GameManager gameManager;
        public SoundManager soundManager;

        private AudioSource audioSource;

        public Rigidbody catPaw;
        public float pawSpeed = 5f;

        public float swipeDistance = 1f;
        public float swipeDuration = 0.5f;
        public float swipeSpeed = 5f;
        public float swipeAngle = 60f;

        public Vector3 idlePosition; // Desired idle position

        public float whackValue = 10f;

        public AudioClip[] swipeSounds; // Array of swipe sound effects

        // Reference to the trail Renderer
        public TrailRenderer trailRenderer;

        // Reference to the Particle System
        public ParticleSystem swipeParticle;

        // Private variables
        private bool hasMadeContactWithTag0 = false;
        private bool isTimeUp = false;

        private Quaternion initialRotation;
        private float resetDuration = 0.5f;

        private bool canSwipe = true;

        // Called when the microgame starts
        protected override void OnGameStart()
        {
            // Initialize variables
            hasMadeContactWithTag0 = false;
            isTimeUp = false;

            initialRotation = catPaw.rotation;

            // Get the AudioSource component
            audioSource = soundManager.GetComponent<AudioSource>();
        }

        // Called when time is up in the microgame
        protected override void OnTimesUp()
        {
            isTimeUp = true;

            audioSource.Stop();

            bool outsideCameraView = AreAllTag0ObjectsOutsideCameraView();

            if (hasMadeContactWithTag0)
            {
                if (!outsideCameraView)
                {
                    gameManager.TriggerIWillBeBackEnding();
                }
            }
            else
            {
                gameManager.TriggerHappyOwnerEnding();
            }
        }

        // Called every frame
        private void Update()
        {
            // Get input direction from WASD keys and set cat paw velocity
            Vector2 direction = stick.normalized;
            if (direction != Vector2.zero)
            {
                // If there's input, move the cat in the input direction
                catPaw.velocity = direction * pawSpeed;
            }
            else
            {
                // If there's no input, move the cat to the idle position
                Vector3 directionToIdle = idlePosition - transform.position;
                catPaw.velocity = directionToIdle.normalized * pawSpeed;
            }

            bool outsideCameraView = AreAllTag0ObjectsOutsideCameraView();

            if (hasMadeContactWithTag0)
                if (outsideCameraView)
                {
                    gameManager.TriggerChaosEnding();
                    if (ending)
                    {
                        audioSource.Stop();
                        ReportGameCompletedEarly();
                        ending = false;
                    }
                }
        }
        bool ending = true;
        // Called when button 1 is pressed
        protected override void OnButton1Pressed(InputAction.CallbackContext context)
        {
            // Check if the cat can swipe
            if (canSwipe)
            {
                // Swipe left
                StartCoroutine(Swipe(swipeAngle, swipeDuration, swipeSpeed));
                PlayRandomSwipeSound();
            }
        }

        

        // Called when button 2 is pressed
        protected override void OnButton2Pressed(InputAction.CallbackContext context)
        {
            // Check if the cat can swipe
            if (canSwipe)
            {
                // Swipe right
                StartCoroutine(Swipe(-swipeAngle, swipeDuration, swipeSpeed));
                PlayRandomSwipeSound();
            }
        }

        // Coroutine for swipe motion
        IEnumerator Swipe(float angle, float duration, float speed)
        {
            // Disable swiping during cooldown
            canSwipe = false;

            // Disable Trail Renderer
            if (trailRenderer != null)
            {
                trailRenderer.enabled = false;
            }

            // Instantiate Particle System
            if (swipeParticle != null)
            {
                //ParticleSystem particleInstance = Instantiate(swipeParticle, catPaw.position, Quaternion.identity);
                swipeParticle.Play();
            }

            //DynamicTextManager.CreateText2D(catPaw.position, "Mew", DynamicTextManager.defaultData);

            Quaternion startRotation = catPaw.rotation;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            float startTime = Time.time;

            while (Time.time < startTime + duration)
            {
                float t = (Time.time - startTime) / duration;
                float step = speed * Time.deltaTime;
                catPaw.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
                yield return null;
            }

            catPaw.rotation = targetRotation;

            // Rotate back to initial rotation
            StartCoroutine(RotateBack(initialRotation, resetDuration, speed));

            // Enable Trail Renderer
            if (trailRenderer != null)
            {
                trailRenderer.enabled = true;
            }

            // Start cooldown
            StartCoroutine(SwipeCooldown());
        }

        // Coroutine to rotate back to the initial rotation
        IEnumerator RotateBack(Quaternion targetRotation, float duration, float speed)
        {
            Quaternion startRotation = catPaw.rotation;
            float startTime = Time.time;

            while (Time.time < startTime + duration)
            {
                float t = (Time.time - startTime) / duration;
                float step = speed * Time.deltaTime;
                catPaw.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
                yield return null;
            }

            catPaw.rotation = targetRotation;
        }

        // Coroutine to control swipe cooldown
        IEnumerator SwipeCooldown()
        {
            // Wait for cooldown duration
            yield return new WaitForSeconds(swipeDuration);

            // Enable swiping
            canSwipe = true;
        }

        // Play a random swipe sound from the array
        private void PlayRandomSwipeSound()
        {
            if (swipeSounds.Length > 0)
            {
                int randomIndex = Random.Range(0, swipeSounds.Length);
                AudioSource.PlayClipAtPoint(swipeSounds[randomIndex], transform.position);
            }
        }

        // Check if all Tag0 objects are outside camera view
        private bool AreAllTag0ObjectsOutsideCameraView()
        {
            GameObject[] objectsWithTag0 = GameObject.FindGameObjectsWithTag("Tag0");
            Camera mainCamera = Camera.main;

            foreach (var obj in objectsWithTag0)
            {
                Vector3 viewportPoint = mainCamera.WorldToViewportPoint(obj.transform.position);
                if (viewportPoint.z > 0 && viewportPoint.x > 0 && viewportPoint.x < 1 && viewportPoint.y > 0 && viewportPoint.y < 1)
                {
                    return false; // Object is inside camera view
                }
            }

            return true; // All objects are outside camera view
        }

        // OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Tag0"))
            {
                hasMadeContactWithTag0 = true;
            }
        }
    }
}
