namespace WebTotalCommander.FileAccess.Models.Common;

public class FolderGetAllQueryModel
{
    public string Path { get; set; }
    public int Limit { get; set; }
    public int Offset { get; set; }
    public FilterModel Filter { get; set; }
    public string SortField { get; set; }
    public string SortDir { get; set; }
}
