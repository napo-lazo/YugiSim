using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoosterPackGenerator : MonoBehaviour
{
    public List<GameObject> cards;

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

    public void GenerateBoosterPack(List<GameObject> cards, int cardQuantity)
    {
        string testSetCode = "LOB-EN";
        Dictionary<string, List<int>> cardIdsByRarity = DBManager.GetCardsFromSet(testSetCode);
        Dictionary<int, int> cardsInCollection = new Dictionary<int, int>();
        Dictionary<int, int> pickedCards = new Dictionary<int, int>();
        Dictionary<int, int> cardsToInsert = new Dictionary<int, int>();
        Dictionary<int, int> cardsToUpdate = new Dictionary<int, int>();
        List<int> cardIds = new List<int>();

        for (int i = 0; i < cardQuantity; i++)
        {
            string rarityChosen = PickRarityPool(BuildRatiosByPackIndex(i));
            int totalCardQuantity = cardIdsByRarity[rarityChosen].Count;
            int pickedCard = cardIdsByRarity[rarityChosen][UnityEngine.Random.Range(0, totalCardQuantity)];

            cardIds.Add(pickedCard);
            if (!pickedCards.ContainsKey(pickedCard))
                pickedCards.Add(pickedCard, 1);
            else
                pickedCards[pickedCard]++;
        }

        cardsInCollection = DBManager.GetListedCardsFromCollectionBySet(testSetCode, pickedCards.Keys.ToList());
        cardsToInsert = pickedCards.Where(x => !cardsInCollection.ContainsKey(x.Key)).ToDictionary(x => x.Key, x => x.Value);
        cardsToUpdate = pickedCards.Where(x => !cardsToInsert.ContainsKey(x.Key)).ToDictionary(x => x.Key, x => x.Value);

        DBManager.InsertCardsIntoCollection(testSetCode, cardsToInsert);

        int quantityCounter = 1;
        while(cardsToUpdate.Count > 0)
        {
            DBManager.UpdateCardsQuantityFromCollection(testSetCode, cardsToUpdate, quantityCounter);
            cardsToUpdate = cardsToUpdate.Where(x => x.Value > quantityCounter).ToDictionary(x => x.Key, x => x.Value);
            quantityCounter++;
        }

        for (int i = 0; i < cardQuantity; i++)
        {
            SetCardImage(cards[i], cardIds[i].ToString());
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        cards = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name.StartsWith("Card")).ToList();
        GenerateBoosterPack(cards, 9);
    }
}
