using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InccApi.Models;

[Table("incc_entries")]
public class InccEntry
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Column(TypeName = "date")]
    public DateTime ReferenceDate { get; set; }
    
    [Required]
    [Range(0.0001, double.MaxValue)]
    [Column(TypeName = "decimal(18,4)")]
    public decimal Value { get; set; }
}
