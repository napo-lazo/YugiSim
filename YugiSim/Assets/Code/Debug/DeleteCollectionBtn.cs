using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeleteCollectionBtn : MonoBehaviour
{
    public Button btn;

    // Start is called before the first frame update
    void Start()
    {
        btn.onClick.AddListener(DBManager.DeleteCollection);
    }
}
