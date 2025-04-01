namespace ToolsManagmentMAUI.Services
{
    public class AlertService
    {
        public async Task<bool> ShowConfirmationAsync(string title, string message)
        {
            return await Application.Current.MainPage.DisplayAlert(title, message, "Tak", "Nie");
        }
    }
}
