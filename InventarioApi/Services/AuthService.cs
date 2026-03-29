using InventarioApi.DTOs;
using InventarioApi.Models;
using InventarioApi.Repositories;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Numerics;
using System.Security.Claims;
using System.Text;

namespace InventarioApi.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;
    private readonly IConfiguration _config;

    public AuthService(IUserRepository userRepo, IConfiguration config)
    {
        _userRepo = userRepo;
        _config = config;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepo.GetByUsernameAsync(request.Username)
            ?? throw new UnauthorizedAccessException("Usuario o contraseña incorrectos");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Usuario o contraseña incorrectos");

        return GenerateToken(user);
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _userRepo.UsernameExistsAsync(request.Username))
            throw new InvalidOperationException("Nombre de usuario ya registrado");

        if (await _userRepo.EmailExistsAsync(request.Email))
            throw new InvalidOperationException("Email ya registrado");

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role
        };

        var created = await _userRepo.CreateAsync(user);
        return GenerateToken(created);
    }

    private LoginResponse GenerateToken(User user)
    {
        var jwtSettings = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.UtcNow.AddMinutes(
            double.Parse(jwtSettings["ExpiryMinutes"]!));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        
        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expiry,
            signingCredentials: creds);

        return new LoginResponse(
            Token: new JwtSecurityTokenHandler().WriteToken(token),
            Username: user.Username,
            Role: user.Role,
            ExpiresAt: expiry);
    }
}