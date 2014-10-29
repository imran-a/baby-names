using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Names
{
    public class Database : DbContext
    {
        public Database()
        {

        }

        public DbSet<FinnishName> FinnishNames { get; set; }
        public DbSet<PakistanName> PakistaniNames { get; set; }
    }

    public class PakistanName
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class FinnishName
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
