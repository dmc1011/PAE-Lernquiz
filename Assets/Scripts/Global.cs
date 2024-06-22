
public static class Global
{
    // Es folgen globale Einstellungen für die App
    // Diese können nicht via Code geändert werden.
    public static readonly int NumQuestionsPerRound = 5;

    // Falls True -> Wir sind im Gameloop und "AktuelleFragerunde" ist valide.
    // Falls False -> Wir sind "irgendwo" und "AktuelleFragerunde" ist default initialisiert (nutzlos).
    public static bool InsideQuestionRound = false;
    public static DataManager.QuestionRound CurrentQuestionRound;

    public static Catalogue tmpCatalogue = new(-1, "Neuer Fragenkatalog", new());
}

