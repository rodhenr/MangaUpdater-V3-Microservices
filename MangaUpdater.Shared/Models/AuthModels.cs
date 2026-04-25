using System;

namespace MangaUpdater.Shared.Models;

public record LoginRequest(string Username, string Password);

public record LoginUserDto(int Id, string Username, string Role);

public record LoginResponseDto(string Token, DateTime ExpiresAt, LoginUserDto User);
