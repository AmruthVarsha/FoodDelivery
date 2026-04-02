using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Domain.Interfaces
{
    public interface IDataSeeder
    {
        public Task GenerateRoles();

        public Task GenerateAdmin();
    }
}
