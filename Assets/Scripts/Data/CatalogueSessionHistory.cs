using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatalogueSessionHistory
{
    public int id;
    public int catalogueId;
    public DateTime sessionDate;
    public int timeSpent;
    public bool isCompleted;
    public bool isErrorFree;

    public CatalogueSessionHistory(int id, int catalogueId, DateTime sessionDate, int timeSpent, bool isCompleted, bool isErrorFree)
    {
        this.id = id;
        this.catalogueId = catalogueId;
        this.sessionDate = sessionDate;
        this.timeSpent = timeSpent;
        this.isCompleted = isCompleted;
        this.isErrorFree = isErrorFree;
    }
}
