using System;
using System.Collections.Generic;
using ASD.Graphs;

namespace asd2
{
    public class City : MarshalByRefObject
    {
        /// <summary>
        /// Sprawdza przecięcie zadanych ulic-odcinków. Zwraca liczbę punktów wspólnych.
        /// </summary>
        /// <returns>0 - odcinki rozłączne, 
        /// 1 - dokładnie jeden punkt wspólny, 
        /// int.MaxValue - odcinki częściowo pokrywają się (więcej niż 1 punkt wspólny)</returns>
        public int CheckIntersection(Street s1, Street s2)
        {
            return SegmentIntersection(s1.p1, s1.p2, s2.p1, s2.p2);
        }
        int SegmentIntersection(Point p1, Point p2, Point p3, Point p4)
        {
            double d1 = Point.CrossProduct(p4 - p3, p1 - p3); // położenie punktu p1 względem odcinka p3p4
            double d2 = Point.CrossProduct(p4 - p3, p2 - p3); ; // położenie punktu p2 względem odcinka p3p4
            double d3 = Point.CrossProduct(p2 - p1, p3 - p1); // położenie punktu p3 względem odcinka p1p2
            double d4 = Point.CrossProduct(p2 - p1, p4 - p1); // położenie punktu p4 względem odcinka p1p2

            double d12 = d1 * d2;
            double d34 = d3 * d4;

            if (d12 > 0 || d34 > 0) return 0;
            if (d12 < 0 || d34 < 0) return 1;
            
            if ((p1 == p3 && p2 == p4) || (p1 == p4 && p2 == p3))
                return int.MaxValue;

            if (p1 == p3 || p1 == p4 || p2 == p3 || p2 == p4)
            {
                Point eq, pf, ps;
                if (p1 == p3) { eq = p1; pf = p2; ps = p4; }
                else if (p1 == p4) { eq = p1; pf = p2; ps = p3; }
                else if (p2 == p3) { eq = p2; pf = p1; ps = p4; }
                else { eq = p2; pf = p1; ps = p3; }

                if (p1.x == p2.x && p3.x == p4.x)
                {
                    if ((pf.y < eq.y && pf.y > ps.y)|| (ps.y < eq.y && ps.y > pf.y) ||
                        (pf.y > eq.y && pf.y < ps.y) || (ps.y > eq.y && ps.y < pf.y))
                        return int.MaxValue;
                }
                else
                {
                    if (p1.x == p2.x || p3.x == p4.x)
                        return 1;

                    double a = (p1.y - p2.y) / (p1.x - p2.x);
                    double b = p2.y - a * p2.x;
                    double c = (p3.y - p4.y) / (p3.x - p4.x);
                    double d = p4.y - c * p4.x;

                    if (a != c || b != d)
                        return 1;

                    if ((pf.y < eq.y && pf.y > ps.y) || (ps.y < eq.y && ps.y > pf.y) ||
                        (pf.y > eq.y && pf.y < ps.y) || (ps.y > eq.y && ps.y < pf.y))
                        return int.MaxValue;
                }
                return 1;
            }

            if (OnRectangle(p1, p3, p4) || OnRectangle(p2, p3, p4) ||
                OnRectangle(p3, p1, p2) || OnRectangle(p4, p1, p2))
            {
                return int.MaxValue;
            }
            return 0;
        }

        bool OnRectangle(Point q, Point p1, Point p2)
        {
            return Math.Min(p1.x, p2.x) <= q.x && q.x <= Math.Max(p1.x, p2.x) &&
             Math.Min(p1.y, p2.y) <= q.y && q.y <= Math.Max(p1.y, p2.y);
        }


        /// <summary>
        /// Sprawdza czy dla podanych par ulic możliwy jest przejazd między nimi (z użyciem być może innych ulic). 
        /// </summary>
        /// <returns>Lista, w której na i-tym miejscu jest informacja czy przejazd między ulicami w i-tej parze z wejścia jest możliwy</returns>
        public bool[] CheckStreetsPairs(Street[] streets, int[] streetsToCheck1, int[] streetsToCheck2)
        {
            int n = streets.Length;

            UnionFind unionFind = new UnionFind(n);
            for (int i = 0; i<n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    int state = CheckIntersection(streets[i], streets[j]);
                    if (state == 1)
                    {
                        unionFind.Union(i, j);
                    }
                    else if (state == int.MaxValue)
                        throw new ArgumentException();
                }
            }

            bool[] result = new bool[streetsToCheck1.Length];
            for (int i = 0; i < streetsToCheck1.Length; i++)
            {
                if (unionFind.Find(streetsToCheck1[i]) == unionFind.Find(streetsToCheck2[i]))
                {
                    result[i] = true;
                }
                else
                    result[i] = false;
            }

            return result;
        }


