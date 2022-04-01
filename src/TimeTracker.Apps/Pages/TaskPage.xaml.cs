using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Apps.Modeles;
using TimeTracker.Apps.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TimeTracker.Apps.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskPage : ContentPage
    {
        public TaskPage(string accessToken,string refreshToken,Projet projet)
        {
            InitializeComponent();
            BindingContext = new TaskViewModel(accessToken,refreshToken,projet);
        }
    }
}