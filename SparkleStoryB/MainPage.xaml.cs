namespace SparkleStory
{
    public partial class MainPage : ContentPage
    {
        public List<string> ItemTypes { get; set; }

        public MainPage()
        {
            InitializeComponent();

            // Define the different customization options
            ItemTypes = new List<string>
            {
                "Hat", "Cape", "Top", "Bottom", "Shoe", "Glove",
                "Face Accessory", "Eye", "Ear", "Weapon", "Hair",
                "Actual Face", "Skin Color"
            };

            // Bind the item types to the menu
            CustomizationMenu.ItemsSource = ItemTypes;
        }

        // Toggle the customization menu visibility
        private void OnCustomizeButtonClicked(object sender, EventArgs e)
        {
            CustomizationMenu.IsVisible = !CustomizationMenu.IsVisible;
        }
    }
}
