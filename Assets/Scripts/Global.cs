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
    public static bool InsideFragerunde = false;
    public static DataManager.Fragerunde AktuelleFragerunde;

}

