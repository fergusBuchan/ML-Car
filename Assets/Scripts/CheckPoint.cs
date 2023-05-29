using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    // Start is called before the first frame update

    public int thisPoint;
    int totalPionts = 3;

    int winners;

    void Start()
    {
        winners = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space") || Input.GetKeyDown("1") || Input.GetKeyDown("2") || Input.GetKeyDown("3"))
        {
            winners = 0;
        }
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        CarController controller = collider.gameObject.GetComponent<CarController>();


        if (controller.lastCheckPoint == 2 && thisPoint == 3)
        {

            if (winners < 2)
            {

                winners++;
                if (collider.gameObject.GetComponent<AIInput>() != null)
                {
                    Debug.Log("best times this generation :" + controller.timer);
                    collider.gameObject.GetComponent<AIInput>().parent = true;
                }
                if (collider.gameObject.GetComponent<RuleBased>() != null)
                {
                    Debug.Log("rule system time: " + controller.timer);
                }
            }
        }

        if (controller.lastCheckPoint == thisPoint - 1)
        {
            controller.lastCheckPoint = thisPoint;
        }

    } // keeps track of what checkpiot we're on, so the AI goes all the way around the track
}
