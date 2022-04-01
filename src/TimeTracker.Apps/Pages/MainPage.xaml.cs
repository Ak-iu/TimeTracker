using System;
using System.Diagnostics;
using Storm.Mvvm.Forms;
using TimeTracker.Apps.ViewModels;

namespace TimeTracker.Apps.Pages
{
    public partial class MainPage : BaseContentPage
    {
        public MainPage(string accessToken, string refreshToken)
        {
            InitializeComponent();
            BindingContext = new MainViewModel(accessToken, refreshToken);
        }
    }
}