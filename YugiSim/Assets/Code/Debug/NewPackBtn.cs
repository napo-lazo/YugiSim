using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewPackBtn : MonoBehaviour
{
    public Button btn;
    public GameObject boosterGenerator;
    private BoosterPackGenerator generatorScript;

    void Start()
    {
        generatorScript = boosterGenerator.GetComponent<BoosterPackGenerator>();
        btn.onClick.AddListener(delegate { generatorScript.GenerateBoosterPack(generatorScript.cards, 9); });
    }
}
