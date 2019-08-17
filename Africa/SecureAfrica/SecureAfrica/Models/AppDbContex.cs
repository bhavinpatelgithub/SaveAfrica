using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureAfrica.Models
{
    public class AppDbContex : IdentityDbContext<ApplicationUser>
    {
        public AppDbContex(DbContextOptions<AppDbContex> options) :base(options)
        {

        }
        
        public DbSet<TestModel> TestModels { get; set; }
        public DbSet<Test> Tests { get; set; }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Conventions

        //        // modelBuilder.Conventions.Add(
        //        //new AttributeToColumnAnnotationConvention<TableColumnAttribute, string>(
        //        //    "ServiceTableColumn", (property, attributes) => attributes.Single().ColumnType.ToString()));


        //    // modelBuilder.Entity<Person>()
        //    //.Property(p => p.DisplayName)
        //    //.HasComputedColumnSql("[LastName] + ', ' + [FirstName]");
        //}

    }
}
