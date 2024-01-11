public class Anime
{
    public int ID { get; set; }
    public string Title { get; set; }
    public int Episode { get; set; }
    public int Season { get; set; }
    public string Status { get; set; }
    public string Premiered { get; set; }
    public int Popularity { get; set; }

    public Anime()
    {

    }
    public Anime(int id, string title, int episode, int season, string status, string premiered, int popularity)
    {
        Title = title;
        Episode = episode;
        Season = season;
        Status = status;
        Premiered = premiered;
        Popularity = popularity;
        ID = id;
    }
}
