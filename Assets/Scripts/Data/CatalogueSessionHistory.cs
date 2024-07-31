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

    public CatalogueSessionHistory(int id, int catalogueId, DateTime sessionDate, int timeSpent)
    {
        this.id = id;
        this.catalogueId = catalogueId;
        this.sessionDate = sessionDate;
        this.timeSpent = timeSpent;
    }
}
