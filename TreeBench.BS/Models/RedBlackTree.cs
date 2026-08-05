using System;
using TreeBench.BS.Interfaces;

namespace TreeBench.BS.Models
{
    public class RedBlackTree : BaseBalancedTree
    {
        private enum Color { Red, Black }
        private class Node
        {
            public int Key; public Color Color;
            public Node Left, Right, Parent;
            public Node(int key) { Key = key; Color = Color.Red; }
        }

        private Node root;
        private Node TNULL;

        public RedBlackTree()
        {
            TNULL = new Node(0) { Color = Color.Black };
            root = TNULL;
        }

        public override void Insert(int key)
        {
            Node node = new Node(key) { Left = TNULL, Right = TNULL, Parent = null };
            Node y = null;
            Node x = root;

            while (x != TNULL && x != null)
            {
                stepCount++;
                y = x;
                if (node.Key < x.Key) x = x.Left;
                else if (node.Key > x.Key) x = x.Right;
                else return;
            }

            node.Parent = y;
            if (y == null) root = node;
            else if (node.Key < y.Key) y.Left = node;
            else y.Right = node;

            if (node.Parent == null) { node.Color = Color.Black; count++; return; }
            if (node.Parent.Parent == null) { count++; return; }

            count++;
            FixInsert(node);
        }

        protected override bool SearchInternal(int key) => SearchRec(root, key);
        public override int GetMaxDepth() => GetMaxDepthRec(root);
        public override int GetMinDepth() => GetMinDepthRec(root);

        private void LeftRotate(Node x)
        {
            if (x == null || x.Right == TNULL) return;
            Node y = x.Right;
            x.Right = y.Left;
            if (y.Left != TNULL) y.Left.Parent = x;
            y.Parent = x.Parent;
            if (x.Parent == null) root = y;
            else if (x == x.Parent.Left) x.Parent.Left = y;
            else x.Parent.Right = y;
            y.Left = x;
            x.Parent = y;
            rotationsCount++;
        }

        private void RightRotate(Node y)
        {
            if (y == null || y.Left == TNULL) return;
            Node x = y.Left;
            y.Left = x.Right;
            if (x.Right != TNULL) x.Right.Parent = y;
            x.Parent = y.Parent;
            if (y.Parent == null) root = x;
            else if (y == y.Parent.Right) y.Parent.Right = x;
            else y.Parent.Left = x;
            x.Right = y;
            y.Parent = x;
            rotationsCount++;
        }

        private void FixInsert(Node k)
        {
            Node u;
            while (k != root && k != null && k.Parent != null && k.Parent.Color == Color.Red)
            {
                // 🛡️ EN BÜYÜK GÜVENLİK KALKANI: Parent.Parent null ise API'yi çökertme, döngüyü kır!
                if (k.Parent.Parent == null) break;

                if (k.Parent == k.Parent.Parent.Right)
                {
                    u = k.Parent.Parent.Left;
                    if (u != null && u.Color == Color.Red)
                    {
                        u.Color = Color.Black;
                        k.Parent.Color = Color.Black;
                        k.Parent.Parent.Color = Color.Red;
                        k = k.Parent.Parent;
                    }
                    else
                    {
                        if (k == k.Parent.Left)
                        {
                            k = k.Parent;
                            RightRotate(k);
                        }
                        if (k.Parent != null) k.Parent.Color = Color.Black;
                        if (k.Parent != null && k.Parent.Parent != null)
                        {
                            k.Parent.Parent.Color = Color.Red;
                            LeftRotate(k.Parent.Parent);
                        }
                    }
                }
                else
                {
                    u = k.Parent.Parent.Right;
                    if (u != null && u.Color == Color.Red)
                    {
                        u.Color = Color.Black;
                        k.Parent.Color = Color.Black;
                        k.Parent.Parent.Color = Color.Red;
                        k = k.Parent.Parent;
                    }
                    else
                    {
                        if (k == k.Parent.Right)
                        {
                            k = k.Parent;
                            LeftRotate(k);
                        }
                        if (k.Parent != null) k.Parent.Color = Color.Black;
                        if (k.Parent != null && k.Parent.Parent != null)
                        {
                            k.Parent.Parent.Color = Color.Red;
                            RightRotate(k.Parent.Parent);
                        }
                    }
                }
            }
            if (root != null) root.Color = Color.Black;
        }

        private bool SearchRec(Node node, int key)
        {
            stepCount++;
            if (node == null || node == TNULL || key == node.Key) return node != null && node != TNULL;
            return key < node.Key ? SearchRec(node.Left, key) : SearchRec(node.Right, key);
        }

