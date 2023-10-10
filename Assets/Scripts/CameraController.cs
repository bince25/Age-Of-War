using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public int Boundary = 50;
    public int speed = 5;
    private int theScreenWidth;
    private int theScreenHeight;
    void Start()
    {
        theScreenWidth = Screen.width;
	    theScreenHeight = Screen.height;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.mousePosition.x > theScreenWidth - Boundary)
        {
            this.transform.position = new Vector3(this.transform.position.x + speed * Time.deltaTime, this.transform.position.y, this.transform.position.z); // move on +X axis
        }

        if (Input.mousePosition.x < 0 + Boundary)
        {
            this.transform.position = new Vector3(this.transform.position.x - speed * Time.deltaTime, this.transform.position.y, this.transform.position.z); // move on -X axis
        }
    }
}
