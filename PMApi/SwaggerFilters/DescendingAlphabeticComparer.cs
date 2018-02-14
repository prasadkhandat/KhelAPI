using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMApi.SwaggerFilters
{
    public class DescendingAlphabeticComparer
    {
        public int Compare(string x, string y)
        {
            // Write whatever comparer you'd like to here.
            // Yours would likely involve parsing the strings and having
            // more complex logic than this....
            return (string.Compare(x, y));
        }
    }
}