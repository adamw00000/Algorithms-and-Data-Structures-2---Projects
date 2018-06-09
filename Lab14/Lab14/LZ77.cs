using System;
using System.Collections.Generic;
using System.Text;

namespace ASD
{
    public class LZ77 : MarshalByRefObject
    {
        /// <summary>
        /// Odkodowywanie napisu zakodowanego algorytmem LZ77. Dane kodowanie jest poprawne (nie trzeba tego sprawdzać).
        /// </summary>
        /// <param name="encoding"></param>
        public string Decode(List<EncodingTriple> encoding)
        {
            StringBuilder sb = new StringBuilder();

            int cap = encoding.Count;
            foreach (EncodingTriple triple in encoding)
                cap += triple.c;

            sb.Capacity = cap;

            foreach (EncodingTriple triple in encoding)
            {
                int start = sb.Length - 1 - triple.p;

                for (int i = 0; i < triple.c; i++)
                {
                    sb.Append(sb[start + i]);
                }
                sb.Append(triple.s);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Kodowanie napisu s algorytmem LZ77
        /// </summary>
        /// <param name="s"></param>
        /// <param name="maxP"></param>
        /// <returns></returns>
        public List<EncodingTriple> Encode(string s, int maxP)
        {
            List<EncodingTriple> ret = new List<EncodingTriple>();

            int n = s.Length;
            int lastEncodedPos = -1;

            while (lastEncodedPos < n - 1)
            {
                int wEnd = lastEncodedPos;
                int wStart = Math.Max(0, wEnd - maxP);
                int rStart = wEnd + 1;
                int rEnd = s.Length - 1;

                bool flag = false;

                int c = 0;
                int j = 0;
                (j, c) = KMPMainOPT(s, rStart, rEnd, wStart, rEnd);

                if (c != -1)
                    flag = true;

                if (!flag)
                {
                    ret.Add(new EncodingTriple(0, 0, s[rStart + 0]));
                    lastEncodedPos++;
                }
                else
                {
                    ret.Add(new EncodingTriple(wEnd - wStart + 1 - (j + 1), c, s[rStart + c]));
                    lastEncodedPos += c + 1;
                }
            }

            return ret;
        }

        private (int p, int c) KMPMainOPT(string s, int xStart, int xEnd, int yStart, int yEnd)
        {
            int n = yEnd - yStart + 1;
            int m = xEnd - xStart + 1;

            int i, j;
            int[] P = new int[m];

            if (m == 1)
            {
                P[0] = 0;
            }
            else if (m == 2)
            {
                P[0] = P[1] = 0;
            }

            int jMax = -1;
            int iMax = -1;
            int jCurrent = 1;

            for (j = i = 0; i <= n - m - 1; i += Math.Max(j - P[j], 1))
            {
                for (j = P[j]; j < m - 1 && s[yStart + i + j] == s[xStart + j]; ++j) ;

                if (j > 0 && j > jMax)
                {
                    jMax = j;
                    iMax = i;
                }

                if (j > jCurrent)
                {
                    KMPHelperOPT(s, xStart, xEnd, jCurrent + 1, j, P);
                    jCurrent = j;
                }
            }
            return (iMax, jMax);
        }

        private void KMPHelperOPT(string s, int xStart, int xEnd, int jCurrent, int jMax, int[] P)
        {
            int m = xEnd - xStart + 1;
            int t, j;

            t = P[jCurrent - 1];
            for (j = jCurrent; j < jMax + 1; ++j)
            {
                while (t > 0 && s[xStart + t] != s[xStart + j - 1])
                    t = P[t];
                if (s[xStart + t] == s[xStart + j - 1]) ++t;
                P[j] = t;
            }
        }
    }

    [Serializable]
    public struct EncodingTriple
    {
        public int p, c;
        public char s;

        public EncodingTriple(int p, int c, char s)
        {
            this.p = p;
            this.c = c;
            this.s = s;
        }
    }
}
