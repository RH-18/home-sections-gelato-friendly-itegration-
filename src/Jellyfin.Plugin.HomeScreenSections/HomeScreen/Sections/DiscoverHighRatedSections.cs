using System.Collections.Generic;
using MediaBrowser.Controller.Library;

namespace Jellyfin.Plugin.HomeScreenSections.HomeScreen.Sections;

public abstract class HighRatedDiscoverSection : DiscoverSection
{
    private readonly string _sectionId;
    private readonly string _endpoint;
    private readonly IReadOnlyDictionary<string, string> _parameters;

    protected HighRatedDiscoverSection(
        IUserManager userManager,
        string sectionId,
        string displayText,
        string endpoint,
        string? genre = null)
        : base(userManager)
    {
        _sectionId = sectionId;
        _endpoint = endpoint;
        DisplayText = displayText;
        _parameters = BuildParameters(genre);
    }

    public override string? Section => _sectionId;

    public override string? DisplayText { get; set; }
        = string.Empty;

    protected override string JellyseerEndpoint => _endpoint;

    protected override IReadOnlyDictionary<string, string>? JellyseerParameters => _parameters;

    private static IReadOnlyDictionary<string, string> BuildParameters(string? genre)
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>
        {
            ["voteAverageGte"] = "8",
            ["voteCountGte"] = "800",
            ["sortBy"] = "popularity.desc"
        };

        if (!string.IsNullOrWhiteSpace(genre))
        {
            parameters["genre"] = genre;
        }

        return parameters;
    }
}

public class DiscoverTrendingHighRatedSection : HighRatedDiscoverSection
{
    public DiscoverTrendingHighRatedSection(IUserManager userManager)
        : base(userManager, "DiscoverTrendingHighRated", "🔥 Trending", "/api/v1/discover/trending")
    {
    }
}

public class DiscoverPopularMoviesHighRatedSection : HighRatedDiscoverSection
{
    public DiscoverPopularMoviesHighRatedSection(IUserManager userManager)
        : base(userManager, "DiscoverPopularMoviesHighRated", "🎬 Popular Movies", "/api/v1/discover/movies")
    {
    }
}

public class DiscoverPopularTvHighRatedSection : HighRatedDiscoverSection
{
    public DiscoverPopularTvHighRatedSection(IUserManager userManager)
        : base(userManager, "DiscoverPopularTvHighRated", "📺 Popular TV", "/api/v1/discover/tv")
    {
    }
}

public class DiscoverMoviesActionHighRatedSection : HighRatedDiscoverSection
{
    public DiscoverMoviesActionHighRatedSection(IUserManager userManager)
        : base(userManager, "DiscoverMoviesActionHighRated", "Movie – Action", "/api/v1/discover/movies", "28")
    {
    }
}

public class DiscoverMoviesAdventureHighRatedSection : HighRatedDiscoverSection
{
    public DiscoverMoviesAdventureHighRatedSection(IUserManager userManager)
        : base(userManager, "DiscoverMoviesAdventureHighRated", "Movie – Adventure", "/api/v1/discover/movies", "12")
    {
    }
}

public class DiscoverTvActionAdventureHighRatedSection : HighRatedDiscoverSection
{
    public DiscoverTvActionAdventureHighRatedSection(IUserManager userManager)
        : base(userManager, "DiscoverTvActionAdventureHighRated", "TV – Action & Adventure", "/api/v1/discover/tv", "10759")
    {
    }
}

public class DiscoverMoviesThrillerHighRatedSection : HighRatedDiscoverSection
{
    public DiscoverMoviesThrillerHighRatedSection(IUserManager userManager)
        : base(userManager, "DiscoverMoviesThrillerHighRated", "Movie – Thriller", "/api/v1/discover/movies", "53")
    {
    }
}

public class DiscoverMoviesMysteryHighRatedSection : HighRatedDiscoverSection
{
    public DiscoverMoviesMysteryHighRatedSection(IUserManager userManager)
        : base(userManager, "DiscoverMoviesMysteryHighRated", "Movie – Mystery", "/api/v1/discover/movies", "9648")
    {
    }
}

public class DiscoverTvMysteryHighRatedSection : HighRatedDiscoverSection
{
    public DiscoverTvMysteryHighRatedSection(IUserManager userManager)
        : base(userManager, "DiscoverTvMysteryHighRated", "TV – Mystery", "/api/v1/discover/tv", "9648")
    {
    }
}

public class DiscoverMoviesCrimeHighRatedSection : HighRatedDiscoverSection
{
    public DiscoverMoviesCrimeHighRatedSection(IUserManager userManager)
        : base(userManager, "DiscoverMoviesCrimeHighRated", "Movie – Crime", "/api/v1/discover/movies", "80")
    {
    }
}

public class DiscoverTvCrimeHighRatedSection : HighRatedDiscoverSection
{
    public DiscoverTvCrimeHighRatedSection(IUserManager userManager)
        : base(userManager, "DiscoverTvCrimeHighRated", "TV – Crime", "/api/v1/discover/tv", "80")
    {
    }
}

public class DiscoverMoviesFamilyHighRatedSection : HighRatedDiscoverSection
{
    public DiscoverMoviesFamilyHighRatedSection(IUserManager userManager)
        : base(userManager, "DiscoverMoviesFamilyHighRated", "Movie – Family", "/api/v1/discover/movies", "10751")
    {
    }
}

