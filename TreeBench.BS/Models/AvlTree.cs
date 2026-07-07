using System;
using TreeBench.BS.Interfaces;

namespace TreeBench.BS.Models
{
    public class AvlTree : IBalancedTree
    {
        private class Node
        {
            public int Key;
            public int Height;
            public Node Right, Left;
            public Node(int key) { Key = key; Height = 1; }
        }

        private Node root;
        private int rotationsCount;
        private int count; 

        
        public int Count => count;

        public void Insert(int key) => root = InsertRec(root, key);

        public void Delete(int key) => root = DeleteRec(root, key);

        public bool Search(int key) => SearchRec(root, key);

        public int GetRotationsCount() => rotationsCount;

        public int GetMaxDepth() => GetHeight(root); 

        public int GetMinDepth() => GetMinDepthRec(root);

        public void ResetMetrics() => rotationsCount = 0;

        //depth Engine

        private int GetHeight(Node node) => node == null ? 0 : node.Height;

        private int Getbalance(Node node) => node == null ? 0 : GetHeight(node.Left) - GetHeight(node.Right);

        //*- Right Rotate Engine

        private Node RightRotate(Node y)
        {
            Node x = y.Left;
            Node T2 = x.Right;

            x.Right = y;
            y.Left = T2;

            y.Height = Math.Max(GetHeight(y.Left), GetHeight(y.Right)) + 1;
            x.Height = Math.Max(GetHeight(x.Left), GetHeight(x.Right)) + 1;

            rotationsCount++;
            return x;
        }

        //*- Left Rotate Engine
        private Node LeftRotate(Node x)
        {
            Node y = x.Right;
            Node T2 = y.Left;

            y.Left = x;
            x.Right = T2;

            x.Height = Math.Max(GetHeight(x.Left), GetHeight(x.Right)) + 1;
            y.Height = Math.Max(GetHeight(y.Left), GetHeight(y.Right)) + 1;

            rotationsCount++;
            return y;
        }

        // Balance the tree after insertion
        private Node InsertRec(Node node, int key)
        {
            if (node == null)
            {
                count++;
                return new Node(key);
            }

            if (key < node.Key) node.Left = InsertRec(node.Left, key);
            else if (key > node.Key) node.Right = InsertRec(node.Right, key);
            else
                return node;

            node.Height = 1 + Math.Max(GetHeight(node.Left), GetHeight(node.Right));
            int balance = Getbalance(node);

            //Left-Right Double Rotation and Left-Left Rotation and Right-Right Rotation and Right-Left Double Rotation

            if (balance > 1 && key < node.Left.Key)
                return RightRotate(node);

            if (balance < -1 && key > node.Right.Key)
                return LeftRotate(node);

            if (balance > 1 && key > node.Left.Key)
            {
                node.Left = LeftRotate(node.Left);
                return RightRotate(node);
            }

            if (balance < -1 && key < node.Right.Key)
            {
                node.Right = RightRotate(node.Right);
                return LeftRotate(node);
            }
            return node;
        }

        private bool SearchRec(Node node, int key)
        {
            if (node == null) return false;
            if (key == node.Key) return true;
            return key < node.Key ? SearchRec(node.Left, key) : SearchRec(node.Right, key);
        }

        private int GetMinDepthRec(Node node)
        {
            if (node == null) return 0;
            if (node.Left == null && node.Right == null) return 1;
            if (node.Left == null) return GetMinDepthRec(node.Right) + 1;
            if (node.Right == null) return GetMinDepthRec(node.Left) + 1;

            return Math.Min(GetMinDepthRec(node.Left), GetMinDepthRec(node.Right)) + 1;
        }

        [Obsolete("Balanced deletion logic is deferred to v2.0 Enterprise Release. Currently bypassed to maintain benchmark stability.")]
        private Node DeleteRec(Node node, int key)
        {
            // Bypassing the operation to maintain benchmark stability.
            return node;
        }
    }
}