        /// <summary>
        /// Zwraca punkt przecięcia odcinków s1 i s2.
        /// W przypadku gdy nie ma jednoznacznego takiego punktu rzuć wyjątek ArgumentException
        /// </summary>
        public Point GetIntersectionPoint(Street s1, Street s2)
        {
            //znajdź współczynniki a i b prostych y=ax+b zawierających odcinki s1 i s2
            //uwaga na proste równoległe do osi y
            //uwaga na odcinki równoległe o wspólnych końcu
            //porównaj równania prostych, aby znaleźć ich punkt wspólny
            if (CheckIntersection(s1, s2) != 1)
                throw new ArgumentException();

            if (s1.p1.x == s1.p2.x && s2.p1.x == s2.p2.x)
            {
                if (s1.p1.y == s2.p1.y) return s1.p1;
                if (s1.p1.y == s2.p2.y) return s1.p1;
                if (s1.p2.y == s2.p1.y) return s1.p2;
                return s1.p2;
            }
            else if (s1.p1.x == s1.p2.x)
            {
                double a = (s2.p1.y - s2.p2.y) / (s2.p1.x - s2.p2.x);
                double b = s2.p2.y - a * s2.p2.x;

                double x = s1.p1.x;
                Func<double, double> f = (x1) => (a * x1 + b);
                return new Point(x, f(x));
            }
            else if (s2.p1.x == s2.p2.x)
            {
                double c = (s1.p1.y - s1.p2.y) / (s1.p1.x - s1.p2.x);
                double d = s1.p2.y - c * s1.p2.x;

                double x = s2.p1.x;
                Func<double, double> f = (x1) => (c * x1 + d);
                return new Point(x, f(x));
            }
            else
            {
                double a = (s1.p1.y - s1.p2.y) / (s1.p1.x - s1.p2.x);
                double b = s1.p2.y - a * s1.p2.x;

                double c = (s2.p1.y - s2.p2.y) / (s2.p1.x - s2.p2.x);
                double d = s2.p2.y - c * s2.p2.x;

                if (a == c)
                {
                    if (s1.p1 == s2.p1) return s1.p1;
                    if (s1.p1 == s2.p2) return s1.p1;
                    if (s1.p2 == s2.p1) return s1.p2;
                    if (s1.p2 == s2.p2) return s1.p2;
                }
                double x = (d - b) / (a - c);
                Func<double, double> f = (x1) => (a * x1 + b);
                return new Point(x, f(x));
            }
        }


        /// <summary>
        /// Sprawdza możliwość przejazdu między dzielnicami-wielokątami district1 i district2,
        /// tzn. istnieją para ulic, pomiędzy którymi jest przejazd 
        /// oraz fragment jednej ulicy należy do obszaru jednej z dzielnic i fragment drugiej należy do obszaru drugiej dzielnicy
        /// </summary>
        /// <returns>Informacja czy istnieje przejazd między dzielnicami</returns>
        public bool CheckDistricts(Street[] streets, Point[] district1, Point[] district2, out List<int> path, out List<Point> intersections)
        {
            int n = streets.Length;
            Graph g = new AdjacencyListsGraph<HashTableAdjacencyList>(false, n + 2);

            int source = n;
            int target = n + 1;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < district1.Length; j++)
                {
                    int result = CheckIntersection(streets[i], new Street(district1[j], district1[(j + 1) % district1.Length]));
                    if (result == 1 || result == int.MaxValue)
                    {
                        g.AddEdge(source, i);
                    }
                }
                for (int j = 0; j < district2.Length; j++)
                {
                    int result = CheckIntersection(streets[i], new Street(district2[j], district2[(j + 1) % district2.Length]));
                    if (result == 1 || result == int.MaxValue)
                    {
                        g.AddEdge(i, target);
                    }
                }
                for (int j = i + 1; j < n; j++)
                {
                    int result = CheckIntersection(streets[i], streets[j]);
                    if (result == 1)
                    {
                        g.AddEdge(i, j);
                    }
                    else if (result == int.MaxValue)
                    {
                        throw new ArgumentException();
                    }
                }
            }

            g.DijkstraShortestPaths(source, out PathsInfo[] d);

            path = new List<int>();
            intersections = new List<Point>();

            if (d[target].Dist.IsNaN())
                return false;

            Edge[] edges = PathsInfo.ConstructPath(source, target, d);
            int previous = -1;

            for (int i = 0; i < edges.Length - 1; i++)
            {
                path.Add(edges[i].To);
                if (previous != -1)
                {
                    intersections.Add(GetIntersectionPoint(streets[previous], streets[edges[i].To]));
                }
                previous = edges[i].To;
            }

            return true;
        }
    }

    [Serializable]
    public struct Point
    {
        public double x;
        public double y;

        public Point(double px, double py) { x = px; y = py; }

        public static Point operator +(Point p1, Point p2) { return new Point(p1.x + p2.x, p1.y + p2.y); }

        public static Point operator -(Point p1, Point p2) { return new Point(p1.x - p2.x, p1.y - p2.y); }

        public static bool operator ==(Point p1, Point p2) { return p1.x == p2.x && p1.y == p2.y; }

        public static bool operator !=(Point p1, Point p2) { return !(p1 == p2); }

        public override bool Equals(object obj) { return base.Equals(obj); }

        public override int GetHashCode() { return base.GetHashCode(); }

        public static double CrossProduct(Point p1, Point p2) { return p1.x * p2.y - p2.x * p1.y; }

        public override string ToString() { return String.Format("({0},{1})", x, y); }
    }

    [Serializable]
    public struct Street
    {
        public Point p1;
        public Point p2;

        public Street(Point p1, Point p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }
    }
}