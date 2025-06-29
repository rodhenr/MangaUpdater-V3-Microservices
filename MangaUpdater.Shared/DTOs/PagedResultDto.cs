namespace MangaUpdater.Shared.DTOs;

public record PagedResultDto<T>(List<T> Items, int TotalItems, int PageNumber, int PageSize, int TotalPages);