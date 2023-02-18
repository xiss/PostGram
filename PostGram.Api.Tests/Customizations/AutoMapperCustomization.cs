using AutoFixture;
using AutoMapper;

namespace PostGram.Api.Tests.Customizations
{
    public class AutoMapperCustomization : ICustomization
    {
        private readonly IMapper _mapper;

        public AutoMapperCustomization(Type profileType)
        {
            _mapper = new Mapper(new MapperConfiguration(options => options.AddProfile(profileType)));
        }

        public void Customize(IFixture fixture)
        {
            fixture.Inject(_mapper);
        }
    }
}
