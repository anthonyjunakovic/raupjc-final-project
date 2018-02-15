using System.Collections.Generic;

namespace FinalProject.Models
{
    public class PostModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool Owned = false;
        public string OwnerName { get; set; }
        public string ImageURL { get; set; }

        public IDictionary<string, string> GetRouteData()
        {
            IDictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("id", Id.ToString());
            return dict;
        }
    }
}
