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
        private TaskViewModel _vm;
        public TaskPage(Projet projet)
        {
            InitializeComponent();
            _vm = new TaskViewModel(projet);
            BindingContext = _vm;
        }

        protected override void OnAppearing()
        {
            _vm.GetTasks();
        }
    }
}