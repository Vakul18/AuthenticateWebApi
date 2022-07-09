namespace AWSDeployDemo.Authenticate
{
    public interface IJwtAuth{
        string Authenticate(string user, string password);
    }
}