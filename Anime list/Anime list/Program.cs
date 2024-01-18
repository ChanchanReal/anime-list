using System.Drawing;
using System.Runtime.CompilerServices;
using HtmlAgilityPack;
using static System.Net.WebRequestMethods;

/// <summary>
/// This project is based on myanimelist.net
/// </summary>

AnimeListHandler animeList = new AnimeListHandler();
animeList.Start();

// unused
public enum Status { Finished, Unwatched, On_Hold}