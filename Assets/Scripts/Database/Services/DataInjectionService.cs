using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Client = Supabase.Client;

public class DataInjectionService : MonoBehaviour
{
    //public static Client _supabase;
    //private DataController _dataController = new DataController();

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public async static void InjectCatalogues(int id, Catalogue catalogue)
    {
        Client _supabase = SupabaseClientProvider.GetClient();

        try 
        {
            var c = new Models.Catalogue
            {
                Name = catalogue.name,
                TopicId = id
            };

            var insert = await _supabase.From<Models.Catalogue>().Insert(c);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public async static void InjectQuestions(int id, Catalogue catalogue)
    {
        Client _supabase = SupabaseClientProvider.GetClient();

        try
        {
            foreach (var question in catalogue.questions)
            {
                var q = new Models.Question
                {
                    CatalogueId = id,
                    Text = question.text,
                    DisplayName = question.name,
                };

                var q_response = await _supabase.From<Models.Question>().Insert(q);
                int insertedQuestionId = q_response.Model.Id;
                Debug.Log("Q ID: " + insertedQuestionId);

                var answers = question.answers.Select(answer => new Models.Answer
                {
                    QuestionId = insertedQuestionId,
                    Text = answer.text,
                    IsCorrect = answer.isCorrect,
                }).ToList();

                var a_response = await _supabase.From<Models.Answer>().Insert(answers);

                Debug.Log($"Inserted {answers.Count} answers for question {insertedQuestionId}");
            }
        }
        catch(Exception e)
        {
            Debug.LogException(e);
        }
    }

    public async static void InjectAchievements()
    {
        Client _supabase = SupabaseClientProvider.GetClient();

        List<Achievement> achievements = new()
        {
            new Achievement("Flawless", AchievementPopup.Grade.Bronze, "Schlie�e zum ersten Mal einen gesamten Katalog ohne einen einzigen Fehler ab", "Du hast zum ersten Mal einen Katalog ohne Fehler abgeschlossen!", false, null),
            new Achievement("Flawless", AchievementPopup.Grade.Silver, "Meistere einen Katalog f�nfmal ohne Fehler", "Du hast einen Katalog f�nfmal ohne Fehler abgeschlossen!", false, null),
            new Achievement("Flawless", AchievementPopup.Grade.Gold, "Erreiche Perfektion, indem du einen Katalog zehnmal fehlerfrei abschlie�t", "Du hast einen Katalog zehnmal ohne Fehler abgeschlossen!", false, null),

            new Achievement("Multitalent", AchievementPopup.Grade.Bronze, "Beweise deine Vielseitigkeit, indem du f�nf Katalog-Durchl�ufe ohne Fehler meisterst", "Du hast f�nf Katalog-Durchl�ufe ohne Fehler abgeschlossen!", false, null),
            new Achievement("Multitalent", AchievementPopup.Grade.Silver, "Zeige deine umfassende Expertise, indem du zehn Katalog-Durchl�ufe fehlerfrei abschlie�t", "Du hast zehn Katalog-Durchl�ufe ohne Fehler abgeschlossen!", false, null),
            new Achievement("Multitalent", AchievementPopup.Grade.Gold, "Demonstriere dein K�nnen, indem du 25 Katalog-Durchl�ufe ohne einen einzigen Fehler meisterst", "Du hast 25 Katalog-Durchl�ufe ohne Fehler abgeschlossen!", false, null),

            new Achievement("Besserwisser", AchievementPopup.Grade.Bronze, "Beantworte 50 Fragen korrekt und zeige, dass du auf dem richtigen Weg bist", "Du hast 50 Fragen richtig beantwortet!", false, null),
            new Achievement("Besserwisser", AchievementPopup.Grade.Silver, "Beweise dein Wissen mit 500 richtig beantworteten Fragen", "Du hast 500 Fragen richtig beantwortet!", false, null),
            new Achievement("Besserwisser", AchievementPopup.Grade.Gold, "Zeige deine herausragenden Kenntnisse, indem du 1000 Fragen korrekt beantwortest", "Du hast 1000 Fragen richtig beantwortet!", false, null),

            new Achievement("Daylies", AchievementPopup.Grade.Bronze, "Schlie�e f�nfmal den Daily Task erfolgreich ab", "Du hast den Daily Task f�nfmal abgeschlossen!", false, null),
            new Achievement("Daylies", AchievementPopup.Grade.Silver, "Erreiche das Ziel, indem du f�nfzehnmal den Daily Task meisterst", "Du hast den Daily Task f�nfzehnmal abgeschlossen!", false, null),
            new Achievement("Daylies", AchievementPopup.Grade.Gold, "Zeige Ausdauer, indem du drei�igmal den Daily Task erfolgreich abschlie�t", "Du hast den Daily Task drei�igmal abgeschlossen!", false, null),

            new Achievement("Randomat", AchievementPopup.Grade.Bronze, "Schlie�e zehn Random Quiz Runden erfolgreich ab", "Du hast zehn Random Quiz Runden abgeschlossen!", false, null),
            new Achievement("Randomat", AchievementPopup.Grade.Silver, "Beweise deine Flexibilit�t, indem du f�nfzig Random Quiz Runden meisterst", "Du hast 50 Random Quiz Runden abgeschlossen!", false, null),
            new Achievement("Randomat", AchievementPopup.Grade.Gold, "Zeige deine Ausdauer, indem du hundert Random Quiz Runden erfolgreich abschlie�t", "DU hast 100 Random Quiz Runden abgeschlossen!", false, null),

            new Achievement("Hartn�ckig", AchievementPopup.Grade.Bronze, "Beantworte insgesamt 1000 Fragen", "Du hast 1000 Fragen beantwortet!", false, null),
            new Achievement("Hartn�ckig", AchievementPopup.Grade.Silver, "Beantworte insgesamt 5000 Fragen", "Du hast 5000 Fragen beantwortet!", false, null),
            new Achievement("Hartn�ckig", AchievementPopup.Grade.Gold, "Beantworte insgesamt 10000 Fragen", "Du hast 10000 Fragen beantwortet!", false, null),

            new Achievement("Fokus", AchievementPopup.Grade.Bronze, "Verbringe 30 Minuten in einem einzigen Katalog", "Du hast 30 Minuten in einem Katalog verbracht!", false, null),
            new Achievement("Fokus", AchievementPopup.Grade.Silver, "Verbringe 60 Minuten in einem einzigen Katalog", "Du hast 60 Minuten in einem Katalog verbracht!", false, null),
            new Achievement("Fokus", AchievementPopup.Grade.Gold, "Verbringe 120 Minuten in einem einzigen Katalog", "Du hast 120 Minuten in einem Katalog verbracht!", false, null),

            new Achievement("Zeitmanagement", AchievementPopup.Grade.Bronze, "Verbringe insgesamt 300 Minuten in Katalogen", "Du hast insgesamt 300 Minuten in Katalogen verbracht!", false, null),
            new Achievement("Zeitmanagement", AchievementPopup.Grade.Silver, "Verbringe insgesamt 600 Minuten in Katalogen", "Du hast insgesamt 600 Minuten in Katalogen verbracht!", false, null),
            new Achievement("Zeitmanagement", AchievementPopup.Grade.Gold, "Verbringe insgesamt 1200 Minuten in Katalogen", "Du hast insgesamt 1200 Minuten in Katalogen verbracht!", false, null),

            new Achievement("Random Flawless", AchievementPopup.Grade.Bronze, "Schlie�e zehn Random Quiz Runden ohne Fehler ab", "Du hast zehn Random Quiz Runden ohne Fehler abgeschlossen!", false, null),
            new Achievement("Random Flawless", AchievementPopup.Grade.Silver, "Schlie�e 25 Random Quiz Runden ohne Fehler ab", "Du hast 25 Random Quiz Runden ohne Fehler abgeschlossen!", false, null),
            new Achievement("Random Flawless", AchievementPopup.Grade.Gold, "Schlie�e 50 Random Quiz Runden ohne Fehler ab", "Du hast 50 Random Quiz Runden ohne Fehler abgeschlossen!", false, null),

            new Achievement("Intensiv", AchievementPopup.Grade.Bronze, "Verbringe 15 Minuten an einem Tag in Katalogen", "Du hast 15 Minuten an einem Tag in Katalogen verbracht!", false, null),
            new Achievement("Intensiv", AchievementPopup.Grade.Silver, "Verbringe 30 Minuten an einem Tag in Katalogen", "Du hast 30 Minuten an einem Tag in Katalogen verbracht!", false, null),
            new Achievement("Intensiv", AchievementPopup.Grade.Gold, "Verbringe 60 Minuten an einem Tag in Katalogen", "Du hast 60 Minuten an einem Tag in Katalogen verbracht!", false, null),

            new Achievement("Flei�ig", AchievementPopup.Grade.Bronze, "Schlie�e 25 Quiz-Durchl�ufe ab", "Du hast 25 Quiz-Durchl�ufe abgeschlossen!", false, null),
            new Achievement("Flei�ig", AchievementPopup.Grade.Silver, "Schlie�e 50 Quiz-Durchl�ufe ab", "Du hast 25 Quiz-Durchl�ufe abgeschlossen!", false, null),
            new Achievement("Flei�ig", AchievementPopup.Grade.Gold, "Schlie�e 100 Quiz-Durchl�ufe ab", "Du hast 25 Quiz-Durchl�ufe abgeschlossen!", false, null)
        };

        var achs = achievements.Select(a => new Models.Achievement
        {
            Name = a.name,
            Grade = a.grade == AchievementPopup.Grade.Bronze ? "Bronze" : (a.grade == AchievementPopup.Grade.Silver ? "Silber" : "Gold"),
            Description = a.description,
            PopupText = a.popupText,
        }).ToList();

        var a_response = await _supabase.From<Models.Achievement>().Insert(achs);
    }
}
