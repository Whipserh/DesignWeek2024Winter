using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

namespace team16
{
    public class CatMovement : MicrogameInputEvents
    {
        // Public variables
        public GameManager gameManager;
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
        private Vector2 perlinOffset;
        private bool gameStarted = false;
        private Quaternion initialRotation;
        private float resetDuration = 0.5f;
        private bool canSwipe = true; // Indicates whether the cat can swipe

        // Awake is called when the script instance is being loaded
        void Awake()
        {
            // Initialize the Perlin noise offset
            perlinOffset = new Vector2(Random.value * 1000, Random.value * 1000);
        }

        // Called when the microgame starts
        protected override void OnGameStart()
        {
            gameStarted = true;
            initialRotation = catPaw.rotation;
        }

        // Called when time is up in the microgame
        protected override void OnTimesUp()
        {
            // Trigger happy owner ending if time is up and no action was taken
            if (canSwipe)
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
        }

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
                ParticleSystem particleInstance = Instantiate(swipeParticle, catPaw.position, Quaternion.identity);
                particleInstance.Play();
            }

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

        // Called when the cat paw collides with another object
        private void OnCollisionEnter(Collision collision)
        {
            // Check if the collision is with a valid object
            if (collision.gameObject.CompareTag("Tag0"))
            {
                // Apply an impulse force to the collided object
                Rigidbody collidedRigidbody = collision.gameObject.GetComponent<Rigidbody>();
                if (collidedRigidbody != null)
                {
                    // Add an impulse force in the direction of the cat paw's movement
                    Vector3 forceDirection = catPaw.velocity.normalized;
                    collidedRigidbody.AddForce(forceDirection * whackValue, ForceMode.Impulse);
                }

                // Check if all Tag0 objects are outside camera view
                bool allObjectsOutsideCameraView = true;
                GameObject[] tag0Objects = GameObject.FindGameObjectsWithTag("Tag0");
                foreach (GameObject obj in tag0Objects)
                {
                    if (IsObjectInView(obj))
                    {
                        allObjectsOutsideCameraView = false;
                        break;
                    }
                }

                if (allObjectsOutsideCameraView)
                {
                    // Trigger chaos ending
                    ReportGameCompletedEarly();
                    gameManager.TriggerChaosEnding();
                }
            }
        }

        // Check if an object is outside camera view
        private bool IsObjectInView(GameObject obj)
        {
            Vector3 screenPoint = Camera.main.WorldToViewportPoint(obj.transform.position);
            return screenPoint.x < 0 || screenPoint.x > 1 || screenPoint.y < 0 || screenPoint.y > 1;
        }
    }
}
