using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telekomunikacja1
{
    class Program
    {
        static void Main(string[] args)
        {
            Hamming ham = new Hamming();
            FileMenager fileMenager = new FileMenager();
            fileEncoder fileEncoder = new fileEncoder();


            bool[,] H = new bool[,]
            {
                //a1    a2     a3     a4     a5     a6     a7     a8     c1     c2     c3     c4     c5     c6     c7     c8
                {true,  true,  true,  true,  false, false, false, false, true,  false, false, false, false, false, false, false},
                {true,  true,  false, false, true,  true,  false, false, false, true,  false, false, false, false, false, false},
                {true,  false, true,  false, true,  false, true,  false, false, false, true,  false, false, false, false, false},
                {false, true,  false, true,  false, true,  true,  false, false, false, false, true,  false, false, false, false},
                {true,  true,  true,  false, true,  false, false, true,  false, false, false, false, true,  false, false, false},
                {true,  false, false, true,  false, true,  false, true,  false, false, false, false, false, true,  false, false},
                {false, true,  true,  true,  true,  false, true,  true,  false, false, false, false, false, false, true,  false},
                {true,  true,  true,  false, false, true,  true,  true,  false, false, false, false, false, false, false, true }
            };
            bool loop = true;

            while (loop)
            {
                Console.WriteLine("podaj które zadanie chcesz pokazać: (1, 2, 3 , lub stop aby zakończyć)");
                string podpunkt = Console.ReadLine();

                switch (podpunkt)
                {
                    case "1":

                        Console.WriteLine("\n\n//1.1    1.1    1.1    1.1    1.1    1.1    1.1    1.1    1.1    1.1    1.1    1.1    1.1    1.1    1.1    1.1    1.1");

                        bool[] B1 = new bool[] { true, true, true, true, false, false, false, false };
                        Console.WriteLine("\nwejściowy wektor B: [" + string.Join(" ", B1) + "]");

                        bool[] T1 = ham.GetTFromBH(B1, H);
                        Console.WriteLine("\nwektor z dodanymi bitami parzystości T: [" + string.Join(" ", T1) + "]");

                        T1[5] = !T1[5];
                        Console.WriteLine("\nzamieniono bit o indeksie 5 na przeciwny");
                        Console.WriteLine("błędny wektor TE: [" + string.Join(" ", T1) + "]");
                        Console.WriteLine("\nCzy wektor T jest poprawny: " + ham.checkT(T1, H));

                        bool[] E1 = ham.detectSingleError(T1, H);
                        Console.WriteLine("\nwektor błędu E: [" + string.Join(" ", E1) + "]");

                        ham.fixT(T1, E1);
                        Console.WriteLine("\nnaprawiony wektor T: [" + string.Join(" ", T1) + "]");
                        Console.WriteLine("Czy wektor T jest poprawny: " + ham.checkT(T1, H));
                        break;

                    case "2":

                        Console.WriteLine("\n\n//1.2    1.2    1.2    1.2    1.2    1.2    1.2    1.2    1.2    1.2    1.2    1.2    1.2    1.2    1.2    1.2    1.2");

                        bool[] B2 = new bool[] { false, true, false, true, false, true, false, true };
                        Console.WriteLine("\nwejściowy wektor B: [" + string.Join(" ", B2) + "]");

                        bool[] T2 = ham.GetTFromBH(B2, H);
                        Console.WriteLine("\nwektor z dodanymi bitami parzystości T: [" + string.Join(" ", T2) + "]");

                        T2[7] = !T2[7];
                        T2[13] = !T2[13];
                        Console.WriteLine("\nzamieniono bity o indeksie 7 i 13 na przeciwne");
                        Console.WriteLine("błędny wektor TE: [" + string.Join(" ", T2) + "]");
                        Console.WriteLine("\nCzy wektor T jest poprawny: " + ham.checkT(T2, H));

                        bool[] E2 = ham.detectDoubleError(T2, H);
                        Console.WriteLine("\nwektor błędu E: [" + string.Join(" ", E2) + "]");

                        ham.fixT(T2, E2);
                        Console.WriteLine("\nnaprawiony wektor T: [" + string.Join(" ", T2) + "]");
                        Console.WriteLine("Czy wektor T jest poprawny: " + ham.checkT(T2, H));
                        break;

                    case "3":

                        Console.WriteLine("\n\n//1.3    1.3    1.3    1.3    1.3    1.3    1.3    1.3    1.3    1.3    1.3    1.3    1.3    1.3    1.3    1.3    1.3");

                        Console.WriteLine("\npodaj nazwe pliku z którego chcesz odczytać bajty: ");
                        string inputFile = Console.ReadLine();

                        byte[] fileContent = fileMenager.readFile(inputFile);
                        byte[] output;
                        Console.WriteLine("\nCo chcesz zrobić z plikiem (zaszyfrować [wpisz E] / odszyfrować [wpisz D])");
                        string operation = Console.ReadLine();
                        if (operation == "E")
                        {
                            output = fileEncoder.EncryptFile(fileContent, H);
                        }
                        else if (operation == "D")
                        {
                            output = fileEncoder.DecryptFile(fileContent, H);
                        }
                        else
                        {
                            output = new byte[0];
                        }
                        Console.WriteLine("tablica bajtów końcowa: " + string.Join(" ", output));

                        Console.WriteLine("\npodaj nazwe pliku do zapisu: ");

                        string outputFile = Console.ReadLine();
                        fileMenager.saveFile(output, outputFile);

                        break;

                    case "stop":
                        loop = false;
                        break;

                }
                Console.WriteLine("\n\n");
            }
        }
    }
}
