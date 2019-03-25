using FluentDbTools.Common.Abstractions;

namespace FluentDbTools.Contracts.DefaultConfigs
{
    public class DbConfigCredentials : IDbConfigCredentials
    {
        protected DefaultDbConfigValues Defaults;

        public DbConfigCredentials(DefaultDbConfigValues defaultDbConfigValues = null)
        {
            Defaults = defaultDbConfigValues ?? new DefaultDbConfigValues();
        }

        private string UserField;
        public virtual string User
        {
            get => UserField ?? Defaults.GetDefaultUser?.Invoke();
            set => UserField = value;
        }

        private string PasswordField;
        public virtual string Password
        {
            get => PasswordField ?? Defaults.GetDefaultPassword?.Invoke();
            set => PasswordField = value;
        }

        private string AdminUserField;
        public virtual string AdminUser
        {
            get => AdminUserField ?? Defaults.GetDefaultAdminUser?.Invoke();
            set => AdminUserField = value;
        }

        private string AdminPasswordField;
        public virtual string AdminPassword
        {
            get => AdminPasswordField ?? Defaults.GetDefaultAdminPassword?.Invoke();
            set => AdminPasswordField = value;
        }
    }
}