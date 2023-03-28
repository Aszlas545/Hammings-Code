using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telekomunikacja1
{
    class Hamming
    {
        //--------------------------------------------------------------------------------------------
        //funckja na podstawie podanej wiadomości (tablica bitów) tworzy wektor T                     
        //na podstawie macierzy H dodając bity parzystości służące do sprawdzenia ewentualnego błędu  
        //--------------------------------------------------------------------------------------------
        public bool[] GetTFromBH(bool[] B, bool[,] H)
        {
            int Hhei = H.GetLength(0);
            int Blen = B.Length;

            bool[] T = new bool[Blen + Hhei];

            for (int i = 0; i < Blen; i++)
            {
                T[i] = B[i];
            }

            //---------------------------------------------------------------------------------------
            //mnożenie macierzy H tylko na kolumnach o indeksie do długości wektora Bi wektora B tak,
            //aby obliczyć które z bitów parzystości powinny być 0 a które 1
            //---------------------------------------------------------------------------------------
            int[] sums = new int[Hhei];
            for (int x = 0; x < Hhei; x++)
            {
                for (int y = 0; y < Blen; y++)
                {
                    bool value = B[y] & H[x, y];
                    if (value)
                    {
                        sums[x] = sums[x] + 1;
                    }
                }
            }

            int j = 0;

            //-----------------------------------------------------------------------------------------------------------
            //konwersja sumy obliczonej w poprzedniej pętli na bity oraz dodanie ich do wektora T jako bity parzystości
            //-----------------------------------------------------------------------------------------------------------
            for (int i = Blen; i < T.Length; i++)
            {
                T[i] = Convert.ToBoolean(sums[j] % 2);
                j++;
            }

            return T;
        }

        //-------------------------------------------------------------------------------------------------
        //funckja odpowiedzialna za to, aby sprawdzić czy dany wektor T jest poprawny względem macierzy H,
        //w skrócie sprawdzenie czy nie wystąpił błąd transmisji 
        //-------------------------------------------------------------------------------------------------
        public bool checkT(bool[] T, bool[,] H)
        {

            int len = T.Length;
            int hei = H.GetLength(0);

            int[] sums = new int[hei];
            //-------------------------------
            //mnożenie macierzy H i wktora T
            //-------------------------------
            for (int x = 0; x < hei; x++)
            {
                for (int y = 0; y < len; y++)
                {
                    bool value = T[y] & H[x, y];
                    if (value)
                    {
                        sums[x] = sums[x] + 1;
                    }
                }
            }

            //------------------------------------------------------------------------------------------------------------
            //sprawdzenie czy w wyniku mnożenia wszystkie współrzędne wektora dają z dzielenia modulo przez 2 reszte zero
            //------------------------------------------------------------------------------------------------------------
            for (int i = 0; i < sums.Length; i++)
            {
                int check = sums[i] % 2;
                if (check == 1)
                {
                    return false;
                }
            }

            return true;
        }

        //-----------------------------------------------------------------------------
        //funkcja sprawdzająca czy wstąpił pojedynczy błąd zwracająca wektor błędu E
        //który po zsumowanie z błędnym wektorem T(E) da poprawny wektor T
        //-----------------------------------------------------------------------------
        public bool[] detectSingleError(bool[] TE, bool[,] H)
        {
            int len = TE.Length;
            int hei = H.GetLength(0);

            bool[] E = Enumerable.Repeat(false, len).ToArray();
            bool[] errorCode = new bool[hei];
            bool[] errorCheck = new bool[hei];
            int[] sums = new int[hei];

            //------------------------------------------------------------
            //obliczenie sumy błędu poprzez mnożenie macierzy T(E) oraz H
            //------------------------------------------------------------
            for (int x = 0; x < hei; x++)
            {
                for (int y = 0; y < len; y++)
                {
                    bool value = TE[y] & H[x, y];
                    if (value)
                    {
                        sums[x] = sums[x] + 1;
                    }
                }
            }

            //----------------------------------
            //konwersja otrzymanej sumy na bity
            //----------------------------------
            for (int i = 0; i < hei; i++)
            {
                errorCode[i] = Convert.ToBoolean(sums[i] % 2);
            }

            //-----------------------------------------------------------------------------------
            //zamiana bitu wektora E w którym zwrócona suma błędu jest równa kolumnie macierzy H
            //-----------------------------------------------------------------------------------
            for (int y = 0; y < len; y++)
            {
                for (int x = 0; x < hei; x++)
                {
                    errorCheck[x] = H[x, y];
                }
                if (errorCheck.SequenceEqual(errorCode))
                {
                    E[y] = true;
                    return E;
                }
            }
            return E;
        }

        //-----------------------------------------------------------------------------
        //funkcja sprawdzająca czy wstąpił podwójny błąd zwracająca wektor błędu E
        //który po zsumowanie z błędnym wektorem T(E) da poprawny wektor T
        //-----------------------------------------------------------------------------
        public bool[] detectDoubleError(bool[] TE, bool[,] H)
        {
            int len = TE.Length;
            int hei = H.GetLength(0);
            bool[] E = Enumerable.Repeat(false, len).ToArray();
            bool[] errorCode = new bool[hei];
            bool[] errorCheck = new bool[hei];
            int[] sums = new int[hei];

            //------------------------------------------------------------
            //obliczenie sumy błędu poprzez mnożenie macierzy T(E) oraz H
            //------------------------------------------------------------
            for (int x = 0; x < hei; x++)
            {
                for (int y = 0; y < len; y++)
                {
                    bool value = TE[y] & H[x, y];
                    if (value)
                    {
                        sums[x] = sums[x] + 1;
                    }
                }
            }

            //----------------------------------
            //konwersja otrzymanej sumy na bity
            //----------------------------------
            for (int i = 0; i < hei; i++)
            {
                errorCode[i] = Convert.ToBoolean(sums[i] % 2);
            }

            //-----------------------------------------------------------------------------------
            //zamiana bitu wektora E w którym zwrócona suma błędu jest równa kolumnie macierzy H
            //-----------------------------------------------------------------------------------
            for (int i = 0; i < len; i++)
            {
                for (int j = i + 1; j < len; j++)
                {
                    for (int x = 0; x < hei; x++)
                    {
                        if (H[x, i] != H[x, j])
                        {
                            errorCheck[x] = true;
                        }
                        if (errorCheck[x] != errorCode[x])
                        {
                            break;
                        }
                    }

                    if (errorCheck.SequenceEqual(errorCode))
                    {
                        E[i] = true;
                        E[j] = true;
                        return E;
                    }
                    else
                    {
                        errorCheck = Enumerable.Repeat(false, hei).ToArray();
                    }
                }
            }
            return E;
        }


        //--------------------------------------------------------------------------------------------------------
        //funkcja naprawiająca wektor T(E) na podstawie wektora E poprzez zmiane błędnych bitów na przeciwny znak
        //--------------------------------------------------------------------------------------------------------
        public void fixT(bool[] T, bool[] E)
        {
            for (int i = 0; i < T.Length; i++)
            {
                if (E[i] == true)
                {
                    T[i] = !T[i];
                }
            }
        }

        //------------------------------------------------------
        //Usuwa nadmiarowe bity, tak aby została sama wiadomość
        //------------------------------------------------------
        public bool[] RemoveExcessBits(bool[] T, bool[,] H)
        {
            int BLen = H.GetLength(1) - H.GetLength(0);
            bool[] B = new bool[BLen];

            for (int i = 0; i < BLen; i++)
            {
                B[i] = T[i];
            }

            return B;
        }

        //-------------------------------------------------------------------------------------
        //Następne 2 funckje są neistotne dla działania programu były one użyte do tego,
        //aby zweryfikować poprwaność macierzy H ze względu na jej wygórowane działania
        //-------------------------------------------------------------------------------------
        public int[] detectDoubleErrorTest(bool[] TE, bool[,] H)
        {
            int len = TE.Length;
            int hei = H.GetLength(0);
            bool[] E = Enumerable.Repeat(false, len).ToArray();
            bool[] errorCode = new bool[hei];
            bool[] errorCheck = new bool[hei];
            int[] sums = new int[hei];

            for (int x = 0; x < hei; x++)
            {
                for (int y = 0; y < len; y++)
                {
                    bool value = TE[y] & H[x, y];
                    if (value)
                    {
                        sums[x] = sums[x] + 1;
                    }
                }
            }

            for (int i = 0; i < hei; i++)
            {
                errorCode[i] = Convert.ToBoolean(sums[i] % 2);
            }
            Console.WriteLine("sums: " + string.Join(" ", sums));
            Console.WriteLine("eCode: " + string.Join(" ", errorCode));
            for (int i = 0; i < len; i++)
            {
                for (int j = i + 1; j < len; j++)
                {
                    for (int x = 0; x < hei; x++)
                    {
                        if (H[x, i] != H[x, j])
                        {
                            errorCheck[x] = true;
                        }
                        if (errorCheck[x] != errorCode[x])
                        {
                            break;
                        }
                    }

                    Console.WriteLine(i + " " + j + "eCheck: " + string.Join(" ", errorCheck));
                    if (errorCheck.SequenceEqual(errorCode))
                    {
                        Console.WriteLine("błąd na miejscu: " + i + " oraz " + j);
                        E[i] = true;
                        E[j] = true;
                        int[] retval = new int[] { i, j };
                        return retval;
                    }
                    else
                    {
                        errorCheck = Enumerable.Repeat(false, hei).ToArray();
                    }
                }
            }
            Console.WriteLine("brak błędu?");
            int[] u = new int[] { 0, 0 };
            return u;
        }
        public void lookForError(bool[] T, bool[,] H)
        {
            int rnd1;
            int rnd2;
            Random rnd = new Random();
            while (true)
            {

                rnd1 = rnd.Next(0, 16);
                rnd2 = rnd.Next(0, 16);
                while (rnd1 == rnd2)
                {
                    rnd2 = rnd.Next(0, 16);
                }
                Console.WriteLine(rnd1 + " " + rnd2);
                T[rnd1] = !T[rnd1];
                T[rnd2] = !T[rnd2];
                int[] check = detectDoubleErrorTest(T, H);
                int[] check2 = new int[] { rnd1, rnd2 };
                int[] check3 = new int[] { rnd2, rnd1 };
                if (check.SequenceEqual(check2) || check.SequenceEqual(check3))
                {
                    Console.WriteLine("good ");
                }
                else
                {
                    Console.ReadLine();
                }
                T[rnd1] = !T[rnd1];
                T[rnd2] = !T[rnd2];
            }
        }
    }
}
