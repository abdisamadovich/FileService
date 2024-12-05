using System.ComponentModel.DataAnnotations;

namespace WebTotalCommander.FileAccess.Models.Common;

public class FilterDefinitionModel
{
    [Required]
    public string Field { get; set; }
    [Required]
    public string Operator { get; set; }

    public string Value { get; set; }
}
