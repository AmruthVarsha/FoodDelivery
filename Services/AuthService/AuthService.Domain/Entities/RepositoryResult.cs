using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Domain.Entities
{
    public class RepositoryResult
    {
        public bool Succeded {  get; set; }
        public string? Error { get; set; }

        public static RepositoryResult Ok() { return new RepositoryResult { Succeded = true }; }
        public static RepositoryResult Fail(string error) { return new RepositoryResult { Succeded = false,Error = error }; }
    }
}
