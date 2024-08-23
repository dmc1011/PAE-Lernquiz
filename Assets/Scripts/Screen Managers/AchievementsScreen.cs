using System.Collections.Generic;
using UnityEngine;

public class AchievementsScreen : MonoBehaviour
{

    [SerializeField] private Transform scrollTransform;
    [SerializeField] private AchievementPopup achievement;

    void Start()
    {
        List<string> names = new List<string>
        {
            "I Bims 1 Achievement",
            "AFK",
            "John Johnsons John",
            "Beispiel nummer drei",
            "Du bist super",
            "Level 3",
            "5 Minuten der Stille",
            "Pubertär",
            "Maschine",
            "Spätaufsteher"
        };

        List<string> texts = new List<string>
        {
            "Du hast alles gegeben. Weiter so",
            "Das Spiel war 20 Minuten länger online als du",
            "Frag nicht warum. Es ist die Wahrheit",
            "Das ist kein Beispiel, sondern mein Ernst!",
            "Alles ist relativ, auch die Anzahl richtiger Antworten",
            "Hier könnte Ihre Werbung stehen.",
            "Misinterpretation ausgeschlossen. Nein, witklich.",
            "Beherrsche mehrere verbotene Techniken, auch den verbotenen Kick!",
            "Legenden besagen, man solle schlafende Monster nicht wecken"
        };

        List<AchievementPopup.Grade> grades = new List<AchievementPopup.Grade>
        {
            AchievementPopup.Grade.None,
            AchievementPopup.Grade.Bronze,
            AchievementPopup.Grade.Silver,
            AchievementPopup.Grade.Gold,
        };

        for (int i = 0; i < 50; i++)
        {
            AchievementPopup entry = Instantiate(achievement, scrollTransform);
            entry.SetData(
                grades[Random.Range(0, grades.Count)],
                names[Random.Range(0, names.Count)],
                texts[Random.Range(0, texts.Count)]
            );
        }
    }

    void Update()
    {
        
    }
}
