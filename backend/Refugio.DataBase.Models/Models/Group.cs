using System;

namespace Refugio.DataBase.Models.Models
{
	public class Group
	{
        public long Id { get; set; }

        public string? Name { get; set; }

        public string? Activity { get; set; }

        public string? Description { get; set; }

        public string? City { get; set; }

        public string? Country { get; set; }

        public string? Type { get; set; }

        public string? MembersCount { get; set; }

        public string? Place { get; set; }

        public bool? IsClosed { get; set; }

        public List<User> Users { get; set; } = new List<User>();
    }
}