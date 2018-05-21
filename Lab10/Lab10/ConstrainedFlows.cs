using ASD.Graphs;

namespace ASD
{
    public class ConstrainedFlows : System.MarshalByRefObject
    {
        // testy, dla których ma być generowany obrazek
        // graf w ostatnim teście ma bardzo dużo wierzchołków, więc lepiej go nie wyświetlać
        public static int[] circulationToDisplay = {  };
        public static int[] constrainedFlowToDisplay = {  };

        /// <summary>
        /// Metoda znajdująca cyrkulację w grafie, z określonymi żądaniami wierzchołków.
        /// Żądania opisane są w tablicy demands. Szukamy funkcji, która dla każdego wierzchołka będzie spełniała warunek:
        /// suma wartości na krawędziach wchodzących - suma wartości na krawędziach wychodzących = demands[v]
        /// </summary>
        /// <param name="G">Graf wejściowy, wagi krawędzi oznaczają przepustowości</param>
        /// <param name="demands">Żądania wierzchołków</param>
        /// <returns>Graf reprezentujący wynikową cyrkulację.
        /// Reprezentacja cyrkulacji jest analogiczna, jak reprezentacja przepływu w innych funkcjach w bibliotece.
        /// Należy zwrócić kopię grafu G, gdzie wagi krawędzi odpowiadają przepływom na tych krawędziach.
        /// Zwróć uwagę na rozróżnienie sytuacji, kiedy mamy zerowy przeływ na krawędzi (czyli istnieje
        /// krawędź z wagą 0) od sytuacji braku krawędzi.
        /// Jeśli żądana cyrkulacja nie istnieje, zwróć null.
        /// </returns>
        /// <remarks>
        /// Nie można modyfikować danych wejściowych!
        /// Złożoność metody powinna być asymptotycznie równa złożoności metody znajdującej największy przeływ (z biblioteki).
        /// </remarks>
        public Graph FindCirculation(Graph G, double[] demands)
        {
            int n = G.VerticesCount;
            Graph g = G.IsolatedVerticesGraph(true, n + 2);
            double sumIn = 0;
            double sumOut = 0;
            for (int v = 0; v < n; v++)
            {
                if (demands[v] < 0)
                {
                    g.AddEdge(n, v, System.Math.Abs(demands[v]));
                    sumIn += System.Math.Abs(demands[v]);
                }
                if (demands[v] > 0)
                {
                    g.AddEdge(v, n + 1, System.Math.Abs(demands[v]));
                    sumOut += System.Math.Abs(demands[v]);
                }
                foreach (Edge e in G.OutEdges(v))
                {
                    g.AddEdge(e);
                }
            }

            if (sumIn != sumOut)
                return null;

            (double value, Graph flow) = g.PushRelabelMaxFlow(n, n + 1);

            if (value != sumIn)
                return null;

            Graph result = G.IsolatedVerticesGraph();

            for (int v = 0; v < n; v++)
            {
                foreach (Edge e in flow.OutEdges(v))
                {
                    if (e.To != n + 1)
                        result.AddEdge(e);
                }
            }

            return result;
        }

        /// <summary>
        /// Funkcja zwracająca przepływ z ograniczeniami, czyli przepływ, który dla każdej z krawędzi
        /// ma wartość pomiędzy dolnym ograniczeniem a górnym ograniczeniem.
        /// Zwróć uwagę, że interesuje nas *jakikolwiek* przepływ spełniający te ograniczenia.
        /// </summary>
        /// <param name="source">źródło</param>
        /// <param name="sink">ujście</param>
        /// <param name="G">graf wejściowy, wagi krawędzi oznaczają przepustowości (górne ograniczenia)</param>
        /// <param name="lowerBounds">kopia grafu G, wagi krawędzi oznaczają dolne ograniczenia przepływu</param>
        /// <returns>Graf reprezentujący wynikowy przepływ (analogicznie do poprzedniej funkcji i do reprezentacji
        /// przepływu w funkcjach z biblioteki.
        /// Jeśli żądany przepływ nie istnieje, zwróć null.
        /// </returns>
        /// <remarks>
        /// Nie można modyfikować danych wejściowych!
        /// Złożoność metody powinna być asymptotycznie równa złożoności metody znajdującej największy przeływ (z biblioteki).
        /// </remarks>
        /// <hint>Wykorzystaj poprzednią część zadania.
        /// </hint>
        public Graph FindConstrainedFlow(int source, int sink, Graph G, Graph lowerBounds)
        {
            int n = G.VerticesCount;
            double[] demands = new double[n];

            Graph workGraph = G.Clone();

            for (int v = 0; v < n; v++)
            {
                foreach (Edge e in lowerBounds.OutEdges(v))
                {
                    workGraph.ModifyEdgeWeight(e.From, e.To, -lowerBounds.GetEdgeWeight(e.From, e.To));
                    demands[e.To] -= e.Weight;
                    demands[e.From] += e.Weight;
                }
            }

            bool collision = false;

            if (double.IsNaN(workGraph.GetEdgeWeight(sink, source)))
                workGraph.AddEdge(sink, source, double.MaxValue);
            else
            {
                workGraph.ModifyEdgeWeight(sink, source,
                    double.MaxValue - workGraph.GetEdgeWeight(sink, source));
                collision = true;
            }

            Graph result = FindCirculation(workGraph, demands);
            if (result == null)
                return result;

            if (!collision)
                result.DelEdge(sink, source);
            else
                result.ModifyEdgeWeight(sink, source, -result.GetEdgeWeight(sink, source));


            for (int v = 0; v < n; v++)
            {
                foreach (Edge e in lowerBounds.OutEdges(v))
                {
                    result.ModifyEdgeWeight(e.From, e.To, lowerBounds.GetEdgeWeight(e.From, e.To));
                }
            }

            return result;  // zmienić
        }

    }
}