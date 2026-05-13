using ABLibrary.Core;

namespace MyAbMobileApp;

public partial class MainPage : ContentPage
{
    private readonly ABManager _abManager;

    public MainPage(ABManager abManager)
    {
        InitializeComponent();

        _abManager = abManager;
    }

    private async void OnInitClicked(object sender, EventArgs e)
    {
        try
        {
            await _abManager.InitAsync("mobile-app");

            var variant = _abManager.GetVariant("button_test");

            VariantLabel.Text = $"Variant: {variant}";

            StatusLabel.Text = "Initialized!";
        }
        catch (Exception ex)
        {
            StatusLabel.Text = ex.Message;
        }
    }

    private async void OnTrackClicked(object sender, EventArgs e)
    {
        try
        {
            await _abManager.TrackAsync(
                "button_test",
                "user_123",
                "conversion");

            StatusLabel.Text = "Event sent!";
        }
        catch (Exception ex)
        {
            StatusLabel.Text = ex.Message;
        }
    }
}