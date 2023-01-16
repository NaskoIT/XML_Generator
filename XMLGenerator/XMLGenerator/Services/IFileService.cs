namespace XMLGenerator.Services
{
    public interface IFileService
    {
        string Save(string content, string extension);

        byte[] Read(string fileName);
    }
}
