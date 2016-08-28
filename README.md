# UserAndRoles


 This demo shows a way to extend *ASP .NET Identity 2* - adding navigation properties between `ApplicatiionUser` and `IdentityRole`.
 
### Requirements

  * Visual studio 2015
  * SQL Server
  * 2 GB RAM
  * ASP .NET knowledge
  
### Installation
  
  * Download this repository or clone it.
  * Open `.sln` file with Visual Studio.
  * Check `Web.Config` and set the server.
  * Compile the project and run.
  
### Source code

[UserAndRoles/UserAndRolesDemo/UserAndRolesDemo/
](/UserAndRolesDemo/UserAndRolesDemo)

### Description

For this tutorial is used default ASP .Net MVC 5 Template with Individual users.
By default when you try access the roles of a user it will return a collection of `IdentityUserRole` which is a relation mapping between users and roles:
```csharp
ICollection<IdentityUserRole> roles = new ApplicationDbContext().Users.FirstOrDefault().Roles;
    
foreach(var role in roles)
{
    Console.WriteLine($"{role.UserId} {role.RoleId}");
    // But how to get the role name ?
}
```

Of course `UserManager<User>` can be used. It have a method for GetRoles(stirng id). But in most situations a user can be associated with many roles, so this method must be called for each user. `RoleManage` doesn't have an option to get users for a role.

To deal with User and Roles in ASP. NET, you should make some changes:<br/>
* Open `IdentityModels.cs`.
* Create class `UserRoles` that inherits `IdentityUserRole` and add two navigation properties:
```csharp
public class UserRoles : IdentityUserRole
{
    public virtual User User { get; set; }
    public virtual Role Role { get; set; }
}
```
* Create class `Role` that inherits `IdentityRole<string, UserRoles>`.
* Don't forget to generate Id for the role. (If the Id is number, you can set the auto-increment option when OnModelCreating occurs):
```csharp

public class Role : IdentityRole<string, UserRoles>
{
    public Role()
    {
        this.Id = Guid.NewGuid().ToString();
    }
}
```
* You can rename `ApplicationUser` as in this example it is just `User`.
* Inherit `IdentityUser<string, IdentityUserLogin, UserRoles, IdentityUserClaims>`:
* GenerateUserIdentityAsync should change the type of parameter to `UserManager<User, string>`
* Don't forget to generate Id. Same as the `Role` class.
* You can extend this class with additional properties. 
```csharp
public class User : IdentityUser<string, IdentityUserLogin, UserRoles, IdentityUserClaim>
{
   public User()
   {
      this.Id = Guid.NewGuid().ToString();
   }

    public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User, string> manager)
    {
        // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
        var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
        // Add custom user claims here
        return userIdentity;
    }

    public int Age { get; set; }
   
    public string Description { get; set; }
}
```

* Now `ApplicationDbContext` should inherits `IdentityDbContext<User, Role, string, IdentityUserLogin, UserRoles, IdentityUserClaim>`
```csharp
public class ApplicationDbContext : IdentityDbContext<User, Role, string, IdentityUserLogin, UserRoles, IdentityUserClaim>
{
    public ApplicationDbContext()
        : base("DefaultConnection")
    {
    }

    public static ApplicationDbContext Create()
    {
        return new ApplicationDbContext();
    }
}
```

When you try to compile the project you will receive few errors. In `IdentityConfig.cs` `ApplicationUserManager` shoud be changed a little. Inherit `UserManager<User, string>`, change the type of parameter in the constructor to `IUserStore<User, string>`. In Create method: initializtion of UserStore should be changed. **Everywhere `UserStore` is required, use it like** : **new UserStore&lt;User, Role, string, IdentityUserLogin, UserRoles, IdentityUserClaim>(context)**: 

