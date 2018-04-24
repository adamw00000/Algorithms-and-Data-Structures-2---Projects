using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASD
{
    /// <summary>
    /// struktura przechowująca punkt
    /// </summary>
    [Serializable]
    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Point (int x, int y)
        {
            X = x;
            Y = y;
        }
        public override string ToString()
        {
            return $"({X},{Y})";
        }
    }

    public class AdventurePlanner: MarshalByRefObject
    {
        /// <summary>
        /// największy rozmiar tablicy, którą wyświetlamy
        /// ustaw na 0, żeby nic nie wyświetlać
        /// </summary>
        public int MaxToShow = 0;

      
        /// <summary>
        /// Znajduje optymalną pod względem liczby znalezionych skarbów ścieżkę,
        /// zaczynającą się w lewym górnym rogu mapy (0,0), a kończącą się w prawym
        /// dolnym rogu (X-Y-1).
        /// Za każdym razem możemy wykonać albo krok w prawo albo krok w dół.
        /// Pierwszym polem ścieżki powinno być (0,0), a ostatnim polem (X-1,Y-1).        
        /// </summary>
        /// <param name="treasure">liczba znalezionych skarbów</param>
        /// <param name="path">znaleziona ścieżka</param>
        /// <remarks>
        /// Złożoność rozwiązania to O(X * Y).
        /// </remarks>
        /// <returns></returns>
        public int FindPathThere(int[,] treasure, out List<Point> path)
        {
            int length = treasure.GetLength(0);
            int width = treasure.GetLength(1);

            Point[,] previousPoint = new Point[length, width];
            int[,] maxValue = new int[length, width];

            maxValue[0, 0] = treasure[0, 0];
            previousPoint[0, 0] = new Point(0, 0);

            for (int x = 1; x < length; x++)
            {
                maxValue[x, 0] = maxValue[x - 1, 0] + treasure[x, 0];
                previousPoint[x, 0] = new Point(x - 1, 0);
            }
            for (int y = 1; y < width; y++)
            {
                maxValue[0, y] = maxValue[0, y - 1] + treasure[0, y];
                previousPoint[0, y] = new Point(0, y - 1);
            }

            for (int x = 1; x < length; x++)
            {
                for (int y = 1; y < width; y++)
                {

                    int maxValueRight = maxValue[x - 1, y] + treasure[x, y];
                    int maxValueDown = maxValue[x, y - 1] + treasure[x, y];

                    maxValue[x, y] = maxValueRight;
                    previousPoint[x, y] = new Point(x - 1, y);

                    if (maxValueDown > maxValueRight)
                    {
                        maxValue[x, y] = maxValueDown;
                        previousPoint[x, y] = new Point(x, y - 1);
                    }
                }
            }

            path = new List<Point>();
            path.Add(new Point(length - 1, width - 1));
            int X = length - 1;
            int Y = width - 1;
            while (X != 0 || Y != 0)
            {
                Point p = previousPoint[X, Y];
                path.Add(p);
                X = p.X;
                Y = p.Y;
            } 
            path.Reverse();

            return maxValue[length - 1, width - 1];
            //return -1;
        }

      
        /// <summary>
        /// Znajduje optymalną pod względem liczby znalezionych skarbów ścieżkę,
        /// zaczynającą się w lewym górnym rogu mapy (0,0), dochodzącą do prawego dolnego rogu (X-1,Y-1), a 
        /// następnie wracającą do lewego górnego rogu (0,0).
        /// W pierwszy etapie możemy wykonać albo krok w prawo albo krok w dół. Po osiągnięciu pola (x-1,Y-1)
        /// zacynamy wracać - teraz możemy wykonywać algo krok w prawo albo krok w górę.
        /// Pierwszym i ostatnim polem ścieżki powinno być (0,0).
        /// Możemy założyć, że X,Y >= 2.
        /// </summary>
        /// <param name="treasure">liczba znalezionych skarbów</param>
        /// <param name="path">znaleziona ścieżka</param>
        /// <remarks>
        /// Złożoność rozwiązania to O(X^2 * Y) lub O(X * Y^2).
        /// </remarks>
        /// <returns></returns>
        public int FindPathThereAndBack(int[,] treasure, out List<Point> path)
        {
            int length = treasure.GetLength(0);
            int width = treasure.GetLength(1);

            //(int x, int y, int i)[,,] previousPoint = new (int x, int y, int i)[length, width, length];
            int[,,] maxValue = new int[length, width, length];

            maxValue[0, 0, 0] = treasure[0, 0];
            //previousPoint[0, 0, 0] = (0, 0, 0);

            for (int x = 0; x < length; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    for (int i = 0; i < length; i++)
                    {
                        //previousPoint[x, y, i] = (-1, -1, -1);
                        if (x + y - i < 0)
                        {
                            continue;
                        }
                        if (x + y - i >= width)
                        {
                            continue;
                        }
                        if (x == 0 && y == 0)
                            continue;
                        if (x == length - 1 && y == width - 1)
                        {
                            if (x > 0 && y > 0)
                            {
                                maxValue[x, y, i] = Math.Max(maxValue[x - 1, y, i], maxValue[x, y - 1, i - 1]) + treasure[length - 1, width - 1];
                                //previousPoint[x, y, i] = (maxValue[x, y, i] == maxValue[x - 1, y, i]) ?
                                    //(x - 1, y, i) : (x, y - 1, i - 1);
                            }
                            else if (x > 0 && y == 0)
                            {
                                maxValue[x, y, i] = maxValue[x - 1, y, i - 1] + treasure[length - 1, width - 1];
                                //previousPoint[x, y, i] = (x - 1, y, i - 1);
                            }
                            else if (x == 0 && y > 0)
                            {
                                maxValue[x, y, i] = maxValue[x, y - 1, i] + treasure[length - 1, width - 1];
                                //previousPoint[x, y, i] = (x, y - 1, i);
                            }
                            continue;
                        }
                        if (length == 1)
                        {
                            maxValue[x, y, i] = maxValue[x, y - 1, i] + treasure[x, y];
                            //previousPoint[x, y, i] = (x, y - 1, i);
                            continue;
                        }
                        if (width == 1)
                        {
                            maxValue[x, y, i] = maxValue[x - 1, y, i - 1] + treasure[x, y];
                            //previousPoint[x, y, i] = (x - 1, y, i - 1);
                            continue;
                        }
                        if (x == i)
                        {
                            maxValue[x, y, i] = 0;
                            //previousPoint[x, y, i] = x > 0 ? (x - 1, y, i - 1) : (x, y - 1, y);
                            continue;
                        }

                        int thisAreaLoot = treasure[x, y] + treasure[i, x + y - i];

                        int maxValueRightRight = y > 0 && x + y - i > 0 ? (maxValue[x, y - 1, i] + thisAreaLoot) : -1;
                        int maxValueRightDown = y > 0 && i > 0 ? (maxValue[x, y - 1, i - 1] + thisAreaLoot) : -1;
                        int maxValueDownRight = x > 0 && x + y - i > 0 ? (maxValue[x - 1, y, i] + thisAreaLoot) : -1;
                        int maxValueDownDown = x > 0 && i > 0 ? (maxValue[x - 1, y, i - 1] + thisAreaLoot) : -1;


                        maxValue[x, y, i] = Math.Max(Math.Max(Math.Max(maxValueRightRight, maxValueDownRight), maxValueRightDown), maxValueDownDown);

                        //if (maxValue[x, y, i] == maxValueRightRight)
                        //{
                        //    previousPoint[x, y, i] = (x, y - 1, i);
                        //}
                        //else if (maxValue[x, y, i] == maxValueRightDown)
                        //{
                        //    previousPoint[x, y, i] = (x, y - 1, i - 1);
                        //}
                        //else if (maxValue[x, y, i] == maxValueDownRight)
                        //{
                        //    previousPoint[x, y, i] = (x - 1, y, i);
                        //}
                        //else if(maxValue[x, y, i] == maxValueDownDown)
                        //{
                        //    previousPoint[x, y, i] = (x - 1, y, i - 1);
                        //}
                    }
                }
            }

            int pathSize = 2 * (length - 1 + width - 1) + 1;
            Point[] pathTable = new Point[pathSize];
            pathTable[length - 1 + width - 1] = new Point(length - 1, width - 1);

            //(int, int, int) currentPoint = previousPoint[length - 1, width - 1, length - 1];
            (int, int, int) currentPoint = FindPreviousPoint(treasure, maxValue, length - 1, width - 1, length - 1);
            for (int i = length - 1 + width - 1 - 1; i >= 0; i--)
            {
                pathTable[i] = new Point(currentPoint.Item1, currentPoint.Item2);
                pathTable[pathSize - i - 1] = new Point(currentPoint.Item3, currentPoint.Item1 + currentPoint.Item2 - currentPoint.Item3);
                //currentPoint = previousPoint[currentPoint.Item1, currentPoint.Item2, currentPoint.Item3];
                currentPoint = FindPreviousPoint(treasure, maxValue, currentPoint.Item1, currentPoint.Item2, currentPoint.Item3);
            }

            path = pathTable.ToList();

            return maxValue[length - 1, width - 1, length - 1];
        }

        (int, int, int) FindPreviousPoint(int [,] treasure, int [,,] maxValue, int x, int y, int i)
        {
            int length = treasure.GetLength(0);
            int width = treasure.GetLength(1);
            if (x == length - 1 && y == width - 1)
            {
                if (x > 0 && y > 0)
                {
                    return (maxValue[x, y, i] == maxValue[x - 1, y, i]) ?
                        (x - 1, y, i) : (x, y - 1, i - 1);
                }
                else if (x > 0 && y == 0)
                {
                    return (x - 1, y, i - 1);
                }
                else if (x == 0 && y > 0)
                {
                    return (x, y - 1, i);
                }
            }
            if (length == 1)
            {
                return (x, y - 1, i);
            }
            if (width == 1)
            {
                return (x - 1, y, i - 1);
            }
            if (x == i)
            {
                return x > 0 ? (x - 1, y, i - 1) : (x, y - 1, y);
            }

            int thisAreaLoot = treasure[x, y] + treasure[i, x + y - i];

            int maxValueRightRight = y > 0 && x + y - i > 0 ? (maxValue[x, y - 1, i] + thisAreaLoot) : -1;
            int maxValueRightDown = y > 0 && i > 0 ? (maxValue[x, y - 1, i - 1] + thisAreaLoot) : -1;
            int maxValueDownRight = x > 0 && x + y - i > 0 ? (maxValue[x - 1, y, i] + thisAreaLoot) : -1;
            int maxValueDownDown = x > 0 && i > 0 ? (maxValue[x - 1, y, i - 1] + thisAreaLoot) : -1;

            if (maxValue[x, y, i] == maxValueRightRight)
            {
                return (x, y - 1, i);
            }
            else if (maxValue[x, y, i] == maxValueRightDown)
            {
                return (x, y - 1, i - 1);
            }
            else if (maxValue[x, y, i] == maxValueDownRight)
            {
                return (x - 1, y, i);
            }
            else if (maxValue[x, y, i] == maxValueDownDown)
            {
                return (x - 1, y, i - 1);
            }
            return (0, 0, 0);
        }
    }
}
