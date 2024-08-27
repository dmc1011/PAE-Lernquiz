using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class HelpManager : MonoBehaviour
{
    [SerializeField] private Button[] questionButtons = new Button[20];
    [SerializeField] private SideMenu sideMenuPanel; 
    [SerializeField] private TMP_Text sideMenuText;
    [SerializeField] private TMP_Text sideMenuHeader;
    [SerializeField] private Transform scrollTransform;
    [SerializeField] private GameObject evaluationButtonPrefab;
    private List<string> answerList = new List<string>();
    private List<string> questionList = new List<string>();

    void Start()
    {
        questionList.Add("Wie funktioniert der Random Quiz-Modus und wie kann ich ihn spielen?");
        questionList.Add("Was ist der Evaluation Screen und welche Informationen finde ich dort?");
        questionList.Add("Wie kann ich die Ergebnisse eines abgeschlossenen Quizzes oder einer Daily Task einsehen?");
        questionList.Add("Was ist das Levelsystem und wie sammle ich Punkte, um im Level aufzusteigen?");
        questionList.Add("Welche Vorteile erhalte ich beim Erreichen höherer Level?");
        questionList.Add("Wie kann ich meine Statistiken einsehen und was zeigen sie an?");
        questionList.Add("Was bedeutet die „Richtigkeitsrate“ in meinen Statistiken?");
        questionList.Add("Wie vergleiche ich meine Daily Task-Richtigkeitsrate mit meinen anderen Leistungen?");
        questionList.Add("Was ist der Frageneditor und wie kann ich damit eigene Fragenkataloge erstellen?");
        questionList.Add("Wie importiere ich Fragenkataloge in die App?");
        questionList.Add("Kann ich Fragen aus meinen eigenen Katalogen auch in Daily Tasks oder Random Quizzes verwenden?");
        questionList.Add("Wie speichere ich Quizfragen, die ich später erneut durchgehen möchte?");
        questionList.Add("Was sind die Achievements und wie kann ich sie freischalten?");
        questionList.Add("Wie kann ich den Fortschritt meiner Achievements verfolgen?");
        questionList.Add("Wie funktioniert das Übungsbuch und wie verwende ich es am besten?");
        questionList.Add("Wie erreiche ich die „Flawless“ Achievements und was bedeuten sie?");
        questionList.Add("Kann ich sehen, wie viele Punkte mir noch fehlen, um ein höheres Level zu erreichen?");
        questionList.Add("Was passiert, wenn ich eine Frage aus dem Übungsbuch dreimal richtig beantworte?");
        questionList.Add("Wie wird die Zeit berechnet, die ich in den Katalogen verbracht habe?");
        questionList.Add("Wie funktioniert das Zeitmanagement-Feature und wie kann ich davon profitieren?");


        answerList.Add("Der Random Quiz-Modus wählt zufällig sechs Fragen aus einem ausgewählten Fragenkatalog aus. Um diesen Modus zu spielen, wähle im Hauptmenü den 'Random Quiz Modus', wähle deinen gewünschten Fragenkatalog aus und beginne das Quiz. Du musst alle Fragen beantworten, um die Runde abzuschließen.");
        answerList.Add("Der Evaluation Screen zeigt die Ergebnisse deines gerade abgeschlossenen Quizzes oder Daily Tasks an. Du kannst dort die richtigen und falschen Antworten einsehen, sowie deine Erfolgsquote und die Anzahl der richtigen Antworten vergleichen.");
        answerList.Add("Nach dem Abschluss eines Quizzes oder einer Daily Task gelangst du automatisch zum Evaluation Screen. Dort kannst du die Ergebnisse überprüfen und bei Bedarf einzelne Fragen noch einmal durchsehen.");
        answerList.Add("Das Levelsystem basiert auf den Punkten, die du durch das Beantworten von Fragen und das Abschließen von Aufgaben verdienst. Je mehr Punkte du sammelst, desto höher steigst du im Level auf. Punkte erhältst du durch erfolgreiche Quizzes, Daily Tasks und andere Aktivitäten innerhalb der App.");
        answerList.Add("Höhere Levels bieten dir verschiedene Vorteile, wie Zugang zu speziellen Features, exklusive Achievements oder zusätzliche Funktionen. Die genauen Vorteile werden dir angezeigt, sobald du ein neues Level erreichst.");
        answerList.Add("Der Statistik Screen zeigt detaillierte Statistiken über deine Leistungen an, einschließlich deiner Richtigkeitsrate in Daily Tasks, die Genauigkeit in den verschiedenen Katalogen und andere Leistungsdaten. Du kannst diese Statistiken nutzen, um deine Fortschritte zu überwachen und gezielt an deinen Schwächen zu arbeiten.");
        answerList.Add("Die „Richtigkeitsrate“ zeigt den Prozentsatz der korrekt beantworteten Fragen in einem bestimmten Bereich oder über einen bestimmten Zeitraum hinweg an. Sie gibt dir einen Überblick über deine Leistung und Genauigkeit.");
        answerList.Add("Im Statistik Screen kannst du die Richtigkeitsrate deiner Daily Tasks mit anderen Statistiken vergleichen. Hier siehst du, wie gut du bei Daily Tasks im Vergleich zu anderen Quiz-Modi oder Katalogen abschneidest.");
        answerList.Add("Der Frageneditor ermöglicht es dir, eigene Fragenkataloge zu erstellen oder bestehende Kataloge zu importieren. Du kannst neue Fragen hinzufügen, bestehende bearbeiten oder löschen, und deine Kataloge nach deinen Bedürfnissen anpassen.");
        answerList.Add("Um Fragenkataloge zu importieren, gehe zum Frageneditor und wähle die Option zum Importieren von Katalogen. Befolge die Anweisungen, um deine Kataloge hochzuladen und in der App verfügbar zu machen.");
        answerList.Add("Ja, Fragen aus deinen eigenen Katalogen können in Daily Tasks und Random Quizzes verwendet werden, sofern sie korrekt in der App integriert sind. Du kannst auswählen, ob du deine eigenen Kataloge für diese Modi nutzen möchtest.");
        answerList.Add("Quizfragen, die du später erneut durchgehen möchtest, kannst du im Übungsbuch speichern. Wenn du eine Frage falsch beantwortest oder manuell speicherst, wird sie in diesem Bereich aufgeführt, bis du sie erfolgreich beantwortest.");
        answerList.Add("Achievements sind besondere Erfolge, die du durch bestimmte Leistungen in der App freischalten kannst. Dazu gehören das Erreichen bestimmter Punktzahlen, das Abschließen von Aufgaben ohne Fehler oder das Erreichen hoher Levels. Du kannst die Achievements im entsprechenden Bereich der App einsehen und verfolgen.");
        answerList.Add("Deinen Fortschritt bei Achievements kannst du im Abschnitt „Achievements“ der App verfolgen. Dort siehst du, welche Achievements du bereits freigeschaltet hast und welche Anforderungen noch erfüllt werden müssen.");
        answerList.Add("Das Übungsbuch speichert alle Fragen, die du falsch beantwortet hast oder manuell gespeichert hast, damit du sie später erneut durchgehen kannst. Um das Beste aus dem Übungsbuch herauszuholen, solltest du regelmäßig die dort gespeicherten Fragen üben, bis du sie korrekt beantwortest.");
        answerList.Add("„Flawless“ Achievements werden erreicht, wenn du einen Fragenkatalog oder eine Quiz-Runde ohne Fehler abschließt. Diese Erfolge zeigen, dass du bestimmte Bereiche perfekt beherrschst. Die genauen Anforderungen sind in der Achievements-Liste der App aufgeführt.");
        answerList.Add("Ja, im Level-Bereich der App kannst du sehen, wie viele Punkte dir noch fehlen, um das nächste Level zu erreichen. Dort findest du auch eine Übersicht über die benötigten Punkte für jedes Level.");
        answerList.Add("Wenn du eine Frage aus dem Übungsbuch dreimal hintereinander richtig beantwortest, wird diese Frage automatisch aus dem Übungsbuch entfernt. Dies zeigt an, dass du die Frage erfolgreich gemeistert hast.");
        answerList.Add("Die Zeit, die du in den Katalogen verbringst, wird durch die Dauer gemessen, in der du aktiv Fragen beantwortest oder durch den Katalog navigierst. Diese Zeit wird in den Statistiken aufgezeichnet und angezeigt.");
        answerList.Add("Das Zeitmanagement-Feature zeigt dir, wie viel Zeit du insgesamt in verschiedenen Katalogen verbracht hast. Du kannst diese Informationen nutzen, um deinen Lernaufwand besser zu planen und gezielt an Bereichen zu arbeiten, in denen du mehr Zeit investieren möchtest.");

        float startY = 1000;
        float yOffset = -200;

        for (int i = 0; i < questionList.Count; i++)
        {
            GameObject newButton = Instantiate(evaluationButtonPrefab, scrollTransform);
            
            TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = questionList[i];
            }

            float newYPosition = startY + (i * yOffset); // Sequentially reduce y by 200 for each button

            RectTransform buttonRectTransform = newButton.GetComponent<RectTransform>();
            if (buttonRectTransform != null)
            {
                buttonRectTransform.anchoredPosition = new Vector2(buttonRectTransform.anchoredPosition.x, newYPosition);
            }
            
            int buttonIndex = i; 
            newButton.GetComponent<Button>().onClick.AddListener(() => HelpButtonPressed(buttonIndex));
        }

    }

    public void HelpButtonPressed(int buttonIndex)
    {
        if (buttonIndex >= 0 && buttonIndex < answerList.Count)
        {
            string answerToShow = answerList[buttonIndex];
            string questionToShow = questionList[buttonIndex];
            UpdateSideMenu(answerToShow, questionToShow);
        }
        else
        {
            Debug.LogWarning("Button index out of range.");
        }
    }

    private void UpdateSideMenu(string answer, string question)
    {
        if (sideMenuText != null)
        {
            sideMenuText.text = answer;
            sideMenuHeader.text = question;
        }
        if (sideMenuPanel != null)
        {
            sideMenuPanel.ToggleMenu();
        }
    }
}
