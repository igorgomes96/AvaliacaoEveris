using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoProdutosTest
{
    public class ObjectDeepCompare<T> : IEqualityComparer<T>
    {
        public bool Equals(T obj, T another)
        {
            if (ReferenceEquals(obj, another)) return true;
            if ((obj == null) || (another == null)) return false;
            //Compare two object's class, return false if they are difference
            if (obj.GetType() != another.GetType()) return false;

            var result = true;
            //Get all properties of obj
            //And compare each other
            foreach (var property in obj.GetType().GetProperties())
            {
                var objValue = property.GetValue(obj);
                var anotherValue = property.GetValue(another);
                if (!objValue.Equals(anotherValue)) result = false;
            }

            return result;
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }
}
