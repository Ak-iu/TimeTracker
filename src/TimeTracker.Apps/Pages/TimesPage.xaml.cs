using TimeTracker.Apps.Modeles;
using TimeTracker.Apps.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TimeTracker.Apps.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimesPage : ContentPage
    {
        public TimesPage(Projet projet, Tache tache)
        {
            InitializeComponent();
            BindingContext = new TimesViewModel(projet,tache);
        }
    }
}