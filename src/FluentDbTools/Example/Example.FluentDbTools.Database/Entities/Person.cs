using System;

namespace Example.FluentDbTools.Database.Entities
{
    public class Person
    {
        public Guid PersonId { get; set; } = Guid.NewGuid();
        public int SequenceNumber { get; set; } = 0;
        public bool Alive { get; set; } = true;
        public string Username { get; set; } = "Ola";
        public string Password { get; set; } = "Nordmann";
    }

    public class Parent
    {
        public Guid ParentId { get; set; } = Guid.NewGuid();
    }

}