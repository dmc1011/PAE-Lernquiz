using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Global
{
    // Es folgen globale Einstellungen für die App
    // Diese können nicht via Code geändert werden.
    public static readonly int NumQuestionsPerRound = 6;

    // Falls True -> Wir sind im Gameloop und "AktuelleFragerunde" ist valide.
    // Falls False -> Wir sind "irgendwo" und "AktuelleFragerunde" ist default initialisiert (nutzlos).
    public static bool InsideQuestionRound = false;
    public static DataManager.QuestionRound CurrentQuestionRound;

    // Das hier ist die "Referenzgröße" für 16:9 (Größe in Pixeln des Canvas)
    // Ausgehend davon wird alles auf die tatsächlich vorhandene Auflösung skaliert.
    // Das gilt aber nur für 16:9 -> TODO UI-Design-Team. damit es auch z.B. bei 21:9 geht.
    public static float width = 304;
    public static float height = 544;

}

