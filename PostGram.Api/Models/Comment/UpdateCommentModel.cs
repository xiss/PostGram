namespace PostGram.Api.Models.Comment
{
    public class UpdateCommentModel
    {
        public Guid Id { get; set; }
        public string NewBody { get; set; } = null!;
    }
}