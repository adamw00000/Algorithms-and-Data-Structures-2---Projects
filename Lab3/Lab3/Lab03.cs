
using ASD.Graphs;
using System;
using System.Collections.Generic;
namespace ASD
{

    // Klasy Lab03Helper NIE WOLNO ZMIENIAĆ !!!
    //public class Lab03Helper : System.MarshalByRefObject
    //{
    //    public Graph SquareOfGraph(Graph graph) => graph.SquareOfGraph();
    //    public Graph LineGraph(Graph graph, out (int x, int y)[] names) => graph.LineGraph(out names);
    //    public int VertexColoring(Graph graph, out int[] colors) => graph.VertexColoring(out colors);
    //    public int StrongEdgeColoring(Graph graph, out Graph coloredGraph) => graph.StrongEdgeColoring(out coloredGraph);
    //}

    public class Lab03Helper : System.MarshalByRefObject
    {
        public Graph SquareOfGraph(Graph graph) => graph.SquareOfGraph();
        public Graph LineGraph(Graph graph, out int[,] names)
        {
            Graph g = graph.LineGraph(out (int x, int y)[] _names);
            if (_names == null)
                names = null;
            else
            {
                names = new int[_names.Length, 2];
                for (int i = 0; i < _names.Length; ++i)
                {
                    names[i, 0] = _names[i].x;
                    names[i, 1] = _names[i].y;
                }
            }
            return g;
        }
        public int VertexColoring(Graph graph, out int[] colors) => graph.VertexColoring(out colors);
        public int StrongEdgeColoring(Graph graph, out Graph coloredGraph) => graph.StrongEdgeColoring(out coloredGraph);
    }

    // Uwagi do wszystkich metod
    // 1) Grafy wynikowe powinny być reprezentowane w taki sam sposób jak grafy będące parametrami
    // 2) Grafów będących parametrami nie wolno zmieniać
    static class Lab03
    {

        // 0.5 pkt
        // Funkcja zwracajaca kwadrat grafu graph.
        // Kwadratem grafu nazywamy graf o takim samym zbiorze wierzcholkow jak graf bazowy,
        // 2 wierzcholki polaczone sa krawedzia jesli w grafie bazowym byly polaczone krawedzia badz sciezka zlozona z 2 krawedzi
        public static Graph SquareOfGraph(this Graph graph)
        {
            Graph g = graph.IsolatedVerticesGraph();

            for (int i = 0; i < graph.VerticesCount; i++)
            {
                foreach (var e in graph.OutEdges(i))
                {
                    double gWeight = g.GetEdgeWeight(e.From, e.To);

                    if (double.IsNaN(gWeight))
                        g.AddEdge(e.From, e.To, 1);

                    foreach (var e2 in graph.OutEdges(e.To))
                    {
                        if (e.From != e2.To)
                        {
                            double g2Weight = g.GetEdgeWeight(e.From, e2.To);

                            if (double.IsNaN(g2Weight))
                                g.AddEdge(e.From, e2.To, 1);
                        }
                    }

                }
            }

            return g;
        }

        // 2 pkt
        // Funkcja zwracająca Graf krawedziowy grafu graph
        // Wierzcholki grafu krawedziwego odpowiadaja krawedziom grafu bazowego,
        // 2 wierzcholki grafu krawedziwego polaczone sa krawedzia
        // jesli w grafie bazowym z krawędzi odpowiadającej pierwszemu z nic można przejść 
        // na krawędź odpowiadającą drugiemu z nich przez wspólny wierzchołek.
        //
        // (w grafie skierowanym: 2 wierzcholki grafu krawedziwego polaczone sa krawedzia
        // jesli wierzcholek koncowy krawedzi odpowiadajacej pierwszemu z nich
        // jest wierzcholkiem poczatkowym krawedzi odpowiadajacej drugiemu z nich)
        //
        // do tablicy names nalezy wpisac numery wierzcholkow grafu krawedziowego,
        // np. dla wierzcholka powstalego z krawedzi <0,1> do tabeli zapisujemy krotke (0, 1) - przyda się w dalszych etapach
        //
        // UWAGA: Graf bazowy może być skierowany lub nieskierowany, graf krawędziowy zawsze jest nieskierowany.
        public static Graph LineGraph(this Graph graph, out (int x, int y)[] names)
        {
            int numberOfEdges = 0;

            Graph nameGraph = graph.IsolatedVerticesGraph();
            names = new(int x, int y)[graph.EdgesCount];

            AssignNames(graph, names, ref numberOfEdges, nameGraph);

            Graph edgeGraph = nameGraph.IsolatedVerticesGraph(false, numberOfEdges);

            CreateEdgeGraph(nameGraph, edgeGraph);

            return edgeGraph;
        }

