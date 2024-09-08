using AutoFixture;
using Microsoft.EntityFrameworkCore;
using PostGram.DAL;
using PostGram.DAL.Entities;

namespace PostGram.Api.Tests.Customizations;

public class CommentCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<Comment>(c => c.With(post => post.IsDeleted, false));
    }
}