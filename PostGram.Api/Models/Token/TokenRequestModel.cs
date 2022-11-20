namespace PostGram.Api.Models.Token
{
    public record TokenRequestModel
    {
        public TokenRequestModel(string login, string password)
        {
            Login = login;
            Password = password;
        }

        public string Login { get; init; }
        public string Password { get; init; }
    }
}