using Postgrest.Attributes;
using Postgrest.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Table("topics")]
public class TopicModel : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; }
}