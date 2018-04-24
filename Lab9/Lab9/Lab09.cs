using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{

    // DEFINICJA
    // Skojarzeniem indukowanym grafu G nazywamy takie skojarzenie M,
    // ze żadne dwa konce roznych krawedzi z M nie sa polaczone krawedzia w G

    // Uwagi do obu metod
    // 1) Grafow bedacych parametrami nie wolno zmieniac
    // 2) Parametrami są zawsze grafy nieskierowane (nie trzeba tego sprawdzac)

    public class Lab09 : MarshalByRefObject
    {
        //Graph workGraph;

        double weightSum = 0;
        double bestWeightSum = 0;
        List<Edge> m;
        List<Edge> bestMatching;

        /// <summary>
        /// Funkcja znajduje dowolne skojarzenie indukowane o rozmiarze k w grafie graph
        /// </summary>
        /// <param name="graph">Badany graf nieskierowany</param>
        /// <param name="k">Rozmiar szukanego skojarzenia indukowanego</param>
        /// <param name="matching">Znalezione skojarzenie (lub null jeśli nie ma)</param>
        /// <returns>true jeśli znaleziono skojarzenie, false jesli nie znaleziono</returns>
        /// <remarks>
        /// Punktacja:  2 pkt, w tym
        ///     1.5  -  dzialajacy algorytm (testy podstawowe)
        ///     0.5  -  testy wydajnościowe
        /// </remarks>
        public bool InducedMatching(Graph graph, int k, out Edge[] matching)
        {
            bool[] used = new bool[graph.VerticesCount];

            //for (int v = 0; v < graph.VerticesCount; v++)
            //{
            //    if (graph.OutDegree(v) == 0)
            //        used[v] = true;
            //}

            m = new List<Edge>();
            bool result = FindMatchingOfSize(graph, k, 0, used);
            
            matching = result ? m.ToArray() : null;
            return result;
        }

        private bool FindMatchingOfSize(Graph g, int k, int nextVertex, bool[] used)
        {
            if (m.Count == k)
                return true;
            for (int v = nextVertex; v < g.VerticesCount; v++)
            {
                if (k - m.Count > g.VerticesCount - v + 1)
                    return false;
                if (used[v])
                    continue;
                foreach (var e in g.OutEdges(v))
                {
                    if (used[e.To])
                        continue;
                    if (e.From > e.To)
                        continue;
                    m.Add(e);

                    bool[] usedClone = new bool[used.Length];
                    used.CopyTo(usedClone, 0);

                    usedClone[e.From] = true;
                    usedClone[e.To] = true;

                    foreach (Edge f in g.OutEdges(e.From))
                    {
                        usedClone[f.To] = true;
                    }
                    foreach (Edge f in g.OutEdges(e.To))
                    {
                        usedClone[f.To] = true;
                    }

                    if (FindMatchingOfSize(g, k, v + 1, usedClone))
                        return true;
                    
                    m.Remove(e);
                }
            }
            return false;
        }
        
        /// <summary>
        /// Funkcja znajduje skojarzenie indukowane o maksymalnej sumie wag krawedzi w grafie graph
        /// </summary>
        /// <param name="graph">Badany graf nieskierowany</param>
        /// <param name="matching">Znalezione skojarzenie (jeśli puste to tablica 0-elementowa)</param>
        /// <returns>Waga skojarzenia</returns>
        /// <remarks>
        /// Punktacja:  2 pkt, w tym
        ///     1.5  -  dzialajacy algorytm (testy podstawowe)
        ///     0.5  -  testy wydajnościowe
        /// </remarks>
        /// 

        public double MaximalInducedMatching(Graph graph, out Edge[] matching)
        {
            weightSum = 0;
            bestWeightSum = 0;
            bestMatching = new List<Edge>();
            bool[] used = new bool[graph.VerticesCount];
            
            m = new List<Edge>();
            FindMaxMatching(graph, 0, used);

            matching = bestMatching.ToArray();
            return bestWeightSum;
        }

        private void FindMaxMatching(Graph g, int nextVertex, bool[] used)
        {
            bool isMaximal = true;
            for (int v = nextVertex; v < g.VerticesCount; v++)
            {
                if (used[v])
                    continue;
                foreach (var e in g.OutEdges(v))
                {
                    if (used[e.To])
                        continue;
                    if (e.From > e.To)
                        continue;

                    isMaximal = false;
                    bool[] usedClone = new bool[used.Length];
                    used.CopyTo(usedClone, 0);

                    m.Add(e);
                    weightSum += e.Weight;

                    usedClone[e.From] = true;
                    usedClone[e.To] = true;
                    
                    foreach (Edge f in g.OutEdges(e.From))
                    {
                        usedClone[f.To] = true;
                    }
                    foreach (Edge f in g.OutEdges(e.To))
                    {
                        usedClone[f.To] = true;
                    }

                    FindMaxMatching(g, v + 1, usedClone);

                    m.Remove(e);
                    weightSum -= e.Weight;
                }
            }

            if (isMaximal)
            {
                if (weightSum > bestWeightSum)
                {
                    bestWeightSum = weightSum;
                    bestMatching = new List<Edge>(m);
                }
            }
            
        }
        //funkcje pomocnicze

    }
}


