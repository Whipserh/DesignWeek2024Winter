using UnityEngine;
namespace team16
{
    public class ShowBoundaries : MicrogameInputEvents
    {
        // Define the boundaries of the game area
        public Bounds boundaries;

        void OnDrawGizmosSelected()
        {
            // Draw boundaries using Gizmos
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }

        void Update()
        {
            // Print transform values using Debug.Log
        }
    }
}