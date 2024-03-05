using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

namespace team99
{
    public class CatMovement : MicrogameInputEvents
    {
        // Public variables
        public Rigidbody catPaw;
        public float pawSpeed = 5f;
        public float swipeDistance = 1f;
        public float swipeDuration = 0.5f;
        public LayerMask interactableLayer;
        public Vector2 minBounds;
        public Vector2 maxBounds;
        public float speed = 1.0f;
        public AnimationCurve perlinCurve;
        public float lerpFactor = 1f;

        // Private variables
        private bool isHoldingObject = false;
        private Coroutine movementCoroutine;
        private Vector2 perlinOffset;
        private bool gameStarted = false;

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
            // Start the coroutine for moving the cat with Perlin noise
            movementCoroutine = StartCoroutine(MoveWithPerlinNoise());
        }

        // Called when time is up in the microgame
        protected override void OnTimesUp()
        {
            // Reset the cat's position
            ResetPosition();
        }

        // Coroutine to move the cat with Perlin noise
        IEnumerator MoveWithPerlinNoise()
        {
            while (true)
            {
                // Calculate Perlin noise values for X and Y directions
                float perlinX = Mathf.PerlinNoise(perlinOffset.x + Time.time * speed, 0);
                float perlinY = Mathf.PerlinNoise(0, perlinOffset.y + Time.time * speed);

                // Evaluate the Perlin noise values with the specified curve
                float adjustedPerlinX = perlinCurve.Evaluate(perlinX);
                float adjustedPerlinY = perlinCurve.Evaluate(perlinY);

                // Map the adjusted Perlin noise values to the min and max bounds
                float mappedX = Mathf.Lerp(minBounds.x, maxBounds.x, adjustedPerlinX);
                float mappedY = Mathf.Lerp(minBounds.y, maxBounds.y, adjustedPerlinY);

                // Create the target position for the cat
                Vector3 targetPosition = new Vector3(mappedX, mappedY, transform.position.z);

                // Move the cat towards the target position with lerping
                transform.position += (targetPosition - transform.position) * lerpFactor * Time.deltaTime;

                // Wait for the next frame
                yield return null;
            }
        }

        // Called every frame
        private void Update()
        {
            // Get input direction from WASD keys and set cat paw velocity
            Vector2 direction = stick.normalized;
            catPaw.velocity = direction * pawSpeed;
        }

        // Called when button 1 is pressed
        protected override void OnButton1Pressed(InputAction.CallbackContext context)
        {
            // Swipe left
            StartCoroutine(Swipe(-swipeDistance, swipeDuration));
        }

        // Called when button 2 is pressed
        protected override void OnButton2Pressed(InputAction.CallbackContext context)
        {
            // Swipe right
            StartCoroutine(Swipe(swipeDistance, swipeDuration));
        }

        // Coroutine for swipe motion
        IEnumerator Swipe(float distance, float duration)
        {
            Vector3 startPosition = catPaw.position;
            Vector3 endPosition = startPosition + new Vector3(distance, 0, 0);
            float startTime = Time.time;

            while (Time.time < startTime + duration)
            {
                float t = (Time.time - startTime) / duration;
                catPaw.position = Vector3.Lerp(startPosition, endPosition, t);
                yield return null;
            }

            catPaw.position = endPosition;
        }

        // Reset the cat's position
        private void ResetPosition()
        {
            if (movementCoroutine != null)
            {
                StopCoroutine(movementCoroutine);
            }
            StartCoroutine(LerpBackToCenter());
        }

        // Coroutine to lerp the cat back to the center position
        IEnumerator LerpBackToCenter()
        {
            float elapsedTime = 0;
            Vector3 startPosition = transform.position;
            Vector3 centerPosition = new Vector3((minBounds.x + maxBounds.x) / 2f, (minBounds.y + maxBounds.y) / 2f, transform.position.z);

            while (elapsedTime < 2f)
            {
                transform.position = Vector3.Lerp(startPosition, centerPosition, elapsedTime / 2f);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = centerPosition;
        }
    }
}
