using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Database
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public Account Owner { get; set; }
        public string PostURL { get; set; }
        public bool Deleted { get; set; }

        public Post()
        {
            Deleted = false;
        }

        public Post(string Title, Account Owner, string PostURL) : this()
        {
            this.Title = Title;
            this.Owner = Owner;
            this.PostURL = PostURL;
        }

        public override bool Equals(object obj)
        {
            if (obj is Account)
            {
                if (obj is null)
                {
                    return false;
                }
                return (Id == ((Account)obj).Id);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return Title;
        }

        public static bool operator ==(Post obj1, Post obj2)
        {
            if (obj2 is null)
            {
                if (obj1 is null)
                {
                    return true;
                }
                return obj1.Equals(obj2);
            }
            return obj2.Equals(obj1);
        }

        public static bool operator !=(Post obj1, Post obj2)
        {
            return !(obj1 == obj2);
        }
    }
}
