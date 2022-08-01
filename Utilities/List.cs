using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailsIO.Utilities
{
    public static class ListUtility
    {
        public static string Commaize(IEnumerable<string> list)
        {
            var last = list.LastOrDefault();
            return (last != null) ?
                list.Aggregate((acc, x) => acc + ", " + (x == last ? "and " : "") + x) :
                string.Empty;
        }
    }
}
