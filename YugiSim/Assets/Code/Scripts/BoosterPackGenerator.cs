using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoosterPackGenerator : MonoBehaviour
{
    private void SetCardImage(GameObject card, string imageName)
    {
        Transform front = card.transform.Find("Front");
        Renderer frontMaterial = front.GetComponent<Renderer>();
        Texture2D frontTexture = Resources.Load<Texture2D>(imageName);

        frontMaterial.material.mainTexture = frontTexture;
    }

    private string PickRarityPool(List<Tuple<string, float>> ratios)
    {
        float randomCeiling = 0;

        foreach(Tuple<string, float> ratio in ratios)
        {
            randomCeiling += ratio.Item2;
        }

        float randomNumber = UnityEngine.Random.Range(0f, randomCeiling);
        string selectedRarity = null;

        foreach (Tuple<string, float> ratio in ratios) 
        {
            randomNumber -= ratio.Item2;

            if (randomNumber <= 0)
            {
                selectedRarity = ratio.Item1;
                if (ratio.Item1 == "SP")
                    Debug.Log("Short print was picked");
                break;
            }
        }

        return selectedRarity;
    }

    private List<Tuple<string, float>> BuildRatiosByPackIndex(int index)
    {
        List<Tuple<string, float>> ratios = new List<Tuple<string, float>>();

        if (index < 8)
        {
            ratios.Add(new Tuple<string, float>("SP", 0.05f));
            ratios.Add(new Tuple<string, float>("C", 0.95f));
        }
        else
        {
            ratios.Add(new Tuple<string, float>("R", 0.90f));
            ratios.Add(new Tuple<string, float>("SR", 0.6f));
            ratios.Add(new Tuple<string, float>("UR", 0.03f));
            ratios.Add(new Tuple<string, float>("ScR", 0.01f));
        }

        return ratios;
    }

    private void GenerateBoosterPack(List<GameObject> cards, int cardQuantity)
    {
        Dictionary<string, List<int>> cardIdsByRarity = DBManager.GetCardsFromSet("LOB-EN");
        List<int> pickedCards = new List<int>();

        for (int i = 0; i < cardQuantity; i++)
        {
            string rarityChosen = PickRarityPool(BuildRatiosByPackIndex(i));
            int totalCardQuantity = cardIdsByRarity[rarityChosen].Count;
            int pickedCard = cardIdsByRarity[rarityChosen][UnityEngine.Random.Range(0, totalCardQuantity)];
            pickedCards.Add(pickedCard);
            SetCardImage(cards[i], pickedCard.ToString());
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        List<GameObject> cards = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name.StartsWith("Card")).ToList();
        GenerateBoosterPack(cards, 9);
    }
}
