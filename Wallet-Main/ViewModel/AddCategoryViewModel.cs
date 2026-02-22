//namespace Wallet_Main.ViewModel
//{
//    [QueryProperty(nameof(Type), "TypeOfCategory")]
//    public partial class AddCategoryViewModel:ObservableObject
//    {
//        [ObservableProperty]
//        [NotifyPropertyChangedFor(nameof(IsValid))]
//        private Color selectedColor;
//        [ObservableProperty]
//        [NotifyPropertyChangedFor(nameof(IsValid))]
//        private string selectedIcon;
//        [ObservableProperty]
//        [NotifyPropertyChangedFor(nameof(IsValid))]
//        private string name;
//        [ObservableProperty]
//        private string type;

//        public bool IsValid => !string.IsNullOrWhiteSpace(Name) && SelectedColor != null && SelectedIcon != null;

//        public List<Color> ColorsList { get; set; } = new List<Color>
//    {
//        Color.FromArgb("#FF5252"),
//        Color.FromArgb("#FF4081"),
//        Color.FromArgb("#E040FB"),
//        Color.FromArgb("#7C4DFF"),
//        Color.FromArgb("#536DFE"),
//        Color.FromArgb("#448AFF"),
//        Color.FromArgb("#40C4FF"),
//        Color.FromArgb("#18FFFF"),
//        Color.FromArgb("#64FFDA"),
//        Color.FromArgb("#69F0AE"),
//        Color.FromArgb("#B2FF59"),
//        Color.FromArgb("#EEFF41"),
//        Color.FromArgb("#FFFF00"),
//        Color.FromArgb("#FFD740"),
//        Color.FromArgb("#FFAB40"),
//        Color.FromArgb("#FF6E40"),
//        Color.FromArgb("#BCAAA4"),
//        Color.FromArgb("#B0BEC5")
//    };
//        public List<string> IconsList { get; set; } = new List<string>
//    {
//        "🛒", "🍞", "🥩", "🍎", "🍔", "🍕", "☕", "🍺", "🍷",
//        "🚗", "🚌", "🚆", "✈️", "🚲", "🛴", "⛽", "🅿️", "🔧",
//        "🏠", "💡", "💧", "🔥", "🗑️", "📱", "🌐", "🛠️", "🛋️",
//        "🛍️", "🎁", "👗", "👕", "👟", "👜", "💍", "🧴",
//        "🎬", "🎮", "🎵", "📚", "🎫", "🎳", "🏕️", "🏈",
//        "💊", "🏥", "🦷", "🏋️‍♂️", "🧘‍♀️", "💇‍♀️", "🧼",
//        "👶", "🧸", "🐶", "🐱", "🐾",
//        "💰", "💳", "🏦", "💼", "🎓", "💸", "❓"
//    };
//        //private readonly IPopupService PopupService;

//        public AddCategoryViewModel()
//        {

//        }
//        [RelayCommand]
//        public void OnSaveClicked()
//        {
//            Wallet_lib.Categories category = new(Name, Type, SelectedIcon, SelectedColor.ToString());
//            //await CloseWithAnimation(category);
//        }

//    }
//}
