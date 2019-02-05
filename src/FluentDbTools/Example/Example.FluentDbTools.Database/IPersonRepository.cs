using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Example.FluentDbTools.Database.Entities;

namespace Example.FluentDbTools.Database
{
    public interface IPersonRepository
    {
        Task InsertPerson(Person person, IDbConnection dbConnection);
        Task<IEnumerable<Person>> SelectPersons(Guid[] ids, IDbConnection dbConnection);
        Task<Person> SelectPerson(Guid id, IDbConnection dbConnection);
        Task UpdatePerson(Person person, IDbConnection dbConnection);
        Task DeletePerson(Guid id, IDbConnection dbConnection);
    }
}