using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

namespace team16
{
    public class CatMovement : MicrogameInputEvents
    {
        // Public variables
        public GameManager gameManager;
        public SoundManager soundManager;

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
            bool madeContact = CheckContactWithObjects();
            bool outsideBoundaries = CheckObjectsOutsideBoundaries();

            Debug.Log("Contact with objects: " + madeContact);
            Debug.Log("Objects outside boundaries: " + outsideBoundaries);

            // Trigger appropriate ending based on contact and boundaries
            if (madeContact)
            {
                if (!outsideBoundaries)
                {
                    gameManager.TriggerIWillBeBackEnding();
                }
                else
                {
                    gameManager.TriggerChaosEnding();
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

        // Check if the cat paw collides with any object
        // Check if the cat paw collides with any object tagged as "Tag0"
        private bool CheckContactWithObjects()
        {
            Collider[] colliders = Physics.OverlapSphere(catPaw.position, swipeDistance);
            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Tag0"))
                {
                    Debug.Log("Contact with object tagged as 'Tag0' detected.");
                    return true;
                }
            }
            Debug.Log("No contact with objects tagged as 'Tag0'.");
            return false;
        }


        // Check if all Tag0 objects are outside boundary constraints
        private bool CheckObjectsOutsideBoundaries()
        {
            GameObject boundary = GameObject.FindGameObjectWithTag("Tag10");
            if (boundary != null)
            {
                bool isOutsideBoundaries = !boundary.GetComponent<BoundaryConstraint>().IsWithinBoundaries(catPaw.position);
                Debug.Log("Objects outside boundaries: " + isOutsideBoundaries);
                return isOutsideBoundaries;
            }
            Debug.Log("No boundary object found.");
            return false;
        }

    }
}
