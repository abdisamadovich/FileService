namespace WebTotalCommander.Service.ViewModels.Common;

public class FolderGetAllQueryViewModel
{
    public string Path { get; set; }
    public int Limit { get; set; }
    public int Offset { get; set; }
    public FilterViewModel Filter { get; set; }
    public string SortField { get; set; }
    public string SortDir { get; set; }
}
