using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication4.Models;

namespace WebApplication4.DataContext
{
    public class Data:DbContext
    {
        public Data(DbContextOptions<Data> options) : base(options)
        {


        }

        public virtual DbSet<User> users { get; set; }
        public virtual  DbSet<Token> token { get; set; }
    }
}
