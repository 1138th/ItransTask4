using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace WebApplication4.Data;

[Table("AspNetUsers")]
public class User : IdentityUser
{
    public string? LastOnline { get; set; }
    public bool Status { get; set; }
}