using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// MS: Hinweis: Die ganzen "ToString()" Methoden sind nur zum debuggen. Kann man wegmachen wenn sie stören.

public static class DataManager
{
    
    public struct QuestionRound
    {
        public int CatalogueIndex; // Index in der Liste aller Kataloge
        public List<int> Questions; // Indices der Fragen im Katalog
        public List<int> ChosenAnswers; // Werte aus [0, 3] -> Kann man bei der Auswertung verwerten.
        public int QuestionCounter; // Zählt hoch bis AnzahlFragenProFragerunde, danach endet die Fragerunde.
    }


    public struct ImageOrText
    {
        public bool isImage;
        public Sprite image;
        public string text;

        public ImageOrText(string t)
        {
            isImage = false;
            image = null; // leeres Bild als default
            text = t;
        }
        public override readonly string ToString()
        {
            if (isImage)
            {
                return "ImageOrText[image = " + image.ToString() + "]";
            }
            return "ImageOrText[Text = \"" + text + "\"]";
        }
    }
}
