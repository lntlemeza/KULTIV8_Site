using KULTIV8.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KULTIV8.Data;

public class ApplicationDBContext : IdentityDbContext<UserModel>
{
    //Extablishing the connection with EF

    //The Options are used to configure the behaviour of the context and its underlying databse connections.
    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
    {

    }

    //Creating the Post Table
    public DbSet<Post> Posts { get; set; }

}
