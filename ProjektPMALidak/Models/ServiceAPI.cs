using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProjektPMALidak.Models
{
    //6f8ce16b3e66b4057b7d9a117d4e7d2e muj API key na MAL API  link na API dokumentaci ... https://myanimelist.net/apiconfig/references/api/v2#section/Versioning

    public class APIservices
    {

        public List<Node> GetNodes(AnimeList list)
        {
            List<Node> nodes = new List<Node>();
            foreach (Anime anime in list.data)
            {
                nodes.Add(anime.node);
            }
            return nodes;
        }

        public HttpClient getClient()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-MAL-CLIENT-ID", $"{token}");
            return client;
        }

        private string baseUrl = "https://api.myanimelist.net/v2/";
        private string token = "6f8ce16b3e66b4057b7d9a117d4e7d2e";
        public Node GetAnime(int id, string[] fields)
        {
            string fieldString = "";
            foreach(string s in fields)
            {
                fieldString += $"{s},";
            }
            string url = $"{baseUrl}anime/{id}?fields={fieldString}";
            Node series;

            using (HttpClient client = getClient())
            {


                HttpResponseMessage response = client.GetAsync(url).GetAwaiter().GetResult();
                string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    series = JsonSerializer.Deserialize<Node>(responseBody);
                }
                else
                {
                    // Handle the unsuccessful response here
                    series = null;
                }
            }
            return series;
        }
        //https://api.myanimelist.net/v2/users/@me/animelist?fields=list_status&limit=4

        //provedení je trožku kostrbaťejší, ale v princupu: querry je vyhledávaná fráze (metodu vyhledávání si řeší API, k tomu se nedostanu); limit je počet poležek
        //které vyhledávání vyhodí; userList určuje jestly se vyhledává show podle API nebo list od specifického uživatele, myList určuje jestly se chce robrazit list
        //právě přihlášeného uživatele (není zatím implementováno); jelikož API dokáže vrátit max 100 serii najednou, tak musim udělat prasárnu nextPage, pyšnej na to nejsem
        public AnimeList GetAnimeList(string querry, bool userList = false, int number = 5, bool myList = false)
        {
            AnimeList templist;
            AnimeList seriesList;
            string url;

            url = $"{baseUrl}anime?q={querry}&limit={number}";
            seriesList = new AnimeList();
            if (userList)
            {
                if (myList)
                {
                    throw new NotImplementedException();
                    //querry = "@me";
                }
                url = $"{baseUrl}users/{querry}/animelist?fields=list_status&limit=100";
            }

            bool frist = true;
            using (HttpClient client = getClient())
            {
                do
                {
                    HttpResponseMessage response = client.GetAsync(url).GetAwaiter().GetResult();
                    string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    /* Tadyto je jeden z nejhnusnějčích kusů kódu co jsem kdy napsal, ale funguje to, tak na to šahat nebudu */
                    if (response.IsSuccessStatusCode && response.StatusCode.ToString() == "OK")
                    {
                        templist = JsonSerializer.Deserialize<AnimeList>(responseBody);
                        if (!frist)
                        {
                            foreach (Anime item in templist.data)
                            {
                                seriesList.data.Add(item);
                            }
                            url = templist.paging.next;
                        }
                        else
                        {
                            seriesList = templist;
                            frist = false;
                        }
                    }
                    else
                    {
                        templist = null;
                    }
                }
                while (templist != null && templist.paging.next != null && userList) ;
            }
            return seriesList;
        }

    }
    public class AnimeList
    {
        public List<Anime> data { get; set; }
        public Paging paging { get; set; }
    }

    public class Anime
    {
        public Node node { get; set; }
        public ListStatus list_status { get; set; }
    }

    public class MainPicture
    {
        public string medium { get; set; }
        public string large { get; set; }
    }

    public class AlternativeTitles
    {
        public List<string> synonyms { get; set; }
        public string en { get; set; }
        public string ja { get; set; }
    }

    public class Genre
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Studio
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Node
    {
        public int id { get; set; }
        public string title { get; set; }
        public MainPicture main_picture { get; set; }
        public AlternativeTitles alternative_titles { get; set; }
        public string start_date { get; set; }
        public string synopsis { get; set; }
        public double mean { get; set; }
        public List<Genre> genres { get; set; }
        public List<Studio> studios { get; set; }
    }

    public class ListStatus
    {
        public string status { get; set; }
        public int score { get; set; }
        public int num_watched_episodes { get; set; }
        public bool is_rewatching { get; set; }
        public DateTime updated_at { get; set; }
    }

    public class Paging
    {
        public string next { get; set; }
    }

}
