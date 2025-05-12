using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CatalogueDTO
{
    public int id;
    public string name;
    public string topicName;
    public bool isPrivate;
    public Guid createdBy;

    public CatalogueDTO(int id, string name, string topicName, bool isPrivate, Guid createdBy)
    {
        this.id = id;
        this.name = name;
        this.topicName = topicName;
        this.isPrivate = isPrivate;
        this.createdBy = createdBy;
    }
}
