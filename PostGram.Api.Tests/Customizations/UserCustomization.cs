using AutoFixture;
using AutoFixture.Kernel;
using PostGram.DAL.Entities;
using System.Reflection;

namespace PostGram.Api.Tests.Customizations;

public class UserCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<User>(c => c.With(post => post.IsDelete, false));
    }
}