using System;
using System.Collections.Generic;


namespace Dispatcher
{
    public sealed class ObjectPool<T>
        where T : new()
    {
        private int growSize = 20;
        private T[] pool;

        private int nextIndex = 0;

        public ObjectPool(int rSize)
        {
            Resize(rSize, false);
        }

        public ObjectPool(int rSize, int rGrowSize)
        {
            growSize = rGrowSize;
            Resize(rSize, false);
        }

        public int Length
        {
            get { return pool.Length; }
        }

        public int Available
        {
            get { return pool.Length - nextIndex; }
        }

        public int Allocated
        {
            get { return nextIndex; }
        }

        public T Allocate()
        {
            T lItem = default(T);

            if (nextIndex >= pool.Length)
            {
                if (growSize > 0)
                {
                    Resize(pool.Length + growSize, true);
                }
                else
                {
                    return lItem;
                }
            }

            if (nextIndex >= 0 && nextIndex < pool.Length)
            {
                lItem = pool[nextIndex];
                nextIndex++;
            }

            return lItem;
        }


        public void Release(T instance)
        {
            if (nextIndex > 0)
            {
                nextIndex--;
                pool[nextIndex] = instance;
            }
        }

        public void Reset()
        {
            int lLength = growSize;
            if (pool != null) { lLength = pool.Length; }

            Resize(lLength, false);

            nextIndex = 0;
        }

        public void Resize(int rSize, bool rCopyExisting)
        {
            lock (this)
            {
                int lCount = 0;

                T[] lNewPool = new T[rSize];

                if (pool != null && rCopyExisting)
                {
                    lCount = pool.Length;
                    Array.Copy(pool, lNewPool, Math.Min(lCount, rSize));
                }

                for (int i = lCount; i < rSize; i++)
                {
                    lNewPool[i] = new T();
                }

                pool = lNewPool;
            }
        }
    }
}