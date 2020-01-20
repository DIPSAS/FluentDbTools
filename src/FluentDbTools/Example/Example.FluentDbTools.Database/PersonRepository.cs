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
        private readonly IDbConfigSchemaTargets DbConfigConfig;

        public PersonRepository(
            ILogger<PersonRepository> logger,
            IDbConfigSchemaTargets dbConfigConfig)
        {
            Logger = logger;
            DbConfigConfig = dbConfigConfig;
        }
        
        public Task InsertPerson(Person person, IDbConnection dbConnection)
        {
            Logger.LogDebug($"Inserting person with id: {person.PersonId}");
            return Insert.InsertPerson.Execute(
                dbConnection,
                DbConfigConfig,
                person);
        }

        public Task<IEnumerable<Person>> SelectPersons(Guid[] ids, IDbConnection dbConnection)
        {
            Logger.LogDebug($"Selecting multiple persons");
            return Select.SelectPersons.Execute(
                dbConnection,
                DbConfigConfig,
                ids);
        }

        public Task<Person> SelectPerson(Guid id, IDbConnection dbConnection)
        {
            Logger.LogDebug($"Selecting person with id: {id}");
            return Select.SelectPerson.Execute(
                dbConnection,
                DbConfigConfig,
                id);
        }

        public Task<long> CountAlivePersons(IDbConnection dbConnection)
        {
            Logger.LogDebug($"Count how many persons alive.");
            return Select.SelectPersonsCount.Execute(
                dbConnection,
                DbConfigConfig);
        }

        public Task UpdatePerson(Person person, IDbConnection dbConnection)
        {
            Logger.LogDebug($"Updating person with id: {person.PersonId}");
            return Update.UpdatePerson.Execute(
                dbConnection,
                DbConfigConfig,
                person);
        }

        public Task DeletePerson(Guid id, IDbConnection dbConnection)
        {
            Logger.LogDebug($"Deleting person with id: {id}");
            return Delete.DeletePerson.Execute(
                dbConnection,
                DbConfigConfig,
                id);
        }
    }
}