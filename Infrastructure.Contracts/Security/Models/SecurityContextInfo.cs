namespace Infrastructure.Contracts.Security.Models
{
    public class SecurityContextInfo
    {
        //public UserInfo User { get; set; }

        public bool IsAdministrator { get; set; }

        public bool IsManager { get; set; }
    }
}