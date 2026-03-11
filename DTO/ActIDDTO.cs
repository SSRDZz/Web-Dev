using System.ComponentModel.DataAnnotations;

namespace KMITL_WebDev_MiniProject.DTO;
public class ActIDDTO
{
    [Required]
    public Guid ActID {get; set;}
}