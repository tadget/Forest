namespace Tadget
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class Tuple<T>
    {
        public T x, y, z;

        public Tuple(T x, T y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
