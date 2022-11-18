namespace PostGram.Api.Models.Attachment
{
    public class FileInfoModel
    {
        public FileInfoModel(string name, string mimeType, string path)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            MimeType = mimeType ?? throw new ArgumentNullException(nameof(mimeType));
            Path = path ?? throw new ArgumentNullException(nameof(path));
        }

        public string Name { get; set; } 
        public string MimeType { get; set; } 
        public string Path { get; set; } 
    }
}