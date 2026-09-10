//using Emgu.CV;
//using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace c666wayServ
{
    internal class Program
    {
        public static void swapTwoNumber(ref Char a, ref Char b)
        {
            Char temp = a;
            a = b;
            b = temp;
        }
        public static void prnPermut(Char[] list, int k, int m)
        {
            int i;
            if (k == m)
            {
                for (i = 0; i <= m; i++)
                {
                    Console.Write("{0}", list[i]);
                }
                Console.Write(" ");
            }
            else
            {
                for (i = k; i <= m; i++)
                {
                    swapTwoNumber(ref list[k], ref list[i]);
                    prnPermut(list, k + 1, m);
                    swapTwoNumber(ref list[k], ref list[i]);
                }
            }
        }

        /*************************************/
        public static String swapString(String a, int i, int j)
        {
            char[] b = a.ToCharArray();
            char ch;
            ch = b[i];
            b[i] = b[j];
            b[j] = ch;
            //Converting characters from array into single string  
            return string.Join("", b);
        }

        public static void generatePermutation(String str, int start, int end)
        {
            //Prints the permutations  
            if (start == end - 1)
            {
                Console.WriteLine(str);
                String temp = str;
                String mensaje = "Í#Ýà`ì8¹®öD,srÛ<k~íFô¾Ëõzd";
                Int32 k = 0;
                while (k < 255)
                {
                    Int32 j = 0;
                    while (j < 10)
                    {
                        String sms1 = String.Empty;
                        String sms2 = String.Empty;
                        String sms3 = String.Empty;
                        String sms4 = String.Empty;
                        String sms5 = String.Empty;
                        String sms6 = String.Empty;

                        for (Int32 l = 0; l < mensaje.Length; l++)
                        {
                            Char cM = mensaje[l];
                            Char cC = temp[(l + j) % temp.Length];

                            sms1 += Convert.ToChar(Math.Abs(cC + cM + k));
                            sms2 += Convert.ToChar(Math.Abs(cC - cM + k));
                            sms3 += Convert.ToChar(Math.Abs(cM - cC + k));
                            sms4 += Convert.ToChar(Math.Abs(cC + cM - k));
                            sms5 += Convert.ToChar(Math.Abs(cC - cM - k));
                            sms6 += Convert.ToChar(Math.Abs(cM - cC - k));
                        }

                        if (sms1.Contains("ttp://") ||
                            sms1.Contains("enjin") ||
                            sms1.Contains("//:ptt") ||
                            sms1.Contains("nijne") ||
                            sms1.Contains(".io/") ||
                            sms1.Contains("/oi.") ||
                            sms1.Contains(".com/") ||
                            sms1.Contains("/moc."))
                        {
                            string file = @"C:\logs\TheMonolith.txt";
                            File.AppendAllText(file, sms1 + "\r\n");

                        }
                        if (sms2.Contains("ttp://") || sms2.Contains("enjin") || sms2.Contains("//:ptt") || sms2.Contains("nijne")
                            || sms2.Contains(".io/") || sms2.Contains("/oi.") || sms2.Contains(".com/") || sms2.Contains("/moc."))
                        {
                            string file = @"C:\logs\TheMonolith.txt";

                            File.AppendAllText(file, sms2 + "\r\n");
                        }
                        if (sms3.Contains("ttp://") || sms3.Contains("enjin") || sms3.Contains("//:ptt") || sms3.Contains("nijne")
                            || sms3.Contains(".io/") || sms3.Contains("/oi.") || sms3.Contains(".com/") || sms3.Contains("/moc."))
                        {
                            string file = @"C:\logs\TheMonolith.txt";

                            File.AppendAllText(file, sms3 + "\r\n");
                        }
                        if (sms4.Contains("ttp://") ||
                            sms4.Contains("enjin") ||
                            sms4.Contains("//:ptt") ||
                            sms4.Contains("nijne") ||
                            sms4.Contains(".io/") ||
                            sms4.Contains("/oi.") ||
                            sms4.Contains(".com/") ||
                            sms4.Contains("/moc."))
                        {
                            string file = @"C:\logs\TheMonolith.txt";

                            File.AppendAllText(file, sms4 + "\r\n");
                        }
                        if (sms5.Contains("ttp://") || sms5.Contains("enjin") || sms5.Contains("//:ptt") || sms5.Contains("nijne")
                            || sms5.Contains(".io/") || sms5.Contains("/oi.") || sms5.Contains(".com/") || sms5.Contains("/moc."))
                        {
                            string file = @"C:\logs\TheMonolith.txt";

                            File.AppendAllText(file, sms5 + "\r\n");
                        }
                        if (sms6.Contains("ttp://") || sms6.Contains("enjin") || sms6.Contains("//:ptt") || sms6.Contains("nijne")
                            || sms6.Contains(".io/") || sms6.Contains("/oi.") || sms6.Contains(".com/") || sms6.Contains("/moc."))
                        {
                            string file = @"C:\logs\TheMonolith.txt";

                            File.AppendAllText(file, sms6 + "\r\n");
                        }


                        j++;
                    }

                    k++;
                }
            }
            else
            {
                for (int i = start; i < end; i++)
                {
                    //Swapping the string by fixing a character  
                    str = swapString(str, start, i);
                    //Recursively calling function generatePermutation() for rest of the characters   
                    generatePermutation(str, start + 1, end);
                    //Backtracking and swapping the characters again.  
                    str = swapString(str, start, i);
                }
            }
        }


        //////////////////////////////////////////////////////////////////////


        static List<List<T>> GetPermutationsWithRept<T>(IEnumerable<T> list, int length)
        {
            if (length == 1)
            {
                return list.Select(t => (new T[] { t }).ToList()).ToList();
            }
            else
            {
                return GetPermutationsWithRept(list, length - 1)
                    .SelectMany(t => list,
                        (t1, t2) => (t1.Concat(new T[] { t2 })).ToList()).ToList();
            }
        }

        // The main function that recursively prints
        // all repeated permutations of the given string.
        // It uses data[] to store all permutations one by one
        static void allLexicographicRecur(String str, char[] data,
                                          int last, int index)
        {
            int length = str.Length;

            // One by one fix all characters at the given index
            // and recur for the subsequent indexes
            for (int i = 0; i < length; i++)
            {

                // Fix the ith character at index and if
                // this is not the last index then
                // recursively call for higher indexes
                data[index] = str[i];

                // If this is the last index then print
                // the string stored in data[]
                if (index == last)
                {
                    String temp = new String(data);
                    String mensaje = "Í#Ýà`ì8¹®öD,srÛ<k~íFô¾Ëõzd";
                    Int32 k = 0;
                    while (k < 255)
                    {
                        Int32 j = 0;
                        while (j < 10)
                        {
                            String sms1 = String.Empty;
                            String sms2 = String.Empty;
                            String sms3 = String.Empty;
                            String sms4 = String.Empty;
                            String sms5 = String.Empty;
                            String sms6 = String.Empty;

                            for (Int32 l = 0; l < mensaje.Length; l++)
                            {
                                Char cM = mensaje[l];
                                Char cC = temp[(l + j) % temp.Length];

                                sms1 += Convert.ToChar(Math.Abs(cC + cM + k));
                                sms2 += Convert.ToChar(Math.Abs(cC - cM + k));
                                sms3 += Convert.ToChar(Math.Abs(cM - cC + k));
                                sms4 += Convert.ToChar(Math.Abs(cC + cM - k));
                                sms5 += Convert.ToChar(Math.Abs(cC - cM - k));
                                sms6 += Convert.ToChar(Math.Abs(cM - cC - k));
                            }

                            if (sms1.Contains("ttp://")  ||
                                sms1.Contains("enjin")    ||
                                sms1.Contains("//:ptt")   ||
                                sms1.Contains("nijne")    ||
                                sms1.Contains(".io/")     ||
                                sms1.Contains("/oi.")    ||
                                sms1.Contains(".com/")    ||
                                sms1.Contains("/moc.")    )
                            {
                                string file = @"C:\logs\TheMonolith.txt";                                                                
                                File.AppendAllText(file, sms1 + "\r\n");

                            }
                            if (sms2.Contains("ttp://") || sms2.Contains("enjin") || sms2.Contains("//:ptt") || sms2.Contains("nijne")
                                || sms2.Contains(".io/") || sms2.Contains("/oi.") || sms2.Contains(".com/") || sms2.Contains("/moc."))
                            {
                                string file = @"C:\logs\TheMonolith.txt";

                                File.AppendAllText(file, sms2 + "\r\n");
                            }
                            if (sms3.Contains("ttp://") || sms3.Contains("enjin") || sms3.Contains("//:ptt") || sms3.Contains("nijne")
                                || sms3.Contains(".io/") || sms3.Contains("/oi.") || sms3.Contains(".com/") || sms3.Contains("/moc."))
                            {
                                string file = @"C:\logs\TheMonolith.txt";

                                File.AppendAllText(file, sms3 + "\r\n");
                            }
                            if (sms4.Contains("ttp://") ||
                                sms4.Contains("enjin") ||
                                sms4.Contains("//:ptt") ||
                                sms4.Contains("nijne") ||
                                sms4.Contains(".io/") ||
                                sms4.Contains("/oi.") ||
                                sms4.Contains(".com/") ||
                                sms4.Contains("/moc."))
                            {
                                string file = @"C:\logs\TheMonolith.txt";

                                File.AppendAllText(file, sms4 + "\r\n");
                            }
                            if (sms5.Contains("ttp://") || sms5.Contains("enjin") || sms5.Contains("//:ptt") || sms5.Contains("nijne")
                                || sms5.Contains(".io/") || sms5.Contains("/oi.") || sms5.Contains(".com/") || sms5.Contains("/moc."))
                            {
                                string file = @"C:\logs\TheMonolith.txt";

                                File.AppendAllText(file, sms5 + "\r\n");
                            }
                            if (sms6.Contains("ttp://") || sms6.Contains("enjin") || sms6.Contains("//:ptt") || sms6.Contains("nijne")
                                || sms6.Contains(".io/") || sms6.Contains("/oi.") || sms6.Contains(".com/") || sms6.Contains("/moc."))
                            {
                                string file = @"C:\logs\TheMonolith.txt";

                                File.AppendAllText(file, sms6 + "\r\n");
                            }


                            j++;
                        }

                        k++;
                    }                    
                }
                else
                {
                    allLexicographicRecur(str, data, last,
                                               index + 1);
                }
            }
        }

        // This function sorts input string, allocate memory
        // for data(needed for allLexicographicRecur()) and calls
        // allLexicographicRecur() for printing all permutations
        static void allLexicographic(String str)
        {
            int length = str.Length;

            // Create a temp array that will be used by
            // allLexicographicRecur()
            char[] data = new char[length + 1];
            char[] temp = str.ToCharArray();

            // Sort the input string so that we get all
            // output strings in lexicographically sorted order
            Array.Sort(temp);
            str = new String(temp);

            // Now print all permutations
            allLexicographicRecur(str, data, length - 1, 0);
        }
        /**/

        static IEnumerable<string> GetVariations2(string s)
        {
            int[] indexes = new int[s.Length];
            StringBuilder sb = new StringBuilder();

            while (IncrementIndexes(indexes, s.Length))
            {
                sb.Clear();
                for (int i = 0; i < indexes.Length; i++)
                {
                    if (indexes[i] != 0)
                    {
                        sb.Append(s[indexes[i] - 1]);
                    }
                }
                yield return sb.ToString();
            }
        }

        static bool IncrementIndexes2(int[] indexes, int limit)
        {
            for (int i = 0; i < indexes.Length; i++)
            {
                indexes[i]++;
                if (indexes[i] > limit)
                {
                    indexes[i] = 1;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        static void GetVariations(string s)
        {
            int[] indexes = new int[s.Length];
            StringBuilder sb = new StringBuilder();

            while (IncrementIndexes(indexes, s.Length))
            {
                sb.Clear();
                for (int i = 0; i < indexes.Length; i++)
                {
                    if (indexes[i] != 0)
                    {
                        sb.Append(s[indexes[i] - 1]);
                    }
                }
                //yield return sb.ToString();                
                String temp = sb.ToString();
                //Task.Run(() =>
                //{

                    Console.WriteLine(temp);
                String mensaje = "Í#Ýà`ì8¹®öD,srÛ<k~íFô¾Ëõzd";
                Int32 k = 0;
                while (k < 255)
                {
                    Int32 j = 0;
                    while (j < 10)
                    {
                        String sms1 = String.Empty;
                        String sms2 = String.Empty;
                        String sms3 = String.Empty;
                        String sms4 = String.Empty;
                        String sms5 = String.Empty;
                        String sms6 = String.Empty;

                        for (Int32 l = 0; l < mensaje.Length; l++)
                        {
                            Char cM = mensaje[l];
                            Char cC = temp[(l + j) % temp.Length];

                            sms1 += Convert.ToChar(Math.Abs(cC + cM + k));
                            sms2 += Convert.ToChar(Math.Abs(cC - cM + k));
                            sms3 += Convert.ToChar(Math.Abs(cM - cC + k));
                            sms4 += Convert.ToChar(Math.Abs(cC + cM - k));
                            sms5 += Convert.ToChar(Math.Abs(cC - cM - k));
                            sms6 += Convert.ToChar(Math.Abs(cM - cC - k));
                        }

                        if (sms1.Contains("ttp://") ||
                            sms1.Contains("enjin") ||
                            sms1.Contains("//:ptt") ||
                            sms1.Contains("nijne") ||
                            sms1.Contains("enjin.io/") ||
                            sms1.Contains("/oi.nijne") ||
                            sms1.Contains(".com/") ||
                            sms1.Contains("/moc."))
                        {
                            string file = @"C:\logs\TheMonolith.txt";
                            File.AppendAllText(file, sms1 + "_Magic:" + temp + ":\r\n");

                        }
                        if (sms2.Contains("ttp://") || sms2.Contains("enjin") || sms2.Contains("//:ptt") || sms2.Contains("nijne")
                            || sms2.Contains("enjin.io/") || sms2.Contains("/oi.nijne") || sms2.Contains(".com/") || sms2.Contains("/moc."))
                        {
                            string file = @"C:\logs\TheMonolith.txt";

                            File.AppendAllText(file, sms2 + "_Magic:" + temp + ":\r\n");
                        }
                        if (sms3.Contains("ttp://") || sms3.Contains("enjin") || sms3.Contains("//:ptt") || sms3.Contains("nijne")
                            || sms3.Contains("enjin.io/") || sms3.Contains("/oi.nijne") || sms3.Contains(".com/") || sms3.Contains("/moc."))
                        {
                            string file = @"C:\logs\TheMonolith.txt";

                            File.AppendAllText(file, sms3 + "_Magic:" + temp + ":\r\n");
                        }
                        if (sms4.Contains("ttp://") ||
                            sms4.Contains("enjin") ||
                            sms4.Contains("//:ptt") ||
                            sms4.Contains("nijne") ||
                            sms4.Contains("enjin.io/") ||
                            sms4.Contains("/oi.nijne") ||
                            sms4.Contains(".com/") ||
                            sms4.Contains("/moc."))
                        {
                            string file = @"C:\logs\TheMonolith.txt";

                            File.AppendAllText(file, sms4 + "_Magic:" + temp + ":\r\n");
                        }
                        if (sms5.Contains("ttp://") || sms5.Contains("enjin") || sms5.Contains("//:ptt") || sms5.Contains("nijne")
                            || sms5.Contains("enjin.io/") || sms5.Contains("/oi.nijne") || sms5.Contains(".com/") || sms5.Contains("/moc."))
                        {
                            string file = @"C:\logs\TheMonolith.txt";

                            File.AppendAllText(file, sms5 + "_Magic:" + temp + ":\r\n");
                        }
                        if (sms6.Contains("ttp://") || sms6.Contains("enjin") || sms6.Contains("//:ptt") || sms6.Contains("nijne")
                            || sms6.Contains("enjin.io/") || sms6.Contains("/oi.nijne") || sms6.Contains(".com/") || sms6.Contains("/moc."))
                        {
                            string file = @"C:\logs\TheMonolith.txt";

                            File.AppendAllText(file, sms6 + "_Magic:" + temp + ":\r\n");
                        }


                        j++;
                    }

                    k++;
                }
                //});
            }
        }

        static bool IncrementIndexes(int[] indexes, int limit)
        {
            for (int i = 0; i < indexes.Length; i++)
            {
                indexes[i]++;
                if (indexes[i] > limit)
                {
                    indexes[i] = 1;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        static void Main(string[] args)
        {
            //string str = String.Empty;
            //for (Int32 iC = 0; iC < 127; iC++)
            //{
            //    str += Convert.ToChar(iC);
            //}
            ////str = "ABC";
            //GetVariations(str);

            ////return;

            //int n, i;


            //for (n = 1; n < str.Length; n++)
            //{

            //    Console.Write("\n The Permutations with a combination of {0} digits are : \n", n);
            //    prnPermut(str.ToArray(), 0, n - 1);
            //    Console.Write("\n\n");
            //}
            ////return;

            //String hexString = "cd23dde000000000000000000000000000000000000000000000000000000000000001600000000000000000000000000000000000000000000000000000000000000001000000000000000000000000000000000000000000000000000000000000000100000000000000000000000003ec388fb9aef6442c7372db3c6b7eed93469c0b00000000000000000000000000000000000000000000f4becbf58b027a640000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000c546865204d6f6e6f6c6974680000000000000000000000000000000000000000";

            ////System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            ////byte[] dBytes = encoding.GetBytes(hexString);

            //byte[] dBytes = StringToByteArray(hexString).ToArray();//
            //byte[] bytes = dBytes.Where(b => b > 0).ToArray();

            //string UTF8 = Encoding.UTF8.GetString(bytes);
            //string Unicode = Encoding.Unicode.GetString(bytes);
            //string BigEndianUnicode = Encoding.BigEndianUnicode.GetString(bytes);
            //string ASCII = Encoding.ASCII.GetString(bytes);

            //String s = String.Empty;
            //foreach (Byte b in dBytes)
            //{
            //    if (b > 0)
            //    {
            //        s += Convert.ToChar(b);
            //    }
            //}

            //String stop = "";

            //while (true)
            //{
            //    ////Int32 iC = -1;
            //    ////Int32 iD = -1;
            //    ////foreach (Char c in s)
            //    ////{
            //    ////    if(iC != -1)
            //    ////    {
            //    ////        iD = iC - c;
            //    ////    }

            //    ////    iC = c;



            //    ////}

            //}


            //QRCodeDetector qrD = new QRCodeDetector();
            //Image<Bgr, Byte> img1 = new Image<Bgr, Byte>("_QR.bmp");
            //IOutputArray arrayPoints = null;
            //if (qrD.Detect(img1, arrayPoints))
            //{
            //    String code = qrD.DecodeCurved(img1, arrayPoints);
            //}
            //else
            //{
            //    Console.WriteLine("A por puas...");
            //}


            //c666wayServ server = new c666wayServ();
            //Console.ReadKey();

            String mensaje = File.ReadAllText("c:\\logs\\2WhatIsTheQuest.txt"); ;



            //String mensaje = "Í#Ýà`ì8¹®öD,srÛ<k~íF";

            ////////List<Char> lC = "ô¾Ëõzdô¾Ëõzdô¾Ëõzdô¾Ëõzdô¾Ëõzdô¾Ëõzdô¾Ëõzdô¾Ëõzdô¾Ëõzdô¾Ëõzdô¾Ëõzdô¾Ëõzd".Reverse().ToList();
            //////List<Char> lC = "The MonolithThe MonolithThe MonolithThe MonolithThe MonolithThe MonolithThe MonolithThe MonolithThe Monolith".Reverse().ToList();

            ////////String clave = "The MonolithThe MonolithThe MonolithThe MonolithThe MonolithThe MonolithThe MonolithThe MonolithThe Monolith";
            ////////String clave = "ô¾Ëõzdô¾Ëõzdô¾Ëõzdô¾Ëõzdô¾Ëõzdô¾Ëõzdô¾Ëõzdô¾Ëõzdô¾Ëõzdô¾Ëõzdô¾Ëõzdô¾Ëõzd";
            //////String clave = String.Empty;
            //////foreach (Char c in lC)
            //////{
            //////    clave += c;
            //////}

            string str = String.Empty; // "TSARY\r\nUE NSG";
            for (Int32 iC = 32; iC < 127; iC++)
            {
                str += Convert.ToChar(iC);
            }

            ////String str = "ABC";
            ////int len = str.Length;
            ////generatePermutation(str, 0, 1);

            ////return;

            //for (Int32 lC = 1; lC < str.Length; lC++)
            //{ 
            //    for (Int32 l = 0; l < str.Length / lC; l+= lC)
            //    {
            //        String str2 = str.Substring(l, lC);
            //        allLexicographic(str2);
            //    }
            //}

            //return;
            Int32 z = 0;
            Int32 jFiles = 0;
            while (z < 100)
            {
                z++;

                List<List<Char>> listP = GetPermutationsWithRept(str, z);
                List<Task> tasks = new List<Task>();
                foreach (List<Char> cL in listP)
                {
                    String t = String.Empty;
                    foreach (Char c in cL)
                    {
                        t += c;
                    }
                    Console.WriteLine(t);
                    Task task = Task.Factory.StartNew(() =>
                    {
                        String temp = t;
                        Int32 k = 0;
                        while (k < 1)
                        {
                            Int32 j = 0;
                            while (j < 10)
                            {
                                String sms1 = String.Empty;
                                String sms2 = String.Empty;
                                String sms3 = String.Empty;
                                String sms4 = String.Empty;
                                String sms5 = String.Empty;
                                String sms6 = String.Empty;

                                for (Int32 i = 0; i < mensaje.Length; i++)
                                {
                                    Char cM = mensaje[i];
                                    Char cC = temp[(i + j) % temp.Length];

                                    sms1 += Convert.ToChar(Math.Abs(cC + cM + k));
                                    sms2 += Convert.ToChar(Math.Abs(cC - cM + k));
                                    sms3 += Convert.ToChar(Math.Abs(cM - cC + k));
                                    sms4 += Convert.ToChar(Math.Abs(cC + cM - k));
                                    sms5 += Convert.ToChar(Math.Abs(cC - cM - k));
                                    sms6 += Convert.ToChar(Math.Abs(cM - cC - k));
                                }

                                if (sms1.Contains("http://") || sms2.Contains("http://") || sms3.Contains("http://") ||
                                                sms4.Contains("http://") || sms5.Contains("http://") || sms6.Contains("http://") ||
                                                sms1.Contains("jointhequest") || sms2.Contains("jointhequest") || sms3.Contains("jointhequest") ||
                                                sms4.Contains("jointhequest") || sms5.Contains("jointhequest") || sms6.Contains("jointhequest") ||
                                                sms1.Contains("quest.io/") || sms2.Contains("quest.io/") || sms3.Contains("quest.io/") ||
                                                sms4.Contains("quest.io/") || sms5.Contains("quest.io/") || sms6.Contains("quest.io/"))
                                {
                                    String stop = "menosWOWpero WOW WOWO";

                                    File.WriteAllText("c:\\logs\\crazy\\sms1" + jFiles.ToString() + ".txt",
                                        "k:" + k.ToString() + "\rtemp:" + temp + "\r" + sms1);
                                    File.WriteAllText("c:\\logs\\crazy\\sms2" + jFiles.ToString() + ".txt",
                                        "k:" + k.ToString() + "\rtemp:" + temp + "\r" + sms2);
                                    File.WriteAllText("c:\\logs\\crazy\\sms3" + jFiles.ToString() + ".txt",
                                        "k:" + k.ToString() + "\rtemp:" + temp + "\r" + sms3);
                                    File.WriteAllText("c:\\logs\\crazy\\sms4" + jFiles.ToString() + ".txt",
                                        "k:" + k.ToString() + "\rtemp:" + temp + "\r" + sms4);
                                    File.WriteAllText("c:\\logs\\crazy\\sms5" + jFiles.ToString() + ".txt",
                                        "k:" + k.ToString() + "\rtemp:" + temp + "\r" + sms5);
                                    File.WriteAllText("c:\\logs\\crazy\\sms6" + jFiles.ToString() + ".txt",
                                        "k:" + k.ToString() + "\rtemp:" + temp + "\r" + sms6);
                                    jFiles++;
                                    Console.WriteLine(".....FINDED....");
                                }


                                j++;
                            }

                            k++;

                        }
                    });
                    tasks.Add(task);
                    if (tasks.Count > 1000)
                    {
                        Task.WaitAll();
                        tasks.Clear();
                    }
                }
            }
        }

            
        

        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars - 1; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

        //public static List<byte> StringToByteArray(String hex)
        //{
        //    int NumberChars = hex.Length / 2;
        //    List<byte> bytes = new List<byte>();
        //    using (var sr = new StringReader(hex))
        //    {
        //        for (int i = 0; i < NumberChars; i++)
        //        {
        //            try
        //            {
        //                bytes.Add(Convert.ToByte(new string(new char[2] { (char)sr.Read(), (char)sr.Read() }), 16));
        //            }
        //            catch (Exception ex)
        //            { 

        //            }

        //        }
        //    }
        //    return bytes;
        //}

        private static void Swap(ref char a, ref char b)
        {
            if (a == b) return;

            var temp = a;
            a = b;
            b = temp;
        }

        public static void GetPer(char[] list)
        {
            int x = list.Length - 1;
            GetPer(list, 0, x);
        }

        private static void GetPer(char[] list, int k, int m)
        {
            if (k == m)
            {
                Console.Write(list);
            }
            else
                for (int i = k; i <= m; i++)
                {
                    Swap(ref list[k], ref list[i]);
                    GetPer(list, k + 1, m);
                    Swap(ref list[k], ref list[i]);
                }
        }
    }

    
}
