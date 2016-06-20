namespace LNU.Courses.Repositories
{
    public interface IHashProvider
    {
        string Encrypt(string stringToHash);
    }
}
