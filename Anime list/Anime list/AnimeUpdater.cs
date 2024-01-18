using System.Reflection;
using HtmlAgilityPack;

public class AnimeUpdater
{
    #region Properties and Fields

    private AnimeListHandler _animeHandler;
    private List<Anime> animes = new List<Anime>();
    private List<string> animeTitle = new List<string>();
    private List<string> animeInfoUrls = new List<string>();
    private List<string> popularities = new List<string>();
    private const string myAnimeListUrl = "https://myanimelist.net/";
    #endregion

    #region Constructor
    public AnimeUpdater(AnimeListHandler handler)
    {
        _animeHandler = handler;
    }
    #endregion

    #region Methods

    public void Update()
    {
        GetAnimeTitles();

        foreach(string title in animeTitle)
        {
            string serializedUrl = ConvertStringToSearchable(title);
            string combinedUrl = Path.Combine(myAnimeListUrl, $"search/all?q={serializedUrl}");
            string url = GetAnimeInformationLinkAttribute(combinedUrl);
            animeInfoUrls.Add(url);
        }

        AnimePopularity();

        int index = 0;
        foreach(string popularity in popularities)
        {
            bool result = int.TryParse(popularity, out int popular);
            animes[index++].Popularity = popular;
        }

        _animeHandler.animeList = animes;

    }
    public void GetAnimeTitles()
    {
        animes = _animeHandler.animeList;

        foreach (Anime anime in animes)
        {
           PropertyInfo[] properties =  anime.GetType().GetProperties();
           foreach (PropertyInfo property in properties)
           {
                if (property.Name == "Title")
                animeTitle.Add((string)property.GetValue(anime));
           }
        }
    }
    private void AnimePopularity()
    {
        foreach (string url in animeInfoUrls)
        {
            using(HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                "AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

                HtmlWeb htmlWeb = new HtmlWeb();
                HtmlDocument htmlDocument = htmlWeb.LoadFromWebAsync(url).Result;

                var popularity = htmlDocument.DocumentNode.SelectSingleNode("//span[@class='numbers popularity']");

                if(popularity != null)
                {
                    string popularityText = popularity.SelectSingleNode("strong").InnerText;
                    string bashRemoved = SubstringRemove(popularityText, "#");
                    popularities.Add(bashRemoved);
                }
            }
        }
    }
    private string GetAnimeInformationLinkAttribute(string url)
    {
        string animeSpecificUrl = "";
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                "AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument htmlDocument = htmlWeb.LoadFromWebAsync(url).Result;

            var animeLink = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='title']//a/@href");
            animeSpecificUrl = animeLink.GetAttributeValue("href", "");
        }

        return animeSpecificUrl;
    }
    private string ConvertStringToSearchable(string title)
    {
        string searchCharacter = "";
        foreach (char txt in title)
        {
            if (txt == ' ')
            {
                searchCharacter += "%20";
            }
            else
                searchCharacter += txt;
        }
        return searchCharacter;
    }

    private string SubstringRemove(string txt, string sub)
    {
        return txt.Replace(sub, string.Empty);
    }
    #endregion
}