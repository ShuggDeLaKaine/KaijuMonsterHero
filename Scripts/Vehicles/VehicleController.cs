using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VehicleController : MonoBehaviour
{
    public SpawnManager sm;
    public StatManager stm;
    public UIManager ui;
    PlayerManager pm;

    //various to required components and child game objects.
    public Transform carTransform;
    public GameObject destructionFX;
    public AudioSource destructionAudio;
    public GameObject crashDetect;          //reference to GameObject child to avoid crashes.
    public GameObject playerDetect;
    public Rigidbody carRB;

    //bools on whether car is parked and which direction they are driving in.
    public bool isParked = false;
    public bool drivingLeft = false;
    public bool drivingRight = false;
    public bool drivingUp = false;
    public bool drivingDown = false;

    //floats for car speed and whether car is braking.
    public float carSpeed = 0.2f;
    public Vector2 carSpeedMinMax;
    public float slowSpeed = 0.05f;
    public bool isBraking = false;

    //misc vars, score points etc.
    private float yOffset = 1.0f;           //offset to be applied to FX instaniation so it slightly raised off ground.
    public int destructionScore;
    public float specialScore;


    void Start()
    {
        InitialiseObject();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isParked) { 
            VehicleDrive();
        }
    }

    //Function to initialise all that needs initialising for the object.
    void InitialiseObject()
    {
        carRB = GetComponent<Rigidbody>();             //attaching Rigidbody component to object.
        carTransform = GetComponent<Transform>();      //attaching Transform component to object.

        pm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        //sm = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>();
        stm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<StatManager>();
        ui = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();

        carSpeedMinMax = new Vector2(0.20f, 0.35f);
        carSpeed = Random.Range(carSpeedMinMax.x, carSpeedMinMax.y);

        VehicleDirection();
        //isDestroyed = Animator.StringToHash("isDestroyed"); //setting animator bool isDestroyed from string to int, creates less garbage.
    }

    //
    void VehicleDirection()
    {
        if (carTransform.rotation.y < 10.0f) {
            drivingLeft = true;
        } else if (carTransform.rotation.y > 160.0f && carTransform.rotation.y < 200.0f) {
            drivingRight = true;
        } else if (carTransform.rotation.y > 70.0f && carTransform.rotation.y < 110.0f) {
            drivingUp = true;
        } else if (carTransform.rotation.y > 250 && carTransform.rotation.y < 290.0f) {
            drivingDown = true;
        }
    }

    //function to move the cars.
    void VehicleDrive()
    {
        //had issue with cars moving during pause, this prevents.
        if(ui.isPaused == false)
        { 
            carTransform.Translate(0.0f, 0.0f, carSpeed);
        }
        else
        {
            carTransform.Translate(0.0f, 0.0f, 0.0f);
        }
    }

    //OnCollisionEnter to check whether the object has collided with the players collision box.
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            DestroyThisVehicle();
        }
    }

    //
    public void DestroyThisVehicle()
    {
        //send this objects destruction score, special and stats to the StatManager.
        stm.currentScore += destructionScore;
        stm.vehiclesDestroyed++;
        pm.playerCurrentSpecial += specialScore;
        //get FXs from its object pool and bring into scene.
        sm.SpawnFXs(sm.carExFXPool, sm.selectedCarExFX, gameObject);
        //get SFX from its object pool and bring into scene.
        sm.SpawnFXs(sm.sfxVehDesPool, sm.selectedSFXVehDes, gameObject);
        //deactivate and pop back into vehicle pool.
        sm.BackToPool(sm.vehiclePool, sm.vehiclePoolPosition, gameObject);
    }

    //
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "ClockwiseTurn") {
            if (drivingLeft) {
                drivingLeft = false;
                drivingUp = true;
                carTransform.Rotate(Vector3.up, 90.0f, Space.Self);
                NewCarSpeed();
            } else if (drivingUp) {
                drivingUp = false;
                drivingRight = true;
                carTransform.Rotate(Vector3.up, -270.0f, Space.Self);
                NewCarSpeed();
            } else if (drivingRight) {
                drivingRight = false;
                drivingDown = true;
                carTransform.Rotate(Vector3.up, 90.0f, Space.Self);
                NewCarSpeed();
            } else if (drivingDown) {
                drivingDown = false;
                drivingLeft = true;
                carTransform.Rotate(Vector3.up, 90.0f, Space.Self);
                NewCarSpeed();
            }
        }

        if (collision.gameObject.tag == "AntiClockwiseTurn") {
            if (drivingLeft) {
                drivingLeft = false;
                drivingDown = true;
                carTransform.Rotate(Vector3.up, -90.0f, Space.Self);
                NewCarSpeed();
            } else if (drivingUp) {
                drivingUp = false;
                drivingLeft = true;
                carTransform.Rotate(Vector3.up, 270.0f, Space.Self);
                NewCarSpeed();
            } else if (drivingRight) {
                drivingRight = false;
                drivingUp = true;
                carTransform.Rotate(Vector3.up, -90.0f, Space.Self);
                NewCarSpeed();
            } else if (drivingDown) {
                drivingDown = false;
                drivingRight = true;
                carTransform.Rotate(Vector3.up, 270.0f, Space.Self);
                NewCarSpeed();
            }
        }

        if (collision.gameObject.tag == "Player")
        {
            carSpeed = 0.0f;
            isParked = true;
            //Instaniate person at spawn point on car. 
        }
    }

    //Func to set new carSpeed using the carSpeedMin-Max random range.
    private void NewCarSpeed()
    {
        carSpeed = Random.Range(carSpeedMinMax.x, carSpeedMinMax.y);
    }

    //VEHICLES: Whilst car detect trigger collider is colliding with vehicle collider.
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Vehicle")
        {
            //carRB.isKinematic = true;
            isBraking = true;
            carSpeed = slowSpeed;
            //make the other car speed up a bit, otherwise they all lock into same speeds.
            other.gameObject.GetComponent<VehicleController>().carSpeed += 0.1f;
        }
    }

    //VEHICLES: On leaving close contact with another car, pick a new speed and speed up.
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Vehicle")
        {
            //carRB.isKinematic = false;
            isBraking = false;
            float newSpeed;
            newSpeed = Random.Range((slowSpeed + 0.05f), (slowSpeed + 0.15f));
            carSpeed = newSpeed;
        }
    }


}
