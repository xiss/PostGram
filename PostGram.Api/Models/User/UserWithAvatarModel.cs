namespace PostGram.Api.Models.User
{
    public class UserWithAvatarModel : UserModel
    {
        public UserWithAvatarModel(UserModel model, Func<UserModel, string> linkGenerator) 
            : base(model.Id , model.Name, model.Surname, model.Patronymic, model.Email, model.Login, model.BirthDate)
        {
            AvatarLink = linkGenerator?.Invoke(model);
        }

        public string? AvatarLink { get; set; }

    }
}