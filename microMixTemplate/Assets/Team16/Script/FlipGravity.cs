using UnityEngine;
namespace team16
{
    public class FlipGravity : MicrogameInputEvents
    {
        // Boolean to track if gravity is currently flipped
        private bool gravityFlipped = false;

        // Start is called before the first frame update
        void Start()
        {
            // Set gravity to default along the z-axis
            //Physics.gravity = new Vector3(0, 0, 9.81f);
        }
    }
}