using System;
using System.Collections.ObjectModel;
using Microcharts;
using SkiaSharp;
using TimeTracker.Apps.Modeles;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TimeTracker.Apps.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChartPage : ContentPage
    {
        private static readonly SKColor[] Colors =
        {
            SKColor.Parse("#eeeeee"),
            SKColor.Parse("#dddddd"),
            SKColor.Parse("#cccccc"),
            SKColor.Parse("#bbbbbb"),
            SKColor.Parse("#aaaaaa"),
            SKColor.Parse("#999999"),
            SKColor.Parse("#888888"),
            SKColor.Parse("#777777"),
            SKColor.Parse("#666666"),
            SKColor.Parse("#555555"),
            SKColor.Parse("#444444"),
            SKColor.Parse("#333333"),
        };

        public ChartPage(string projectName, Collection<Tache> taches)
        {
            InitializeComponent();
            Title.Text = projectName + " tasks chart";
            var entries = new ChartEntry[taches.Count];
            var i = 0;
            foreach (var task in taches)
            {
                var totalTimes = task.GetTotalTimes();
                entries[i] = new ChartEntry(totalTimes.Ticks)
                {
                    Label = task.Nom,
                    Color = Colors[i%Colors.Length],
                    ValueLabelColor = Colors[i%Colors.Length],
                    TextColor = SKColors.Black
                };
                i++;
            }
            ChartTime.Chart = new PieChart {Entries = entries};
        }
        
        public ChartPage(Collection<Projet> projets)
        {
            InitializeComponent();
            Title.Text = "Projects Chart";
            var entries = new ChartEntry[projets.Count];
            var i = 0;
            foreach (var projet in projets)
            {
                var totalTimes = projet.GetTotalTimes();
                entries[i] = new ChartEntry(totalTimes.Ticks)
                {
                    Label = projet.Nom,
                    Color = Colors[i%Colors.Length],
                    ValueLabelColor = Colors[i%Colors.Length],
                    TextColor = SKColors.Black
                };
                i++;
            }
            ChartTime.Chart = new PieChart {Entries = entries};
        }
    }
}