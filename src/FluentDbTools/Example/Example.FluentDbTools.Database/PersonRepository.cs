using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using FluentDbTools.Common.Abstractions;
using Example.FluentDbTools.Database.Entities;
using Microsoft.Extensions.Logging;

namespace Example.FluentDbTools.Database
{
    public class PersonRepository : IPersonRepository
    {
        private readonly ILogger Logger;
        private readonly IDbConfig DbConfig;

        public PersonRepository(
            ILogger<PersonRepository> logger,
            IDbConfig dbConfig)
        {
            Logger = logger;
            DbConfig = dbConfig;
        }
        
        public Task InsertPerson(Person person, IDbConnection dbConnection)
        {
            Logger.LogDebug($"Inserting person with id: {person.Id}");
            return Insert.InsertPerson.Execute(
                dbConnection,
                DbConfig,
                person);
        }

        public Task<IEnumerable<Person>> SelectPersons(Guid[] ids, IDbConnection dbConnection)
        {
            Logger.LogDebug($"Selecting multiple persons");
            return Select.SelectPersons.Execute(
                dbConnection,
                DbConfig,
                ids);
        }

        public Task<Person> SelectPerson(Guid id, IDbConnection dbConnection)
        {
            Logger.LogDebug($"Selecting person with id: {id}");
            return Select.SelectPerson.Execute(
                dbConnection,
                DbConfig,
                id);
        }

        public Task<long> CountAlivePersons(IDbConnection dbConnection)
        {
            Logger.LogDebug($"Count how many persons alive.");
            return Select.SelectPersonsCount.Execute(
                dbConnection,
                DbConfig);
        }

        public Task UpdatePerson(Person person, IDbConnection dbConnection)
        {
            Logger.LogDebug($"Updating person with id: {person.Id}");
            return Update.UpdatePerson.Execute(
                dbConnection,
                DbConfig,
                person);
        }

        public Task DeletePerson(Guid id, IDbConnection dbConnection)
        {
            Logger.LogDebug($"Deleting person with id: {id}");
            return Delete.DeletePerson.Execute(
                dbConnection,
                DbConfig,
                id);
        }
    }
}