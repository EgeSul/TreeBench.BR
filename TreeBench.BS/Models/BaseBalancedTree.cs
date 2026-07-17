using System;
using TreeBench.BS.Interfaces;

namespace TreeBench.BS.Models
{
    public abstract class BaseBalancedTree : IBalancedTree
    {
        protected int count;
        protected int rotationsCount;

        public int Count => count;
        public int GetRotationsCount() => rotationsCount;
        public void ResetMetrics() => rotationsCount = 0;

        public bool Search(int key)
        {
            if (count == 0) return false;
            return SearchInternal(key);
        }

        public void Delete(int key)
        {
            if (count == 0) return;

            if (!Search(key)) return;

            DeleteInternal(key);
        }

        protected abstract bool SearchInternal(int key);
        protected abstract void DeleteInternal(int key);

        public abstract int GetMaxDepth();
        public abstract int GetMinDepth();
        public abstract void Insert(int key);
    }
}