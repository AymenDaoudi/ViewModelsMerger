using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ViewModelsMerger.Helpers
{
    internal class PropertyComparer : IEqualityComparer<Tuple<PropertyInfo, object>>
    {
        public bool Equals(Tuple<PropertyInfo, object> x, Tuple<PropertyInfo, object> y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return false;
            return x.Item1.Name == y.Item1.Name;
        }

        public int GetHashCode(Tuple<PropertyInfo, object> obj)
        {
            //if (ReferenceEquals(obj, null)) return 0;

            //// Get the hash code for the Name field if it is not null. 
            //int hashPropertyInfo = obj.Item1 == null ? 0 : obj.Item1.GetHashCode();

            //// Get the hash code for the Code field. 
            //int hashPropertyInfoObject = obj.Item2.GetHashCode();

            //// Calculate the hash code for the product. 
            //return hashPropertyInfo ^ hashPropertyInfoObject;
            return 0;
        }
    }
}
