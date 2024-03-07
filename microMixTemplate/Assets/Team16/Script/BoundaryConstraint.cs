using UnityEngine;

public class BoundaryConstraint : MicrogameInputEvents
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

    // Check if a position is within the boundaries
    // Check if a position is within the boundaries defined by the scale of the GameObject
    public bool IsWithinBoundaries(Vector3 position)
    {
        // Get the scale of the GameObject
        Vector3 scale = transform.localScale;

        // Check if the position is within the boundaries defined by the scale
        return Mathf.Abs(position.x) <= scale.x / 2f &&
               Mathf.Abs(position.y) <= scale.y / 2f &&
               Mathf.Abs(position.z) <= scale.z / 2f;
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
