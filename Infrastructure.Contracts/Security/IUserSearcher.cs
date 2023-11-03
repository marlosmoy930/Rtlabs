namespace Infrastructure.Contracts.Security
{
    public interface IUserSearcher
    {
        public bool DoesUserExist(string userName);
    }
}