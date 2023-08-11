using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication4.Models;

namespace WebApplication4.Hubs
{
    public interface ILoginHub
    {
        Task Login(Token token);
        Task Create(bool result);
    }
}
