
public static class Global
{
    // Es folgen globale Einstellungen f�r die App
    // Diese k�nnen nicht via Code ge�ndert werden.
    public static readonly int NumQuestionsPerRound = 5;

    // Falls True -> Wir sind im Gameloop und "AktuelleFragerunde" ist valide.
    // Falls False -> Wir sind "irgendwo" und "AktuelleFragerunde" ist default initialisiert (nutzlos).
    public static bool InsideQuestionRound = false;
    public static DataManager.DailyTask CurrentDailyTask;
    public static DataManager.QuestionRound CurrentQuestionRound;

}

