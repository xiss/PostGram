namespace PostGram.Api.Models
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