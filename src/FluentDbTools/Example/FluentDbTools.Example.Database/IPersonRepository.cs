using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentDbTools.Example.Database.Entities;

namespace FluentDbTools.Example.Database
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