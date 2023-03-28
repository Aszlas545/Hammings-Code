using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telekomunumProjektum
{
    class fileEncoder
    {
        Hamming ham = new Hamming();
        List<bool[]> inputList = new List<bool[]>();
        List<bool[]> outputList = new List<bool[]>();

        //---------------------------------------------------------------------------------
        //Funkcja, która z każdy bajt z tablicy bajtów zaczytanej z pliku na tablice booli
        //następnie dodaje każdą z tablic do listy (inputList
        //---------------------------------------------------------------------------------
        private void getBitsFromBytes(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                bool[] BoolArray = new bool[8];
                for (int j = 0; j < 8; j++)
                    BoolArray[j] = (bytes[i] & (1 << j)) != 0;
                Array.Reverse(BoolArray);
                inputList.Add(BoolArray);
            }
        }

        //--------------------------------------------------------------------------------------------------------
        //Funkcja która tablice wektora T (16 bitów / 2 bajty) rozdiela na 2 tablice o długości 8 bitów (1 bajta)
        //następnie dodaje je do tablicy outputList aby poprawnie zapisać je do pliku
        //--------------------------------------------------------------------------------------------------------
        private void splitTinto2(bool[] T)
        {
            bool[] halfT1 = new bool[8];
            bool[] halfT2 = new bool[8];
            for (int i = 0; i < 8; i++)
            {
                halfT1[i] = T[i];
                halfT2[i] = T[i + 8];
            }
            outputList.Add(halfT1);
            outputList.Add(halfT2);
        }

        //------------------------------------------------
        //Funkcja zamienijąca tablice booli na jeden bajt
        //------------------------------------------------
        private byte ConvertBoolArrayToByte(bool[] source)
        {
            byte result = 0;
            int index = 8 - source.Length;

            foreach (bool b in source)
            {
                if (b)
                    result |= (byte)(1 << (7 - index));

                index++;
            }

            return result;
        }

        //-------------------------------------------------------------------------------------------------------------------
        //Funkcja która z 2 bitów tworzy jedną tablicę zawierjącą wiadomość oraz bity parzystości i dodaje je do outputListy
        //-------------------------------------------------------------------------------------------------------------------
        public void fuseintoOne(bool[] T1, bool[] T2)
        {
            bool[] T = new bool[16];
            for (int i = 0; i < 8; i++)
            {
                T[i] = T1[i];
                T[i+8] = T2[i];
            }
            outputList.Add(T);
        }

        //------------------------------------------------------------------------------------------------------
        //Funckja kodująca na podstawie macierzy H zawartość tablicy bajtów które mają zostać zapisane do pliku
        //------------------------------------------------------------------------------------------------------
        public byte[] EncryptFile(byte[] fileContent, bool[,] H)
        {
            //-----------------------
            //wyczyszczenie obu list
            //-----------------------
            inputList = new List<bool[]>();
            outputList = new List<bool[]>();

            //-----------------------------------------------------------------------------
            //dodanie do input listy zawartości odczytanego pliku w postaci tablic 8 booli
            //-----------------------------------------------------------------------------
            getBitsFromBytes(fileContent);

            //-------------------------------------------------------------------------------------------------------------------------------------
            //obliczanie bitów parzystości na podstawie bitów z wiadomośckią jakto T oraz dzielenie ich na 2 bajty oraz dodanie ich do outputListy 
            //-------------------------------------------------------------------------------------------------------------------------------------
            for (int i = 0; i < inputList.Count; i++)
            {
                splitTinto2(ham.GetTFromBH(inputList[i], H));
            }

            //---------------------------------------------------------------------------
            //stworzenie i zapełneinie tablicy bajtów zawierających zakodowaną wiadomość 
            //---------------------------------------------------------------------------

            byte[] bytes = new byte[outputList.Count];
            
            for (int i = 0; i < outputList.Count; i++)
            {
                bytes[i] = (ConvertBoolArrayToByte(outputList[i]));
            }
            
            return bytes;
        }


        //------------------------------------------------------------------------------------------------------------------------------------
        //Funckja odkodowywująca i poprawiająca ewentualne błędy na podstawie macierzy H w tablicy bajtów które mają zostać zapisane do pliku
        //------------------------------------------------------------------------------------------------------------------------------------
        public byte[] DecryptFile(byte[] fileContent, bool[,] H)
        {
            //-----------------------
            //wyczyszczenie obu list
            //-----------------------
            inputList = new List<bool[]>();
            outputList = new List<bool[]>();

            //-----------------------------
            //zmienne do testowania błędów
            //-----------------------------
            bool[] Etest = Enumerable.Repeat(false, 16).ToArray();
            bool[] E;

            //-----------------------------------------------------------------------------
            //dodanie do input listy zawartości odczytanego pliku w postaci tablic 8 booli
            //-----------------------------------------------------------------------------
            getBitsFromBytes(fileContent);

            //------------------------------------------------------------------------------
            //łączenie 8 bitów z wiadomością oraz 8 bitów parzystości w wektory (tablice) T 
            //------------------------------------------------------------------------------
            for (int i = 0; i < inputList.Count; i+=2)
            {
                fuseintoOne(inputList[i], inputList[i + 1]);
            }

            //---------------------------------------------------------------------------
            //stworzenie i zapełneinie tablicy bajtów zawierających odkodowaną wiadomość 
            //---------------------------------------------------------------------------

            byte[] bytes = new byte[outputList.Count];

            //------------------------------------------------------
            //Detekcja błedów i ich naprawa jeżeli takowe wystąpiły
            //------------------------------------------------------
            for (int i = 0; i < outputList.Count; i++)
            {
                E = ham.detectSingleError(outputList[i], H);
                if (E.SequenceEqual(Etest))
                {
                    E = ham.detectDoubleError(outputList[i], H);
                    if (E.SequenceEqual(Etest))
                    {
                        
                    }
                    else
                    {
                        ham.fixT(outputList[i], E);
                    }
                }
                else
                {
                    ham.fixT(outputList[i], E);
                }

                //--------------------------------------------------------------------------------------------------------------
                //Usunięcie bitów parzystości i dodanie ich do tablicy bajtów zawierających odkodowany test gotowy do zapisania
                //--------------------------------------------------------------------------------------------------------------
                outputList[i]= ham.RemoveExcessBits(outputList[i], H);
                bytes[i] = (ConvertBoolArrayToByte(outputList[i]));
            }
            return bytes;
        }
    }
}
