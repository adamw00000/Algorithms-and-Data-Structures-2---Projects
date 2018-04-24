using ASD.Graphs;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace ASD
{

    public class Maze : MarshalByRefObject
    {

        int length;
        int width;
        /// <summary>
        /// Wersje zadania I oraz II
        /// Zwraca najkrótszy możliwy czas przejścia przez labirynt bez dynamitów lub z dowolną ich liczbą
        /// </summary>
        /// <param name="maze">labirynt</param>
        /// <param name="withDynamite">informacja, czy dostępne są dynamity 
        /// Wersja I zadania -> withDynamites = false, Wersja II zadania -> withDynamites = true</param>
        /// <param name="path">zwracana ścieżka</param>
        /// <param name="t">czas zburzenia ściany (dotyczy tylko wersji II)</param> 
        public int FindShortestPath(char[,] maze, bool withDynamite, out string path, int t = 0)
        {
            length = maze.GetLength(0);
            width = maze.GetLength(1);

            int nVertices = length * width;
            int start = 0, end = 0;

            Graph g = new AdjacencyListsGraph<AVLAdjacencyList>(true, nVertices);

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int v = ij2v(i, j);

                    if (maze[i, j] == 'S')
                        start = v;
                    if (maze[i, j] == 'E')
                        end = v;
                    
                    if(withDynamite)
                    {
                        if (i + 1 < length && maze[i + 1, j] != 'X')
                        {
                            g.AddEdge(v, ij2v(i + 1, j), 1);
                        }
                        else if (i + 1 < length)
                        {
                            g.AddEdge(v, ij2v(i + 1, j), t);
                        }
                        if (j + 1 < width && maze[i, j + 1] != 'X')
                        {
                            g.AddEdge(v, ij2v(i, j + 1), 1);
                        }
                        else if (j + 1 < width)
                        {
                            g.AddEdge(v, ij2v(i, j + 1), t);
                        }
                        if (i - 1 >= 0 && maze[i - 1, j] != 'X')
                        {
                            g.AddEdge(v, ij2v(i - 1, j), 1);
                        }
                        else if (i - 1 >= 0)
                        {
                            g.AddEdge(v, ij2v(i - 1, j), t);
                        }
                        if (j - 1 >= 0 && maze[i, j - 1] != 'X')
                        {
                            g.AddEdge(v, ij2v(i, j - 1), 1);
                        }
                        else if (j - 1 >= 0)
                        {
                            g.AddEdge(v, ij2v(i, j - 1), t);
                        }
                    }
                    else
                    {
                        if (maze[i, j] != 'X')
                        {
                            if (i + 1 < length && maze[i + 1, j] != 'X')
                            {
                                g.AddEdge(v, ij2v(i + 1, j), 1);
                                g.AddEdge(ij2v(i + 1, j), v, 1);
                            }
                            if (j + 1 < width && maze[i, j + 1] != 'X')
                            {
                                g.AddEdge(ij2v(i, j + 1), v, 1);
                                g.AddEdge(v, ij2v(i, j + 1), 1);
                            }
                        }
                    }
                }
            }

            g.DijkstraShortestPaths(start, out PathsInfo[] d);

            if ((int)d[end].Dist == int.MinValue)
            {
                path = "";
                return -1;
            }

            Edge[] pathEdges = PathsInfo.ConstructPath(start, end, d);

            StringBuilder builder = new StringBuilder();

            (int x, int y) = v2ij(pathEdges[0].From);
            foreach (var e in pathEdges)
            {
                (int xEnd, int yEnd) = v2ij(e.To);
                if (xEnd == x + 1)
                    builder.Append('S');
                if (xEnd == x - 1)
                    builder.Append('N');
                if (yEnd == y + 1)
                    builder.Append('E');
                if (yEnd == y - 1)
                    builder.Append('W');
                x = xEnd;
                y = yEnd;
            }

            path = builder.ToString(); // tej linii na laboratorium nie zmieniamy!
            return (int)d[end].Dist;
        }
        
        int ij2v(int i, int j)
        {
            return i * width + j;
        }
        (int, int) v2ij(int v)
        {
            return (v / width, v % width);
        }

        /// <summary>
        /// Wersja III i IV zadania
        /// Zwraca najkrótszy możliwy czas przejścia przez labirynt z użyciem co najwyżej k lasek dynamitu
        /// </summary>
        /// <param name="maze">labirynt</param>
        /// <param name="k">liczba dostępnych lasek dynamitu, dla wersji III k=1</param>
        /// <param name="path">zwracana ścieżka</param>
        /// <param name="t">czas zburzenia ściany</param>
        public int FindShortestPathWithKDynamites(char[,] maze, int k, out string path, int t)
        {
            length = maze.GetLength(0);
            width = maze.GetLength(1);

            k = k + 1;

            int nVertices = length * width * k;
            int start = 0;
            List<int> end = new List<int>();

            Graph g = new AdjacencyListsGraph<AVLAdjacencyList>(true, nVertices);

            for (int p = 0; p < k; p++)
            {
                for (int i = 0; i < length; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        int v = ij2v(p * length + i, j);

                        if (maze[i, j] == 'S' && p == 0)
                            start = v;
                        if (maze[i, j] == 'E')
                            end.Add(v);

                        if (i + 1 < length && maze[i + 1, j] != 'X')
                        {
                            g.AddEdge(v, ij2v(p*length + i + 1, j), 1);
                        }
                        else if (i + 1 < length && p < k - 1)
                        {
                            g.AddEdge(v, ij2v((p + 1)*length + i + 1, j), t);
                        }

                        if (j + 1 < width && maze[i, j + 1] != 'X')
                        {
                            g.AddEdge(v, ij2v(p * length + i, j + 1), 1);
                        }
                        else if (j + 1 < width && p < k - 1)
                        {
                            g.AddEdge(v, ij2v((p+1)*length + i, j + 1), t);
                        }

                        if (i - 1 >= 0 && maze[i - 1, j] != 'X')
                        {
                            g.AddEdge(v, ij2v(p * length + i - 1, j), 1);
                        }
                        else if (i - 1 >= 0 && p < k - 1)
                        {
                            g.AddEdge(v, ij2v((p + 1)*length + i - 1, j), t);
                        }

                        if (j - 1 >= 0 && maze[i, j - 1] != 'X')
                        {
                            g.AddEdge(v, ij2v(p * length + i, j - 1), 1);
                        }
                        else if (j - 1 >= 0 && p < k - 1)
                        {
                            g.AddEdge(v, ij2v((p + 1) * length + i, j - 1), t);
                        }
                    }

                }
            }


            g.DijkstraShortestPaths(start, out PathsInfo[] d);

            //path = null; // tej linii na laboratorium nie zmieniamy!

            int bestPath = int.MaxValue;
            int bestEnd = -1;
            for (int i = 0; i<end.Count; i++)
            {
                if ((int)d[end[i]].Dist < bestPath && (int)d[end[i]].Dist != int.MinValue)
                {
                    bestPath = (int)d[end[i]].Dist;
                    bestEnd = end[i];
                }
            }

            if (bestPath == int.MinValue || bestPath == int.MaxValue)
            {
                path = "";
                return -1;
            }


            Edge[] pathEdges = PathsInfo.ConstructPath(start, bestEnd, d);

            StringBuilder builder = new StringBuilder();

            (int x, int y) = v2ij(pathEdges[0].From);
            foreach (var e in pathEdges)
            {
                (int xEnd, int yEnd) = v2ij(e.To);
                if (x % length < length - 1 && (xEnd == x + 1 || xEnd == x + length + 1))
                    builder.Append('S');
                if (x % length > 0 && (xEnd == x - 1 || xEnd == x + length - 1))
                    builder.Append('N');
                if (y % width < width - 1 && (yEnd == y + 1 || yEnd == y + length + 1))
                    builder.Append('E');
                if (y % width > 0 && (yEnd == y - 1 || yEnd == y + length - 1))
                    builder.Append('W');
                x = xEnd;
                y = yEnd;
            }

            path = builder.ToString();

            return bestPath;
        }
        
    }
}