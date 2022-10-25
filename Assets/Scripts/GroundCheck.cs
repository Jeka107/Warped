using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float distance;
    [SerializeField] private float groundLineWidth;
    [SerializeField] private int groundLineResolution;
    [SerializeField] private float groundCheckRadius;

    private bool grounded = false;

    private void Update()
    {
        DrawNoiseArea();
    }

    //drawing spehre around the player for better understanding if player is on ground.
    private void DrawNoiseArea()
    {
        if (groundCheckRadius > 0)
        {
            float angle = 0;

            for (int i = 0; i < groundLineResolution; i++)
            {
                angle = i * (360 / groundLineResolution);

                float x = transform.position.x + groundCheckRadius * Mathf.Sin(angle * Mathf.Deg2Rad);
                float y = transform.position.z + groundCheckRadius * Mathf.Cos(angle * Mathf.Deg2Rad);

                Vector3 vertexPos = new Vector3(x, transform.position.y, y);

                Debug.DrawRay(vertexPos, Vector3.down, Color.green);
                Debug.DrawRay(transform.position, Vector3.down, Color.blue);

                if (Physics.Raycast(vertexPos, -transform.up, distance, whatIsGround)||Physics.Raycast(transform.position, -transform.up, distance, whatIsGround))
                {
                    grounded = true;
                }
                else
                { 
                    grounded = false;
                }
            }
        }
    }
    public bool GetGrounded()
    {
        return grounded;
    }
}
