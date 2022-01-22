using System;
using FluentDbTools.Contracts;
#pragma warning disable CS1591

namespace FluentDbTools.Common.Abstractions
{
    public class InvalidAdminValue
    {
        public InvalidAdminType InvalidAdminType { get; set; }
        public string Value { get; set; }
        public string[] ConfigurationKeys { get; set; }

        public ArgumentException GeneratedArgumentException { get; set; }

        public bool IsAdminUser() => InvalidAdminType == InvalidAdminType.AdminUser;
        public bool IsAdminPassword() => InvalidAdminType == InvalidAdminType.AdminPassword;
    }
}