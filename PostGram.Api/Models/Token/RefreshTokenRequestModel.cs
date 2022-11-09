namespace PostGram.Api.Models.Token
{
    public class RefreshTokenRequestModel
    {
        public RefreshTokenRequestModel(string refreshToken)
        {
            RefreshToken = refreshToken;
        }

        public string RefreshToken { get; set; }
    }
}