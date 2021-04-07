using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //references to the 3 cameras.
    public Camera mainCamera;
    public Camera longCamera;
    public Camera streetCamera;

    //var for transform of the target the camera is following.
    private Transform camTarget;
    //var for transform of the lookAt object on the player object.
    private Transform lookAtPoint;

    //float for smoothed out camera speed.
    public float smoothSpeed = 0.125f;

    //vector3 to store distance between player and camera
    private Vector3 mainCamOffset;
    private Vector3 longCamOffset;
    private Vector3 streetCamOffset;


    void Start()
    {
        //setting the cameras target to transform position of the game object Player.
        camTarget = GameObject.Find("Player").transform;
        //setting the point the camera will look at the the child object on the player object.
        lookAtPoint = GameObject.Find("LookAtPoint").transform;

        //attaching cameras to this scripts.
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        longCamera = GameObject.FindGameObjectWithTag("LongCamera").GetComponent<Camera>();
        streetCamera = GameObject.FindGameObjectWithTag("StreetCamera").GetComponent<Camera>();

        //setting positions of the cameras.
        mainCamOffset = new Vector3(-40.0f, 60.0f, -60.0f);
        longCamOffset = new Vector3(-20.0f, 50.0f, 40.0f);
        streetCamOffset = new Vector3(0.0f, 1.0f, -70.0f);
        
        //setting main camera enabled and the others to disabled. 
        mainCamera.enabled = true;
        longCamera.enabled = false;
        streetCamera.enabled = false;
    }

    void Update()
    {
        
    }

    //player movement in Update, want camera to follow this so using LateUpdate
    void FixedUpdate()
    {
        CameraUpdates();
    }

    //function to have all three cameras position updating to move with Players movement, with their own offsets.
    void CameraUpdates()
    {
        if (this.tag == "MainCamera")
        {
            CameraPosition(mainCamOffset);
        }
        else if (this.tag == "LongCamera")
        {
            CameraPosition(longCamOffset);
        }
        else if (this.tag == "StreetCamera")
        {
            CameraPosition(streetCamOffset);
        }
    }

    //function setting the position of the camera in relation to the player.
    void CameraPosition(Vector3 position)
    {
        Vector3 desiredPosition = camTarget.position + position;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        this.transform.position = smoothedPosition;
        //keeps the camera looking at it's target. 
        this.transform.LookAt(lookAtPoint);
    }

    //function for a toggle through of cameras.
    //to be used with the UIManager: Switch Camera Button
    public void SwitchCamera()
    {
        if (mainCamera.enabled == true)
        {
            mainCamera.enabled = false;
            longCamera.enabled = true;
            streetCamera.enabled = false;
        } 
        else if (longCamera.enabled == true)
        {
            mainCamera.enabled = false;
            longCamera.enabled = false;
            streetCamera.enabled = true;
        }
        else if (streetCamera.enabled == true)
        {
            mainCamera.enabled = true;
            longCamera.enabled = false;
            streetCamera.enabled = false;
        }
    }
}
