using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackManager : MonoBehaviour
{
    public GameObject[] tracks;

    void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            tracks[0].SetActive(true);
            tracks[1].SetActive(false);
            tracks[2].SetActive(false);
        }
        if (Input.GetKeyDown("2"))
        {
            tracks[0].SetActive(false);
            tracks[1].SetActive(true);
            tracks[2].SetActive(false);
        }
        if (Input.GetKeyDown("3"))
        {
            tracks[0].SetActive(false);
            tracks[1].SetActive(false);
            tracks[2].SetActive(true);
        }
    }// method to change between tracks
}
