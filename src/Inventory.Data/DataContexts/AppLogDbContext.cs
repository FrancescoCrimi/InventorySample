using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Data.DataContexts
{
    public class AppLogDbContext : DbContext
    {
        public AppLogDbContext(DbContextOptions options)
            :base(options)
        {
        }

        public DbSet<AppLog> Logs { get; set; }
    }
}
