using ProjektPMALidak.Models;
using System.Collections.ObjectModel;

namespace ProjektPMALidak.Pages;

public class SearchViewModel : BindableObject
{
    public SearchViewModel(List<Anime> l)
    {
        searchItems = new ObservableCollection<SearchItem>();
        foreach (Anime item in l)
        {
            SearchItem searchItem = new SearchItem();
            searchItem.id = item.node.id;
            searchItem.title = item.node.title;
            searchItem.picture = item.node.main_picture.medium;
            if (item.list_status != null)
            {
                searchItem.status = item.list_status.status;
                searchItem.score = item.list_status.score;
            }
            searchItems.Add(searchItem);
        }
    }

    private ObservableCollection<SearchItem> searchItems;
    public ObservableCollection<SearchItem> SearchItems
    {
        get
        {
            return searchItems;
        }
    }


    public class SearchItem
    {
        public int id {get; set;}
        public string title { get; set; }
        public string picture { get; set; }
        public int score { get; set; }
        public string status { get; set; }
    }

}


public partial class SeachPage : ContentPage
{
    APIservices services;
    AnimeList nodes;
    SearchViewModel itemsModel;
    public SeachPage(string querry, bool list = false)
    {
        InitializeComponent();
        services = new APIservices();
        try
        {
            if (!list)
            {
                this.Title = $"Výsledky pro: {querry}";
                nodes = services.GetAnimeList(querry, number: 50);
            }
            else
            {
                this.Title = $"List uživatele: {querry}";
                nodes = services.GetAnimeList(querry, userList: true);
            }
            itemsModel = new SearchViewModel(nodes.data);
            BindingContext = itemsModel;
        }
        catch
        {
            DisplayAlert("Chyba", "Nebyly nalezeny vysledky", "Ok");
        }
    }

    private async void ShowView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SearchViewModel.SearchItem selected = (SearchViewModel.SearchItem)e.CurrentSelection[0];
        await Navigation.PushAsync(new ShowDetailsPage(selected.id));
        //((CollectionView)sender).SelectedItem = null; //tohle by mìlo fungovat, ale z nìjakýho mì neznámího dùvodu to pøeruší e.CurrentSelection, nevim jak je to vubec možný
    }

    private void NameFilter_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e.NewTextValue) && e.NewTextValue.Length > 2)
        {
            ShowView.ItemsSource = itemsModel.SearchItems;
        }
        else
        {
            ShowView.ItemsSource = itemsModel.SearchItems.Where(i => i.title.ToLower().Contains(e.NewTextValue.ToLower()));
        }
    }
}