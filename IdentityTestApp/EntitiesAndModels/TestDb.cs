using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace IdentityTestApp.EntitiesAndModels;

public class TestDb
{
    public int Id { get; set; }
    public string? Name { get; set; }
    
    [ForeignKey("Name")]
    public IdentityUser IdentityUser { get; set; }
}