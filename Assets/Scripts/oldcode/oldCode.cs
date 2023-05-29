using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oldCode : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
/*
 *  - look, somethimes I forget what I coded and I need to go back
 * /



//void Propogate(AIInput parent1, AIInput parent2)
//{


//    int inc = 0;


//    bool[] changes = new bool[99];

//    for (int i = 0; i < changes.Length; i++)
//    {
//        changes[i] = true;
//    }

//    changes = changes.OrderBy(x => Random.Range(0, changes.Length)).ToArray();
//    // randomised bool list the same length as the seed


//    float[][,] genome1 = parent1.GetWeights();
//    float[][,] genome2 = parent2.GetWeights();


//    //iterate through both genes, then we swap random elements to create unique children

//    for (int layer = 0; layer < genome1.Length; layer++)
//    {
//        for (int node = 0; node < genome1[layer].GetLength(0); node++)
//        {
//            for (int weight = 0; weight < genome1[layer].GetLength(1); weight++)
//            {

//                if (changes[inc])
//                {
//                    float strand1 = genome1[layer][node, weight];
//                    float strand2 = genome2[layer][node, weight];

//                    genome1[layer][node, weight] = strand2;
//                    genome2[layer][node, weight] = strand1;
//                }
//                inc++;
//            }
//        }
//    }

//    parent1.SetWeights(genome1);
//    parent2.SetWeights(genome2);

//    float[][] constantGene1 = parent1.GetConstants();
//    float[][] constantGene2 = parent2.GetConstants();

//    for (int layer = 1; layer < constantGene1.Length; layer++)
//    {
//        for (int node = 0; node < constantGene1[layer].Length; node++)
//        {
//            if (changes[inc])
//            {
//                float constant1 = constantGene1[layer][node];
//                float constant2 = constantGene2[layer][node];

//                constantGene1[layer][node] = constant2;
//                constantGene2[layer][node] = constant1;
//            }
//            inc++;

//            // swap the baises
//        }
//    }

//    parent1.SetConstants(constantGene1);
//    parent2.SetConstants(constantGene2);
//}

//float[][,] WieghtMitosis(AIInput inGenome)
//{
//    float[][,] genome = inGenome.GetWeights();

//    int geneSize = 0;

//    for (int i = 0; i < genome.Length; i++)
//    {
//        geneSize += genome[i].GetLength(0) * genome[i].GetLength(1);
//    }

//    bool[] changes = new bool[geneSize];

//    for (int i = 0; i < changes.Length; i++)
//    {
//        changes[i] = true;
//    }

//    //changes = changes.OrderBy(x => Random.Range(0, changes.Length)).ToArray();

//    int inc = 0;

//    for (int layer = 0; layer < genome.Length; layer++)
//    {
//        for (int node = 0; node < genome[layer].GetLength(0); node++)
//        {
//            for (int weight = 0; weight < genome[layer].GetLength(1); weight++)
//            {

//                //if(changes[inc])
//                //{

//                genome[layer][node, weight] = Random.Range(-0.5f, 0.5f);

//                //}
//                inc++;
//            }
//        }
//    }

//    return genome;
//}

//float[][] ConstantMitosis(AIInput inGenome)
//{
//    float[][] constantGene = inGenome.GetConstants();

//    int geneSize = 0;

//    for (int i = 0; i < constantGene.Length; i++)
//    {
//        geneSize += constantGene[i].Length;
//    }

//    bool[] changes = new bool[geneSize];

//    for (int i = 0; i < changes.Length / 5; i++)
//    {
//        changes[i] = true;
//    }

//    //changes = changes.OrderBy(x => Random.Range(0, changes.Length)).ToArray();

//    int inc = 0;

//    for (int layer = 1; layer < constantGene.Length; layer++)
//    {
//        for (int node = 0; node < constantGene[layer].Length; node++)
//        {

//            //if (changes[inc])
//            //{

//            constantGene[layer][node] = Random.Range(0.5f, -0.5f);

//            //}
//            inc++;

//            // swap the baises
//        }
//    }

//    return constantGene;
//}
//// this is a function to get rid of the worst prefroming gene and replace it with a mutation of the best one, this is so that we don't have the same pairs of parents every generation
///
  public void Fitness()
{
    AIInput[] AIs = new AIInput[cars.Length];

    for (int i = 0; i < cars.Length; i++)
    {
        AIs[i] = cars[i].GetComponent<AIInput>();
    }

    AIs = AIs.OrderBy(x => x.lapTime).ThenByDescending(x => x.distance).ToArray();
    // we order the array to select the best (we conpare how many checkpionts or how far they've gone in total for when the cars don't complete the track)

    float[][,] bestWeights = WieghtMitosis(AIs[0]);
    float[][] bestConstants = ConstantMitosis(AIs[0]);

    for (int i = 0; i < AIs.Length; i += 2)
    {
        Propogate(AIs[i], AIs[i + 1], i);
    }

    //AIs[AIs.Length - 1].SetWeights(bestWeights);
    //AIs[AIs.Length - 1].SetConstants(bestConstants);
    // this is us getting riud of the worst preforming gene with the best one, so that genes will interact with more other genes, and we can get rid of cars that do nothing

    for (int i = 0; i < cars.Length; i++)
    {
        AIs[i].reset();

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

        bool[] swaps = new bool[geneSize];

        for (int i = 0; i < swaps.Length * 0.5; i++)
        {
            swaps[i] = true;
        }
        // setting half of the weights to be swapped

        shuffle(swaps);
        // then we can suffle it so we end up with random swaps

        int inc = 0;

        for (int i = 0; i < weights.Length; i++)
        {
            // FOR every node (exept the last layer), we swap values where the swap array says to 

            for (int j = 0; j < weights[i].GetLength(0); j++)
            {

                for (int k = 0; k < weights[i].GetLength(1); k++)
                {
                    if (swaps[inc])
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
            for (int j = 0; j < constants[i].Length; j++)
            {
                if (swaps[inc])
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

*/