public class DiscoverTvFamilyHighRatedSection : HighRatedDiscoverSection
{
    public DiscoverTvFamilyHighRatedSection(IUserManager userManager)
        : base(userManager, "DiscoverTvFamilyHighRated", "TV – Family", "/api/v1/discover/tv", "10751")
    {
    }
}

public class DiscoverTvKidsHighRatedSection : HighRatedDiscoverSection
{
    public DiscoverTvKidsHighRatedSection(IUserManager userManager)
        : base(userManager, "DiscoverTvKidsHighRated", "TV – Kids", "/api/v1/discover/tv", "10762")
    {
    }
}

public class DiscoverMoviesAnimationHighRatedSection : HighRatedDiscoverSection
{
    public DiscoverMoviesAnimationHighRatedSection(IUserManager userManager)
        : base(userManager, "DiscoverMoviesAnimationHighRated", "Movie – Animation", "/api/v1/discover/movies", "16")
    {
    }
}

public class DiscoverTvAnimationHighRatedSection : HighRatedDiscoverSection
{
    public DiscoverTvAnimationHighRatedSection(IUserManager userManager)
        : base(userManager, "DiscoverTvAnimationHighRated", "TV – Animation", "/api/v1/discover/tv", "16")
    {
    }
}

public class DiscoverMoviesRomanceHighRatedSection : HighRatedDiscoverSection
{
    public DiscoverMoviesRomanceHighRatedSection(IUserManager userManager)
        : base(userManager, "DiscoverMoviesRomanceHighRated", "Movie – Romance", "/api/v1/discover/movies", "10749")
    {
    }
}

public class DiscoverMoviesComedyHighRatedSection : HighRatedDiscoverSection
{
    public DiscoverMoviesComedyHighRatedSection(IUserManager userManager)
        : base(userManager, "DiscoverMoviesComedyHighRated", "Movie – Comedy", "/api/v1/discover/movies", "35")
    {
    }
}

public class DiscoverTvComedyHighRatedSection : HighRatedDiscoverSection
{
    public DiscoverTvComedyHighRatedSection(IUserManager userManager)
        : base(userManager, "DiscoverTvComedyHighRated", "TV – Comedy", "/api/v1/discover/tv", "35")
    {
    }
}

public class DiscoverMoviesDramaHighRatedSection : HighRatedDiscoverSection
{
    public DiscoverMoviesDramaHighRatedSection(IUserManager userManager)
        : base(userManager, "DiscoverMoviesDramaHighRated", "Movie – Drama", "/api/v1/discover/movies", "18")
    {
    }
}

public class DiscoverTvDramaHighRatedSection : HighRatedDiscoverSection
{
    public DiscoverTvDramaHighRatedSection(IUserManager userManager)
        : base(userManager, "DiscoverTvDramaHighRated", "TV – Drama", "/api/v1/discover/tv", "18")
    {
    }
}

public class DiscoverMoviesHorrorHighRatedSection : HighRatedDiscoverSection
{
    public DiscoverMoviesHorrorHighRatedSection(IUserManager userManager)
        : base(userManager, "DiscoverMoviesHorrorHighRated", "Movie – Horror", "/api/v1/discover/movies", "27")
    {
    }
}

public class DiscoverMoviesScienceFictionHighRatedSection : HighRatedDiscoverSection
{
    public DiscoverMoviesScienceFictionHighRatedSection(IUserManager userManager)
        : base(userManager, "DiscoverMoviesScienceFictionHighRated", "Movie – Science Fiction", "/api/v1/discover/movies", "878")
    {
    }
}

public class DiscoverMoviesFantasyHighRatedSection : HighRatedDiscoverSection
{
    public DiscoverMoviesFantasyHighRatedSection(IUserManager userManager)
        : base(userManager, "DiscoverMoviesFantasyHighRated", "Movie – Fantasy", "/api/v1/discover/movies", "14")
    {
    }
}

public class DiscoverTvSciFiFantasyHighRatedSection : HighRatedDiscoverSection
{
    public DiscoverTvSciFiFantasyHighRatedSection(IUserManager userManager)
        : base(userManager, "DiscoverTvSciFiFantasyHighRated", "TV – Sci-Fi & Fantasy", "/api/v1/discover/tv", "10765")
    {
    }
}

public class DiscoverMoviesHistoryHighRatedSection : HighRatedDiscoverSection
{
    public DiscoverMoviesHistoryHighRatedSection(IUserManager userManager)
        : base(userManager, "DiscoverMoviesHistoryHighRated", "Movie – History", "/api/v1/discover/movies", "36")
    {
    }
}

public class DiscoverMoviesWarHighRatedSection : HighRatedDiscoverSection
{
    public DiscoverMoviesWarHighRatedSection(IUserManager userManager)
        : base(userManager, "DiscoverMoviesWarHighRated", "Movie – War", "/api/v1/discover/movies", "10752")
    {
    }
}

public class DiscoverTvWarPoliticsHighRatedSection : HighRatedDiscoverSection
{
    public DiscoverTvWarPoliticsHighRatedSection(IUserManager userManager)
        : base(userManager, "DiscoverTvWarPoliticsHighRated", "TV – War & Politics", "/api/v1/discover/tv", "10768")
    {
    }
}

public class DiscoverTvSoapHighRatedSection : HighRatedDiscoverSection
{
    public DiscoverTvSoapHighRatedSection(IUserManager userManager)
        : base(userManager, "DiscoverTvSoapHighRated", "TV – Soap", "/api/v1/discover/tv", "10766")
    {
    }
}
