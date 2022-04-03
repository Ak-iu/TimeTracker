using Storm.Mvvm.Forms;
using TimeTracker.Apps.ViewModels;

namespace TimeTracker.Apps.Pages
{
    public partial class MainPage : BaseContentPage
    {
        private MainViewModel _vm;
        public MainPage(string accessToken, string refreshToken)
        {
            InitializeComponent();
            _vm = new MainViewModel(accessToken,refreshToken);
            BindingContext = _vm;
        }

        protected override void OnAppearing()
        {
            _vm.GetProjects();
        }
    }
}