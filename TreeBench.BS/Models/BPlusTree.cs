using System;
using System.Collections.Generic;

namespace TreeBench.BS.Models
{
    public class BPlusTree : BaseBalancedTree
    {
        private const int M = 3;

        private class BPlusNode
        {
            public bool IsLeaf;
            public List<int> Keys = new List<int>();
            public List<BPlusNode> Children = new List<BPlusNode>();
            public BPlusNode Next;

            public BPlusNode(bool isLeaf)
            {
                IsLeaf = isLeaf;
            }
        }

        private BPlusNode root;

        public BPlusTree()
        {
            root = new BPlusNode(true); 
        }

        // --- INSERT Engine ---
        public override void Insert(int key)
        {
            BPlusNode r = root;

            if (r.Keys.Count == M - 1)
            {
                BPlusNode s = new BPlusNode(false);
                root = s;
                s.Children.Add(r);
                SplitChild(s, 0, r);
                InsertNonFull(s, key);
            }
            else
            {
                InsertNonFull(r, key);
            }
            count++;
        }

        private void InsertNonFull(BPlusNode node, int key)
        {
            int i = node.Keys.Count - 1;

            if (node.IsLeaf)
            {
                while (i >= 0 && node.Keys[i] > key) i--;
                node.Keys.Insert(i + 1, key);
            }
            else
            {
                while (i >= 0 && node.Keys[i] > key) i--;
                i++;
                BPlusNode child = node.Children[i];

                if (child.Keys.Count == M - 1)
                {
                    SplitChild(node, i, child);
                    if (node.Keys[i] < key) i++;
                }
                InsertNonFull(node.Children[i], key);
            }
        }

        private void SplitChild(BPlusNode parent, int i, BPlusNode child)
        {
            BPlusNode z = new BPlusNode(child.IsLeaf);
            int mid = (M - 1) / 2;

            parent.Keys.Insert(i, child.Keys[mid]);
            parent.Children.Insert(i + 1, z);

            z.Keys.AddRange(child.Keys.GetRange(mid + (child.IsLeaf ? 0 : 1), child.Keys.Count - mid - (child.IsLeaf ? 0 : 1)));
            child.Keys.RemoveRange(mid, child.Keys.Count - mid);

            if (!child.IsLeaf)
            {
                z.Children.AddRange(child.Children.GetRange(mid + 1, child.Children.Count - mid - 1));
                child.Children.RemoveRange(mid + 1, child.Children.Count - mid - 1);
            }
            else
            {
                z.Next = child.Next;
                child.Next = z;
            }
            rotationsCount++;
        }

        protected override bool SearchInternal(int key)
        {
            BPlusNode current = root;

            while (!current.IsLeaf)
            {
                int i = 0;
                while (i < current.Keys.Count && key >= current.Keys[i]) i++;
                current = current.Children[i];
            }

            return current.Keys.Contains(key);
        }

        // --- DELETE Engine ---
        protected override void DeleteInternal(int key)
        {
            BPlusNode current = root;
            while (!current.IsLeaf)
            {
                int i = 0;
                while (i < current.Keys.Count && key >= current.Keys[i]) i++;
                current = current.Children[i];
            }
            if (current.Keys.Contains(key))
            {
                current.Keys.Remove(key);
                count--;
            }
        }

        // --- TOPOLOGY Metrics ---
        public override int GetMaxDepth()
        {
            int depth = 0;
            BPlusNode current = root;
            while (current != null)
            {
                depth++;
                current = current.IsLeaf ? null : current.Children[0];
            }
            return depth;
        }

        public override int GetMinDepth() => GetMaxDepth();
    }
}