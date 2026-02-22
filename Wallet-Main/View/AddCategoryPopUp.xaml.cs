using CommunityToolkit.Maui.Views;
using Wallet_lib;

namespace Wallet_Main;

public partial class AddCategoryPopUp : Popup
{
    private double currentY;


    //private Color selectedColor;

    //private string selectedIcon;

    //private string name;

    private string Type;
    public List<Color> ColorsList { get; set; } = new List<Color>
    {
        Color.FromArgb("#FF5252"),
        Color.FromArgb("#FF4081"),
        Color.FromArgb("#E040FB"),
        Color.FromArgb("#7C4DFF"),
        Color.FromArgb("#536DFE"),
        Color.FromArgb("#448AFF"),
        Color.FromArgb("#40C4FF"),
        Color.FromArgb("#18FFFF"),
        Color.FromArgb("#64FFDA"),
        Color.FromArgb("#69F0AE"),
        Color.FromArgb("#B2FF59"),
        Color.FromArgb("#EEFF41"),
        Color.FromArgb("#FFFF00"),
        Color.FromArgb("#FFD740"),
        Color.FromArgb("#FFAB40"),
        Color.FromArgb("#FF6E40"),
        Color.FromArgb("#BCAAA4"),
        Color.FromArgb("#B0BEC5")
    };
    public List<string> IconsList { get; set; } = new List<string>
    {
        "🛒", "🍞", "🥩", "🍎", "🍔", "🍕", "☕", "🍺", "🍷",
        "🚗", "🚌", "🚆", "✈️", "🚲", "🛴", "⛽", "🅿️", "🔧",
        "🏠", "💡", "💧", "🔥", "🗑️", "📱", "🌐", "🛠️", "🛋️",
        "🛍️", "🎁", "👗", "👕", "👟", "👜", "💍", "🧴",
        "🎬", "🎮", "🎵", "📚", "🎫", "🎳", "🏕️", "🏈",
        "💊", "🏥", "🦷", "🏋️‍♂️", "🧘‍♀️", "💇‍♀️", "🧼",
        "👶", "🧸", "🐶", "🐱", "🐾",
        "💰", "💳", "🏦", "💼", "🎓", "💸", "❓"
    };

    public AddCategoryPopUp(string type)
    {
        InitializeComponent();
        Type = type;
        BindingContext = this;

        var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
        Size = new Size(mainDisplayInfo.Width / mainDisplayInfo.Density, 450);
    }

    public async void CloseWithAnimation(Categories? category)
    {
        await Pop.TranslateTo(0, 600, 250, Easing.CubicIn);
        Close(category);
    }
    public async void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Running:
                if (e.TotalY > 0)
                {
                    Pop.TranslationY = e.TotalY;
                    currentY = e.TotalY;

                }
                break;
            case GestureStatus.Completed:
                if (currentY > 200)
                {
                    CloseWithAnimation(null);
                }
                else
                {
                    await Pop.TranslateTo(0, 0, 200, Easing.CubicOut);
                }
                currentY = 0;
                break;
        }
    }
    public void On_Form_Changed(object sender, EventArgs e)
    {
        if(CategoryName.Text != "" && CategoryName.Text != null)
        {
            if(SelectedColor.SelectedItem != null && SelectedIcon.SelectedItem != null)
            {
                Zapisz_Button.IsEnabled = true;
                return;
            }
        }
        Zapisz_Button.IsEnabled = false;
    }
    public void On_Save_Clicked(object sender, EventArgs e)
    {
        Color color = (Color)SelectedColor.SelectedItem;
        Wallet_lib.Categories category = new(CategoryName.Text, Type, SelectedIcon.SelectedItem.ToString(), color.ToHex());
        Close(category);
    }


}