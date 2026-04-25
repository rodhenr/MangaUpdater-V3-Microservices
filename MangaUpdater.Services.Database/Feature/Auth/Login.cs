using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MangaUpdater.Services.Database.Database;
using MangaUpdater.Shared.Helpers;
using MangaUpdater.Shared.Exceptions;
using MangaUpdater.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MangaUpdater.Services.Database.Feature.Auth;

public record LoginCommand(LoginRequest Request) : IRequest<LoginResponseDto>;

public class LoginHandler : IRequestHandler<LoginCommand, LoginResponseDto>
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public LoginHandler(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Where(u => u.Username == request.Request.Username)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null || !PasswordHasher.Verify(request.Request.Password, user.PasswordHash)) 
            throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized, "Invalid credentials");

        if (!user.Role.Equals("admin", StringComparison.CurrentCultureIgnoreCase)) 
            throw new HttpResponseException(System.Net.HttpStatusCode.Forbidden, "Insufficient role");

        var jwtKey = _configuration["Jwt:Key"] ?? "dev-secret-change-me";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddHours(8);

        var claims = new[] {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

        var userDto = new LoginUserDto(user.Id, user.Username, user.Role);
        return new LoginResponseDto(tokenStr, expires, userDto);
    }
}
