using System.Collections.Generic;

namespace Watchmud.Web.Models
{
    public class FacebookData<T>
    {
        public IList<T> Data { get; set; }
    }
}