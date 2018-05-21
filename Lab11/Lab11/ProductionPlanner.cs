using ASD.Graphs;
using System;

namespace ASD
{
    public class ProductionPlanner : MarshalByRefObject
    {
        /// <summary>
        /// Flaga pozwalająca na włączenie wypisywania szczegółów skonstruowanego planu na konsolę.
        /// Wartość <code>true</code> spoeoduje wypisanie planu.
        /// </summary>
        public bool ShowDebug { get; } = false;

        /// <summary>
        /// Część 1. zadania - zaplanowanie produkcji telewizorów dla pojedynczego kontrahenta.
        /// </summary>
        /// <remarks>
        /// Do przeprowadzenia testów wyznaczających maksymalną produkcję i zysk wymagane jest jedynie zwrócenie obiektu <see cref="PlanData"/>.
        /// Testy weryfikujące plan wymagają przypisania tablicy z planem do parametru wyjściowego <see cref="weeklyPlan"/>.
        /// </remarks>
        /// <param name="production">
        /// Tablica obiektów zawierających informacje o produkcji fabryki w kolejnych tygodniach.
        /// Wartości pola <see cref="PlanData.Quantity"/> oznaczają limit produkcji w danym tygodniu,
        /// a pola <see cref="PlanData.Value"/> - koszt produkcji jednej sztuki.
        /// </param>
        /// <param name="sales">
        /// Tablica obiektów zawierających informacje o sprzedaży w kolejnych tygodniach.
        /// Wartości pola <see cref="PlanData.Quantity"/> oznaczają maksymalną sprzedaż w danym tygodniu,
        /// a pola <see cref="PlanData.Value"/> - cenę sprzedaży jednej sztuki.
        /// </param>
        /// <param name="storageInfo">
        /// Obiekt zawierający informacje o magazynie.
        /// Wartość pola <see cref="PlanData.Quantity"/> oznacza pojemność magazynu,
        /// a pola <see cref="PlanData.Value"/> - koszt przechowania jednego telewizora w magazynie przez jeden tydzień.
        /// </param>
        /// <param name="weeklyPlan">
        /// Parametr wyjściowy, przez który powinien zostać zwrócony szczegółowy plan sprzedaży.
        /// </param>
        /// <returns>
        /// Obiekt <see cref="PlanData"/> opisujący wyznaczony plan.
        /// W polu <see cref="PlanData.Quantity"/> powinna znaleźć się maksymalna liczba wyprodukowanych telewizorów,
        /// a w polu <see cref="PlanData.Value"/> - wyznaczony maksymalny zysk fabryki.
        /// </returns>
        public PlanData CreateSimplePlan(PlanData[] production, PlanData[] sales, PlanData storageInfo,
            out SimpleWeeklyPlan[] weeklyPlan)
        {
            int weeks = production.Length;

            if (weeks == 0 || storageInfo.Quantity < 0 || storageInfo.Value < 0 || weeks != sales.Length)
                throw new ArgumentException();
            for (int i = 0; i < weeks; i++)
            {
                if (production[i].Quantity < 0 || production[i].Value < 0
                   || sales[i].Quantity < 0 || sales[i].Value < 0)
                {
                    throw new ArgumentException();
                }
            }

            Graph g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, weeks + 2);
            Graph c = g.IsolatedVerticesGraph();
            //n - source
            //n+1 - target

            for (int i = 0; i < weeks; i++)
            {
                g.AddEdge(weeks, i, production[i].Quantity);
                c.AddEdge(weeks, i, production[i].Value);

                g.AddEdge(i, weeks + 1, sales[i].Quantity);
                c.AddEdge(i, weeks + 1, -sales[i].Value);

                if (i < weeks - 1)
                {
                    g.AddEdge(i, i + 1, storageInfo.Quantity);
                    c.AddEdge(i, i + 1, storageInfo.Value);
                }
            }
            (double value, double cost, Graph flow) = g.MinCostFlow(c, weeks, weeks + 1, true, MaxFlowGraphExtender.FordFulkersonDinicMaxFlow, MaxFlowGraphExtender.MKMBlockingFlow);

            weeklyPlan = new SimpleWeeklyPlan[weeks];

            for (int i = 0; i < weeks; i++)
            {
                weeklyPlan[i].UnitsProduced = (int)flow.GetEdgeWeight(weeks, i);
                weeklyPlan[i].UnitsSold = (int)flow.GetEdgeWeight(i, weeks + 1);
                if (i < weeks - 1)
                    weeklyPlan[i].UnitsStored = (int)flow.GetEdgeWeight(i, i + 1);
                else
                    weeklyPlan[i].UnitsStored = 0;
            }

            return new PlanData { Quantity = (int)value, Value = -cost };
        }

