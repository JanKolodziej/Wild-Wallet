using CommunityToolkit.Maui.Views;
namespace Wallet_Main
{
	public partial class CategoryPopUp : Popup
	{
		public List<Wallet_lib.Categories> Categories { get; set; }
		public CategoryPopUp(List<Wallet_lib.Categories> categoryList)
		{
			InitializeComponent();
            Wallet_lib.Categories newCategory = new() {Name = "Dodaj Kategorie",Color = "#2E7D32", Icon= "📝", Type="Income" };
			categoryList.Add(newCategory);
			Categories = categoryList;
			
			BindingContext = this;
            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            Size = new Size(mainDisplayInfo.Width / mainDisplayInfo.Density, 450);
        }

		public void OnCategorySelected(object sender, SelectionChangedEventArgs e)
		{
			Wallet_lib.Categories? selectedCategory = e.CurrentSelection.FirstOrDefault() as Wallet_lib.Categories;
			if(selectedCategory != null )
			{
				Close(selectedCategory);
			}
		}
	}
}