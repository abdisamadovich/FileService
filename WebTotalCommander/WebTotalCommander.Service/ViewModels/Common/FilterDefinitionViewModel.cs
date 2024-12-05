using System.ComponentModel.DataAnnotations;

namespace WebTotalCommander.Service.ViewModels.Common;

public class FilterDefinitionViewModel
{
    [Required]
    public string Field { get; set; }
    [Required]
    public string Operator { get; set; }

    public string Value { get; set; }
}
