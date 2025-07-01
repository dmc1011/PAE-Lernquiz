using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelDashboardManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelDisplay;
    [SerializeField] private TextMeshProUGUI xpDisplay;
    // Start is called before the first frame update
    void Start()
    {
        int currentLevel = PlayerPrefs.GetInt("PlayerLevel");
        int totalXp = PlayerPrefs.GetInt("TotalXp");
        int neededXp = PlayerPrefs.GetInt("XpForNextLevel");

        levelDisplay.text = $"Level {currentLevel}";
        xpDisplay.text =
            $"Erfahrungspunkte:\n" +
            $"{totalXp} XP\n\n" +
            $"Nächstes Level bei:\n" +
            $"{neededXp} XP";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
