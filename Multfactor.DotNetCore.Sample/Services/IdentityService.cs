namespace Multfactor.DotNetCore.Sample.Services
{
    /// <summary>
    /// Identity service demo
    /// </summary>
    public class IdentityService
    {
        public bool ValidateUser(string login, string password, out string role)
        {
            role = null;

            //actually you shoud never store and compare password in plain text
            //we recomment using https://github.com/BcryptNet/bcrypt.net

            //also, this is just a demo

            if (login == "user@example.com" && password == "qwerty")
            {
                role = "user";
                return true;
            }

            if (login == "admin@example.com" && password == "qwerty")
            {
                role = "admin";
                return true;
            }

            return false;
        }
    }
}
