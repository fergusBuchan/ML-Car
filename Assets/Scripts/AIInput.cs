using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInput : MonoBehaviour
{

    [Header("AI Testing")]

    public float tester = 0.0f;

    public float distance;

    public float lapTime;

    public int lastCheckPoint;

    public bool parent;

    public float[] outputWatch = { 6.0f, 9.0f };
    public float[] testInput = { 1.0f, 0.5f, -9.0f };


    public int[] layers = { 4, 4, 2 };

    // layers is the number of nodes in each layer of the network (inc inputs and outputs)
    /*
      [4][4][2]
      [ ][ ][ ]
      [ ][ ][ ]
      [ ][ ]
      [ ][ ]
    */
    // like this


    float[][] nodes;
    float[][,] weights;// it looks a bit funky using 2d and 1d arrays for this but it means there's less clutter
    float[][] constants;



    CarController carControls;

    public float[][,] GetWeights()
    {
        return weights;
    }

    public float[][] GetConstants()
    {
        return constants;
    }

    public void SetWeights(float[][,] newWeights, float[][] newConstatnts)
    {
        for (int i = 0; i < weights.Length; i++)
        {

            for (int j = 0; j < weights[i].GetLength(0); j++)
            {

                for (int k = 0; k < weights[i].GetLength(1); k++)
                {
                    weights[i][j, k] = newWeights[i][j, k];
                }
            }
        }

        for (int i = 1; i < constants.Length; i++)
        {
            for (int j = 0; j < constants[i].Length; j++)
            {
                constants[i][j] = newConstatnts[i][j];
            }
        }
        // have to init weights
    }

    void Awake()
    {

        carControls = GetComponent<CarController>();

        lapTime = 999.9f;
        CreateNuralNet();

        parent = false;
    }
    //awake is called when the car first added to the scene, so it itits some varibales

    public void reset()
    {
        lapTime = 999.9f;
        carControls.reset();
        parent = false;
    }
    //reset is called when a new generation of cars is created

    void Start()
    {
    }
 
    // Update is called once per frame
    void Update()
    { 

        distance = carControls.distance;
        lastCheckPoint = carControls.lastCheckPoint;
        lapTime = carControls.lapTime;

        RaycastHit2D[] rays = carControls.GetRayCast();
        float[] ifInput =  { rays[0].distance, rays[1].distance, rays[2].distance, rays[3].distance };


        float[] networkOutput = litterallyAnIfStatement(ifInput); 
        // get the ray cast from the car and put it thorugh the neural net

        Vector2 inputVector;

        inputVector.x = ( networkOutput[0] * 2 - 1.0f );  // we change the range of the output from  ( 0 / 1 ) -> ( -1 / 1 )
        inputVector.y = ( networkOutput[1] * 2 - 1.0f );

        carControls.SetInputVector(inputVector);
        // returning the results to the car controller

        if (parent)
        {
            GetComponent<SpriteRenderer>().color = Color.green;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
        //this is some code for the car to turn green when it's selected
    }

    public void CreateNuralNet()
    {
        CreateNodes();
        CreateWeights();
    }
    void CreateNodes()
    {
        nodes = new float[layers.Length][];

        for (int i = 0; i < layers.Length; i++)
        {
            nodes[i] = new float[layers[i]];
        }// make new nodes equivilant to the values in layers[]
    }

    void CreateWeights()
    {
        weights = new float[layers.Length - 1][,];

        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = new float[layers[i], layers[i + 1]];

            // FOR every node (exept the last layer), create a number of weights equivilant to the next layer's number of nodes

            for (int j = 0; j < weights[i].GetLength(0); j++)
            {

                for (int k = 0; k < weights[i].GetLength(1); k++)
                {
                    weights[i][j, k] = Random.Range(-0.5f, 0.5f);  //weights are randomised
                }
            }
        }

        constants = new float[layers.Length][];
        constants[0] = new float[] { 0.0f }; // the first layer doesn't need constants, so this is just junk

        for (int i = 1; i < layers.Length; i++)
        {
            constants[i] = new float[layers[i]];

            for (int j = 0; j < constants[i].Length; j++)
            {
                constants[i][j] = Random.Range(-0.5f, 0.5f); //constants are randomised
            }
        }

    }

    public void Propogate(AIInput spouse)
    {
        int geneSize = 0;

        for (int i = 0; i < weights.Length; i++)
        {
            geneSize += weights[i].GetLength(0) * weights[i].GetLength(1);
        }

        for (int i = 0; i < constants.Length; i++)
        {
            geneSize += constants[i].Length;
        }

        int split = Random.Range(0, geneSize);
        // setting half of the weights to be swapped

        int inc = 0;

        for (int i = 0; i < weights.Length; i++)
        {
            // FOR every node (exept the last layer), we swap values up unitl the split

            for (int j = 0; j < weights[i].GetLength(0); j++)
            {

                for (int k = 0; k < weights[i].GetLength(1); k++)
                {
                    if(inc > split)
                    {
                        float w1 = weights[i][j, k];
                        float w2 = spouse.weights[i][j, k];

                        weights[i][j, k] = w2;
                        spouse.weights[i][j, k] = w1;
                    }
                    inc++;
                }
            }
        }

        for (int i = 1; i < constants.Length; i++)
        {
            // same here

            for (int j = 0; j < constants[i].Length; j++)
            {
                if (inc > split)
                {
                    float c1 = constants[i][j];
                    float c2 = spouse.constants[i][j];

                    constants[i][j] = c2;
                    spouse.constants[i][j] = c1;
                }
                inc++;
            }
        }
    }

    public void Mutation(float mutationFactor)
    {
        Mathf.Clamp(mutationFactor, 0.0f, 1.0f);// mutationfactor should be between 0 - 1

        int geneSize = 0;

        for (int i = 0; i < weights.Length; i++) {
            geneSize += weights[i].GetLength(0) * weights[i].GetLength(1);
        }

        for (int i = 0; i < constants.Length; i++)
        {
            geneSize += constants[i].Length;
        }

        bool[] mutations = new bool[geneSize];

        for (int i = 0; i < mutations.Length * mutationFactor; i++)
        {
            mutations[i] = true;
        }
        // making a big array to represent all the wieghts we can mutate

        shuffle(mutations);
        // then we can suffle it so we end up with random muatations

        int inc = 0;

        for (int i = 0; i < weights.Length; i++)
        { 
            // FOR every node (exept the last layer), we make a new gene in random places

            for (int j = 0; j < weights[i].GetLength(0); j++)
            {

                for (int k = 0; k < weights[i].GetLength(1); k++)
                {
                    if (mutations[inc])
                    {
                        weights[i][j, k] = Random.Range(-0.5f, 0.5f);
                    }
                    inc++;
                }
            }
        }

        for (int i = 1; i < constants.Length; i++)
        {
            for (int j = 0; j < constants[i].Length; j++)
            {
                if (mutations[inc])
                {
                    constants[i][j] = Random.Range(-0.5f, 0.5f);
                }
                inc++;
            }
        }

    }

    float[] litterallyAnIfStatement(float[] inputs){
        // IF car is driving in the wrong directions THEN don't do that


        float[] outputs = new float[] { 0.0f };

        if (inputs.Length != nodes[0].Length)
        {
            Debug.LogError("inputs to AI are incompatable with the neural net size");
            // catching when the input does not match the one we are expecting
            return null;
        }
        else
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                nodes[0][i] = inputs[i];
            }
            // we set the first row of nodes to the inputs
            // so we don't need to calculate the first row


            for (int currentLayer = 1; currentLayer < nodes.Length; currentLayer++)
            {
                for (int currentNode = 0; currentNode < nodes[currentLayer].Length; currentNode++)
                {
                    float sumOfSums = 0.0f;
                    for (int prevNode = 0; prevNode < nodes[currentLayer - 1].Length; prevNode++)
                    {
                        sumOfSums += nodes[currentLayer - 1][prevNode] * weights[currentLayer - 1][prevNode, currentNode];
                        
                    }//getting the sum of all previous nodes with thier wieghts relitive to this node

                    nodes[currentLayer][currentNode] = sigmoid(sumOfSums + constants[currentLayer][currentNode]);
                    // sigma
                }
            }

                int lastLayer = nodes.Length - 1;

                outputs = new float[nodes[lastLayer].Length];

                for (int i = 0; i < nodes[lastLayer].Length; i++)
                {
                    outputs[i] = nodes[lastLayer][i];
                }
                //asssining the output as the last layer of the 'net

                outputWatch = outputs;
            return outputs;
        }
    }


    // GENERIC FUNCTIONS ---


    float sigmoid(float input) // quick function, just so code's easier to read
    {
        return( 1 / (1 + Mathf.Exp(-input)) ); // makes everything 0 - 1
    }


    void shuffle(bool[] array)// function to randomise the order of an array
    {
        int CurrentIndex = array.Length;
        for (int i = 0; i < array.Length; i++)
        {
            int swapIndex = Random.Range(0, array.Length);

            bool b1 = array[i];
            bool b2 = array[swapIndex];

            array[i] = b2;
            array[swapIndex] = b1;
        }
    }
}
