using UnityEngine;

public class ShowBoundaries : MicrogameInputEvents
{
    void OnDrawGizmosSelected()
    {
        // Draw boundaries using Gizmos
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }

    void Update()
    {
        // Print transform values using Debug.Log
        Debug.Log("Position: " + transform.position);
        Debug.Log("Rotation: " + transform.rotation.eulerAngles);
        Debug.Log("Scale: " + transform.localScale);
    }
}
