﻿using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FitnessTracker
{
    public interface IDocDbRepository<T> where T : class
    {
        Task<Document> CreateItemAsync(T item);

        Task DeleteItemAsync(string id);

        Task<T> GetItemAsync(string id);

        Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate);

        Task<Document> UpdateItemAsync(string id, T item);
    }
}
