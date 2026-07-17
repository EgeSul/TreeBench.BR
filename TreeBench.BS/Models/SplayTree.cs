using System;
using System.Data;

namespace TreeBench.BS.Models
{
    public class SplayTree : BaseBalancedTree
    {
        private class Node
        {
            public int Key;
            public Node Left, Right;
            public Node(int key) { Key = key; }
        }

        private Node root;


        public override void Insert(int key)
        {
            if (root == null)
            {
                root = new Node(key);
                count++;
                return;
            }

            root = Splay(root, key);

            if (root.Key == key) return;

            Node newNode = new Node(key);
            if (key < root.Key)
            {
                newNode.Right = root;
                newNode.Left = root.Left;
                root.Left = null;
            }
            else
            {
                newNode.Left = root;
                newNode.Right = root.Right;
                root.Right = null;
            }

            root = newNode;
            count++;
        }

        protected override bool SearchInternal(int key)
        {
            root = Splay(root, key);
            return root.Key == key;
        }

        protected override void DeleteInternal(int key)
        {
            root = Splay(root, key);

            if (root.Left == null)
            {
                root = root.Right;
            }
            else
            {
                Node rightSubtree = root.Right;
                root = root.Left;
                root = Splay(root, key);
                root.Right = rightSubtree;
            }

            count--;
        }

        public override int GetMaxDepth() => GetMaxDepthRec(root);

        public override int GetMinDepth() => GetMinDepthRec(root);


        private Node RightRotate(Node x)
        {
            Node y = x.Left;
            x.Left = y.Right;
            y.Right = x;
            rotationsCount++;
            return y;
        }

        private Node LeftRotate(Node x)
        {
            Node y = x.Right;
            x.Right = y.Left;
            y.Left = x;
            rotationsCount++;
            return y;
        }

        private Node Splay(Node root, int key)
        {
            if (root == null || root.Key == key) return root;

            if (key < root.Key)
            {
                if (root.Left == null) return root;

                if (key < root.Left.Key)
                {
                    root.Left.Left = Splay(root.Left.Left, key);
                    root = RightRotate(root);
                }
                else if (key > root.Left.Key)
                {
                    root.Left.Right = Splay(root.Left.Right, key);
                    if (root.Left.Right != null)
                        root.Left = LeftRotate(root.Left);
                }

                return (root.Left == null) ? root : RightRotate(root);
            }
            else
            {
                if (root.Right == null) return root;

                if (key < root.Right.Key)
                {
                    root.Right.Left = Splay(root.Right.Left, key);
                    if (root.Right.Left != null)
                        root.Right = RightRotate(root.Right);
                }
                else if (key > root.Right.Key)
                {
                    root.Right.Right = Splay(root.Right.Right, key);
                    root = LeftRotate(root);
                }

                return (root.Right == null) ? root : LeftRotate(root);
            }
        }

        private int GetMaxDepthRec(Node node)
        {
            if (node == null) return 0;
            return Math.Max(GetMaxDepthRec(node.Left), GetMaxDepthRec(node.Right)) + 1;
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