using System;
using System.Collections.Generic;

namespace Lab08
{

    public class Lab08 : MarshalByRefObject
    {

        /// <summary>
        /// funkcja do sprawdzania czy da się ustawić k elementów w odległości co najmniej dist od siebie
        /// </summary>
        /// <param name="a">posortowana tablica elementów</param>
        /// <param name="dist">zadany dystans</param>
        /// <param name="k">liczba elementów do wybrania</param>
        /// <param name="exampleSolution">Wybrane elementy</param>
        /// <returns>true - jeśli zadanie da się zrealizować</returns>
        public bool CanPlaceElementsInDistance(int[] a, int dist, int k, out List<int> exampleSolution)
        {
            exampleSolution = new List<int>();
            if (a.Length > 0)
            {
                exampleSolution.Add(a[0]);
            }

            for (int i = 1; i<a.Length; i++)
            {
                if ((double)a[i] - (double)dist >= (double)exampleSolution[exampleSolution.Count - 1])
                {
                    exampleSolution.Add(a[i]);
                    if (exampleSolution.Count == k)
                        break;
                }
            }

            if (exampleSolution.Count < k)
            {
                exampleSolution = null;
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// Funkcja wybiera k elementów tablicy a, tak aby minimalny dystans pomiędzy dowolnymi dwiema liczbami (spośród k) był maksymalny
        /// </summary>
        /// <param name="a">posortowana tablica elementów</param>
        /// <param name="k">liczba elementów do wybrania</param>
        /// <param name="exampleSolution">Wybrane elementy</param>
        /// <returns>Maksymalny możliwy dystans między wybranymi elementami</returns>
        public int LargestMinDistance(int[] a, int k, out List<int> exampleSolution)
        {
            exampleSolution = null;
            if (a[0] < 0)
                throw new ArgumentException();
            if (a.Length <= 1 || a.Length > 100000)
                throw new ArgumentException();
            if (k <= 1 || k > a.Length)
                throw new ArgumentException();

            int maxDistance = a[a.Length - 1] - a[0];
            int avgDistance = (int)Math.Ceiling((decimal)maxDistance / (k - 1));

            List<int> solution = null;
            int minDist = 0;
            int maxDist = avgDistance;
            int dist = minDist + (maxDist - minDist) / 2;

            bool canPlace = CanPlaceElementsInDistance(a, maxDist, k, out solution);
            if (canPlace)
            {
                exampleSolution = solution;
                return maxDist;
            }

            while (true)
            {
                dist = (minDist + maxDist) / 2;
                canPlace = CanPlaceElementsInDistance(a, dist, k, out solution);
                if (canPlace)
                {
                    exampleSolution = solution;
                    if (minDist == dist)
                    {
                        break;
                    }
                    minDist = dist;
                }
                else
                {
                    if (maxDist == dist)
                        break;
                    maxDist = dist;
                }
            }

            return dist;
        }

    }

}
