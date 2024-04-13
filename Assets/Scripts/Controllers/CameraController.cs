using UnityEngine;
using Mirror;

public class CameraController : NetworkBehaviour
{
    [SerializeField]
    private int Boundary = 50;
    [SerializeField]
    private float initialSpeed = 5;
    [SerializeField]
    private float maxSpeed = 20f;
    [SerializeField]
    private float acceleration = 0.5f;
    [SerializeField]
    private int edgesPosition = 9;

    private int theScreenWidth;
    private int theScreenHeight;

    private float currentSpeed;

    void Start()
    {
        if (!isLocalPlayer) // Ensure this script only runs for the local player's camera
        {
            enabled = false;
            return;
        }

        theScreenWidth = Screen.width;
        theScreenHeight = Screen.height;
        currentSpeed = initialSpeed;
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        bool movingRight = (Input.mousePosition.x > theScreenWidth - Boundary || Input.GetKey(KeyCode.RightArrow));
        bool movingLeft = (Input.mousePosition.x < 0 + Boundary || Input.GetKey(KeyCode.LeftArrow));

        // Adjust the speed based on whether the player is moving
        if (movingRight || movingLeft)
        {
            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, initialSpeed, maxSpeed);
        }
        else
        {
            currentSpeed = initialSpeed;
        }

        // Ensure the camera stays within the bounds
        if (this.transform.position.x <= -edgesPosition)
        {
            this.transform.position = new Vector3(-edgesPosition, this.transform.position.y, this.transform.position.z); // Left Edge
        }
        else if (this.transform.position.x >= edgesPosition)
        {
            this.transform.position = new Vector3(edgesPosition, this.transform.position.y, this.transform.position.z); // Right Edge
        }

        // Move the camera based on input and current speed
        if (movingRight)
        {
            this.transform.position += Vector3.right * currentSpeed * Time.deltaTime; // move on +X axis
        }
        else if (movingLeft)
        {
            this.transform.position -= Vector3.right * currentSpeed * Time.deltaTime; // move on -X axis
        }
    }
}
