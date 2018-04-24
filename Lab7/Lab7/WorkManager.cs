using System;
using System.Linq;

namespace ASD
{
    public class WorkManager : MarshalByRefObject
    {
        int[] result;
        int w1 = 0;
        int w2 = 0;
        int w1blocks;
        int w2blocks;
        int bestDifference;
        int[] bestResult;
        /// <summary>
        /// Implementacja wersji 1
        /// W tablicy blocks zapisane s¹ wagi wszystkich bloków do przypisania robotnikom.
        /// Ka¿dy z nich powinien mieæ przypisane bloki sumie wag równej expectedBlockSum.
        /// Metoda zwraca tablicê przypisuj¹c¹ ka¿demu z bloków jedn¹ z wartoœci:
        /// 1 - jeœli blok zosta³ przydzielony 1. robotnikowi
        /// 2 - jeœli blok zosta³ przydzielony 2. robotnikowi
        /// 0 - jeœli blok nie zosta³ przydzielony do ¿adnego robotnika
        /// Jeœli wymaganego podzia³u nie da siê zrealizowaæ metoda zwraca null.
        /// </summary>
        public int[] DivideWorkersWork(int[] blocks, int expectedBlockSum)
        {
            result = new int[blocks.Length];
            w1 = 0;
            w2 = 0;
            if (!DivideWork(blocks, expectedBlockSum, 0))
                return null;

            return result;
        }
        bool DivideWork(int[] blocks, int expectedBlockSum, int i)
        {
            if (w1 == expectedBlockSum && w2 == expectedBlockSum)
                return true;
            if (i == blocks.Length)
                return false;
            if (w1 > expectedBlockSum || w2 > expectedBlockSum)
                return false;

            if (w1 + blocks[i] <= expectedBlockSum)
            {
                result[i] = 1;
                w1 += blocks[i];
                if (DivideWork(blocks, expectedBlockSum, i + 1))
                    return true;
                w1 -= blocks[i];
            }

            if (w2 + blocks[i] <= expectedBlockSum)
            {
                result[i] = 2;
                w2 += blocks[i];
                if (DivideWork(blocks, expectedBlockSum, i + 1))
                    return true;
                w2 -= blocks[i];
            }

            result[i] = 0;
            if (DivideWork(blocks, expectedBlockSum, i + 1))
                return true;

            return false;
        }

        /// <summary>
        /// Implementacja wersji 2
        /// Parametry i wynik s¹ analogiczne do wersji 1.
        /// </summary>
        public int[] DivideWorkWithClosestBlocksCount(int[] blocks, int expectedBlockSum)
        {
            result = new int[blocks.Length];
            bestResult = null;
            w1 = 0;
            w2 = 0;
            w1blocks = 0;
            w2blocks = 0;
            bestDifference = int.MaxValue;

            if (blocks.Sum() < 2 * expectedBlockSum)
                return null;
            int i = 0;
            DivideWorkEqually(blocks, ref expectedBlockSum, ref i);

            return bestResult;
        }
        void DivideWorkEqually(int[] blocks, ref int expectedBlockSum, ref int i)
        {
            if (bestDifference == 0)
                return;
            if (w1 == expectedBlockSum && w2 == expectedBlockSum)
            {
                if (Math.Abs(w1blocks - w2blocks) < bestDifference)
                {
                    if (bestResult == null)
                    {
                        bestResult = new int[result.Length];
                    }
                    result.CopyTo(bestResult, 0);
                    bestDifference = Math.Abs(w1blocks - w2blocks);
                }
                return;
            }
            if (i == blocks.Length)
                return;
            if (w1 > expectedBlockSum || w2 > expectedBlockSum)
                return;

            if (w1 + blocks[i] <= expectedBlockSum)
            {
                result[i] = 1;
                w1 += blocks[i];
                w1blocks++;
                i++;
                DivideWorkEqually(blocks, ref expectedBlockSum, ref i);
                i--;
                w1 -= blocks[i];
                w1blocks--;
            }

            if (w2 + blocks[i] <= expectedBlockSum)
            {
                result[i] = 2;
                w2 += blocks[i];
                w2blocks++;
                i++;
                DivideWorkEqually(blocks, ref expectedBlockSum, ref i);
                i--;
                w2 -= blocks[i];
                w2blocks--;
            }

            result[i] = 0;
            i++;
            DivideWorkEqually(blocks, ref expectedBlockSum, ref i);
            i--;
        }
        // Mo¿na dopisywaæ pola i metody pomocnicze

    }
}

