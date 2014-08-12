namespace ABServicios.BLL.DataInterfaces
{
    public interface ITempStorageAccessor
    {
        string GetBlobPath(string fileName, string partialPath);
        string ReadText(string fileName, string partialPath = null);
        void WriteText(string content, string fileName, string partialPath = null);
        void DeleteTempFolder(string partialPath);
    }
}