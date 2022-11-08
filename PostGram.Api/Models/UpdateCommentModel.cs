namespace PostGram.Api.Models
{
    public class UpdateCommentModel
    {
        public Guid Id { get; set; }
        public string NewBody { get; set; } = null!;
    }
}