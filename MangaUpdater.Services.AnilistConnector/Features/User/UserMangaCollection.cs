using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using MangaUpdater.Services.AnilistConnector.Models;
using MangaUpdater.Shared.DTOs;

namespace MangaUpdater.Services.AnilistConnector.Features.User;

[RegisterScoped]
public class UserMangaCollection
{
    private readonly GraphQLHttpClient _client;
   
    public UserMangaCollection()
    {
      _client = new GraphQLHttpClient("https://graphql.anilist.co", new SystemTextJsonSerializer());
    }

    public async Task<List<AnilistUserMangaCollectionDto>> GetCollection(string username)
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
      var request = new GraphQLRequest
      {
        Query = query,
        Variables = new { username }
      };

      try
      {
        var response = await _client.SendQueryAsync<UserMangaCollectionResponse>(request);

        return response.Data.MediaListCollection?.Lists
          .SelectMany(x => x.Entries)
          .Select(x => new AnilistUserMangaCollectionDto
          (
            x.Media.Id ?? 0,
            x.Media.IdMal ?? 0,
            x.Media.Status,
            x.Media.AverageScore,
            x.Media.CountryOfOrigin,
            x.Media.CountryOfOrigin == "JP" ? x.Media.Title.Romaji : x.Media.Title.English,
            x.Progress ?? 0,
            $"https://myanimelist.net/manga/{x.Media.IdMal}",
            x.Media.SiteUrl,
            x.Media.CoverImage.ExtraLarge,
            x.Media.Genres
          ))
          .ToList() ?? [];
      } catch (GraphQLHttpRequestException ex)
      {
        return [];
      }
    }
}