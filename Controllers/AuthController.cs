using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StockFlow360.Application.DTOs;
using StockFlow360.Domain;
using StockFlow360.Infrastructure.Data;

namespace StockFlow360.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDTO dto)
    {
        var user = new ApplicationUser
        {
            UserName = dto.Username,
            Email = dto.Email
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        // Only allow "Supplier" or fallback to default "Cashier"
        string assignedRole = dto.Role?.ToLower() == "supplier" ? "Supplier" : "Cashier";

        if (!await _roleManager.RoleExistsAsync(assignedRole))
            return BadRequest($"Role '{assignedRole}' does not exist.");

        await _userManager.AddToRoleAsync(user, assignedRole);
        await _signInManager.SignInAsync(user, isPersistent: false);

        return Ok($"User registered and logged in as {assignedRole}.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return Unauthorized("Invalid email or password");

        var result = await _signInManager.PasswordSignInAsync(user, dto.Password, isPersistent: false, lockoutOnFailure: false);

        if (!result.Succeeded)
            return Unauthorized("Invalid login attempt");

        return Ok("User logged in successfully.");
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok("User logged out successfully.");
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("set-role")]
    public async Task<IActionResult> SetUserRole(SetUserRoleDTO dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.UserEmail);
        if (user == null)
            return NotFound("User not found");

        if (!await _roleManager.RoleExistsAsync(dto.NewRole))
            return BadRequest("Invalid role");

        var currentRoles = await _userManager.GetRolesAsync(user);
        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded)
            return BadRequest("Failed to remove current roles");

        var addResult = await _userManager.AddToRoleAsync(user, dto.NewRole);
        if (!addResult.Succeeded)
            return BadRequest("Failed to assign new role");

        return Ok($"User role updated to {dto.NewRole}");
    }

}
