using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication4.Models;

namespace WebApplication4.Hubs
{ 
   
    public class LoginHub : Hub<ILoginHub>
    {
        readonly IConfiguration _configuration;
        readonly DataContext.Data _context;
        public LoginHub(IConfiguration configuration, DataContext.Data context)
        {
            _configuration = configuration;
            _context = context;
        }
        public async Task Create(string userName, string password)
        {
            await _context.users.AddAsync(new User
            {
                Password = password,
                Username = userName
            });

            await Clients.Caller.Create(await _context.SaveChangesAsync() > 0);
        }
        public async Task Login(string userName, string password)
        {
            User user = await _context.users.FirstOrDefaultAsync(u => u.Username == userName && u.Password == password);
            Token token = null;
            if (user != null)
            {
                TokenHandler tokenHandler = new TokenHandler(_configuration);
                token = tokenHandler.CreateAccessToken(5);
                user.RefreshToken = token.RefreshToken;
                user.RefreshTokenEndDate = token.Expiration.AddMinutes(3);
                await _context.SaveChangesAsync();
            }
            await Clients.Caller.Login(user != null ? token : null);
        }
        public async Task RefreshTokenLogin(string refreshToken)
        {
            User user = await _context.users.FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);
            Token token = null;
            if (user != null && user?.RefreshTokenEndDate > DateTime.Now)
            {
                TokenHandler tokenHandler = new TokenHandler(_configuration);
                token = tokenHandler.CreateAccessToken(1);

                user.RefreshToken = token.RefreshToken;
                user.RefreshTokenEndDate = token.Expiration.AddMinutes(1);
                await _context.SaveChangesAsync();
            }
            await Clients.Caller.Login(user != null ? token : null);
        }
        public async Task<string> GetAccessToken()
        {
            Token token = null;
            User user = await _context.users.FirstOrDefaultAsync();
            if (user != null && user?.RefreshTokenEndDate > DateTime.Now)
            {
                TokenHandler tokenHandler = new TokenHandler(_configuration);
                token = tokenHandler.CreateAccessToken(1);

                user.RefreshToken = token.RefreshToken;
                user.RefreshTokenEndDate = token.Expiration.AddMinutes(1);
                await _context.SaveChangesAsync();
            }
            return token != null ? token.AccessToken : null;
        }
    }
}
