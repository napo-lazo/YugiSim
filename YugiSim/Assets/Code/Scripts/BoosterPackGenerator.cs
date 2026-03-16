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

    private void GenerateBoosterPack(List<GameObject> cards, int cardQuantity)
    {
        List<int> cardIds = DBManager.GetCardsFromSet("LOB-EN");
        List<int> pickedCards = new List<int>();
        int totalCardQuantity = cardIds.Count;

        for (int i = 0; i < cardQuantity; i++)
        {
            int pickedCard = cardIds[Random.Range(0, totalCardQuantity)];
            pickedCards.Add(pickedCard);
            SetCardImage(cards[i], pickedCard.ToString());
            Debug.Log(pickedCard);
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        List<GameObject> cards = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name.StartsWith("Card")).ToList();
        GenerateBoosterPack(cards, 9);
    }
}
