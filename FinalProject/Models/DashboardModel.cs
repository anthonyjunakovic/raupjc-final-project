using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class DashboardModel
    {
        public bool IsFacebook;

        public DashboardModel(bool IsFacebook = false)
        {
            this.IsFacebook = IsFacebook;
        }
    }
}
