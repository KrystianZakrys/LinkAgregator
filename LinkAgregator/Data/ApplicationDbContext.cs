using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LinkAgregator.Data.Models;

namespace LinkAgregator.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Link> Links { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
