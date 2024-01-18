using System;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
public class MALDataRetriever
{
    #region Properties and Fields

    private AnimeListHandler listHandler;
    private const string url = "https://myanimelist.net/";

    #endregion

    #region Constructor
    public MALDataRetriever(AnimeListHandler listHandler)
    {
        this.listHandler = listHandler;
    }
    #endregion

    #region Methods
    public void SearchAnime(string title)
    {
        // convert title to searched data string
        // implementing able to check info of specific anime

        string titleSearch = ConvertStringToSearchable(title);
        string search = Path.Combine(url, $"search/all?q={titleSearch}");

        using (HttpClient http = new HttpClient())
        {
            http.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                "AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

            try
            {
                HtmlWeb htmlWeb = new HtmlWeb();
                Task<HtmlDocument> htmlDoc = LoadHtml(search, htmlWeb);
                htmlDoc.Wait();
                HtmlDocument htmlDocument = htmlDoc.Result;
                var result = htmlDocument.DocumentNode.SelectNodes("//div[@class='list di-t w100']//div[@class='title']");
      
                foreach (var anime in result)
                {
                    string animeTitle = RemoveSubstring(anime.InnerText, "add");
                    Console.WriteLine(animeTitle.Trim());
                }
                Console.WriteLine();
            }
            catch
            {

            }
        }
    }
    public void DisplaySeasonalAnime()
    {
        string seasonUrl = Path.Combine(url, "anime/season");
        using (HttpClient http = new HttpClient())
        {
            http.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                "AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
            try
            {
                HtmlWeb htmlWeb = new HtmlWeb();
                Task<HtmlDocument> html = LoadHtml(seasonUrl, htmlWeb);
                html.Wait();
                HtmlDocument htmlDocument = html.Result;

                var titles = htmlDocument.DocumentNode.SelectNodes("//h2[@class='h2_anime_title']");
                //var episode = htmlDocument.DocumentNode.SelectNodes("//");
                //var score = htmlDocument.DocumentNode.SelectNodes("//span[@class='js-score']");


                foreach (var title in titles)
                {
                    Console.WriteLine(title.InnerText);
                }
            }
            catch (AggregateException)
            {

            }
        }
    }
    /// <summary>
    /// Update the popularity in the anime list
    /// </summary>

    [Obsolete("To do: Implement this method.", true)]
    public void UpdatePopularity()
    {
        
    }

    /// <summary>
    /// The goal of this method is search a single anime
    /// display info, popularity,episode,etc
    /// </summary>

    [Obsolete("To do: Implement this method.", true)]
    public void SearchSingleAnime()
    {

    }
    private Task<HtmlDocument> LoadHtml(string url, HtmlWeb web)
    {
        return Task.Run(() =>
        {
            return web.LoadFromWebAsync(url);

        });
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

    private string RemoveSubstring(string inputStr, string substring)
    {
        return inputStr.Replace(substring, string.Empty);
    }
    #endregion
}
