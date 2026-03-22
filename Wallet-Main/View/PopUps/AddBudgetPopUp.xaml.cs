using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Wallet_lib;

namespace Wallet_Main;

public partial class AddBudgetPopUp : Popup
{
    public ObservableCollection<Categories> Categories { get; set; }
    private Categories _selectedCategory;
    public Categories SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            _selectedCategory = value;
            OnPropertyChanged();
        }
    }
    private readonly WalletService _service;
    public Wallet_lib.Budget? Budget { get; set; }
    public AddBudgetPopUp(List<Categories> categories, WalletService service, Wallet_lib.Budget? budget)
    {
        InitializeComponent();
        var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
        Size = new Size(mainDisplayInfo.Width / mainDisplayInfo.Density, 450);
        BindingContext = this;
        Categories = new ObservableCollection<Categories>(categories);
        _service = service;
        Budget = budget;
        if (Budget != null)
        {
            SelectedCategory = Budget.Category;
            AmountEntry.Text = Budget.Goal.ToString();
        }
    }


    private void OnSaveClicked(object sender, EventArgs e)
    {


        var newBudget = new Budget
        {
            Goal = decimal.Parse(AmountEntry.Text),
            CategoryId = SelectedCategory.Id,
            Category = SelectedCategory
        };

        Close(newBudget);
    }
    [RelayCommand]
    public async Task Show_Category_PopUp()
    {
        CategoryPopUp pop = new(Categories.ToList());
        var result = await Shell.Current.ShowPopupAsync(pop);
        if (result is Wallet_lib.Categories selected)
        {
            SelectedCategory = selected;
            if (!Categories.Contains(SelectedCategory))
            {
                Categories.Add(SelectedCategory);
                _service.Add_Category(SelectedCategory);
            }

        }
    }

    private void AmountEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (decimal.TryParse(e.NewTextValue, out _) && SelectedCategory != null)
        {
            SaveButton.IsEnabled = true;
        }
        else
        {
            SaveButton.IsEnabled = false;
        }
    }

    private double currentY;

    public async void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Running:
                if (e.TotalY > 0)
                {
                    MainPop.TranslationY = e.TotalY;
                    currentY = e.TotalY;
                }
                break;
            case GestureStatus.Completed:
                if (currentY > 200)
                {
                    await CloseWithAnimation();
                }
                else
                {
                    await MainPop.TranslateTo(0, 0, 200, Easing.CubicOut);
                }
                currentY = 0;
                break;
        }
    }

    public async Task CloseWithAnimation()
    {
        await MainPop.TranslateTo(0, 600, 250, Easing.CubicIn);
        Close(null); // Zamykamy okienko i zwracamy null, tak jak przy anulowaniu
    }
}