using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{
    public class CyclesFinder : MarshalByRefObject
    {
        /// <summary>
        /// Sprawdza czy graf jest drzewem
        /// </summary>
        /// <param name="g">Graf</param>
        /// <returns>true jeśli graf jest drzewem</returns>
        public bool IsTree(Graph g)
        {
            if (g.Directed)
                throw new ArgumentException("Directed graph");

            g.DFSearchAll(null, null, out int cc);

            if (cc > 1 || g.EdgesCount > g.VerticesCount + 1) return false;

            return true;
        }

        /// <summary>
        /// Wyznacza cykle fundamentalne grafu g względem drzewa t.
        /// Każdy cykl fundamentalny zawiera dokadnie jedną krawędź spoza t.
        /// </summary>
        /// <param name="g">Graf</param>
        /// <param name="t">Drzewo rozpinające grafu g</param>
        /// <returns>Tablica cykli fundamentalnych</returns>
        /// <remarks>W przypadku braku cykli zwracać pustą (0-elementową) tablicę, a nie null</remarks>
        public Edge[][] FindFundamentalCycles(Graph g, Graph t)
        {
            if (g.Directed)
                throw new ArgumentException("Directed graph");

            List<Edge[]> fundamentalCycles = new List<Edge[]>();

            //GraphExport export = new GraphExport();
            //export.Export(g);
            //export.Export(t);

            for (int x = 0; x < t.VerticesCount; x++)
            {
                for (int y = x + 1; y < t.VerticesCount; y++)
                {
                    if (!double.IsNaN(g.GetEdgeWeight(x, y)) && double.IsNaN(t.GetEdgeWeight(x, y)))
                    {
                        List<int> cycle = new List<int>();
                        t.DFSearchFrom(x, v =>
                        {
                            cycle.Add(v);
                            if (v == y) return false;
                            return true;
                        }, v =>
                        {
                            cycle.Remove(v);
                            return true;
                        });

                        Edge[] cycleEdges = new Edge[cycle.Count];

                        for (int i = 0; i < cycle.Count - 1; i++)
                        {
                            cycleEdges[i] = new Edge(cycle[i], cycle[i + 1], t.GetEdgeWeight(i, i+1));
                        }
                        cycleEdges[cycle.Count - 1] = new Edge(y, x, g.GetEdgeWeight(y,x));

                        fundamentalCycles.Add(cycleEdges);
                    }
                }
            }

            return fundamentalCycles.ToArray();
        }

        /// <summary>
        /// Dodaje 2 cykle fundamentalne
        /// </summary>
        /// <param name="c1">Pierwszy cykl</param>
        /// <param name="c2">Drugi cykl</param>
        /// <returns>null, jeśli wynikiem nie jest cykl i suma cykli, jeśli wynik jest cyklem</returns>
        public Edge[] AddFundamentalCycles(Edge[] c1, Edge[] c2)
        {

            //Edge[] cycleSum = c1.Union(c2).Where(e => !c1.Intersect(c2).Contains(e) && !c1.Intersect(c2).Contains(new Edge(e.To, e.From))).ToArray();

            List<Edge> cycleSum = new List<Edge>();

            foreach (Edge e in c1)
            {
                if (!c2.Contains(e) && !c2.Contains(new Edge(e.To, e.From)))
                    cycleSum.Add(e);
            }
            foreach (Edge e in c2)
            {
                if (!c1.Contains(e) && !c1.Contains(new Edge(e.To, e.From)))
                    cycleSum.Add(e);
            }

            int vertexCount = cycleSum.Max(x => Math.Max(x.From, x.To)) + 1;

            AdjacencyListsGraph<AVLAdjacencyList> g = new AdjacencyListsGraph<AVLAdjacencyList>(false, vertexCount);

            foreach (var e in cycleSum)
            {
                g.AddEdge(e.From, e.To, 1);
            }

            int skladowe = 1;
            for (int i = 0; i<g.VerticesCount; i++)
            {
                if (g.OutDegree(i) != 2)
                {
                    if (g.OutDegree(i) != 0)
                        return null;
                    else skladowe++;
                }
            }

            g.DFSearchAll(null, null, out int cc);
            if (cc != skladowe) return null;

            List<Edge> ret = new List<Edge>();
            ret.Add(cycleSum[0]);
            int end = cycleSum[0].To;
            Edge currentEdge;
            do
            {
                currentEdge = g.OutEdges(end).Where(e => !ret.Contains(e) && !ret.Contains(new Edge(e.To, e.From))).First();
                end = currentEdge.To;
                ret.Add(currentEdge);
            } while (end != cycleSum[0].From);

            return ret.ToArray();
        }

    }

}
