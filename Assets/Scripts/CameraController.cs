using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private int Boundary = 50;
    [SerializeField]
    private int speed = 5;
    [SerializeField]
    private int edgesPosition = 9;
    
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
        if (this.transform.position.x <= -edgesPosition)
        {
            this.transform.position = new Vector3(-edgesPosition , this.transform.position.y, this.transform.position.z); // Left Edge
        }
        else if (this.transform.position.x >= edgesPosition)
        {
            this.transform.position = new Vector3(edgesPosition , this.transform.position.y, this.transform.position.z); // Right Edge
        }
        if (Input.mousePosition.x > theScreenWidth - Boundary || Input.GetKey(KeyCode.RightArrow))
        {
            this.transform.position = new Vector3(this.transform.position.x + speed * Time.deltaTime, this.transform.position.y, this.transform.position.z); // move on +X axis
        }
        if (Input.mousePosition.x < 0 + Boundary ||  Input.GetKey(KeyCode.LeftArrow))
        {
            this.transform.position = new Vector3(this.transform.position.x - speed * Time.deltaTime, this.transform.position.y, this.transform.position.z); // move on -X axis
        }
    }
}
