using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Catalogue
{
    public string id;
    public string name;
    public string ownerId;
    public List<Question> questions;

    public Catalogue (string id, string name, string ownerId, List<Question> questions)
    {
        this.id = id;
        this.name = name;
        this.ownerId = ownerId;
        this.questions = questions;
    }
}
