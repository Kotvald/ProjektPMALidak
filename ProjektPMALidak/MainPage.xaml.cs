

using ProjektPMALidak;
using ProjektPMALidak.Models;
using ProjektPMALidak.Pages;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace ProjektPMALidak;


//DisplayAlert("ok", "ok", "ok");
//na testovani

public partial class MainPage : ContentPage
{
	public MainPage()
	{
        InitializeComponent();
	}

    private void MyListButton(object sender, EventArgs e)
    {
        Navigation.PushAsync(new SeachPage(UsernameEntry.Text, true));
    }
    private void SeachButton(object sender, EventArgs e)
    {
        Navigation.PushAsync(new SeachPage(SearchEntry.Text));
    }
}

