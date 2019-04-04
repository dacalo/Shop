namespace Shop.UIForms.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using Newtonsoft.Json;
    using Common.Helpers;
    using Common.Models;
    using Common.Services;
    using UIForms.Views;
    using System.Windows.Input;
    using Xamarin.Forms;
    using Shop.UIForms.Helpers;

    public class LoginViewModel: BaseViewModel
    {
        private bool isRunning;
        private bool isEnabled;
        private readonly ApiService apiService;

        #region Properties
        public bool IsRunning
        {
            get => this.isRunning;
            set => this.SetValue(ref this.isRunning, value);
        }

        public bool IsEnabled
        {
            get => this.isEnabled;
            set => this.SetValue(ref this.isEnabled, value);
        }

        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsRemember { get; set; }

        public ICommand LoginCommand => new RelayCommand(Login);

        public ICommand RegisterCommand => new RelayCommand(this.Register);

        public ICommand RememberPasswordCommand => new RelayCommand(this.RememberPassword);


        #endregion

        #region Constructor
        public LoginViewModel()
        {
            this.apiService = new ApiService();
            this.IsEnabled = true;
            this.Email = "jzuluaga55@gmail.com";
            this.Password = "123456";
            this.IsRemember = true;
        }
        #endregion

        #region Methods
        private async void Login()
        {
            if (string.IsNullOrEmpty(Email))
            {
                await Application.Current.MainPage.DisplayAlert(Languages.Error, Languages.EmailMessage, Languages.Accept);
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                await Application.Current.MainPage.DisplayAlert(Languages.Error, Languages.PasswordError, Languages.Accept);
                return;
            }

            this.IsRunning = true;
            this.IsEnabled = false;

            var request = new TokenRequest
            {
                Password = this.Password,
                Username = this.Email
            };

            var url = Application.Current.Resources["UrlAPI"].ToString();
            var response = await this.apiService.GetTokenAsync(url, "/Account", "/CreateToken", request);

            this.IsRunning = false;
            this.IsEnabled = true;

            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(Languages.Error, Languages.LoginError, Languages.Accept);
                return;
            }

            var token = (TokenResponse)response.Result;
            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.Token = token;
            mainViewModel.UserEmail = this.Email;
            mainViewModel.Products = new ProductsViewModel();

            Settings.IsRemember = this.IsRemember;
            Settings.UserEmail = this.Email;
            Settings.UserPassword = this.Password;
            Settings.Token = JsonConvert.SerializeObject(token);

            Application.Current.MainPage = new MasterPage();
            //await Application.Current.MainPage.Navigation.PushAsync(new ProductsPage());
        }

        private async void Register()
        {
            MainViewModel.GetInstance().Register = new RegisterViewModel();
            await Application.Current.MainPage.Navigation.PushAsync(new RegisterPage());
        }

        private async void RememberPassword()
        {
            MainViewModel.GetInstance().RememberPassword = new RememberPasswordViewModel();
            await Application.Current.MainPage.Navigation.PushAsync(new RememberPasswordPage());
        }


        #endregion

    }
}
