using System;
using TreeBench.BS.Interfaces;

namespace TreeBench.BS.Models
{
    public class AvlTree : BaseBalancedTree
    {
        private class Node
        {
            public int Key;
            public int Height;
            public Node Right, Left;
            public Node(int key) { Key = key; Height = 1; }
        }

        private Node root;

        public override void Insert(int key) => root = InsertRec(root, key);

        protected override bool SearchInternal(int key) => SearchRec(root, key);

        protected override void DeleteInternal(int key) => root = DeleteRec(root, key);

        public override int GetMaxDepth() => GetHeight(root);

        public override int GetMinDepth() => GetMinDepthRec(root);

        private int GetHeight(Node node) => node == null ? 0 : node.Height;

        private int Getbalance(Node node) => node == null ? 0 : GetHeight(node.Left) - GetHeight(node.Right);

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

        private Node InsertRec(Node node, int key)
        {
            if (node == null)
            {
                count++;
                return new Node(key);
            }

            if (key < node.Key) node.Left = InsertRec(node.Left, key);
            else if (key > node.Key) node.Right = InsertRec(node.Right, key);
            else return node;

            node.Height = 1 + Math.Max(GetHeight(node.Left), GetHeight(node.Right));
            int balance = Getbalance(node);

            if (balance > 1 && key < node.Left.Key) return RightRotate(node);
            if (balance < -1 && key > node.Right.Key) return LeftRotate(node);

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

        // --- BALANCED AVL DELETION ENGINE (NEW FEATURE v2.0) ---
        private Node DeleteRec(Node root, int key)
        {
            if (root == null) return root;

            if (key < root.Key)
            {
                root.Left = DeleteRec(root.Left, key);
            }
            else if (key > root.Key)
            {
                root.Right = DeleteRec(root.Right, key);
            }
            else
            {
                if ((root.Left == null) || (root.Right == null))
                {
                    Node temp = root.Left ?? root.Right;

                    if (temp == null)
                    {
                        temp = root;
                        root = null;
                    }
                    else
                    {
                        root = temp;
                    }
                    count--;
                }
                else
                {
                    Node temp = GetMinValueNode(root.Right);
                    root.Key = temp.Key;
                    root.Right = DeleteRec(root.Right, temp.Key);
                }
            }

            if (root == null) return root;

            root.Height = Math.Max(GetHeight(root.Left), GetHeight(root.Right)) + 1;
            int balance = Getbalance(root);

            if (balance > 1 && Getbalance(root.Left) >= 0) return RightRotate(root);
            if (balance > 1 && Getbalance(root.Left) < 0)
            {
                root.Left = LeftRotate(root.Left);
                return RightRotate(root);
            }
            if (balance < -1 && Getbalance(root.Right) <= 0) return LeftRotate(root);
            if (balance < -1 && Getbalance(root.Right) > 0)
            {
                root.Right = RightRotate(root.Right);
                return LeftRotate(root);
            }

            return root;
        }

        private Node GetMinValueNode(Node node)
        {
            Node current = node;
            while (current.Left != null) current = current.Left;
            return current;
        }

        private int GetMinDepthRec(Node node)
        {
            if (node == null) return 0;
            if (node.Left == null && node.Right == null) return 1;
            if (node.Left == null) return GetMinDepthRec(node.Right) + 1;
            if (node.Right == null) return GetMinDepthRec(node.Left) + 1;

            return Math.Min(GetMinDepthRec(node.Left), GetMinDepthRec(node.Right)) + 1;
        }
    }
}