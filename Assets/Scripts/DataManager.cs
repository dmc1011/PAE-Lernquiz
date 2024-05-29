using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// MS: Hinweis: Die ganzen "ToString()" Methoden sind nur zum debuggen. Kann man wegmachen wenn sie stören.

public static class DataManager
{
    // Hier werden "alle" Sachen gespeichert.
    // Diese struct muss später irgendwie auf dem Smartphone gespeichert werden.
    public static Data Storage = new("");


    // Die Funktion ist einfach nur so dahingetackert, damit schonmal was hier ist.
    public static void LoadDummyData()
    {
        Storage.Catalogues.Clear();
        for (int i = 0; i < 5; i++)
        {
            Catalogue catalogue = new("");
            string z = Random.Range(100, 999).ToString();
            catalogue.id = "Dummy Katalog ID: " + z;
            catalogue.name = "Dummy Katalog: " + z;
            catalogue.ownerId = "Ich";
            for (int i_question = 0; i_question < 10; i_question++)
            {
                Question q = new("");
                q.id = "some question identifier: " + Random.Range(100, 999).ToString();
                z = Random.Range(100, 999).ToString();
                q.question = new("Finde diese Zahl: " + z);
                List<ImageOrText> answers = new() {
                            new(z),
                            new(Random.Range(100, 999).ToString()),
                            new(Random.Range(100, 999).ToString()),
                            new(Random.Range(100, 999).ToString())
                        };
                q.answers = answers;
                q.correctAnswerIndex = 0; // erste ist immer richtig -> index 0
                catalogue.questions.Add(q);
            }
            Storage.Catalogues.Add(catalogue);
        }
    }

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

        public override readonly string ToString()
        {
            string s = "id = " + id + ", questionInfo = " + questionInfo + ", correctAnswerIndex = " + correctAnswerIndex +
                ", question = [" + question + "], answers = [";
            foreach (ImageOrText a in answers) s += a.ToString() + ", ";
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

        public Catalogue(string a = "")
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
            foreach (Question q in questions) s += q.ToString() + (q.Equals(last) ? ", " : "");
            s += "]\n";
            return s;
        }
    }

    public struct Data
    {
        public List<Catalogue> Catalogues;

        public Data(string a = "")
        {
            Catalogues = new();
        }

        public override readonly string ToString()
        {

            string s = "Catalogues[";
            foreach (Catalogue c in Catalogues) s += c.ToString() + "\n";
            s += "]";
            return s;
        }
    }
}
