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
}
