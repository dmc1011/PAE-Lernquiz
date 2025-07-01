using System;

namespace Entities
{
    [System.Serializable]
    public class Profile
    {
        public Guid userId;
        public string name;
        public string surname;
        public UserRole role;

        public Profile(Guid userId, string name, string surname, UserRole role)
        {
            this.userId = userId;
            this.name = name;
            this.surname = surname;
            this.role = role;

        }
    }
}
