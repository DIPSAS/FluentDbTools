using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Example.FluentDbTools.Database.Entities;

namespace Example.FluentDbTools.Database
{
    public interface IPersonRepository
    {
        Task InsertPerson(Person person);
        Task<IEnumerable<Person>> SelectPersons(Guid[] ids);
        Task<Person> SelectPerson(Guid id);
        Task UpdatePerson(Person person);
        Task DeletePerson(Guid id);
    }
}