using System.Security.AccessControl;
using AutoMapper;
using PostGram.Api.Models;
using PostGram.DAL.Entities;


namespace PostGram.Api
{
    public class MapperProfile:Profile
    {
        public MapperProfile()
        {
            CreateMap<CreateUserModel, User>()
                .ForMember(u => u.Id, m => m.MapFrom(s => Guid.NewGuid()))
                .ForMember(u => u.PasswordHash, m => m.MapFrom(s => Common.HashHelper.GetHash(s.Password)))
                .ForMember(u=>u.BirthDate,m=>m.MapFrom(s=>s.BirthDate.UtcDateTime));

            CreateMap<User, UserModel>();

            CreateMap<Avatar, AttachModel>();
        }
    }
}
