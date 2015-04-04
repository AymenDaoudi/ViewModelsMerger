using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ViewModelsMerger.Helpers
{
    public class PropertyInfoComparer : IEqualityComparer<PropertyInfo>
    {
        public bool Equals(PropertyInfo x, PropertyInfo y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return false;


            return x.Name == y.Name;
        }

        public int GetHashCode(PropertyInfo obj)
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
