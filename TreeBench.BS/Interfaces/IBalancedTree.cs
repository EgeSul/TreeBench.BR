using System;

namespace TreeBench.BS.Interfaces
{
    public interface IBalancedTree
    {
        int Count { get; }

        void Insert(int key);
        void Delete(int key);
        bool Search(int key);

        int GetMaxDepth();
        int GetMinDepth();
        int GetRotationsCount();

        void ResetMetrics();
    }
}