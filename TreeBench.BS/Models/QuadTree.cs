using System;
using System.Collections.Generic;

namespace TreeBench.BS.Models
{
    public class QuadTree : BaseBalancedTree
    {
        private const int CAPACITY = 4; 

        private class Point
        {
            public int X, Y, OriginalKey;
            public Point(int x, int y, int key) { X = x; Y = y; OriginalKey = key; }
        }

        private class Boundary
        {
            public int X, Y, Width, Height;
            public Boundary(int x, int y, int w, int h) { X = x; Y = y; Width = w; Height = h; }

            public bool Contains(Point p)
            {
                return p.X >= X - Width && p.X <= X + Width && p.Y >= Y - Height && p.Y <= Y + Height;
            }
        }

        private class QuadNode
        {
            public Boundary Boundary;
            public List<Point> Points = new List<Point>();
            public bool IsDivided = false;

            public QuadNode NorthWest, NorthEast, SouthWest, SouthEast;

            public QuadNode(Boundary boundary) { Boundary = boundary; }
        }

        private QuadNode root;

        public QuadTree()
        {
            root = new QuadNode(new Boundary(500000, 500000, 500000, 500000));
        }

        // --- INSERT Engine ---
        public override void Insert(int key)
        {
            // Mapping
            Point p = new Point(key % 1000, key / 1000, key);
            if (InsertInternal(root, p))
            {
                count++;
            }
        }

        private bool InsertInternal(QuadNode node, Point p)
        {
            if (!node.Boundary.Contains(p)) return false;

            if (node.Points.Count < CAPACITY && !node.IsDivided)
            {
                node.Points.Add(p);
                return true;
            }

            if (!node.IsDivided)
            {
                Subdivide(node);
            }

            if (InsertInternal(node.NorthWest, p)) return true;
            if (InsertInternal(node.NorthEast, p)) return true;
            if (InsertInternal(node.SouthWest, p)) return true;
            if (InsertInternal(node.SouthEast, p)) return true;

            return false;
        }

        private void Subdivide(QuadNode node)
        {
            int x = node.Boundary.X;
            int y = node.Boundary.Y;
            int w = node.Boundary.Width / 2;
            int h = node.Boundary.Height / 2;

            node.NorthWest = new QuadNode(new Boundary(x - w, y + h, w, h));
            node.NorthEast = new QuadNode(new Boundary(x + w, y + h, w, h));
            node.SouthWest = new QuadNode(new Boundary(x - w, y - h, w, h));
            node.SouthEast = new QuadNode(new Boundary(x + w, y - h, w, h));

            node.IsDivided = true;
            rotationsCount++; 

            var oldPoints = new List<Point>(node.Points);
            node.Points.Clear();
            foreach (var p in oldPoints)
            {
                InsertInternal(node.NorthWest, p);
                InsertInternal(node.NorthEast, p);
                InsertInternal(node.SouthWest, p);
                InsertInternal(node.SouthEast, p);
            }
        }

        // --- SEARCH Engine ---
        protected override bool SearchInternal(int key)
        {
            Point p = new Point(key % 1000, key / 1000, key);
            return QueryInternal(root, p);
        }

        private bool QueryInternal(QuadNode node, Point p)
        {
            if (!node.Boundary.Contains(p)) return false;

            foreach (var point in node.Points)
            {
                if (point.OriginalKey == p.OriginalKey) return true;
            }

            if (node.IsDivided)
            {
                if (QueryInternal(node.NorthWest, p)) return true;
                if (QueryInternal(node.NorthEast, p)) return true;
                if (QueryInternal(node.SouthWest, p)) return true;
                if (QueryInternal(node.SouthEast, p)) return true;
            }

            return false;
        }

        // --- DELETE Engine ---
        protected override void DeleteInternal(int key)
        {
            Point p = new Point(key % 1000, key / 1000, key);
            DeleteInternal(root, p);
        }

        private bool DeleteInternal(QuadNode node, Point p)
        {
            if (!node.Boundary.Contains(p)) return false;

            for (int i = 0; i < node.Points.Count; i++)
            {
                if (node.Points[i].OriginalKey == p.OriginalKey)
                {
                    node.Points.RemoveAt(i);
                    count--;
                    return true;
                }
            }

            if (node.IsDivided)
            {
                if (DeleteInternal(node.NorthWest, p)) return true;
                if (DeleteInternal(node.NorthEast, p)) return true;
                if (DeleteInternal(node.SouthWest, p)) return true;
                if (DeleteInternal(node.SouthEast, p)) return true;
            }

            return false;
        }

        // --- TOPOLOGY Metrics ---
        public override int GetMaxDepth() => GetDepthRec(root);

        public override int GetMinDepth()
        {
            return GetDepthRec(root) / 2;
        }

        private int GetDepthRec(QuadNode node)
        {
            if (node == null || !node.IsDivided) return 1;
            int d1 = GetDepthRec(node.NorthWest);
            int d2 = GetDepthRec(node.NorthEast);
            int d3 = GetDepthRec(node.SouthWest);
            int d4 = GetDepthRec(node.SouthEast);
            return Math.Max(Math.Max(d1, d2), Math.Max(d3, d4)) + 1;
        }
    }
}