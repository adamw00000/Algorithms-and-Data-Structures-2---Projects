using System;
using System.Collections.Generic;

namespace ASD
{
    public class WaterCalculator : MarshalByRefObject
    {

        /*
         * Metoda sprawdza, czy przechodząc p1->p2->p3 skręcamy w lewo 
         * (jeżeli idziemy prosto, zwracany jest fałsz).
         */
        private bool leftTurn(Point p1, Point p2, Point p3)
        {
            Point w1 = new Point(p2.x - p1.x, p2.y - p1.y);
            Point w2 = new Point(p3.x - p2.x, p3.y - p2.y);
            double vectProduct = w1.x * w2.y - w2.x * w1.y;
            return vectProduct > 0;
        }


        /*
         * Metoda wyznacza punkt na odcinku p1-p2 o zadanej współrzędnej y.
         * Jeżeli taki punkt nie istnieje (bo cały odcinek jest wyżej lub niżej), zgłaszany jest wyjątek ArgumentException.
         */
        private Point getPointAtY(Point p1, Point p2, double y)
        {
            if (p1.y != p2.y)
            {
                double newX = p1.x + (p2.x - p1.x) * (y - p1.y) / (p2.y - p1.y);
                if ((newX - p1.x) * (newX - p2.x) > 0)
                    throw new ArgumentException("Odcinek p1-p2 nie zawiera punktu o zadanej współrzędnej y!");
                return new Point(p1.x + (p2.x - p1.x) * (y - p1.y) / (p2.y - p1.y), y);
            }
            else
            {
                if (p1.y != y)
                    throw new ArgumentException("Odcinek p1-p2 nie zawiera punktu o zadanej współrzędnej y!");
                return new Point((p1.x + p2.x) / 2, y);
            }
        }


        /// <summary>
        /// Funkcja zwraca tablice t taką, że t[i] jest głębokością, na jakiej znajduje się punkt points[i].
        /// 
        /// Przyjmujemy, że pierwszy punkt z tablicy points jest lewym krańcem, a ostatni - prawym krańcem łańcucha górskiego.
        /// </summary>
        public double[] PointDepths(Point[] points)
        {
            int n = points.Length;
            double h = points[0].y;

            double[] depth = new double[n];

            for (int i = 0; i < n; i++)
            {
                if (i > 0 && points[i].x < points[i - 1].x)
                {
                    depth[i] = 0;
                    h = 0;
                    continue;
                }

                if (points[i].y <= h)
                {
                    depth[i] = h - points[i].y;
                }
                else
                {
                    depth[i] = 0;
                    h = points[i].y;
                }
            }

            h = points[n - 1].y;
            for (int i = n - 1; i >= 0; i--)
            {
                if (i < n - 1 && points[i].x > points[i + 1].x)
                {
                    depth[i] = 0;
                    h = 0;
                    continue;
                }

                if (points[i].y <= h)
                {
                    depth[i] = Math.Min(h - points[i].y, depth[i]);
                }
                else
                {
                    depth[i] = 0;
                    h = points[i].y;
                }
            }

            return depth;
        }
        /// <summary>
        /// Funkcja zwraca objętość wody, jaka zatrzyma się w górach.
        /// 
        /// Przyjmujemy, że pierwszy punkt z tablicy points jest lewym krańcem, a ostatni - prawym krańcem łańcucha górskiego.
        /// </summary>
        public double WaterVolume(Point[] points)
        {
            int n = points.Length;
            double[] depth = PointDepths(points);
            double volume = 0;

            for (int i = 0; i < n - 1; i++)
            {
                if (depth[i] == 0 && depth[i + 1] == 0)
                    continue;
                if (depth[i] != 0 && depth[i + 1] != 0)
                {
                    volume += (depth[i] + depth[i + 1]) * (points[i + 1].x - points[i].x) / 2;
                }
                else
                {
                    if (depth[i] != 0)
                    {
                        Point p = getPointAtY(points[i], points[i + 1], points[i].y + depth[i]);
                        volume += depth[i] * (p.x - points[i].x) / 2;
                    }
                    if (depth[i + 1] != 0)
                    {
                        Point p = getPointAtY(points[i], points[i + 1], points[i + 1].y + depth[i + 1]);
                        volume += depth[i + 1] * (points[i + 1].x - p.x) / 2;
                    }
                }
            }
            return volume;
        }
    }

    [Serializable]
    public struct Point
    {
        public double x, y;
        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
