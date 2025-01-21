using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace API_TFG.Models.Domain
{
    public class User : IdentityUser<Guid>
    {
        //Propiedades de navegación
        public ICollection<File> Files { get; set; } = new List<File>();

    }
}
