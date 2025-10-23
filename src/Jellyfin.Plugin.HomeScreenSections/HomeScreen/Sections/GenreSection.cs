using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using Jellyfin.Plugin.HomeScreenSections;
using Jellyfin.Plugin.HomeScreenSections.Configuration;
using Jellyfin.Plugin.HomeScreenSections.Library;
using Jellyfin.Plugin.HomeScreenSections.Model.Dto;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Querying;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace Jellyfin.Plugin.HomeScreenSections.HomeScreen.Sections;

public class GenreSection : IHomeScreenSection
{
    private const int c_itemsPerSection = 20;

    private static readonly GenreCategory[] s_genreCategories =
    {
        new("trending", "🔥 Trending", "/api/v1/discover/trending?voteAverageGte=8&voteCountGte=800&sortBy=popularity.desc", "mixed"),
        new("popular-movies", "🎬 Popular Movies", "/api/v1/discover/movies?voteAverageGte=8&voteCountGte=800&sortBy=popularity.desc", "movie"),
        new("popular-tv", "📺 Popular TV", "/api/v1/discover/tv?voteAverageGte=8&voteCountGte=800&sortBy=popularity.desc", "tv"),
        new("movie-action", "Movie – Action", "/api/v1/discover/movie?voteAverageGte=8&voteCountGte=800&sortBy=popularity.desc&genre=28", "movie"),
        new("movie-adventure", "Movie – Adventure", "/api/v1/discover/movie?voteAverageGte=8&voteCountGte=800&sortBy=popularity.desc&genre=12", "movie"),
        new("tv-action-adventure", "TV – Action & Adventure", "/api/v1/discover/tv?voteAverageGte=8&voteCountGte=800&sortBy=popularity.desc&genre=10759", "tv"),
        new("movie-thriller", "Movie – Thriller", "/api/v1/discover/movie?voteAverageGte=8&voteCountGte=800&sortBy=popularity.desc&genre=53", "movie"),
        new("movie-mystery", "Movie – Mystery", "/api/v1/discover/movie?voteAverageGte=8&voteCountGte=800&sortBy=popularity.desc&genre=9648", "movie"),
        new("tv-mystery", "TV – Mystery", "/api/v1/discover/tv?voteAverageGte=8&voteCountGte=800&sortBy=popularity.desc&genre=9648", "tv"),
        new("movie-crime", "Movie – Crime", "/api/v1/discover/movie?voteAverageGte=8&voteCountGte=800&sortBy=popularity.desc&genre=80", "movie"),
        new("tv-crime", "TV – Crime", "/api/v1/discover/tv?voteAverageGte=8&voteCountGte=800&sortBy=popularity.desc&genre=80", "tv"),
        new("movie-family", "Movie – Family", "/api/v1/discover/movie?voteAverageGte=8&voteCountGte=800&sortBy=popularity.desc&genre=10751", "movie"),
        new("tv-family", "TV – Family", "/api/v1/discover/tv?voteAverageGte=8&voteCountGte=800&sortBy=popularity.desc&genre=10751", "tv"),
        new("tv-kids", "TV – Kids", "/api/v1/discover/tv?voteAverageGte=8&voteCountGte=800&sortBy=popularity.desc&genre=10762", "tv"),
        new("movie-animation", "Movie – Animation", "/api/v1/discover/movie?voteAverageGte=8&voteCountGte=800&sortBy=popularity.desc&genre=16", "movie"),
        new("tv-animation", "TV – Animation", "/api/v1/discover/tv?voteAverageGte=8&voteCountGte=800&sortBy=popularity.desc&genre=16", "tv"),
        new("movie-romance", "Movie – Romance", "/api/v1/discover/movie?voteAverageGte=8&voteCountGte=800&sortBy=popularity.desc&genre=10749", "movie"),
        new("movie-comedy", "Movie – Comedy", "/api/v1/discover/movie?voteAverageGte=8&voteCountGte=800&sortBy=popularity.desc&genre=35", "movie"),
        new("tv-comedy", "TV – Comedy", "/api/v1/discover/tv?voteAverageGte=8&voteCountGte=800&sortBy=popularity.desc&genre=35", "tv"),
        new("movie-drama", "Movie – Drama", "/api/v1/discover/movie?voteAverageGte=8&voteCountGte=800&sortBy=popularity.desc&genre=18", "movie"),
        new("tv-drama", "TV – Drama", "/api/v1/discover/tv?voteAverageGte=8&voteCountGte=800&sortBy=popularity.desc&genre=18", "tv"),
        new("movie-horror", "Movie – Horror", "/api/v1/discover/movie?voteAverageGte=8&voteCountGte=800&sortBy=popularity.desc&genre=27", "movie"),
        new("movie-science-fiction", "Movie – Science Fiction", "/api/v1/discover/movie?voteAverageGte=8&voteCountGte=800&sortBy=popularity.desc&genre=878", "movie"),
        new("movie-fantasy", "Movie – Fantasy", "/api/v1/discover/movie?voteAverageGte=8&voteCountGte=800&sortBy=popularity.desc&genre=14", "movie"),
        new("tv-sci-fi-fantasy", "TV – Sci-Fi & Fantasy", "/api/v1/discover/tv?voteAverageGte=8&voteCountGte=800&sortBy=popularity.desc&genre=10765", "tv"),
        new("movie-history", "Movie – History", "/api/v1/discover/movie?voteAverageGte=8&voteCountGte=800&sortBy=popularity.desc&genre=36", "movie"),
        new("movie-war", "Movie – War", "/api/v1/discover/movie?voteAverageGte=8&voteCountGte=800&sortBy=popularity.desc&genre=10752", "movie"),
        new("tv-war-politics", "TV – War & Politics", "/api/v1/discover/tv?voteAverageGte=8&voteCountGte=800&sortBy=popularity.desc&genre=10768", "tv"),
        new("tv-soap", "TV – Soap", "/api/v1/discover/tv?voteAverageGte=8&voteCountGte=800&sortBy=popularity.desc&genre=10766", "tv")
    };

