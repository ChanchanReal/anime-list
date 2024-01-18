using System.Text.Json;

public class DataManager
{
    private readonly string dataPath; 
    private readonly AnimeListHandler listHandler;

    public DataManager() { }
    public DataManager(AnimeListHandler handler)
    {
        listHandler = handler;
        string appRoamingPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AnimeList");
        dataPath = Path.Combine(appRoamingPath, "AnimeList.json");
    }
    // this method convert anime list to json format and save it to json format.
    public bool SaveAnimeList(List<Anime> animes)
    {
        try
        {
            string? directoryPath = Path.GetDirectoryName(dataPath);
            if (!Directory.Exists(directoryPath))
            {
                if(directoryPath != null)
                Directory.CreateDirectory(directoryPath);
            }

            using (FileStream fileStream = File.Open(dataPath, FileMode.Create))
            using (StreamWriter sw = new StreamWriter(fileStream))
            {
                foreach (Anime anime in animes)
                {
                    string jsonSerializedString = JsonSerializer.Serialize(anime);
                    sw.WriteLine(jsonSerializedString);
                }
            }
            return true;
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Error saving anime list: {ex.Message}");
            return false;
        }
    }
    // this method get strings and deserialized and return it to main handler.
    public bool ReadAnimeData()
    {
        List<Anime>animes = new List<Anime>();
        try
        {
            using (FileStream fileStream = File.Open(dataPath, FileMode.Open))
            using (StreamReader sr = new StreamReader(fileStream))
            {

                while (!sr.EndOfStream)
                {
                    string? token = sr.ReadLine();

                    Anime? deserializedString = JsonSerializer.Deserialize<Anime>(token);
                    if (deserializedString != null)
                        animes.Add(deserializedString);
                }
            }
            listHandler.animeList = animes;
            
        } 
        catch (ArgumentException) { }
        catch ( DirectoryNotFoundException) { }
        return true;
    }
}