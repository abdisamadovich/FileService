namespace WebTotalCommander.FileAccess.Models.Common;

public class SubFilterModel
{
    public string Logic { get; set; }
    public List<FilterDefinitionModel> Filters { get; set; }
}
