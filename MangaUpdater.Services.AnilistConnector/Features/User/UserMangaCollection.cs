using System.Text.Json;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using MangaUpdater.Services.AnilistConnector.Models;
using MangaUpdater.Shared.DTOs;
using MangaUpdater.Shared.Models;
using MediatR;

namespace MangaUpdater.Services.AnilistConnector.Features.User;

public record GetUserMangaCollectionQuery(string Username) : IRequest<List<AnilistUserMangaCollectionDto>>;

public class UserMangaCollection : IRequestHandler<GetUserMangaCollectionQuery, List<AnilistUserMangaCollectionDto>>
{
    private readonly GraphQLHttpClient _client;
    private readonly HttpClient _databaseClient;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
   
    public UserMangaCollection(IHttpClientFactory httpClientFactory)
    {
      _client = new GraphQLHttpClient("https://graphql.anilist.co", new SystemTextJsonSerializer());
      _databaseClient = httpClientFactory.CreateClient("Database");
    }

    public async Task<List<AnilistUserMangaCollectionDto>> Handle(GetUserMangaCollectionQuery request, CancellationToken cancellationToken)
    {
      var query = """
        query GetUserMangaList($username: String!) {
          MediaListCollection(userName: $username, type: MANGA, status: CURRENT) {
            lists {
              entries {
                progress
                media {
                  id
                  idMal
                  countryOfOrigin
                  siteUrl
                  coverImage {
                    extraLarge
                  }
                  title {
                    english
                    romaji
                  }
                  status
                  genres
                  averageScore
                }
              }
            }
          }
        } 
      """;
      var graphQlRequest = new GraphQLRequest
      {
        Query = query,
        Variables = new { username = request.Username }
      };

      try
      {
        var response = await _client.SendQueryAsync<UserMangaCollectionResponse>(graphQlRequest, cancellationToken: cancellationToken);

        var collection = response.Data.MediaListCollection?.Lists
          .SelectMany(x => x.Entries)
          .Where(x => x.Media is not null)
          .Select(x => new AnilistUserMangaCollectionDto
          (
            x.Media!.Id ?? 0,
            x.Media.IdMal ?? 0,
            x.Media.Status ?? string.Empty,
            x.Media.AverageScore ?? 0,
            x.Media.CountryOfOrigin ?? string.Empty,
            GetPreferredTitle(x.Media),
            x.Progress ?? 0,
            x.Media.IdMal.HasValue ? $"https://myanimelist.net/manga/{x.Media.IdMal}" : string.Empty,
            x.Media.SiteUrl ?? string.Empty,
            x.Media.CoverImage?.ExtraLarge ?? string.Empty,
            x.Media.Genres
          ))
            .ToList() ?? [];

            await EnsureMangasExist(response.Data.MediaListCollection?.Lists ?? [], cancellationToken);

          return collection;
      } catch (Exception)
      {
        return [];
      }
    }

    private async Task EnsureMangasExist(List<List> lists, CancellationToken cancellationToken)
    {
      var entries = lists.SelectMany(x => x.Entries).ToList();

      foreach (var entry in entries)
      {
        if (entry.Media?.Id is null)
        {
          continue;
        }

        var ensureRequest = new EnsureMangaRequest(
          entry.Media.Id.Value,
          entry.Media.IdMal,
          entry.Media.Title?.Romaji ?? entry.Media.Title?.English ?? string.Empty,
          entry.Media.Title?.English,
          entry.Media.CoverImage?.ExtraLarge ?? string.Empty,
          true,
          null);

        using var response = await _databaseClient.PostAsJsonAsync("api/manga/ensure", ensureRequest, cancellationToken);
        response.EnsureSuccessStatusCode();
        _ = await response.Content.ReadFromJsonAsync<MangaDto>(JsonOptions, cancellationToken);
      }
    }

    private static string GetPreferredTitle(Media media)
    {
      if (media.CountryOfOrigin == "JP")
      {
        return media.Title?.Romaji ?? media.Title?.English ?? string.Empty;
      }

      return media.Title?.English ?? media.Title?.Romaji ?? string.Empty;
    }
}