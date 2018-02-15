using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class UserModel
    {
        public class PostModel
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string URL { get; set; }

            public IDictionary<string, string> GetRouteData()
            {
                IDictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("id", Id.ToString());
                return dict;
            }
        }

        public int UserID { get; set; }
        public string UserName { get; set; }
        public LinkedList<PostModel> Posts = new LinkedList<PostModel>();
    }
}
