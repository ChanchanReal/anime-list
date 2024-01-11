using System.Data;
using System.Reflection;

public class AnimeListHandler
{
    private DataTable dt = new DataTable();
    private readonly DataManager _dataManager;
    private readonly MALDataRetriever _dataRetriever;
    public List<Anime> animeList = new List<Anime>();

    public AnimeListHandler()
    {
        _dataManager = new DataManager(this);
        _dataRetriever = new MALDataRetriever(this);
        SetDatatableColumn();
    }
   

    public void Start()
    {
        _dataManager.ReadAnimeData();
        AddListToDT();
        DisplayDT();
        while (true)
        {
            
            Menu();

        }
        
    }
    public void Menu()
    {
        Console.WriteLine(@"Enter which choice you want
1. Add anime
2. Display datatable
3. Save Data
4. Edit
5. Search anime
6. Display seasonal anime");
        string choice = Input("Enter choice: ");
        switch (choice.ToUpper())
        {
            case "ADD":
            case "1":
                AddAnime();
                break;

            case "2":
            case "DISPLAY":
                DisplayDT(); 
                break;

            case "3":
            case "SAVE":
                bool success = _dataManager.SaveAnimeList(animeList);
                Console.Clear();
                if (success) ColoredText("Data saved success.\n", ConsoleColor.Yellow);
                break;

            case "4":
            case "EDIT":
                Edit();
                break;

            case "SEARCH":
            case "5":
                string title = Input("Enter anime to search: ");
                Console.Clear();
                _dataRetriever.SearchAnime(title);
                break;

            case "DISPLAYSEASONAL":
            case "6":
                Console.Clear();
                _dataRetriever.DisplaySeasonalAnime();
                break;

            case "SINGLEANIME":
                throw new NotImplementedException("Soon");

            case "EXIT":
            case "QUIT":
                Environment.Exit(0);
                break;
        }
    }
    
    public void AddAnime()
    {
        Anime anime = new Anime();
        int ID = dt.Rows.Count;
        
        PropertyInfo[] properties = anime.GetType().GetProperties();
        foreach (PropertyInfo property in properties)
        {
            if (property.Name != "ID")
            {
                if (property.PropertyType == typeof(string))
                {
                    string setValue = Input($"Enter {property.Name}: ");
                    property.SetValue(anime, setValue);
                }
                else
                {
                    int setValue = Inputs($"Enter {property.Name}: ");
                    property.SetValue(anime, setValue);
                }
            }
            else
            {
                property.SetValue(anime, ID);
                Console.WriteLine(ID);
            }
        }
        animeList.Add(anime);
        dt.Rows.Add(anime.ID, anime.Title, anime.Episode, anime.Season, anime.Status, anime.Premiered, anime.Popularity);
    }

    private void DisplayDT()
    {
        Console.Clear();
        foreach (DataColumn column in dt.Columns)
        {
            if (column.ColumnName == "Title")
                Console.Write($"{column.ColumnName,-21}");
            else if (column.ColumnName == "ID")
                        Console.Write($"{column.ColumnName, -5}");
            else 
                Console.Write($"{column.ColumnName, -17}");
           
        }
        Console.WriteLine();
        Console.WriteLine("__________________________________________________________________________________________________________________");

        foreach (DataRow row in dt.Rows)
        {
            int ID = (int)row["ID"];
            string Title = (string)row["Title"];
            int Episode = (int)row["Episode"];
            int Season = (int)row["Season"];
            string Status = (string)row["Status"];
            string Premiered = (string)row["Premiered"];
            int Popularity = (int)row["Popularity"];

            ColoredTextID(ID, ConsoleColor.Cyan);
            ColoredTextTitle(Title, ConsoleColor.White);
            ColoredText(Episode, ConsoleColor.Yellow);
            ColoredText(Season, ConsoleColor.Green);
            ColoredText(Status, ConsoleColor.Red);
            ColoredText(Premiered, ConsoleColor.Blue);
            Console.Write("#");
            ColoredText(Popularity, ConsoleColor.Green);
            Console.WriteLine();
        }
        Console.WriteLine();
    }
    public void Edit()
    {
        DisplayDT();

        Console.Write("Select ID you want to Edit: ");
        string? id = Console.ReadLine();
        bool success = int.TryParse(id, out int intID);
        foreach (Anime anime in animeList)
        {
            if (intID == anime.ID)
            {
                string? input = Input($"Edit Anime {anime.Title}? ");
                if (input.ToUpper() == "YES" || input.ToUpper() == "Y")
                {
                    PropertyInfo[] properties = anime.GetType().GetProperties();
                    foreach (var property in properties)
                    {
                        Console.Write($"Edit {property.Name}? ");
                        if (Console.ReadLine().ToLower() == "yes")
                        {
                            if (property.PropertyType == typeof(string))
                            {
                                Console.Write($"{property.Name}: ");
                                string? updateValue = Console.ReadLine();
                                property.SetValue(anime, updateValue);
                            }
                            else
                            {
                                try
                                {
                                    Console.Write($"{property.Name}: ");
                                    bool tryConvert = int.TryParse(Console.ReadLine(), out int num);
                                    if (tryConvert == true)
                                        property.SetValue(anime, num);

                                }
                                catch (FormatException)
                                {
                                    Console.WriteLine("You type a string or something try again.");
                                }
                            }
                        }
                            
                    }
                }
            }
        }
        AddListToDT();
    }
    private void AddListToDT()
    {
        
        dt.Rows.Clear();

        foreach (Anime anime in animeList)
        {
            DataRow existingRow = dt.AsEnumerable().FirstOrDefault(row => row.Field<int>("ID") == anime.ID);

            if (existingRow == null)
            dt.Rows.Add(anime.ID, anime.Title, anime.Episode, anime.Season, anime.Status, anime.Premiered, anime.Popularity);
        }
    }
    public void SetDatatableColumn()
    {
        Anime anime = new Anime();
        PropertyInfo[] properties = anime.GetType().GetProperties();
        foreach (PropertyInfo property in properties)
            dt.Columns.Add(property.Name, property.PropertyType);
    }

    private string Input(string txt)
    {
        Console.Write(txt + " ");
        string? input = Console.ReadLine();
        return input;
    }
    private int Inputs(string txt)
    {
        Console.Write(txt + " ");
        int inputs = Convert.ToInt32(Console.ReadLine());
        return inputs;
    }
    private void ColoredText(string txt, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write($"{txt, -17}");
        Console.ResetColor();
    }
    private void ColoredText(int txt, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write($"{txt, -17}");
        Console.ResetColor();
    }
    private void ColoredTextID(int txt, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write($"{txt, -5}");
        Console.ResetColor();
    }
    private void ColoredTextTitle(string txt, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write($"{txt,-21}");
        Console.ResetColor();
    }
}
