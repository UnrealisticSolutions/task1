using System;
namespace API.Interfaces
{
    public class ResetPostInterface
    {
        public string code { get; set; }
        public string email { get; set; }
        public string password { get; set; }
    }
}
