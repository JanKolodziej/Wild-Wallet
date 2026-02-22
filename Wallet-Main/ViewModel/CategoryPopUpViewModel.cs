//namespace Wallet_Main.ViewModel
//{
//    //[QueryProperty(nameof(Categories), "CategoriesList")]
//    public partial class CategoryPopUpViewModel: ObservableObject,IQueryAttributable
//    {
//        [ObservableProperty]
//        private List<Wallet_lib.Categories> categories;
//        public CategoryPopUpViewModel()
//        {
//            Wallet_lib.Categories newCategory = new() { Name = "Dodaj Kategorie", Color = "#2E7D32", Icon = "📝", Type = "Income" };
//            Categories.Add(newCategory);
//        }
//        public void ApplyQueryAttributes(IDictionary<string, object> query)
//        {
//            Categories =query["CategoriesList"] as List<Wallet_lib.Categories>;
//        }
//    }
//}