    private readonly IUserManager m_userManager;

    public string? Section => "Genre";
    public string? DisplayText { get; set; } = "Genre";
    public int? Limit => s_genreCategories.Length;
    public string? Route => null;
    public string? AdditionalData { get; set; }
    public object? OriginalPayload => null;

    public GenreSection(IUserManager userManager)
    {
        m_userManager = userManager;
    }

    public QueryResult<BaseItemDto> GetResults(HomeScreenSectionPayload payload, IQueryCollection queryCollection)
    {
        string? categoryKey = payload.AdditionalData;
        if (string.IsNullOrEmpty(categoryKey))
        {
            return CreateEmptyResult();
        }

        GenreCategory? selectedCategory = s_genreCategories
            .FirstOrDefault(category => string.Equals(category.Key, categoryKey, StringComparison.OrdinalIgnoreCase));

        if (selectedCategory == null)
        {
            return CreateEmptyResult();
        }

        string? jellyseerrBaseUrl = HomeScreenSectionsPlugin.Instance.Configuration.JellyseerrUrl;
        string? jellyseerrApiKey = HomeScreenSectionsPlugin.Instance.Configuration.JellyseerrApiKey;

        if (string.IsNullOrEmpty(jellyseerrBaseUrl) || string.IsNullOrEmpty(jellyseerrApiKey))
        {
            return CreateEmptyResult();
        }

        User? user = m_userManager.GetUserById(payload.UserId);
        if (user?.Username == null)
        {
            return CreateEmptyResult();
        }

        using HttpClient httpClient = new HttpClient
        {
            BaseAddress = new Uri(jellyseerrBaseUrl)
        };

        httpClient.DefaultRequestHeaders.Add("X-Api-Key", jellyseerrApiKey);

        int? jellyseerrUserId = GetJellyseerrUserId(httpClient, user.Username);
        if (!jellyseerrUserId.HasValue)
        {
            return CreateEmptyResult();
        }

        httpClient.DefaultRequestHeaders.Add("X-Api-User", jellyseerrUserId.Value.ToString(CultureInfo.InvariantCulture));

        string[] preferredLanguages = (HomeScreenSectionsPlugin.Instance.Configuration.JellyseerrPreferredLanguages ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        List<BaseItemDto> items = GetCategoryItems(httpClient, selectedCategory, preferredLanguages, jellyseerrBaseUrl);

        return new QueryResult<BaseItemDto>
        {
            Items = items,
            StartIndex = 0,
            TotalRecordCount = items.Count
        };
    }

    public IHomeScreenSection CreateInstance(Guid? userId, IEnumerable<IHomeScreenSection>? otherInstances = null)
    {
        string? nextCategoryKey = GetNextCategoryKey(otherInstances);

        if (nextCategoryKey == null)
        {
            return new GenreSection(m_userManager)
            {
                DisplayText = DisplayText
            };
        }

        GenreCategory? category = s_genreCategories
            .FirstOrDefault(candidate => string.Equals(candidate.Key, nextCategoryKey, StringComparison.OrdinalIgnoreCase));

        if (category == null)
        {
            return new GenreSection(m_userManager)
            {
                DisplayText = DisplayText
            };
        }

        return new GenreSection(m_userManager)
        {
            AdditionalData = category.Key,
            DisplayText = category.Name
        };
    }

    public HomeScreenSectionInfo GetInfo()
    {
        return new HomeScreenSectionInfo
        {
            Section = Section,
            DisplayText = DisplayText,
            AdditionalData = AdditionalData,
            Route = Route,
            Limit = Limit ?? 1,
            OriginalPayload = OriginalPayload,
            ViewMode = SectionViewMode.Portrait,
            AllowViewModeChange = false
        };
    }

    private static int? GetJellyseerrUserId(HttpClient client, string username)
    {
        HttpResponseMessage usersResponse = client
            .GetAsync($"/api/v1/user?q={Uri.EscapeDataString(username)}")
            .GetAwaiter()
            .GetResult();

        if (!usersResponse.IsSuccessStatusCode)
        {
            return null;
        }

        string userResponseRaw = usersResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        JObject? parsedResponse = JObject.Parse(userResponseRaw);
        if (parsedResponse == null)
        {
            return null;
        }

        return parsedResponse
            .Value<JArray>("results")!
            .OfType<JObject>()
            .FirstOrDefault(result => string.Equals(result.Value<string>("jellyfinUsername"), username, StringComparison.OrdinalIgnoreCase))?
            .Value<int?>("id");
    }

    private static List<BaseItemDto> GetCategoryItems(HttpClient client, GenreCategory category, string[] preferredLanguages, string jellyseerrUrl)
    {
        List<BaseItemDto> items = new List<BaseItemDto>();
        HashSet<int> seenIds = new HashSet<int>();

        int page = 1;
        bool addedOnPage;

        do
        {
            addedOnPage = false;

            string endpoint = BuildPagedEndpoint(category.Endpoint, page);
            HttpResponseMessage response = client.GetAsync(endpoint).GetAwaiter().GetResult();

            if (!response.IsSuccessStatusCode)
            {
                break;
            }

            string jsonRaw = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            JObject? jsonResponse = JObject.Parse(jsonRaw);

            if (jsonResponse == null)
            {
                break;
            }

            foreach (JObject item in jsonResponse.Value<JArray>("results")!.OfType<JObject>())
            {
                if (item.Value<bool?>("adult") == true)
                {
                    continue;
                }

                if (preferredLanguages.Length > 0)
                {
                    string? originalLanguage = item.Value<string>("originalLanguage");
                    if (string.IsNullOrEmpty(originalLanguage) || !preferredLanguages.Contains(originalLanguage, StringComparer.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                }

                int? itemId = item.Value<int?>("id");
                if (!itemId.HasValue || !seenIds.Add(itemId.Value))
                {
                    continue;
                }

                if (item.Value<JObject>("mediaInfo") != null)
                {
                    continue;
                }

                items.Add(CreateDiscoverDto(item, category, jellyseerrUrl));
                addedOnPage = true;

                if (items.Count >= c_itemsPerSection)
                {
                    break;
                }
            }

            if (items.Count >= c_itemsPerSection)
            {
                break;
            }

            if (!addedOnPage)
            {
                break;
            }

            page++;
        }
        while (items.Count < c_itemsPerSection);

        return items;
    }

    private static BaseItemDto CreateDiscoverDto(JObject item, GenreCategory category, string jellyseerrUrl)
    {
        string? title = item.Value<string>("title") ?? item.Value<string>("name") ?? string.Empty;
        string? originalTitle = item.Value<string>("originalTitle") ?? item.Value<string>("originalName");
        string? posterPath = item.Value<string>("posterPath") ?? "404";
        string? mediaType = item.Value<string>("mediaType");

        DateTime premiereDate;
        string? releaseDate = item.Value<string>("firstAirDate") ?? item.Value<string>("releaseDate");
        if (!DateTime.TryParse(releaseDate, out premiereDate))
        {
            premiereDate = DateTime.Parse("1970-01-01", CultureInfo.InvariantCulture);
        }

        return new BaseItemDto
        {
            Name = title,
            OriginalTitle = originalTitle,
            SourceType = string.IsNullOrEmpty(mediaType) ? category.DefaultMediaType : mediaType,
            ProviderIds = new Dictionary<string, string>
            {
                { "JellyseerrRoot", jellyseerrUrl },
                { "Jellyseerr", item.Value<int>("id").ToString(CultureInfo.InvariantCulture) },
                { "JellyseerrPoster", posterPath }
            },
            PremiereDate = premiereDate
        };
    }

    private static string BuildPagedEndpoint(string endpoint, int page)
    {
        string normalized = endpoint.StartsWith('/') ? endpoint : "/" + endpoint;
        string separator = normalized.Contains('?') ? "&" : "?";

        if (normalized.Contains("page="))
        {
            return normalized;
        }

        return normalized + separator + "page=" + page.ToString(CultureInfo.InvariantCulture);
    }

    private static QueryResult<BaseItemDto> CreateEmptyResult()
    {
        return new QueryResult<BaseItemDto>
        {
            Items = new List<BaseItemDto>(),
            StartIndex = 0,
            TotalRecordCount = 0
        };
    }

    private string? GetNextCategoryKey(IEnumerable<IHomeScreenSection>? otherInstances)
    {
        HashSet<string> usedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (!string.IsNullOrEmpty(AdditionalData))
        {
            usedKeys.Add(AdditionalData);
        }

        if (otherInstances != null)
        {
            foreach (IHomeScreenSection? section in otherInstances)
            {
                if (!string.IsNullOrEmpty(section?.AdditionalData))
                {
                    usedKeys.Add(section.AdditionalData);
                }
            }
        }

        return s_genreCategories.FirstOrDefault(category => !usedKeys.Contains(category.Key))?.Key;
    }

    private sealed record GenreCategory(string Key, string Name, string Endpoint, string DefaultMediaType);
}