```csharp
public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
{
    var store = new UserStore<User, Role, string, IdentityUserLogin, UserRoles, IdentityUserClaim>(context.Get<ApplicationDbContext>());
    new ApplicationUserManager(store);
    var manager = new ApplicationUserManager(store);
    // Configure validation logic for usernames
    manager.UserValidator = new UserValidator<User>(manager)
    {
        AllowOnlyAlphanumericUserNames = false,
        RequireUniqueEmail = true
    };
    
  /// the rest of the code is not changed.
```

Now check `GetUsersAndTheirRoles()` in `HomeController.cs`. The old way to get roles is not good, because it makes many queries to the database and it may be slow.
```csharp
var userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());
IEnumerable<IdentityUser> dbUsers = userManager.Users.ToList();

foreach (var user in dbUsers)
{
    model.Add(new UserViewModel()
    {
        UserName = user.UserName,
        Roles = userManager.GetRoles(user.Id)
    });
}

```

### The **SOLUTION** :
```csharp
model = new ApplicationDbContext().Users.Include(u => u.Roles).Select(u => new UserViewModel()
{
    Age = u.Age,
    UserName = u.UserName,
    Roles = u.Roles.Select(r => r.Role.Name) // now "r" will have the Role class which has a "Name"
}) 
.ToList();

```
Now this gets all users with their roles with minimum effort (one query).

### Web-Site Explanation

* When the site is initialized, `Global.asax.cs` is configured to create 5 roles (`Admin`,`Editor`,`Manager`,`Developer`,`Tester`) and 1 user associated with `Admin` and  `Manager` roles. And all this into the database.
* To login use **Administrator** for username(email) and **123456** for password
* In the home screen you will see in top information for current logged user and a dropdown menu with roles allows to add current user to the role.

![](/UserAndRolesDemo/UserAndRolesDemo/Content/Images/LoginFirstView.png)
* Below are two tables. In the first table are all users and roles for each one. First column represents the user name and the second column represents the roles associated.
* In the second table are listed all roles and users associated with each one. First column represents the role name and the second users associated with this role.

![](/UserAndRolesDemo/UserAndRolesDemo/Content/Images/LoginFirstView2.png)
* To test functionallity: register few users and associate this user in roles of your choice.
* Sample screenshots: 
![](/UserAndRolesDemo/UserAndRolesDemo/Content/Images/Users.png)
![](/UserAndRolesDemo/UserAndRolesDemo/Content/Images/Roles.png)

 *There are typo Rditor shoild be Editor
 
### Resources

 [Extending identityuserrole in identity 2](http://stackoverflow.com/questions/25857806/extending-identityuserrole-in-identity-2-0)

 [ASP NET Identity Customizing Users and Roles](http://www.codeproject.com/Articles/790720/ASP-NET-Identity-Customizing-Users-and-Roles)
 
 [applicationuser and applicationrole navigation properties in identity 2](http://stackoverflow.com/questions/24823092/applicationuser-and-applicationrole-navigation-properties-in-identity-2-0-0)
 
 [Extending primarykey of identityuserrole in identity 2](http://stackoverflow.com/questions/28721051/extending-primarykey-of-identityuserrole-in-identity-2)
 


#### Tags 
<meta name="keywords" content="ASP,ASP Identity,.NET,Entity Framework,IdentityUserRole,MSSQL,MVC,Extends,Microsoft,Users,Extends" />
 
 <strong> `ASP` </strong>   <strong> `.NET` </strong>  <strong> `ASP Identity` </strong>  <strong> `Entity Framework` </strong>  <strong> `ASP .NET` </strong>  <strong> `MVC` </strong>  <strong> `MSSQL` </strong>  <strong> `IdentityUserRole` </strong>  <strong> `Microsoft` </strong>  <strong> `Extends` </strong>  <strong> `Navigation property` </strong>  <strong> `Relation` </strong>  <strong> `Users` </strong>  <strong> `ASP User` </strong>  <strong> `Role` </strong>  <strong> `ASP Roles` </strong>
 
