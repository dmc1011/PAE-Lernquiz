using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatisticManager : MonoBehaviour
{
    [SerializeField] private GameObject catalogueStatisticPanel;
    [SerializeField] private GameObject dailyTaskStatisticPanel;
    [SerializeField] private TextMeshProUGUI catalogueNameDisplay;
    [SerializeField] private TextMeshProUGUI catalogueLevelDisplay;
    [SerializeField] private TextMeshProUGUI timeValuesDisplay;

    private int currentRunDuration = 0;
    private int averageRunDuration = 0;
    private int totalDuration = 0;
    public static bool isDailyTaskStatistic;

    // Start is called before the first frame update
    void Start()
    {
        if (isDailyTaskStatistic)
        {
            catalogueStatisticPanel.gameObject.SetActive(false);
            dailyTaskStatisticPanel.gameObject.SetActive(true);
        }
        else 
        {
            dailyTaskStatisticPanel.gameObject.SetActive(false);
            catalogueNameDisplay.text = Global.CurrentQuestionRound.catalogue.name;
            //to do: set catalogue level as text
            timeValuesDisplay.text = $"{currentRunDuration}\n\n{averageRunDuration}\n\n{totalDuration}";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