        protected override void DeleteInternal(int key)
        {
            Node node = root;
            while (node != TNULL && node != null)
            {
                stepCount++;
                if (node.Key == key) break;
                node = (key < node.Key) ? node.Left : node.Right;
            }

            if (node == TNULL || node == null) return;

            Node x, y; y = node; Color originalColor = y.Color;

            if (node.Left == TNULL) { x = node.Right; RbTransplant(node, node.Right); }
            else if (node.Right == TNULL) { x = node.Left; RbTransplant(node, node.Left); }
            else
            {
                y = GetMinValueNode(node.Right);
                originalColor = y.Color;
                x = y.Right;

                if (y.Parent == node) { if (x != null) x.Parent = y; }
                else
                {
                    RbTransplant(y, y.Right);
                    y.Right = node.Right;
                    if (y.Right != null) y.Right.Parent = y;
                }
                RbTransplant(node, y);
                y.Left = node.Left;
                if (y.Left != null) y.Left.Parent = y;
                y.Color = node.Color;
            }
            count--;
            if (originalColor == Color.Black && x != TNULL && x != null) FixDelete(x);
        }

        private void RbTransplant(Node u, Node v)
        {
            if (u.Parent == null) root = v;
            else if (u == u.Parent.Left) u.Parent.Left = v;
            else u.Parent.Right = v;
            if (v != null) v.Parent = u.Parent;
        }

        private void FixDelete(Node x)
        {
            Node s;
            while (x != root && x != null && x.Color == Color.Black)
            {
                if (x.Parent == null) break;

                if (x == x.Parent.Left)
                {
                    s = x.Parent.Right;
                    if (s != null && s.Color == Color.Red)
                    {
                        s.Color = Color.Black;
                        x.Parent.Color = Color.Red;
                        LeftRotate(x.Parent);
                        s = x.Parent.Right;
                    }
                    if (s == null || s == TNULL) { x = x.Parent; continue; }

                    if ((s.Left == null || s.Left.Color == Color.Black) && (s.Right == null || s.Right.Color == Color.Black))
                    {
                        s.Color = Color.Red; x = x.Parent;
                    }
                    else
                    {
                        if (s.Right == null || s.Right.Color == Color.Black)
                        {
                            if (s.Left != null) s.Left.Color = Color.Black;
                            s.Color = Color.Red; RightRotate(s); s = x.Parent.Right;
                        }
                        if (s != null)
                        {
                            s.Color = x.Parent.Color;
                            if (s.Right != null) s.Right.Color = Color.Black;
                        }
                        x.Parent.Color = Color.Black; LeftRotate(x.Parent); x = root;
                    }
                }
                else
                {
                    s = x.Parent.Left;
                    if (s != null && s.Color == Color.Red)
                    {
                        s.Color = Color.Black;
                        x.Parent.Color = Color.Red;
                        RightRotate(x.Parent);
                        s = x.Parent.Left;
                    }
                    if (s == null || s == TNULL) { x = x.Parent; continue; }

                    if ((s.Right == null || s.Right.Color == Color.Black) && (s.Left == null || s.Left.Color == Color.Black))
                    {
                        s.Color = Color.Red; x = x.Parent;
                    }
                    else
                    {
                        if (s.Left == null || s.Left.Color == Color.Black)
                        {
                            if (s.Right != null) s.Right.Color = Color.Black;
                            s.Color = Color.Red; LeftRotate(s); s = x.Parent.Left;
                        }
                        if (s != null)
                        {
                            s.Color = x.Parent.Color;
                            if (s.Left != null) s.Left.Color = Color.Black;
                        }
                        x.Parent.Color = Color.Black; RightRotate(x.Parent); x = root;
                    }
                }
            }
            if (x != null) x.Color = Color.Black;
        }

        private Node GetMinValueNode(Node node) { while (node != null && node.Left != TNULL && node.Left != null) node = node.Left; return node; }

        private int GetMaxDepthRec(Node node)
        {
            if (node == null || node == TNULL) return 0;
            return Math.Max(GetMaxDepthRec(node.Left), GetMaxDepthRec(node.Right)) + 1;
        }

        private int GetMinDepthRec(Node node)
        {
            if (node == null || node == TNULL) return 0;
            if ((node.Left == null || node.Left == TNULL) && (node.Right == null || node.Right == TNULL)) return 1;
            if (node.Left == null || node.Left == TNULL) return GetMinDepthRec(node.Right) + 1;
            if (node.Right == null || node.Right == TNULL) return GetMinDepthRec(node.Left) + 1;
            return Math.Min(GetMinDepthRec(node.Left), GetMinDepthRec(node.Right)) + 1;
        }
    }
}