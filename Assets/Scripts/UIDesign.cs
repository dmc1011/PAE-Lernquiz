
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

// MS: Hier sollten
// - sämtliche Farben etc. definiert werden.
// - sämtliche Positionen (wo ist welcher Button, wo sind die Dropdowns, etc.) definiert werden.

// Implementierung: Verschachtelte Structs + static.

public static class UIDesign
{

    public struct Colors
    {
        public struct Global
        {
            public static Color Background = new(0.97f, 0.87f, 0.41f);
        }

        public struct Buttons
        {

            public struct Back
            {
                public static Color Normal = new(0.78f, 0.48f, 0.39f);
                public static Color Hover = new(0.84f, 0.49f, 0.38f);
                public static Color Pressed = new(0.91f, 0.65f, 0.59f);
            };

            public struct Navigation
            {
                public static Color Normal = new(0.39f, 0.18f, 0.91f);
                public static Color Hover = new(0.71f, 0.46f, 1.00f);
                public static Color Pressed = new(0.96f, 0.75f, 0.42f);
            }

            public struct Question
            {
                public static Color Normal = new(0.50f, 0.82f, 0.95f);
                public static Color Hover = new(0.65f, 0.85f, 0.95f);
                public static Color Pressed = new(0.89f, 0.85f, 1.00f);
            }

            public struct Answer
            {
                public static Color Normal = new(0.82f, 0.95f, 0.46f);
                public static Color Hover = new(0.82f, 1.00f, 0.57f);
                public static Color Pressed = new(0.82f, 1.00f, 0.70f);
            }

        }

        public struct Dropdowns {

            public struct Katalogauswahl
            {
                public static Color Normal = new(0.82f, 0.95f, 0.46f);
                public static Color Hover = new(0.82f, 1.00f, 0.57f);
                public static Color Pressed = new(0.82f, 1.00f, 0.70f);
            }

            public struct Fragenauswahl
            {
                public static Color Normal = new(0.82f, 0.95f, 0.46f);
                public static Color Hover = new(0.82f, 1.00f, 0.57f);
                public static Color Pressed = new(0.82f, 1.00f, 0.70f);
            }

        }

        public struct Labels
        {
            public static Color Fragenummer = new(0.80f, 0.40f, 0.20f);
        }
        
    }

    public struct Positions
    {

        public struct Global
        {
            // Das ist die "Referenzgröße" an der wir uns orientieren. Es sind willkürlich gewählte Werte
            // im Endeffekt wird das sowieso auf den Screen skaliert.
            // AspectRatio muss allerdings stimmen. Das kann ggf. später dynamisch angepasst werden falls man
            // nicht "nur" 16:9 supporten will.
            public static float x = 0.0f;
            public static float y = 0.0f;
            public static float width = 306.0f;
            public static float height = 544.0f;
        }

        public struct Buttons
        {
            // Ist aktuell überall gleich.
            public static Rect Back = new(0.15f, 0.075f, 0.15f, 0.075f);

            public struct Home
            {
                public static Rect Start = new(0.5f, 0.65f, 0.6f, 0.1f);
                public static Rect Settings = new(0.5f, 0.55f, 0.6f, 0.1f);
                public static Rect DailyTask = new(0.5f, 0.45f, 0.6f, 0.1f);
                public static Rect Questions = new(0.5f, 0.35f, 0.6f, 0.1f);

            }

            public struct NewGame
            {
                public static Rect Start = new(0.5f, 0.5f, 0.6f, 0.1f);
            }

            public struct Catalogues
            {
                public static Rect Frage = new(0.5f, 0.65f, 0.8f, 0.15f);
                public static Rect Antwort1 = new(0.5f - 0.2f, 0.425f + 0.075f, 0.4f, 0.15f);
                public static Rect Antwort2 = new(0.5f + 0.2f, 0.425f + 0.075f, 0.4f, 0.15f);
                public static Rect Antwort3 = new(0.5f - 0.2f, 0.425f - 0.075f, 0.4f, 0.15f);
                public static Rect Antwort4 = new(0.5f + 0.2f, 0.425f - 0.075f, 0.4f, 0.15f);
            }

            public struct SingleplayerGameloop1
            {
                public static Rect Frage = new(0.5f, 0.65f, 0.8f, 0.15f);
                public static Rect Antwort1 = new(0.5f - 0.2f, 0.425f + 0.075f, 0.4f, 0.15f);
                public static Rect Antwort2 = new(0.5f + 0.2f, 0.425f + 0.075f, 0.4f, 0.15f);
                public static Rect Antwort3 = new(0.5f - 0.2f, 0.425f - 0.075f, 0.4f, 0.15f);
                public static Rect Antwort4 = new(0.5f + 0.2f, 0.425f - 0.075f, 0.4f, 0.15f);
            }

        }

        public struct Dropdowns
        {

            public struct Catalogues
            {
                public static Rect Katalogauswahl = new(0.5f, 0.90f, 0.7f, 0.075f);
                public static Rect Fragenauswahl = new(0.5f, 0.82f, 0.7f, 0.075f);
            }

            public struct NewGame
            {
                public static Rect Katalogauswahl = new(0.5f, 0.60f, 0.7f, 0.075f);
            }
            
        }


    }

}
