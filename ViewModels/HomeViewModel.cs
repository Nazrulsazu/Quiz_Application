using Quiz_App.Models;

namespace Quiz_App.ViewModels
{
    public class HomeViewModel
    {
        public List<Template> LatestTemplates { get; set; }
        public List<Template> PopularTemplates { get; set; }
        public IEnumerable<dynamic> TagCloud { get; set; }
    }
}
