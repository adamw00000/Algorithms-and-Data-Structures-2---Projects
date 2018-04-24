using System;

namespace ASD
{

public class CarpentersBench: System.MarshalByRefObject
    {

        /// <summary>
        /// Metoda pomocnicza - wymagana przez system
        /// </summary>
        public int Cut(int length, int width, int[,] elements, out Cut cuts)
        {
            (int length, int width, int price)[] _elements = new(int length, int width, int price)[elements.GetLength(0)];
            for (int i = 0; i < _elements.Length; ++i)
            {
                _elements[i].length = elements[i, 0];
                _elements[i].width = elements[i, 1];
                _elements[i].price = elements[i, 2];
            }
            return Cut((length, width), _elements, out cuts);
        }

        /// <summary>
        /// Wyznaczanie optymalnego sposobu pocięcia płyty
        /// </summary>
        /// <param name="sheet">Rozmiary płyty</param>
        /// <param name="elements">Tablica zawierająca informacje o wymiarach i wartości przydatnych elementów</param>
        /// <param name="cuts">Opis cięć prowadzących do uzyskania optymalnego rozwiązania</param>
        /// <returns>Maksymalna sumaryczna wartość wszystkich uzyskanych w wyniku cięcia elementów</returns>
        public int Cut((int length, int width) sheet, (int length, int width, int price)[] elements, out Cut cuts)
        {
            int[,] maxValues = new int[sheet.length + 1, sheet.width + 1];

            Cut[,] cutTable = new Cut[sheet.length + 1, sheet.width + 1];

            SetFirstRowAndColumnOfCutTableToZeroValue(sheet, cutTable);

            FillTableWithElements(sheet, elements, maxValues, cutTable);

            for (int x = 1; x < sheet.length + 1; x++)
            {
                for (int y = 1; y < sheet.width + 1; y++)
                {
                    if (cutTable[x, y] == null)
                        SetXYToZeroValue(cutTable, x, y);

                    CheckVerticalCuts(maxValues, cutTable, x, y);
                    CheckHorizontalCuts(maxValues, cutTable, x, y);
                }
            }

            cuts = cutTable[sheet.length, sheet.width];
            return maxValues[sheet.length, sheet.width];
        }

        private void CheckVerticalCuts(int[,] maxValues, Cut[,] cutTable, int x, int y)
        {
            for (int i = 1; i <= y - 1; i++)
            {
                int totalPriceVertical = CalculateVerticalValue(maxValues, x, y, i);

                if (IsBetterThanCurrentCut(totalPriceVertical, maxValues[x, y]))
                {
                    bool isCutVertical = true; 
                    int cutN = i;
                    Cut cutTopLeft = cutTable[x, i];
                    Cut cutBottomright = cutTable[x, y - i];
                    
                    maxValues[x, y] = totalPriceVertical;
                    cutTable[x, y] = new ASD.Cut(x, y, maxValues[x, y], isCutVertical, cutN, cutTopLeft, cutBottomright);
                }
            }
        }

        private void CheckHorizontalCuts(int[,] maxValues, Cut[,] cutTable, int x, int y)
        {
            for (int i = 1; i <= x - 1; i++)
            {
                int totalPriceHorizontal = CalculateHorizontalValue(maxValues, x, y, i);

                if (IsBetterThanCurrentCut(totalPriceHorizontal, maxValues[x, y]))
                {
                    bool isCutVertical = false;
                    int cutN = i;
                    Cut cutTopLeft = cutTable[i, y];
                    Cut cutBottomright = cutTable[x - i, y];

                    maxValues[x, y] = totalPriceHorizontal;
                    cutTable[x, y] = new ASD.Cut(x, y, maxValues[x, y], isCutVertical, cutN, cutTopLeft, cutBottomright);
                }
            }
        }


        private static void FillTableWithElements((int length, int width) sheet, (int length, int width, int price)[] elements, int[,] maxValues, Cut[,] cutTable)
        {
            foreach (var element in elements)
            {
                if (CanElementFitInSheet(sheet, element) && 
                    (cutTable[element.length, element.width] == null || element.price > maxValues[element.length, element.width]))
                {
                    maxValues[element.length, element.width] = element.price;
                    cutTable[element.length, element.width] = new ASD.Cut(element.length, element.width, element.price);
                }
            }
        }

        private static bool CanElementFitInSheet((int length, int width) sheet, (int length, int width, int price) element)
        {
            return element.length <= sheet.length && element.width <= sheet.width;
        }

        private static void SetXYToZeroValue(Cut[,] cutTable, int x, int y)
        {
            cutTable[x, y] = new ASD.Cut(x, y, 0);
        }

        private static void SetFirstRowAndColumnOfCutTableToZeroValue((int length, int width) sheet, Cut[,] cutTable)
        {
            for (int i = 0; i < sheet.length + 1; i++)
            {
                cutTable[i, 0] = new ASD.Cut(i, 0, 0);
            }

            for (int i = 0; i < sheet.width + 1; i++)
            {
                cutTable[0, i] = new ASD.Cut(0, i, 0);
            }
        }

        private static int CalculateHorizontalValue(int[,] maxValues, int x, int y, int i)
        {
            int topCutPriceHorizontal = maxValues[i, y];
            int bottomCutPriceHorizontal = maxValues[x - i, y];
            int totalPriceHorizontal = topCutPriceHorizontal + bottomCutPriceHorizontal;
            return totalPriceHorizontal;
        }

        private static int CalculateVerticalValue(int[,] maxValues, int x, int y, int i)
        {
            int leftCutPriceVertical = maxValues[x, i];
            int rightCutPriceVertical = maxValues[x, y - i];
            int totalPriceVertical = leftCutPriceVertical + rightCutPriceVertical;
            return totalPriceVertical;
        }

        private static bool IsBetterThanCurrentCut(int newValue, int currentValue)
        {
            return newValue > currentValue;
        }
    }

}
