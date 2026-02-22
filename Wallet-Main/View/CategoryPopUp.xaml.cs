using CommunityToolkit.Maui.Views;
namespace Wallet_Main
{
    public partial class CategoryPopUp : Popup
    {
        private double currentY;

        public List<Wallet_lib.Categories> Categories { get; set; }
        private bool newCategoryNull = false;
        public CategoryPopUp(List<Wallet_lib.Categories> cat)
        {
            InitializeComponent();
            Categories = cat;
            Wallet_lib.Categories newCategory = new() { Name = "Dodaj Kategorie", Color = "#2E7D32", Icon = "📝", Type = cat.Last().Type };
            Categories.Add(newCategory);

            BindingContext = this;
            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            Size = new Size(mainDisplayInfo.Width / mainDisplayInfo.Density, 450);
        }

        public void OnCategorySelected(object sender, SelectionChangedEventArgs e)
        {
            Wallet_lib.Categories? selectedCategory = e.CurrentSelection.FirstOrDefault() as Wallet_lib.Categories;
            CloseLogic(selectedCategory);
        }
        public async void CloseLogic(Wallet_lib.Categories? category)
        {
            if (newCategoryNull)
            {
                newCategoryNull = false;
                return;
            }
            if (category != null)
            {
                if (category.Name == "Dodaj Kategorie")
                {

                    AddCategoryPopUp add = new(Categories.Last().Type);
                    //Dictionary<string, object> info = new();
                    //info.Add("TypeOfCategory",Categories.Last().Type);
                    var result = await Shell.Current.ShowPopupAsync(add);
                    if (result is Wallet_lib.Categories cat)
                    {
                        Close(cat);
                        return;
                    }
                    else
                    {
                        newCategoryNull = true;
                        CategoriesCollection.SelectedItem = null;

                        return;

                    }
                }
                else
                {
                    Close(category);
                }

            }
            Wallet_lib.Categories? categorieDefault = Categories.Where(c => c.Name == "Inne").FirstOrDefault();

            if (category == null && categorieDefault != null)
            {
                Close(categorieDefault);
            }
            else if (Categories.FirstOrDefault() != null)
            {
                Close(Categories.First());
            }
        }
        public async Task CloseWithAnimation()
        {
            await MainPop.TranslateTo(0, 600, 250, Easing.CubicIn);
            CloseLogic(null);
        }
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

    }
}