        private static void CreateEdgeGraph(Graph g, Graph edgeGraph)
        {
            for (int i = 0; i < g.VerticesCount; i++)
            {
                foreach (var e in g.OutEdges(i))
                {
                    foreach (var f in g.OutEdges(e.To))
                    {
                        int eName = (int)e.Weight;
                        int fName = (int)f.Weight;
                        double efWeight = edgeGraph.GetEdgeWeight(eName, fName);

                        if (eName != fName && double.IsNaN(efWeight))
                            edgeGraph.AddEdge(eName, fName);
                    }
                }
            }
        }

        private static void AssignNames(Graph graph, (int x, int y)[] names, ref int n, Graph g)
        {
            for (int i = 0; i < graph.VerticesCount; i++)
            {
                foreach (var e in graph.OutEdges(i))
                {
                    double gWeight = g.GetEdgeWeight(e.From, e.To);

                    if (double.IsNaN(gWeight))
                    {
                        g.AddEdge(e.From, e.To, n);
                        names[n++] = (e.From, e.To);
                    }
                }
            }
        }

        // 1 pkt
        // Funkcja znajdujaca poprawne kolorowanie wierzcholkow grafu graph
        // Kolorowanie wierzcholkow jest poprawne, gdy kazde dwa sasiadujace wierzcholki maja rozne kolory
        // Funkcja ma szukać kolorowania wedlug nastepujacego algorytmu zachlannego:
        //
        // Dla wszystkich wierzcholkow (od 0 do n-1) 
        //      pokoloruj wierzcholek v na najmniejszy mozliwy kolor (czyli taki, na ktory nie sa pomalowani jego sasiedzi)
        //
        // Nalezy zwrocic liczbe kolorow, a w tablicy colors zapamietac kolory dla poszczegolnych wierzcholkow
        //
        // UWAGA: Dla grafów skierowanych metoda powinna zgłaszać wyjątek ArgumentException
        public static int VertexColoring(this Graph graph, out int[] colors)
        {
            if (graph.Directed)
                throw new ArgumentException();

            colors = new int[graph.VerticesCount];

            int numberOfColors = 0;
            if (graph.VerticesCount > 0)
                numberOfColors++;

            List<int> neighbourColors = new List<int>();

            for (int i = 0; i < graph.VerticesCount; i++)
            {
                ListNeighbourColors(graph, colors, neighbourColors, i);

                ColorCurrentVertex(colors, ref numberOfColors, neighbourColors, i);
            }

            return numberOfColors;
        }

        private static void ListNeighbourColors(Graph graph, int[] colors, List<int> neighbourColors, int i)
        {
            neighbourColors.Clear();
            foreach (var e in graph.OutEdges(i))
            {
                if (e.To < i)
                {
                    neighbourColors.Add(colors[e.To]);
                }
            }
        }

        private static void ColorCurrentVertex(int[] colors, ref int n, List<int> neighbourColors, int i)
        {
            colors[i] = 0;

            bool isCorrectlyColored = false;
            while (!isCorrectlyColored)
            {
                isCorrectlyColored = true;

                if (colors[i] + 1 > n)
                    n++;

                else if (neighbourColors.Contains(colors[i]))
                {
                    isCorrectlyColored = false;
                    colors[i]++;
                }
            }
        }

        // 0.5 pkt
        // Funkcja znajdujaca silne kolorowanie krawedzi grafu graph
        // Silne kolorowanie krawedzi grafu jest poprawne gdy kazde dwie krawedzie, ktore sa ze soba sasiednie
        // albo sa polaczone inna krawedzia, maja rozne kolory.
        //
        // Nalezy zwrocic nowy graf, ktory bedzie kopia zadanego grafu, ale w wagach krawedzi zostana zapisane znalezione kolory
        // 
        // Wskazowka - to bardzo proste. Nalezy tu wykorzystac wszystkie poprzednie funkcje. 
        // Zastanowic sie co mozemy powiedziec o kolorowaniu wierzcholkow kwadratu grafu krawedziowego - jak sie ma do silnego kolorowania krawedzi grafu bazowego
        public static int StrongEdgeColoring(this Graph graph, out Graph coloredGraph)
        {
            Graph lineGraph = graph.LineGraph(out (int x, int y)[] names);
            lineGraph = lineGraph.SquareOfGraph();
            int n = lineGraph.VertexColoring(out int[] colors);

            Graph g = graph.IsolatedVerticesGraph();

            for (int i = 0; i < colors.Length; i++)
            {
                g.AddEdge(names[i].x, names[i].y, colors[i]);
            }
            coloredGraph = g;
            return n;
        }
    }
}