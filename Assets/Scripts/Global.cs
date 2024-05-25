using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static Global;

public static class Global
{

    // Set this to false for "release" version.
    public static bool DEV = true;

    // Das ist die "referenzgröße" an der wir uns orientieren. Es sind willkürlich gewählte Werte
    // im Endeffekt wird das sowieso auf den Screen skaliert.
    // AspectRatio muss allerdings stimmen. Das kann ggf. später dynamisch angepasst werden falls man
    // nicht "nur" 16:9 supporten will.
    public static float width = 306.0f;
    public static float height = 544.0f;

    // MS: Aktuell ist im Globalen Scope eine Leere Liste an Fragenkatalogen
    // die muss dann vom Lokalen Speicher des Geräts aus gefüllt werden.
    public static Catalogues CatalogueStorage = new("");

    public struct Colors
    {
        public static Color Background = new(0.97f, 0.87f, 0.41f);

        public struct ButtonBack
        {
            public static Color Normal = new(0.78f, 0.48f, 0.39f);
            public static Color Hover = new(0.84f, 0.49f, 0.38f);
            public static Color Pressed = new(0.91f, 0.65f, 0.59f);
        }

        public struct ButtonNavigation
        {
            public static Color Normal = new(0.39f, 0.18f, 0.91f);
            public static Color Hover = new(0.71f, 0.46f, 1.00f);
            public static Color Pressed = new(0.96f, 0.75f, 0.42f);
        }

        public struct ButtonQuestion
        {
            public static Color Normal = new(0.50f, 0.82f, 0.95f);
            public static Color Hover = new(0.65f, 0.85f, 0.95f);
            public static Color Pressed = new(0.89f, 0.85f, 1.00f);
        }
        public struct ButtonAnswer
        {
            public static Color Normal = new(0.82f, 0.95f, 0.46f);
            public static Color Hover = new(0.82f, 1.00f, 0.57f);
            public static Color Pressed = new(0.82f, 1.00f, 0.70f);
        }


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

    public struct Question
    {
        public string id;
        public string questionInfo;
        public int correctAnswerIndex;
        public ImageOrText question;
        public List<ImageOrText> answers;

        public Question(string a = "")
        {
            id = "";
            questionInfo = "";
            correctAnswerIndex = 0;
            question = new("");
            answers = new();
        }

        public override string ToString()
        {
            string s = "id = " + id + ", questionInfo = " + questionInfo + ", correctAnswerIndex = " + correctAnswerIndex +
                ", question = [" + question + "], answers = [";
            foreach(ImageOrText a in answers) s += a.ToString() + ", ";
            s += "]";
            return s;
        }
    }

    public struct Catalogue
    {
        public string id;
        public string name;
        public string ownerId;
        public List<Question> questions;

        public Catalogue (string a = "")
        {
            id = "";
            name = "";
            ownerId = "";
            questions = new();
        }

        public override string ToString()
        {
            string s = "id = " + id + "\nname = " + name + "\nownerId = " + ownerId + "\nquestions = [";
            Question last = questions.Last();
            foreach(Question q in questions) s += q.ToString() + (q.Equals(last) ? ", " : "");
            s += "]\n";
            return s;
        }
    }

    public struct Catalogues
    {
        public List<Catalogue> catalogues;

        public Catalogues (string a = "")
        {
            catalogues = new();
        }

        public readonly void Clear()
        {
            catalogues.Clear();
        }

        public override string ToString()
        {

            string s = "Catalogues[";
            foreach (Catalogue c in catalogues) s += c.ToString() + "\n";
            s += "]";
            return s;
        }
    }

}