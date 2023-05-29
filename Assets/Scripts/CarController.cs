using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("RaceCar Settings")]
    public float driftFactor = 10.0f;
    public float acceleration = 20.0f;
    public float turnSpeed = 60.0f;
    public float maxSpeed = 10.0f;
    public float watcher = 0.0f;

    public float[] rayCastsWatch;

    public float accelerationInput;
    public float steeringInput;

    public float distance;

    public int lastCheckPoint;

    public float timer;
    public float lapTime;

    public bool crashed;

    bool forwards;

    Vector2 previousLoc;
    Vector2 startingPos;

    float roationAngle;

    Rigidbody2D body;

    

    void Awake()
    {
        lastCheckPoint = 0;
        crashed = false;
        distance = 0.0f;
        acceleration = 20.0f;
        turnSpeed = 60.0f;
        lapTime = 999.0f;
        timer = 0.0f;
        body = GetComponent<Rigidbody2D>();
        startingPos = body.position;
        previousLoc = body.position;
    }


    public void reset()
    {
        body.bodyType = RigidbodyType2D.Dynamic;

        crashed = false;

        gameObject.layer = LayerMask.NameToLayer("vehicle");

        body.velocity = new Vector3(0, 0, 0);
        body.angularVelocity = 0.0f;
        distance = 0.0f;
        lapTime = 999.0f;
        timer = 0.0f;
        transform.position = startingPos;
        body.SetRotation(270.0f);
        previousLoc = startingPos;
        lastCheckPoint = 0;
    }


    // Update is called once per frame
    void Update()
    {

    }

    // this update is called even when there ins't a frame
    void FixedUpdate()
    {
        timer += Time.deltaTime;

        Vector2 xyDisplacement = body.position - previousLoc;
        previousLoc = body.position;
        distance += xyDisplacement.magnitude;

        body.drag = getDrag();

        if (body.velocity.magnitude < maxSpeed)
        {
            body.AddForce(getLinearForce(), ForceMode2D.Force);
        }

        if (Vector2.Dot(body.velocity, transform.up) < 0.5)// if we are travelling forwards, using a dot product from the forward vector 
        {
            forwards = false;
        }
        else
        {
            forwards = true;
        }

        body.angularDrag = getRotationalDrag();
        body.AddTorque(getRotaionalForce(), ForceMode2D.Force);
    }

    void OnCollisionEnter2D(Collision2D collider)
    {
        GameObject gameObject = collider.gameObject;
        if (gameObject.layer == LayerMask.NameToLayer("terrain"))
        {
            body.bodyType = RigidbodyType2D.Static;
            crashed = true;
        }
    }

    float getDrag()
    {
        if (Mathf.Abs(accelerationInput) < 0.2f) // if the accelerator is off, slow down
        {
            return 0.5f;
        }

        else if ( (forwards && (accelerationInput > 0) ) || (!forwards && (accelerationInput < 0)) ) // if the car is moving in the same direction as the user wants it to
        {
            return 0.0f;
        }

        else // if the user is trying to go in the oppiste direction we turn on breaks
        {
            return 0.9f;
        }
    }

    Vector2 getLinearForce()
    {
        Vector2 rightVelocity = transform.right * Vector2.Dot(body.velocity, transform.right) * driftFactor * (-1.0f) ; // force to represent cars move forwards easier than sideways

        Vector2 forwardsVelocity;
        if (accelerationInput > 0) {
            forwardsVelocity = transform.up * accelerationInput * acceleration;
        }
        else
        {
            forwardsVelocity = transform.up * accelerationInput * (acceleration / 5);
        }
        return rightVelocity + forwardsVelocity;
    }


    float getRotationalDrag() // you  turn faster when you move faster
    {
        if (Mathf.Abs(steeringInput) < 0.2f) {
            return 99.0f;
        }
        else
        {
            return 0.0f;
        }
    }
    
    float getRotaionalForce()
    {
        float force = 0.0f;

        float turnFactor = body.velocity.magnitude * turnSpeed; // car turns more when you're going faster
        turnFactor = Mathf.Clamp(turnFactor, 0.0f, turnSpeed);

        if (forwards)// if we are travelling forwards
        {
            force = steeringInput * turnFactor * (body.velocity.magnitude) * (-1.0f); // change the direction of spin (cars going backwards turn backwards)
        }
        else
        {
            force = steeringInput * turnFactor * (body.velocity.magnitude) ;
        }
        
        return force - body.angularVelocity; // <- car  will add momentum until it reaches desired velocity
    }

    public void SetInputVector(Vector2 inputVector)
    {
        steeringInput = Mathf.Clamp(inputVector.x, -1.0f, 1.0f);
        accelerationInput = Mathf.Clamp(inputVector.y, -1.0f, 1.0f);
    }

    public RaycastHit2D [] GetRayCast()// < - this is here for the AI
    {


        float[] hitResults = { 0.0f,0.0f,0.0f,0.0f,0.0f,0.0f };
        Vector2 digonalRight = rotateVector(transform.up, 0.46364761f);//<- that's 26 degrees
        Vector2 digonalLeft = rotateVector(transform.up, -0.46364761f);

        RaycastHit2D[] hits = new RaycastHit2D[6];

        float maxDistance = 30.0f;

        int layerMask = LayerMask.GetMask("vehicle") | LayerMask.GetMask("selected") | LayerMask.GetMask("gameLogic"); // the raycast should not interact with other cars

        hits[0] = Physics2D.Raycast(transform.position, digonalRight * 10.0f, 50.0f, ~layerMask); // front right
        hits[1] = Physics2D.Raycast(transform.position, digonalLeft * 10.0f, 50.0f, ~layerMask); // front left
        hits[2] = Physics2D.Raycast(transform.position, transform.right * 10.0f, 50.0f, ~layerMask); // right
        hits[3] = Physics2D.Raycast(transform.position, -transform.right * 10.0f, 50.0f, ~layerMask); // left
        hits[4] = Physics2D.Raycast(transform.position, -digonalRight * 10.0f, 50.0f, ~layerMask); // back right
        hits[5] = Physics2D.Raycast(transform.position, -digonalLeft * 10.0f, 50.0f, ~layerMask); // back left

        return hits;
    }

    public Vector2 rotateVector(Vector2 inVec, float angle)
    {
        Vector2 outVec = new Vector2(
            Mathf.Cos(angle) * inVec.x - Mathf.Sin(angle) * inVec.y,
            Mathf.Sin(angle) * inVec.x + Mathf.Cos(angle) * inVec.y
            );
        return outVec;
    }

    public void stopClock()
    {
        lapTime = timer;
    }

    public Vector2 getPosition()
    {
        return body.position;
    }

    public Vector2 getRight()
    {
        return transform.right;
    }
}