        /// <summary>
        /// Część 2. zadania - zaplanowanie produkcji telewizorów dla wielu kontrahentów.
        /// </summary>
        /// <remarks>
        /// Do przeprowadzenia testów wyznaczających produkcję dającą maksymalny zysk wymagane jest jedynie zwrócenie obiektu <see cref="PlanData"/>.
        /// Testy weryfikujące plan wymagają przypisania tablicy z planem do parametru wyjściowego <see cref="weeklyPlan"/>.
        /// </remarks>
        /// <param name="production">
        /// Tablica obiektów zawierających informacje o produkcji fabryki w kolejnych tygodniach.
        /// Wartość pola <see cref="PlanData.Quantity"/> oznacza limit produkcji w danym tygodniu,
        /// a pola <see cref="PlanData.Value"/> - koszt produkcji jednej sztuki.
        /// </param>
        /// <param name="sales">
        /// Dwuwymiarowa tablica obiektów zawierających informacje o sprzedaży w kolejnych tygodniach.
        /// Pierwszy wymiar tablicy jest równy liczbie kontrahentów, zaś drugi - liczbie tygodni w planie.
        /// Wartości pola <see cref="PlanData.Quantity"/> oznaczają maksymalną sprzedaż w danym tygodniu,
        /// a pola <see cref="PlanData.Value"/> - cenę sprzedaży jednej sztuki.
        /// Każdy wiersz tablicy odpowiada jednemu kontrachentowi.
        /// </param>
        /// <param name="storageInfo">
        /// Obiekt zawierający informacje o magazynie.
        /// Wartość pola <see cref="PlanData.Quantity"/> oznacza pojemność magazynu,
        /// a pola <see cref="PlanData.Value"/> - koszt przechowania jednego telewizora w magazynie przez jeden tydzień.
        /// </param>
        /// <param name="weeklyPlan">
        /// Parametr wyjściowy, przez który powinien zostać zwrócony szczegółowy plan sprzedaży.
        /// </param>
        /// <returns>
        /// Obiekt <see cref="PlanData"/> opisujący wyznaczony plan.
        /// W polu <see cref="PlanData.Quantity"/> powinna znaleźć się optymalna liczba wyprodukowanych telewizorów,
        /// a w polu <see cref="PlanData.Value"/> - wyznaczony maksymalny zysk fabryki.
        /// </returns>
        public PlanData CreateComplexPlan(PlanData[] production, PlanData[,] sales, PlanData storageInfo,
            out WeeklyPlan[] weeklyPlan)
        {
            int weeks = production.Length;
            int clients = sales.GetLength(0);

            if (weeks == 0 || clients == 0 || storageInfo.Quantity < 0 || storageInfo.Value < 0 || weeks != sales.GetLength(1))
                throw new ArgumentException();
            for (int i = 0; i < weeks; i++)
            {
                if (production[i].Quantity < 0 || production[i].Value < 0)
                {
                    throw new ArgumentException();
                }
                for (int clientNr = 0; clientNr < clients; clientNr++)
                {
                    if (sales[clientNr, i].Quantity < 0 || sales[clientNr, i].Value < 0)
                        throw new ArgumentException();
                }
            }

            Graph g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 2 * weeks + 1 + clients + 1);
            Graph c = g.IsolatedVerticesGraph();
            //weeks - weeks+clients-1 - clients
            //weeks+clients - source
            //weeks+clients + 1 - target

            int source = weeks + clients;
            int target = weeks + clients + 1;
            int storageStart = weeks;
            int dummyStorageStart = weeks + clients + 2;

            for (int i = 0; i < weeks; i++)
            {
                g.AddEdge(source, dummyStorageStart + i, production[i].Quantity);
                c.AddEdge(source, dummyStorageStart + i, 0);

                g.AddEdge(dummyStorageStart + i, i, production[i].Quantity);
                c.AddEdge(dummyStorageStart + i, i, production[i].Value);

                g.AddEdge(dummyStorageStart + i, target, production[i].Quantity);
                c.AddEdge(dummyStorageStart + i, target, 0);

                for (int clientNr = 0; clientNr < clients; clientNr++)
                {
                    g.AddEdge(i, storageStart + clientNr, sales[clientNr, i].Quantity);
                    c.AddEdge(i, storageStart + clientNr, -sales[clientNr, i].Value);

                    g.AddEdge(storageStart + clientNr, target, double.MaxValue);
                    c.AddEdge(storageStart + clientNr, target, 0);
                }

                if (i < weeks - 1)
                {
                    g.AddEdge(i, i + 1, storageInfo.Quantity);
                    c.AddEdge(i, i + 1, storageInfo.Value);
                }
            }

            (double value, double cost, Graph flow) = g.MinCostFlow(c, source, target, true, MaxFlowGraphExtender.FordFulkersonDinicMaxFlow, MaxFlowGraphExtender.MKMBlockingFlow);

            for (int i = 0; i < weeks; i++)
            {
                value -= flow.GetEdgeWeight(dummyStorageStart + i, target);
            }

            weeklyPlan = new WeeklyPlan[weeks];
            for (int i = 0; i < weeks; i++)
            {
                weeklyPlan[i].UnitsProduced = (int)flow.GetEdgeWeight(dummyStorageStart + i, i);
                if (i < weeks - 1)
                    weeklyPlan[i].UnitsStored = (int)flow.GetEdgeWeight(i, i + 1);
                else
                    weeklyPlan[i].UnitsStored = 0;
                weeklyPlan[i].UnitsSold = new int[clients];
                for (int clientNr = 0; clientNr < clients; clientNr++)
                {
                    weeklyPlan[i].UnitsSold[clientNr] = (int)flow.GetEdgeWeight(i, weeks + clientNr);
                }
            }

            return new PlanData { Quantity = (int)value, Value = -cost };
        }
    }
}