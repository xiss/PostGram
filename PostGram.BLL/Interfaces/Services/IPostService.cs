using PostGram.DAL.Entities;

namespace PostGram.BLL.Interfaces.Services;

public interface IPostService
{
    Task DeletePostContents(ICollection<PostContent> postContents);
}