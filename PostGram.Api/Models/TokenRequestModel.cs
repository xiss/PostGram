namespace PostGram.Api.Models
{
    public class TokenRequestModel
    {
        public TokenRequestModel(string login, string password)
        {
            Login = login;
            Password = password;
        }

        public string Login { get; set; }
        public string Password { get; set; }
    }
}
