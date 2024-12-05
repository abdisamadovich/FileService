namespace WebTotalCommander.Repository.Settings;

public class FolderSettings
{
    public string MainFolderName { get; }

    public FolderSettings(string mainFolderName)
    {
        this.MainFolderName = mainFolderName;
    }
}
