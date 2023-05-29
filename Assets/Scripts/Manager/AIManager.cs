using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

using System;
using System.IO;

public class AIManager : MonoBehaviour
{
    [Header("Game Settings")]

    public Vector3 startingPosition;

    public GameObject[] cars;
    public GameObject ruleCar;

    [Header("sprites")]
    public Sprite[] sprites;

    // Start is called before the first frame update
    void Start()
    {

        ruleCar = Instantiate(ruleCar, startingPosition, ruleCar.transform.rotation);
        ruleCar.SetActive(false);

        for (int i = 0; i < cars.Length; i++)
        {
            cars[i] = Instantiate(cars[i], startingPosition, cars[i].transform.rotation);
            cars[i].gameObject.GetComponent<SpriteRenderer>().sprite = sprites[i % sprites.Length];

        }
    }

    void Update()
    {
       //this is really inputs from the user --

        if (Input.GetKeyDown("space") || Input.GetKeyDown("1") || Input.GetKeyDown("2") || Input.GetKeyDown("3"))
        {
            Fitness();
            ruleCar.GetComponent<CarController>().reset();
        }// reset cars for new generation

        if (Input.GetMouseButtonDown(0))
        {
            LayerMask layerMask = LayerMask.GetMask("vehicle");

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // need to convert from screenspace to realspace
            RaycastHit2D hit = Physics2D.Raycast(mousePos, mousePos - new Vector3(0.0f,0.0f,-1.0f), 3.0f, layerMask);

            if(hit.collider != null)
            {
                AIInput colliderInfo = hit.collider.gameObject.GetComponent<AIInput>();
                colliderInfo.parent = true;
                hit.collider.gameObject.layer = LayerMask.NameToLayer("selected");
            }
        }//select cars that are to be parents

        if (Input.GetMouseButtonDown(1))
        {
            LayerMask layerMask = LayerMask.GetMask("selected");

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, mousePos - new Vector3(0.0f, 0.0f, -1.0f), 3.0f, layerMask);
 
            if (hit.collider != null)
            {
                AIInput colliderInfo = hit.collider.gameObject.GetComponent<AIInput>();

                colliderInfo.parent = false;
                hit.collider.gameObject.layer = LayerMask.NameToLayer("vehicle");
            }
        }//de - select cars that are to be parents

        if (Input.GetKeyDown("l"))
        {
            loadParents();
        }
        if (Input.GetKeyDown("s"))
        {
            SaveParents();
        }
        //load and save

        if (Input.GetKeyDown("tab"))
        {
            ruleCar.SetActive(!ruleCar.activeSelf);

            for (int i = 0; i < cars.Length; i++)
            {
                cars[i].SetActive(!cars[i].activeSelf);
            }
        }
    }

    void Fitness()
    {
        AIInput[] AIs = new AIInput[cars.Length];

        for (int i = 0; i < cars.Length; i ++ )
        {
            AIs[i] = cars[i].GetComponent<AIInput>();
        }

        List<AIInput> parents = new List<AIInput>();

        for (int i = 0; i < AIs.Length; i++)
        {
            if (AIs[i].parent) {
                parents.Add(AIs[i]);
            }
        }// make list for parents selected by user

        int parentSize = parents.Count;

        if (parentSize > 0) {
            if (parentSize % 2 > 0)
            {
                parents.RemoveAt(parents.Count - 1);
            }// get rid of a parent if there's an odd number

            for (int i = 0; i < parents.Count; i += 2)
            {
                parents[i].Propogate(parents[i + 1]);
            }// make new generation

            for (int i = 0; i < AIs.Length; i++)
            {
                AIs[i].SetWeights(parents[i % parents.Count].GetWeights() , parents[i % parents.Count].GetConstants());
            }// add new generation's offsping, mutated, back into the world
        }

        for (int i = 0; i < cars.Length; i++)
        {
            AIs[i].Mutation(0.05f * i / cars.Length); // <- some cars will mutate more than others
            AIs[i].reset();
        }
    }

    void SaveParents()
    {
        Debug.Log("save");

        AIInput[] AIs = new AIInput[cars.Length];

        for (int i = 0; i < cars.Length; i++)
        {
            AIs[i] = cars[i].GetComponent<AIInput>();
        }

        List<AIInput> parents = new List<AIInput>();
        SaveFile save = new SaveFile();

        for (int i = 0; i < AIs.Length; i++)
        {
            if (AIs[i].parent)
            {
                AISave newData = new AISave();
                newData.weights = AIs[i].GetWeights();
                newData.constants = AIs[i].GetConstants();
                save.parents.Add( newData );
            }
        }

        if (save.parents.Count() > 0)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream file = File.Create(Application.persistentDataPath + "/parents.save");
            formatter.Serialize(file, save);
            file.Close();
        }
    }

    void loadParents()
    {
        Debug.Log("load");

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/parents.save", FileMode.Open);
        SaveFile save = (SaveFile)formatter.Deserialize(file);
        file.Close();

        List<AISave> savedParents = save.parents;
        List <AIInput> parents = new List<AIInput>();

 
        AIInput[] AIs = new AIInput[cars.Length];

        for (int i = 0; i < cars.Length; i++)
        {
            AIs[i] = cars[i].GetComponent<AIInput>();
        }

        if (cars.Length >= savedParents.Count)
        {
            for (int i = 0; i < savedParents.Count; i++)
            {
                AIs[i].SetWeights(savedParents[i].weights, savedParents[i].constants);
                parents.Add(AIs[i]);
            }
        }

        int parentSize = parents.Count;

        if (parentSize > 0)
        {
            if (parentSize % 2 > 0)
            {
                parents.RemoveAt(parents.Count - 1);
            }// get rid of a parent if there's an odd number

            for (int i = 0; i < parents.Count; i += 2)
            {
                parents[i].Propogate(parents[i + 1]);
            }// make new generation

            for (int i = 0; i < AIs.Length; i++)
            {
                AIs[i].SetWeights(parents[i % parents.Count].GetWeights(), parents[i % parents.Count].GetConstants());
            }// add new generation's offsping, mutated, back into the world
        }

        for (int i = 0; i < cars.Length; i++)
        {
            AIs[i].Mutation(0.05f * i / cars.Length); // <- some cars will mutate more than others
            AIs[i].reset();
        }
    }
}
