namespace OnlineLearning.Models
{
    public class StatCardModel
    {
        public string Title { get; set; }
        public string Value { get; set; }
        public string IconClass { get; set; }  
        public string TextColor { get; set; } 

        public StatCardModel(string title, string value, string icon, string color)
        {
            Title = title;
            Value = value;
            IconClass = icon;
            TextColor = color;
        }
    }
}
