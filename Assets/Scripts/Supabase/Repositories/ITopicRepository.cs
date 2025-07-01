using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Entities;

namespace Repositories
{
    public interface ITopicRepository
    {
        Task<List<Topic>> GetAllTopics();
    }
}
