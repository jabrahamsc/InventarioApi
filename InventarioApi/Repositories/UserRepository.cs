using System.Drawing;
using System;
using InventarioApi.Data;
using InventarioApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InventarioApi.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context) => _context = context;

    public async Task<User?> GetByUsernameAsync(string username) =>
        await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

    public async Task<User?> GetByIdAsync(int id) =>
        await _context.Users.FindAsync(id);

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> UsernameExistsAsync(string username) =>
        await _context.Users.AnyAsync(u => u.Username == username);

    public async Task<bool> EmailExistsAsync(string email) =>
        await _context.Users.AnyAsync(u => u.Email == email);
}