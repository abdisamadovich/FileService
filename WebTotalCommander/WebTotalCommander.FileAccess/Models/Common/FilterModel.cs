using System.ComponentModel.DataAnnotations;

namespace WebTotalCommander.FileAccess.Models.Common;

public class FilterModel
{
    private List<SubFilterModel> _subFilters;

    [Required]
    public string Logic { get; set; }
    public List<SubFilterModel> Filters
    {
        get => _subFilters ??= new List<SubFilterModel>();
        set => _subFilters = value;
    }
}
