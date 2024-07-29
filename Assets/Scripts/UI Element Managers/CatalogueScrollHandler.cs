using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CatalogueScrollHandler : MonoBehaviour
{
    [SerializeField] private GameObject catalogueButtonPrefab;
    [SerializeField] private GameObject buttonContainer;


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject catalogueButton = Instantiate(catalogueButtonPrefab, buttonContainer.transform);
            TextMeshProUGUI buttonLabel = catalogueButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonLabel.text = i.ToString();
        }
    }
}
