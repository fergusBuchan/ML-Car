using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RuleBased : MonoBehaviour
{

    public float angle;

    CarController carControls;
    Vector2 inputVector;

    void Awake()
    {
        carControls = GetComponent<CarController>();
    }

    void Update()
    { 

        RaycastHit2D[] rays = carControls.GetRayCast();

        if (Mathf.Abs(rays[0].distance - rays[1].distance) > 1) {// if there wall is closer to the left or right
            if (rays[0].distance > rays[1].distance)
            {
                inputVector.x = -0.2f;
                inputVector.y = 0.1f;
            }
            else
            {
                inputVector.x = 0.2f;
                inputVector.y = 0.1f; 
            }
        }
        else// when there are no walls to our left or right
        {
            inputVector.x = 0.0f;
            inputVector.y = 0.2f;
        }



        carControls.SetInputVector(inputVector);
    }
}
