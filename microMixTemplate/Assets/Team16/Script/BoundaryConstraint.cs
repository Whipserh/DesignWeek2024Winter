using UnityEngine;

public class BoundaryConstraint : MonoBehaviour
{
    public Transform target;
    public GameObject boundary; 

    private Vector2 boundaryMin; 
    private Vector2 boundaryMax; 

    void Start()
    {
        // Calculate the boundary coordinates
        CalculateBoundary();
    }

    void LateUpdate()
    {
        // Ensure that the target stays within the boundary
        Vector3 constrainedPosition = target.position;
        constrainedPosition.x = Mathf.Clamp(constrainedPosition.x, boundaryMin.x, boundaryMax.x);
        constrainedPosition.y = Mathf.Clamp(constrainedPosition.y, boundaryMin.y, boundaryMax.y);
        target.position = constrainedPosition;
    }

    // Calculate the boundary coordinates based on the boundary GameObject's position and scale
    private void CalculateBoundary()
    {
        // Get the boundary's position and scale
        Vector3 boundaryPosition = boundary.transform.position;
        Vector3 boundaryScale = boundary.transform.localScale;

        // Calculate the minimum and maximum boundary coordinates
        boundaryMin = boundaryPosition - boundaryScale / 2f;
        boundaryMax = boundaryPosition + boundaryScale / 2f;
    }

    // Draw the boundary gizmos for visualization in the editor
    private void OnDrawGizmosSelected()
    {
        // Calculate the boundary coordinates if they are not already set
        if (boundaryMin == Vector2.zero && boundaryMax == Vector2.zero && boundary != null)
        {
            CalculateBoundary();
        }

        // Draw the boundary gizmos
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((boundaryMin + boundaryMax) / 2f, boundaryMax - boundaryMin);
    }
}
