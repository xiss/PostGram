namespace PostGram.Api.Models.Attachment
{
    public class FileInfoModel
    {
        public FileInfoModel(string name, string mimeType, string path)
        {
            Name = name;
            MimeType = mimeType;
            Path = path;
        }

        public string Name { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
    }
}