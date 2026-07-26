namespace OnlineLearning.Models
{
    public class NavigationItem
    {
        public string Title { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Icon { get; set; }
        public string RequiredRole { get; set; } // "Admin", "Student", etc.
        public List<NavigationItem> SubItems { get; set; } = new List<NavigationItem>();

        public bool HasSubMenu => SubItems.Any();
    }
}
