using Controllers;
using Entities;
using Microsoft.IdentityModel.Tokens;
using Services;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UseCases;

// to do: rename GameSession / Session
public static class Global
{
    // player prefs keys for daily task -> to do: remove
    public const string LastResetDateKey = "LastResetDate";
    public const string IsDailyTaskCompletedKey = "IsDailyTaskCompleted";

    // monitoring question round
    public static bool InsideQuestionRound = false;
    public static DataManager.QuestionRound CurrentQuestionRound;
    public static List<AnswerHistory> AnswerHistories = new();
    public const int RandomQuizSize = 5;

    // Editor
    public static SceneLoader.EditorType EditorType;
    public static Topic tmpTopic;


    public static GameMode GetGameMode()
    {
        return CurrentQuestionRound.gameMode;
    }

    public static void SetGameMode(GameMode gameMode) {
        CurrentQuestionRound.gameMode = gameMode;
    }

    public static void SetInsideQuestionRound(bool b)
    {
        InsideQuestionRound = b;
    }

    public static Catalogue GetCatalogue()
    {
        return CurrentQuestionRound.catalogue;
    }

    public static void SetCatalogue(Catalogue catalogue)
    {
        CurrentQuestionRound.catalogue = catalogue;
    }

    public static void UpdateQuestion(Question question)
    {
        int questionIndex = CurrentQuestionRound.catalogue.questions.FindIndex(q => q.id == question.id);

        CurrentQuestionRound.catalogue.questions[questionIndex] = question;
    }

    public static List<AnswerHistory> GetAnswerHistories()
    {
        return AnswerHistories;
    }

    // to do:
    public static void AddAnswerHistory(int questionId, bool wasCorrect, int sessionId)
    {
        int id = AnswerHistories.Count;
        AnswerHistory a = new AnswerHistory(id, questionId, DateTime.Now, wasCorrect, sessionId);

        AnswerHistories.Add(a);
    }

    public static void ClearAnswerHistories()
    {
        AnswerHistories = new List<AnswerHistory>();
    }

    public async static void UpdateCatalogueUserData()
    {
        Supabase.Client supabase = SupabaseClientProvider.GetClient();

        SupabaseCatalogueRepository catalogueRepo = new SupabaseCatalogueRepository(supabase);
        SupabaseRequestUseCase supabaseRequestUseCase = new SupabaseRequestUseCase(catalogueRepo);
        FetchCataloguesUseCase cataloguesUseCase = new FetchCataloguesUseCase(catalogueRepo);
        CatalogueController catalogueController = new CatalogueController(cataloguesUseCase, supabaseRequestUseCase);

        await catalogueController.UpdateCatalogueUserData(CurrentQuestionRound.catalogue);
    }

    public static void ClearSession()
    {
        CurrentQuestionRound = new DataManager.QuestionRound();
        InsideQuestionRound = false;
        AnswerHistories = new List<AnswerHistory>();
    }


    // Editor

    public static void SetEditorType(SceneLoader.EditorType editorType)
    {
        EditorType = editorType;
    }


    // monitoring daily task
    public static DataManager.DailyTask CurrentDailyTask;
    public const int DailyTaskSize = 10;
    public static Catalogue tmpCatalogue = null;


    // returns true if tmpCatalogue != null
    public static bool SetTmpCatalogue (Catalogue catalogue)
    {
        if (catalogue == null)
        {
            string topicName = Global.tmpTopic.name;
            Supabase.Client client = SupabaseClientProvider.GetClient();
            bool isPrivate = true;

            tmpCatalogue = new Catalogue(-1, "Neuer Fragenkatalog", 0, 0, 0, 0, 0, 0, new List<Question>(), new List<CatalogueSessionHistory>(), topicName, isPrivate, Guid.Parse(client.Auth.CurrentUser.Id));
            return true;
        }

        /*
        CatalogueTable catalogueTable = SQLiteSetup.Instance.catalogueTable;
        tmpCatalogue = catalogueTable.FindCatalogueByName(catalogueName);
        */

        tmpCatalogue = catalogue;
        return tmpCatalogue != null;
    }

    public static void SetTmpTopic (Topic topic)
    {
        if (topic == null)
        {
            tmpTopic = new Topic("Neues Oberthema");
        }
        else
        {
            tmpTopic = topic;
        }
    }
}

