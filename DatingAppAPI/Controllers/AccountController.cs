using System.Security.Cryptography;
using System.Text;
using DatingAppAPI.DbData;
using DatingAppAPI.DTOs;
using DatingAppAPI.Entities;
using DatingAppAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingAppAPI.Controllers;

public class AccountController:BaseApiController
{
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;

    public AccountController(DataContext dataContext, ITokenService tokenService)
    {
        _context = dataContext;
        _tokenService = tokenService;
    }
    [HttpPost("register")]
    public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)
    {
        if ( await UserExists(registerDto.Username.ToLower()))         return BadRequest("This user name has been taken");

        
            using var hmac = new HMACSHA512();

            var user = new AppUser()
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
            
    }
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(
            x => x.UserName == loginDto.Username);
        if (user == null) return Unauthorized();
        
        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computedHas = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
        
        for(int i=0; i<computedHas.Length; i++)
        {
            if (computedHas[i] != user.PasswordHash[i]) return Unauthorized("password incorrect");
        }

        return new UserDto()
        {
            Username = user.UserName,
            Token = _tokenService.CreateToken(user)
        };

    }

    private async Task<bool> UserExists(string username)
    {
        return await _context.Users.AnyAsync(x => x.UserName ==username);
    }
}