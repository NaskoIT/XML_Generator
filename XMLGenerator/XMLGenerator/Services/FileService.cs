namespace XMLGenerator.Services
{
    public class FileService : IFileService
    {
        public byte[] Read(string fileName)
        {
            return File.ReadAllBytes(fileName);
        }

        public string Save(string content, string extension)
        {
            string path = $"./{Path.GetRandomFileName()}.{extension}";
            File.WriteAllText(path, content);
            return path;
        }
    }
}
