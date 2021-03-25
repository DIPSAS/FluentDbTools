using System;

namespace Example.FluentDbTools.Database.Entities
{
    public class Person
    {
        public Guid PersonId { get; set; } = Guid.NewGuid();
        public int SequenceNumber { get; set; } = 0;
        public bool Alive { get; set; } = true;
        public ExtraInformation ExtraInformation { get; set; }
        public string Username { get; set; } = "Ola";
        public string Password { get; set; } = "Nordmann";
    }

    public class ExtraInformation
    {
        public string Info1 { get; set; }
        public string Info2 { get; set; }
        public object Details { get;set; }
    }

    public class ExtraInformationDetails
    {
        public string Details { get; set; }
    }


    public class Parent
    {
        public Guid ParentId { get; set; } = Guid.NewGuid();
    }

}