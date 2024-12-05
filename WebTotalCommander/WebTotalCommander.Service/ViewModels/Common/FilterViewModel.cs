using System.ComponentModel.DataAnnotations;

namespace WebTotalCommander.Service.ViewModels.Common;

public class FilterViewModel
{
    private List<SubFilterViewModel> _subFilters;

    [Required]
    public string Logic { get; set; }
    public List<SubFilterViewModel> Filters
    {
        get => _subFilters ??= new List<SubFilterViewModel>();
        set => _subFilters = value;
    }
}
