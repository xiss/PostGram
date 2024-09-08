using AutoFixture;
using PostGram.DAL.Entities;

namespace PostGram.Api.Tests.Customizations;

public class PostCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<Post>(c => c.With(post => post.IsDeleted, false));
    }
}