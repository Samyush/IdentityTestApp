using Microsoft.AspNetCore.Identity;

namespace IdentityTestApp.EntitiesAndModels;

//here we are creating a class that inherits from IdentityUser i.e customizing the IdentityUser class to our needs adding properties to it as below
public class AppUsers : IdentityUser
{
    public string FirstName { get; set; }

    public string ImageURL { get; set; }

    public byte[] ProfileImage { get; set; }
}