using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionMethods
{
    public static class Extensions
    {
        public static T To<T, A>(this A a, Func<A, T> fn)
        {
            return fn(a);
        }
    }
    public class Unit {}
}
