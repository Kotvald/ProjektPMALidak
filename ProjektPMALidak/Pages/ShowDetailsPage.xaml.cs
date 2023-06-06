using ProjektPMALidak.Models;

namespace ProjektPMALidak.Pages;

public partial class ShowDetailsPage : ContentPage
{
	APIservices services;
	Node selectedShow;
	public ShowDetailsPage(int id)
	{
		services = new APIservices();
		string[] parametres = { "alternative_titles", "start_date", "synopsis", "mean", "genres", "studios" };
		selectedShow = services.GetAnime(id, parametres);
		InitializeComponent();
		Header.Text = $"{selectedShow.title}";
		AlternativeHeader.Text = $"Jiné názvy:\n{selectedShow.alternative_titles.ja}, {selectedShow.alternative_titles.en}";
        MainImage.Source = selectedShow.main_picture.medium;
		ScoreView.Text = $"{selectedShow.mean}/10";
		DescView.Text = selectedShow.synopsis;
		ShowStartDisplay.Text = selectedShow.start_date;
		string genres = "Žánry: ";
		foreach (Genre g in selectedShow.genres)
		{
			genres += $"{g.name}, ";
		}
        string studios = "Studia: ";
        foreach (Studio s in selectedShow.studios)
        {
            studios += $"{s.name}, ";
        }
        GenresView.Text = genres;
		StudiosView.Text = studios;
	}
}