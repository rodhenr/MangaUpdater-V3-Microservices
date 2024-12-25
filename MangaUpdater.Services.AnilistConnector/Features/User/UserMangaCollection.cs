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

      var response = await _client.SendQueryAsync<UserMangaCollectionResponse>(request);
      
      return response.Data.MediaListCollection?.Lists
        .SelectMany(x => x.Entries)
        .Select(x => new AnilistUserMangaCollectionDto
        (
          x.Media.IdMal ?? 0,
          x.Media.CountryOfOrigin == "JP" ? x.Media.Title.Romaji : x.Media.Title.English,
          x.Progress ?? 0,
          $"https://myanimelist.net/manga/{x.Media.IdMal}",
          x.Media.SiteUrl,
          x.Media.CoverImage.ExtraLarge
        ))
        .ToList() ?? [];
    }
}