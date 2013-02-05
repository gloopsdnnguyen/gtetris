using System;

namespace Server
{
    static class Comparison
    {
        public static Card[] CompareHands(Card[] a, Card[] b)
        {
            int uno = RankHand(a);
            int dos = RankHand(b);
            if (uno > dos)
                return a;
            if (dos > uno)
                return b;

            Array.Sort(a, delegate(Card one, Card two)
            {
                return one.Value.CompareTo(two.Value);
            });
            Array.Sort(b, delegate(Card one, Card two)
            {
                return one.Value.CompareTo(two.Value);
            });

            for (int i = 4; i >= 0; i--)
            {
                if (a[i].Value > b[i].Value)
                    return a;
                if (b[i].Value > a[i].Value)
                    return b;
            }
            return a;
        }

        public static int RankHand(Card[] a)
        {
            int[] b = new int[a.Length];
            for (int i = 0; i < a.Length; i++)
                b[i] = a[i].Value;
            Array.Sort(b);

            bool flush = true;
            for (int i = 0; i < 4; i++)
                if (a[i].Shape != a[i + 1].Shape)
                    flush = false;

            bool straight = true;
            for (int i = 0; i < 4; i++)
                if (b[i + 1] != b[i] + 1)
                    straight = false;
            if (b[0] == 2 && b[1] == 3 && b[2] == 4 && b[3] == 5 && b[4] == 14)
                straight = true;

            if (flush && straight) return 80000;
            if (flush) return 50000;
            if (straight) return 40000;
            if (b[0] == b[3] || b[1] == b[4]) //quad
                return 70000 + b[2];

            bool threesome = false;
            if (b[0] == b[2] || b[1] == b[3] || b[2] == b[4])
                threesome = true;

            //pairs
            int tmp1 = 0, tmp2 = 0;
            for (int i = 0; i < b.Length; i++)
            {
                int count = 0;
                for (int n = 0; n < b.Length; n++)
                    if (b[i] == b[n])
                        count++;
                if (count == 2)
                {
                    if (tmp1 > 0 && b[i] != tmp1) tmp2 = b[i];
                    else { tmp1 = b[i]; }
                }
            }

            if (threesome && tmp1 > 0) return 60000 + b[2];
            if (threesome) return 30000 + b[2];

            if (tmp1 > 0 && tmp2 > 0)
                return (20000 + Math.Max(tmp1, tmp2) * 100 + Math.Min(tmp1, tmp2));

            if (tmp1 > 0)
                return (10000 + tmp1);

            return 0;
        }

        public static Card[] BestPossibleHand(Card[] pocket, Card[] community)
        {
            Card[] winner = new Card[] { pocket[0], pocket[1], community[0], community[1], community[2] };
            Card[] a = new Card[] { pocket[0], pocket[1], community[0], community[1], community[2], community[3], community[4] };
            Card[] tmp = new Card[5];
            for (int i = 0; i < 7; i++)
                for (int n = i + 1; n < 7; n++)
                {
                    for (int j = 0, p = 0; j < 7; j++)
                        if (j != i && j != n)
                        {
                            tmp[p] = a[j];
                            p++;
                        }
                    CompareHands(tmp, winner).CopyTo(winner, 0);
                }
            return winner;
        }
    }
}