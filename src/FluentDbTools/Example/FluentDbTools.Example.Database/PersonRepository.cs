using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Example.Database.Entities;
using Microsoft.Extensions.Logging;

namespace FluentDbTools.Example.Database
{
    public class PersonRepository : IPersonRepository
    {
        private readonly ILogger Logger;
        private readonly IDbProvider DbProvider;
        private readonly IDbConfig DbConfig;

        public PersonRepository(
            ILogger<PersonRepository> logger,
            IDbProvider dbProvider,
            IDbConfig dbConfig)
        {
            Logger = logger;
            DbProvider = dbProvider;
            DbConfig = dbConfig;
        }
        
        public Task InsertPerson(Person person)
        {
            Logger.LogDebug($"Inserting person with id: {person.Id}");
            return Insert.InsertPerson.Execute(
                DbProvider.DbTransaction.Connection,
                DbConfig,
                person);
        }

        public Task<IEnumerable<Person>> SelectPersons(Guid[] ids)
        {
            Logger.LogDebug($"Selecting multiple persons");
            return Select.SelectPersons.Execute(
                DbProvider.DbTransaction.Connection,
                DbConfig,
                ids);
        }

        public Task<Person> SelectPerson(Guid id)
        {
            Logger.LogDebug($"Selecting person with id: {id}");
            return Select.SelectPerson.Execute(
                DbProvider.DbTransaction.Connection,
                DbConfig,
                id);
        }

        public Task UpdatePerson(Person person)
        {
            Logger.LogDebug($"Updating person with id: {person.Id}");
            return Update.UpdatePerson.Execute(
                DbProvider.DbTransaction.Connection,
                DbConfig,
                person);
        }

        public Task DeletePerson(Guid id)
        {
            Logger.LogDebug($"Deleting person with id: {id}");
            return Delete.DeletePerson.Execute(
                DbProvider.DbTransaction.Connection,
                DbConfig,
                id);
        }
    }
}