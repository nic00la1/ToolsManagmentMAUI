namespace ToolsManagmentMAUI.Services
{
    public class AlertService
    {
        public async Task<bool> ShowConfirmationAsync(string title, string message)
        {
            return await Application.Current.MainPage.DisplayAlert(title, message, "OK", "Anuluj");
        }

        public async Task ShowMessageAsync(string title, string message)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, "OK");
        }
    }
}