using FileToVideo_VideoToFile;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace comp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if(args == null && args.Length > 0) 
            {
                Console.WriteLine("Please, copy file to compress in the same directory of this application and type his name. Fe: directory:\\>comp9.exe enwik9");
            }
            else
            {
                List<Byte> result = new List<Byte>();

                Byte[] bytes = System.IO.File.ReadAllBytes(args[0]);
                String bA =  String.Concat(bytes.Select(x => Convert.ToString(x, 2).PadLeft(8, '0')));

                String composedbA = String.Empty;

                for(Int32 i = 0; (i * 2) + 2 < bA.Length; i++)
                {
                    String dosB = bA.Substring(i * 2, 2);

                    String tresB = String.Empty;
                    switch (dosB)
                    {
                        case "00":
                            tresB = "0001";
                            break;

                        case "01":
                            tresB = "0010";
                            break;

                        case "10":
                            tresB = "0011";
                            break;

                        case "11":
                            tresB = "0100";
                            break;
                    }
                    composedbA += tresB;
                }

                String finalS = String.Empty;
                                
                Int32 f = composedbA.Length / 4;
                for (Int32 z = 2; z < f / 2; z++) 
                {
                    Int32 resto = -1;
                    Int32 div = Math.DivRem(f, z,out resto);
                    div *= 4;
                    Int32 maxI = 1;
                    Dictionary<String, List<Int32>> possibilities = new Dictionary<String, List<Int32>>();
                    for (Int32 i = 0; (i * div) + div < composedbA.Length; i++)
                    {
                        String zDivB = composedbA.Substring(i * div, div);

                        if(possibilities.ContainsKey(zDivB))
                        {
                            possibilities[zDivB].Add(i * div);

                            if (possibilities[zDivB].Count > maxI)
                            {
                                maxI = possibilities[zDivB].Count;
                            }

                        }
                        else
                        {
                            possibilities.Add(zDivB, new List<Int32>() { i * div });
                        }
                    }

                    

                    if (maxI > Math.Sqrt(z))
                    {
                        var keyPaies = possibilities.OrderByDescending(x => x.Value.Count).ToList(); // .ToDictionary(x => x.Key, x => x.Value);    

                        Stack<String> stakcSob = new Stack<string>();
                        stakcSob.Push("0000");
                        stakcSob.Push("0101");
                        stakcSob.Push("0110");
                        stakcSob.Push("0111");
                        stakcSob.Push("1000");
                        stakcSob.Push("1001");
                        stakcSob.Push("1010");
                        stakcSob.Push("1011");
                        stakcSob.Push("1100");
                        stakcSob.Push("1101");
                        stakcSob.Push("1110");
                        stakcSob.Push("1111");

                        Dictionary<String, String> dic = new Dictionary<string, string>();
                        for (Int32 el4 = 0; el4 < 12; el4++)
                        {
                            dic.Add(keyPaies[el4].Key, stakcSob.Pop());
                        }

                        //for (Int32 k = 1; k < maxI / 2; k++)
                        //{
                        //    try
                        //    {
                                
                        List<String> listContainsKey = new List<String>();
                        for (Int32 i = 0; (i * div) + div < composedbA.Length; i++)
                        {
                            String zDivB = composedbA.Substring(i * div, div);

                            if (dic.ContainsKey(zDivB))
                            {
                                if (listContainsKey.Contains(zDivB))
                                {
                                    finalS += dic[zDivB];
                                }
                                else
                                {
                                    listContainsKey.Add(zDivB);
                                    finalS += dic[zDivB] + zDivB + dic[zDivB];
                                }
                            }
                            else
                            {
                                finalS += zDivB;
                            }

                        }
                        //    }
                        //    catch (Exception ex)
                        //    {

                        //    }
                        //}
                        break;
                    }
                }

                //////byte[] bs = compSeries(args[0]);
                //Byte[] r = System.IO.File.ReadAllBytes(args[0]);

                //Byte[] priMaxMin = compMaxMin03(r);
                //Byte[] priMaxMin2 = compMaxMin03(priMaxMin);

                //Byte[] huffmanB = HuffmanPrefixs(priMaxMin);


                //Byte[] temp = priMaxMin;
                //while(temp != null)
                //{
                //    temp = priMaxMin;
                //    priMaxMin = compMaxMin(temp);
                //    if(priMaxMin == null)
                //    {
                //        break;
                //    }
                //    if(priMaxMin.Length == temp.Length)
                //    {
                //        priMaxMin = HuffmanPrefixs(priMaxMin);
                //    }
                //}


                String stop = "stop";

                ////Byte[] temp = r;
                ////BigInteger i = 0;
                ////Boolean ori = true;
                ////do
                ////{
                ////    if(i > 0)
                ////    {
                ////        Console.WriteLine("Round " + (i - 1).ToString() + " V");
                ////    }

                ////    Console.WriteLine("trying round " + i.ToString());
                ////    r = temp;
                ////    temp = compPri(r);
                ////    ori = false;
                ////    i++;
                ////    GC.Collect();

                ////}
                ////while (temp != null && temp.Length + 4 < r.Length);
                ////Console.WriteLine("The compression ended in the round " + (i - 1).ToString());

                ////System.IO.File.WriteAllBytes("archive9.bhm", r);

            }

            Console.ReadKey();
        }

        private static Byte[] compSeries(String fileName)
        {
            List<Byte> result = new List<Byte>();

            using (FileStream fS = System.IO.File.OpenRead(fileName))
            {
                List<SeriesDataInfo> series = new List<SeriesDataInfo>();

                Int32 bLength = 105;
                Byte[] bytes = new Byte[bLength];
                Int32 rR = -1;
                String s01 = String.Empty;                                    
                String s01b = String.Empty;
                Int32 offsetNex = 0;

                do
                {

                    rR = fS.Read(bytes, 0, bLength);

                    if (rR > 0)
                    {
                        if (offsetNex == 0)
                        {
                            s01 = getString01FromByteArray(bytes);
                        }
                        else if (offsetNex > 0)
                        {
                            s01b = getString01FromByteArray(bytes);
                            Boolean encontrado = false;
                            for (Int32 minus = 1; minus < 9; minus++)
                            {
                                List<String> eses = new List<String>();
                                Int32 slength = s01.Length / minus;

                                for (Int32 coger = 0; coger < s01.Length - slength; coger += slength)
                                {
                                    eses.Add(s01.Substring(coger, slength));
                                }

                                foreach (String s in eses)
                                {
                                    Int32 indexO = s01b.IndexOf(s);

                                    if (indexO != -1)
                                    {
                                        SeriesDataInfo sDI = series.Where(x => x.s01 == s).SingleOrDefault();

                                        if (sDI == null)
                                        {
                                            series.Add(new SeriesDataInfo() { s01 = s, Count = 2 });
                                        }
                                        else
                                        {
                                            sDI.Count++;
                                        }
                                        encontrado = true;
                                        break;
                                    }
                                }

                                if (encontrado)
                                {
                                    break;
                                }
                            }

                            s01 = s01b;
                        }
                    }

                    offsetNex += bLength;
                }
                while (rR > 0);
           
                Dictionary<String, Int32> dic = new Dictionary<String, Int32>();
                Int32 i = 0;

                List<Int32> indexesInfo = new List<Int32>();

                fS.Position = 0;
                bytes = new Byte[bLength];
                rR = -1;
                s01 = String.Empty;
                series = series.OrderByDescending(x => x.s01.Length).ToList();
                do
                {
                    rR = fS.Read(bytes, 0, bLength);

                    if (rR > 0)
                    {
                        s01 += getString01FromByteArray(bytes);

                        Boolean encontrado = false;
                        foreach (SeriesDataInfo s in series)
                        {
                            Int32 indexF = s01.IndexOf(s.s01);

                            if (indexF == -1)
                            {
                            }
                            else if (indexF > 0)
                            {
                                encontrado = true;
                                String sInit = s01.Substring(0, indexF);
                                Boolean isInDic = dic.ContainsKey(sInit);
                                if (isInDic)
                                {
                                    indexesInfo.Add(dic[sInit]);
                                }
                                else
                                {
                                    indexesInfo.Add(i);
                                    dic.Add(sInit, i);
                                    i++;
                                }

                                isInDic = dic.ContainsKey(s.s01);
                                if (isInDic)
                                {
                                    indexesInfo.Add(dic[s.s01]);
                                }
                                else
                                {
                                    indexesInfo.Add(i);
                                    dic.Add(s.s01, i);
                                    i++;
                                }
                                s01 = s01.Remove(0, indexF + s.s01.Length);
                                break;
                            }
                            else
                            {
                                encontrado = true;
                                Boolean isInDic = dic.ContainsKey(s.s01);
                                if (isInDic)
                                {
                                    indexesInfo.Add(dic[s.s01]);
                                }
                                else
                                {
                                    indexesInfo.Add(i);
                                    dic.Add(s.s01, i);
                                    i++;
                                }
                                s01 = s01.Remove(0, s.s01.Length);
                                break;
                            }
                        }                        
                    }
                }
                while (rR > 0);



                List<Int32> distinctsIndex = indexesInfo.Distinct().OrderBy(x => x).ToList();
                                
                Dictionary<String, Int32> FinalDic = new Dictionary<String, Int32>();
                foreach (Int32 indexIn in distinctsIndex)
                {
                    if(dic.ContainsValue(indexIn))
                    {
                        foreach (String key in dic.Keys)
                        {
                            if (dic[key] == indexIn)
                            {
                                FinalDic.Add(key, indexIn);
                                break;
                            }

                        }
                    }

                }

                // TODO: Ajustar los índices al nuevo diccionario y en indexes info para raspar números de índices


                String stop = "stop";

            }

            
            return result.ToArray();
        }

        private static Byte[] compMaxMin(Byte[] bytes)
        {
            List<Byte> result = new List<Byte>();
                        
            Int32 maxB = -1;
            Int32 minB = -1;
            Boolean isFirst = true;            
            List<MaxMinRoads> roads = new List<MaxMinRoads>();
            MaxMinRoads antRoad = new MaxMinRoads();
            List<Int32> códigosDeControl = new List<Int32>();

            for(Int32 i = 0; i < 256;i++)
            {
                códigosDeControl.Add(i);
            }

            for (Int32 i = 0; i < bytes.Length; i++)
            {                
                Int32 intB = Convert.ToInt32(bytes[i]);

                if(códigosDeControl.Contains(intB))
                {
                    códigosDeControl.Remove(intB);
                }

                Int32 road = intB < 8 ? -1 : intB < 16 ? 4 : intB < 32 ? 3 : intB < 64 ? 2 : intB < 128 ? 1 : -1; //intB < 256 ? 6 : -1;

                if (isFirst)
                {
                    antRoad.IndexIni = i;
                    antRoad.Road = road;
                    maxB = intB;
                    minB = intB;
                    isFirst = false;
                }
                else
                {
                    if(antRoad.Road == -1)
                    {
                        antRoad.IndexIni = i;
                        antRoad.Road = road;
                    }
                    else if(road != antRoad.Road)
                    {
                        if (road > antRoad.Road)
                        {
                            Boolean addToRoads = true;
                            Int32 indexlength = (i - 1) - antRoad.IndexIni;
                            switch (antRoad.Road)
                            {
                                case 1:
                                    if(indexlength < 17)
                                    {
                                        addToRoads = false;
                                    }
                                    break;
                                case 2:
                                    if (indexlength < 9)
                                    {
                                        addToRoads = false;
                                    }
                                    break;
                                case 3:
                                    if (indexlength < 6)
                                    {
                                        addToRoads = false;
                                    }
                                    break;
                                case 4:
                                    if (indexlength < 5)
                                    {
                                        addToRoads = false;
                                    }
                                    break;
                                case -1:
                                    break;

                            }

                            if (addToRoads)
                            {
                                antRoad.IndexEnd = i - 1;
                                roads.Add(antRoad);
                                antRoad = new MaxMinRoads() { IndexIni = i, Road = road };
                            }
                            else
                            {
                                antRoad = new MaxMinRoads() { IndexIni = i, Road = road };
                            }
                        }
                    }

                    if(intB > maxB)
                    {
                        maxB = intB;
                    }
                    else if (intB < minB)
                    {
                        minB = intB;
                    }
                }
            }
            
            List<MaxMinRoadTotal> totals = new List<MaxMinRoadTotal>();

            foreach (MaxMinRoads roa in roads)
            {
                Int32 lenth = roa.IndexEnd - roa.IndexIni;

                MaxMinRoadTotal total = totals.Where(x => x.Length == lenth).SingleOrDefault();

                if(total == null)
                {
                    totals.Add(new MaxMinRoadTotal() { Length = lenth, Count = 1 });
                }
                else
                {
                    total.Count++;
                }
            }

            totals = totals.OrderByDescending(x => x.Count).ToList();

            Byte byteDeControl = 0;
            if (códigosDeControl.Count > 0)
            {
                byteDeControl = Convert.ToByte(códigosDeControl.FirstOrDefault());
                //byteDeControl = Convert.ToByte(minB - 1);
            }
            else
            {
                return null;
            }

            // 
            String sComp = String.Empty;
            Int32 actualR = -1;
            Boolean estamosEnUnCamino = false;
            for (Int32 i = 0; i < bytes.Length; i++)
            {
                MaxMinRoads roadIni = roads.Count > 0 ? roads[0].IndexIni == i ? roads[0] : null : null;
                
                if(roadIni == null)
                {
                    if(estamosEnUnCamino)
                    {
                        MaxMinRoads roadEnd = roads.Count > 0 ? roads[0].IndexEnd == i ? roads[0] : null: null;

                        sComp += getString01FromByte(bytes[i]).Substring(actualR);

                        if (roadEnd != null)
                        {
                            estamosEnUnCamino = false;
                            roads.RemoveAt(0);
                        }

                    }
                    else
                    {
                        sComp += getString01FromByte(bytes[i]);
                    }                                        
                }
                else
                {
                    estamosEnUnCamino = true;
                    Int32 r = roadIni.Road;
                    actualR = r;
                    Int32 l = roadIni.IndexEnd - roadIni.IndexIni;
                    sComp += getString01FromByte(byteDeControl);
                    sComp += FromBase10((r - 1).ToString(), 2).PadLeft(2, '0') + FromBase10(l.ToString(), 2).PadLeft(6, '0');
                    sComp += getString01FromByte(bytes[i]).Substring(r);
                }

                while (sComp.Length > 7)
                {
                    result.Add(getByteFromString01(sComp.Substring(0, 8)));
                    sComp = sComp.Remove(0, 8);
                }
            }
            
            return result.ToArray();
        }

        private static Byte[] compMaxMin02(Byte[] bytes)
        {
            List<Byte> result = new List<Byte>();

            Int32 maxB = -1;
            Int32 minB = -1;
            Boolean isFirst = true;
            List<MaxMinRoads> roads = new List<MaxMinRoads>();
            MaxMinRoads antRoad = new MaxMinRoads();
            List<Int32> códigosDeControl = new List<Int32>();

            for (Int32 i = 0; i < 256; i++)
            {
                códigosDeControl.Add(i);
            }

            for (Int32 i = 0; i < bytes.Length; i++)
            {
                Int32 intB = Convert.ToInt32(bytes[i]);

                if (códigosDeControl.Contains(intB))
                {
                    códigosDeControl.Remove(intB);
                }

                Int32 road = intB < 8 ? 5 : intB < 16 ? 4 : intB < 32 ? 3 : intB < 64 ? 2 : intB < 128 ? 1 : -1; //intB < 256 ? 6 : -1;

                if (isFirst)
                {
                    antRoad.IndexIni = i;
                    antRoad.Road = road;
                    maxB = intB;
                    minB = intB;
                    isFirst = false;
                }
                else
                {
                    if (antRoad.Road == -1)
                    {
                        antRoad.IndexIni = i;
                        antRoad.Road = road;
                    }
                    else if (road != antRoad.Road)
                    {
                        if (road < antRoad.Road)
                        {
                            Boolean addToRoads = true;
                            Int32 indexlength = (i - 1) - antRoad.IndexIni;
                            switch (antRoad.Road)
                            {
                                case 1:
                                    if (indexlength < 17)
                                    {
                                        addToRoads = false;
                                    }
                                    break;
                                case 2:
                                    if (indexlength < 9)
                                    {
                                        addToRoads = false;
                                    }
                                    break;
                                case 3:
                                    if (indexlength < 6)
                                    {
                                        addToRoads = false;
                                    }
                                    break;
                                case 4:
                                    if (indexlength < 5)
                                    {
                                        addToRoads = false;
                                    }
                                    break;
                                case 5:
                                    if (indexlength < 4)
                                    {
                                        addToRoads = false;
                                    }
                                    break;
                            }

                            if (addToRoads)
                            {
                                antRoad.IndexEnd = i - 1;
                                roads.Add(antRoad);
                                antRoad = new MaxMinRoads() { IndexIni = i, Road = road };
                            }
                            else
                            {
                                antRoad = new MaxMinRoads() { IndexIni = i, Road = road };
                            }
                        }
                        else if (road > antRoad.Road)
                        {
                            Int32 k = 0;
                            for (Int32 j = i; j < bytes.Length; j++)
                            {
                                Int32 intBJ = Convert.ToInt32(bytes[j]);
                                Int32 roadJ = intBJ < 8 ? 5 : intBJ < 16 ? 4 : intBJ < 32 ? 3 : intBJ < 64 ? 2 : intBJ < 128 ? 1 : -1; //intB < 256 ? 6 : -1;

                                if (road != roadJ)
                                {
                                    break;
                                }
                                k++;
                            }

                            Boolean addToRoads = true;
                            Int32 indexlength = k;
                            switch (antRoad.Road)
                            {
                                case 1:
                                    if (indexlength < 17)
                                    {
                                        addToRoads = false;
                                    }
                                    break;
                                case 2:
                                    if (indexlength < 9)
                                    {
                                        addToRoads = false;
                                    }
                                    break;
                                case 3:
                                    if (indexlength < 6)
                                    {
                                        addToRoads = false;
                                    }
                                    break;
                                case 4:
                                    if (indexlength < 5)
                                    {
                                        addToRoads = false;
                                    }
                                    break;
                                case 5:
                                    if (indexlength < 4)
                                    {
                                        addToRoads = false;
                                    }
                                    break;
                            }

                            if (addToRoads)
                            {
                                antRoad.IndexEnd = i - 1;
                                roads.Add(antRoad);
                                antRoad = new MaxMinRoads() { IndexIni = i, Road = road };
                            }
                        }                                                
                    }

                    if (intB > maxB)
                    {
                        maxB = intB;
                    }
                    else if (intB < minB)
                    {
                        minB = intB;
                    }
                }
            }

            //List<MaxMinRoadTotal> totals = new List<MaxMinRoadTotal>();

            //foreach (MaxMinRoads roa in roads)
            //{
            //    Int32 lenth = roa.IndexEnd - roa.IndexIni;

            //    MaxMinRoadTotal total = totals.Where(x => x.Length == lenth).SingleOrDefault();

            //    if (total == null)
            //    {
            //        totals.Add(new MaxMinRoadTotal() { Length = lenth, Count = 1 });
            //    }
            //    else
            //    {
            //        total.Count++;
            //    }
            //}

            //totals = totals.OrderByDescending(x => x.Count).ToList();

            Byte byteDeControl = 0;
            if (códigosDeControl.Count > 0)
            {
                byteDeControl = Convert.ToByte(códigosDeControl.FirstOrDefault());
                //byteDeControl = Convert.ToByte(minB - 1);
            }
            else
            {
                return null;
            }

            // 
            String sComp = String.Empty;
            Int32 actualR = -1;
            Boolean estamosEnUnCamino = false;
            for (Int32 i = 0; i < bytes.Length; i++)
            {
                MaxMinRoads roadIni = roads.Count > 0 ? roads[0].IndexIni == i ? roads[0] : null : null;

                if (roadIni == null)
                {
                    if (estamosEnUnCamino)
                    {
                        MaxMinRoads roadEnd = roads.Count > 0 ? roads[0].IndexEnd == i ? roads[0] : null : null;

                        sComp += getString01FromByte(bytes[i]).Substring(actualR);

                        if (roadEnd != null)
                        {
                            estamosEnUnCamino = false;
                            roads.RemoveAt(0);
                        }

                    }
                    else
                    {
                        sComp += getString01FromByte(bytes[i]);
                    }
                }
                else
                {
                    estamosEnUnCamino = true;
                    Int32 r = roadIni.Road;
                    actualR = r;
                    Int32 l = roadIni.IndexEnd - roadIni.IndexIni;
                    sComp += getString01FromByte(byteDeControl);
                    sComp += FromBase10((r - 1).ToString(), 2).PadLeft(2, '0') + FromBase10(l.ToString(), 2).PadLeft(6, '0');
                    sComp += getString01FromByte(bytes[i]).Substring(r);
                }

                while (sComp.Length > 7)
                {
                    result.Add(getByteFromString01(sComp.Substring(0, 8)));
                    sComp = sComp.Remove(0, 8);
                }
            }

            return result.ToArray();
        }

        private static Byte[] compMaxMin03(Byte[] bytes)
        {
            List<Byte> result = new List<Byte>();

            Int32 maxB = -1;
            Int32 minB = -1;
                        
            List<Int32> códigosDeControl = new List<Int32>();
            
            Int32 bC = 2;
            for (; bC < 17; bC++) 
            {
                Boolean isFirst = true;

                if (códigosDeControl.Count > 0)
                {
                    break;
                }

                for (Int32 i = 0; i < BigInteger.Pow(2, bC); i++)
                {
                    códigosDeControl.Add(i);
                }

                for (Int32 i = 0; i < bytes.Length; i++)
                {
                    String s01 = getString01FromByte(bytes[i]);

                    while (s01.Length > bC - 1)
                    {
                        Int32 intB = Convert.ToInt32(ToBase10(s01.Substring(0, bC) ,2));
                        s01 = s01.Remove(0,bC);
                        if (códigosDeControl.Contains(intB))
                        {
                            códigosDeControl.Remove(intB);

                            if (códigosDeControl.Count == 0)
                            {
                                break;
                            }

                        }

                        if (isFirst)
                        {
                            maxB = intB;
                            minB = intB;
                            isFirst = false;
                        }
                        else
                        {
                            if (intB > maxB)
                            {
                                maxB = intB;
                            }
                            else if (intB < minB)
                            {
                                minB = intB;
                            }
                        }
                    }
                    
                }
            }


            Byte byteDeControl = 0;
            if (códigosDeControl.Count > 0)
            {
                byteDeControl = Convert.ToByte(códigosDeControl.FirstOrDefault());
                //byteDeControl = Convert.ToByte(minB - 1);
            }
            else
            {
                return null;
            }

            //// 
            //String sComp = String.Empty;
            //Int32 actualR = -1;
            //Boolean estamosEnUnCamino = false;
            //for (Int32 i = 0; i < bytes.Length; i++)
            //{
            //    MaxMinRoads roadIni = roads.Count > 0 ? roads[0].IndexIni == i ? roads[0] : null : null;

            //    if (roadIni == null)
            //    {
            //        if (estamosEnUnCamino)
            //        {
            //            MaxMinRoads roadEnd = roads.Count > 0 ? roads[0].IndexEnd == i ? roads[0] : null : null;

            //            sComp += getString01FromByte(bytes[i]).Substring(actualR);

            //            if (roadEnd != null)
            //            {
            //                estamosEnUnCamino = false;
            //                roads.RemoveAt(0);
            //            }

            //        }
            //        else
            //        {
            //            sComp += getString01FromByte(bytes[i]);
            //        }
            //    }
            //    else
            //    {
            //        estamosEnUnCamino = true;
            //        Int32 r = roadIni.Road;
            //        actualR = r;
            //        Int32 l = roadIni.IndexEnd - roadIni.IndexIni;
            //        sComp += getString01FromByte(byteDeControl);
            //        sComp += FromBase10((r - 1).ToString(), 2).PadLeft(2, '0') + FromBase10(l.ToString(), 2).PadLeft(6, '0');
            //        sComp += getString01FromByte(bytes[i]).Substring(r);
            //    }

            //    while (sComp.Length > 7)
            //    {
            //        result.Add(getByteFromString01(sComp.Substring(0, 8)));
            //        sComp = sComp.Remove(0, 8);
            //    }
            //}

            return result.ToArray();
        }


        private static Byte[] compPri(Byte[] bytes)
        {
            List<Byte> result = new List<Byte>();

            String sComp = String.Empty;

            for (Int32 i = 0; i < bytes.Length; i++)
            {
                String s01 = getString01FromByte(bytes[i]);

                String s01a = s01.Substring(0, 4);
                String s01b = s01.Substring(4, 4);

                Int32 n1sA = s01a.Where(x => x == '1').Count();
                Int32 n1sB = s01b.Where(x => x == '1').Count();

                if (n1sA % 2 == n1sB % 2 && n1sB % 2 == 0)
                {
                    sComp += "10";

                    switch (s01a)
                    {
                        case "0000":
                            sComp += "001";
                            break;
                        case "0011":
                            sComp += "000";
                            break;
                        case "0101":
                            sComp += "010";
                            break;
                        case "0110":
                            sComp += "011";
                            break;
                        case "1001":
                            sComp += "100";
                            break;
                        case "1010":
                            sComp += "101";
                            break;
                        case "1100":
                            sComp += "111";
                            break;
                        case "1111":
                            sComp += "110";
                            break;
                    }

                    switch (s01b)
                    {
                        case "0000":
                            sComp += "001";
                            break;
                        case "0011":
                            sComp += "000";
                            break;
                        case "0101":
                            sComp += "010";
                            break;
                        case "0110":
                            sComp += "011";
                            break;
                        case "1001":
                            sComp += "100";
                            break;
                        case "1010":
                            sComp += "101";
                            break;
                        case "1100":
                            sComp += "111";
                            break;
                        case "1111":
                            sComp += "110";
                            break;
                    }

                }
                else if (n1sA == n1sB)
                {
                    if(n1sA == 1)
                    {
                        sComp += "01";

                        Boolean found = false;
                        Int32 j = 0;
                        while (!found)
                        {
                            if (s01a[j] == '1')
                            {
                                found= true;
                                break;
                            }
                            j++;
                        }
                        sComp += FromBase10(j.ToString(), 2).PadLeft(2, '0');

                        found = false;
                        j = 0;
                        while (!found)
                        {
                            if (s01b[j] == '1')
                            {
                                found = true;
                                break;
                            }
                            j++;
                        }
                        sComp += FromBase10(j.ToString(), 2).PadLeft(2, '0');

                    }
                    else if(n1sA == 3)
                    {
                        sComp += "11";
                        Boolean found = false;
                        Int32 j = 0;
                        while (!found)
                        {
                            if (s01a[j] == '0')
                            {
                                found = true;
                                break;
                            }
                            j++;
                        }
                        sComp += FromBase10(j.ToString(), 2).PadLeft(2, '0');

                        found = false;
                        j = 0;
                        while (!found)
                        {
                            if (s01b[j] == '0')
                            {
                                found = true;
                                break;
                            }
                            j++;
                        }
                        sComp += FromBase10(j.ToString(), 2).PadLeft(2, '0');
                    }
                    else
                    {
                        throw new Exception("Ein?");
                    }
                }
                else
                {
                    sComp += "00" + s01;
                }

                while (sComp.Length > 7)
                {
                    result.Add(getByteFromString01(sComp.Substring(0, 8)));
                    sComp = sComp.Remove(0, 8);
                }

            }



            return result.ToArray();
        }

        private static Byte[] BigIntegerDecompose(Byte[] bytes)
        {
            List<Byte> result = new List<Byte>();

            BigInteger bI = getBIFromByteArray(bytes); 



            return result.ToArray();
        }

        private static Byte[] HuffmanPrefixs(Byte[] bytes)
        {
            //List<String> resultadosCaoticos = new List<String>();
            List<Byte> resultadoCaótico = new List<Byte>();
            //List<Int32> combinacionesProcesadas = new List<Int32>();

            String rComp = String.Empty;


            // Obtenemos las repeticiones de los prefijos
            List<apzExpCodSearch> códigosRepeticiones = new List<apzExpCodSearch>();
            List<String> repetidos = new List<String>();
            for (Int32 i = 0; i < bytes.Length; i++)
            {
                String s = getString01FromByte(bytes[i]).Substring(0,5);

                if (repetidos.Contains(s))
                {
                    apzExpCodSearch cod = códigosRepeticiones.Where(x => x.S01 == s).SingleOrDefault();
                    cod.Count++;
                }
                else
                {
                    códigosRepeticiones.Add(new apzExpCodSearch() { S01 = s, Count = 1 });
                    repetidos.Add(s);
                }
            }

            códigosRepeticiones = códigosRepeticiones.OrderBy(x => x.Count).ToList();

            //StringBuilder sB = new StringBuilder();

            //foreach (apzExpCodSearch codS in códigosRepeticiones)
            //{
            //    sB.AppendLine(codS.S01 + "    " + codS.Count.ToString().PadLeft(3, ' '));
            //}

            //String s2 = sB.ToString();

            List<apzExpCodSearch> dataNodes = new List<apzExpCodSearch>();

            // CREAR ARBOL DE HUFFMAN PARA OBTENER CÓDIGOS NO PREFIJO

            while (códigosRepeticiones.Count > 1)
            {
                apzExpCodSearch nDer = códigosRepeticiones[0];
                apzExpCodSearch nIzq = códigosRepeticiones[1];

                apzExpCodSearch newNode = new apzExpCodSearch() { Der1 = nDer, Izq0 = nIzq, Count = nDer.Count + nIzq.Count };

                nDer.Padre = newNode;
                nIzq.Padre = newNode;

                if (!String.IsNullOrEmpty(nDer.S01))
                {
                    dataNodes.Add(nDer);
                }
                if (!String.IsNullOrEmpty(nIzq.S01))
                {
                    dataNodes.Add(nIzq);
                }

                códigosRepeticiones.RemoveAt(0);
                códigosRepeticiones.RemoveAt(0);

                códigosRepeticiones.Add(newNode);

                códigosRepeticiones = códigosRepeticiones.OrderBy(x => x.Count).ToList();
            }

            Dictionary<String, String> dic = new Dictionary<String, String>();
            Int32 maxLengthPrefixCode = 0;
            foreach (apzExpCodSearch node in dataNodes)
            {
                String binaryPrefix = String.Empty;

                apzExpCodSearch padre = node.Padre;
                apzExpCodSearch nodeAnt = node;
                Boolean continuar = true;
                do
                {
                    if (padre.Der1 == nodeAnt)
                    {
                        binaryPrefix = String.Concat("1", binaryPrefix);
                    }
                    else if (padre.Izq0 == nodeAnt)
                    {
                        binaryPrefix = String.Concat("0", binaryPrefix);
                    }

                    if (padre.Padre == null)
                    {
                        continuar = false;
                    }
                    nodeAnt = padre;
                    padre = padre.Padre;
                }
                while (continuar);

                if (maxLengthPrefixCode < binaryPrefix.Length)
                {
                    maxLengthPrefixCode = binaryPrefix.Length;
                }

                dic.Add(node.S01, binaryPrefix);
            }

            // Obtenemos el código huffman de los prefijos
            String sComp = String.Empty;
            for (Int32 i = 0; i < bytes.Length; i++)
            {
                String s = getString01FromByte(bytes[i]).Substring(0, 5);

                sComp += dic[s];

                while (sComp.Length > 7)
                {
                    resultadoCaótico.Add(getByteFromString01(sComp.Substring(0, 8)));
                    sComp = sComp.Remove(0, 8);
                }
            }

            for (Int32 i = 0; i < bytes.Length; i++)
            {
                String s = getString01FromByte(bytes[i]).Substring(5, 3);

                sComp += s;

                while (sComp.Length > 7)
                {
                    resultadoCaótico.Add(getByteFromString01(sComp.Substring(0, 8)));
                    sComp = sComp.Remove(0, 8);
                }
            }

            String bytesLength01 = FromBase10(bytes.Length.ToString(), 2).PadLeft(32, '0');            
            resultadoCaótico.Insert(0, getByteFromString01(bytesLength01.Substring(24, 8)));
            resultadoCaótico.Insert(0, getByteFromString01(bytesLength01.Substring(16, 8)));
            resultadoCaótico.Insert(0, getByteFromString01(bytesLength01.Substring(8, 8)));
            resultadoCaótico.Insert(0, getByteFromString01(bytesLength01.Substring(0, 8)));


            return resultadoCaótico.ToArray();
        }


        private static Byte[] comp(Byte[] data, Boolean ori)
        {            
            List<Byte> result = new List<Byte>();

            String compFix = String.Empty;
            
            if (ori)
            {
                // Establecemos la cabecera con la primera vuelta 00000000 = 1 >
                result.Add(getByteFromString01("00000000"));
            }
            else
            {
                // Si es compresión de compresión aumentamos la ronda para llevar el cómputo
                String s01 = getString01FromByte(data[0]);
                Int32 actualRound = Convert.ToInt32(data[0]);
                if(actualRound == 255)
                {
                    return null;
                }
                actualRound++;
                result.Add(getByteFromString01(FromBase10(actualRound.ToString(), 2).PadLeft(8, '0')));

                String s02comL = getString01FromByte(data[1]);
                Int32 compFixL = Convert.ToInt32(ToBase10(s02comL.Substring(0,3), 2));
                Int32 bytesFix = Convert.ToInt32(ToBase10(s02comL.Substring(3, 5), 2));
            }


            String sComp = String.Empty;
            String sBActual = String.Empty;
            String antS = String.Empty;
            Int32 k = -1;
            Int32 fix = 0;
            for (Int32 i = 0; i < data.Length || sBActual.Length > 0; i++)
            {
                if (i < data.Length)
                {
                    sBActual += getString01FromByte(data[i]);
                }
                else
                {
                    sBActual += "00000000";
                    fix++;
                }
                                
                while(sBActual.Length > 2)
                {
                    String s = sBActual.Substring(0, 3);
                    sBActual = sBActual.Remove(0, 3);

                    if (String.IsNullOrEmpty(antS))
                    {
                        antS = s;
                    }
                    else
                    {
                        if (s == antS)
                        {
                            k++;

                            if (k == 1)
                            {
                                sComp += antS + "1" + k.ToString();
                                k = -1;
                                antS = String.Empty;
                            }

                        }
                        else
                        {
                            if (k > -1)
                            {
                                sComp += antS + "1" + k.ToString();
                            }
                            
                            sComp += antS + "0";
                            k = -1;
                            antS = s;
                        }
                    }
                }

                while (sComp.Length > 7)
                {
                    result.Add(getByteFromString01(sComp.Substring(0, 8)));
                    sComp = sComp.Remove(0, 8);
                }
            }

            if (!String.IsNullOrEmpty(antS))
            {
                if (k > -1)
                {
                    sComp += antS + "1" + k.ToString();
                }
                else
                {
                    sComp += antS + "0";
                }
            }

            while (sComp.Length > 7)
            {
                result.Add(getByteFromString01(sComp.Substring(0, 8)));
                sComp = sComp.Remove(0, 8);
            }

            if(sComp.Length > 0)
            {
                Int32 compL = sComp.Length;

                Byte compFixByte = getByteFromString01(sComp.PadRight(8, '0'));
                result.Add(compFixByte);

                String comfixInit = FromBase10(compL.ToString(), 2).PadLeft(3, '0') + FromBase10(fix.ToString(), 2).PadLeft(5, '0');
                result.Insert(1, getByteFromString01(comfixInit));
            }

            return result.ToArray();
        }

        private static String getString01FromByte(Byte dataB)
        {
            return Convert.ToString(dataB, 2).PadLeft(8, '0');
        }

        private static Byte getByteFromString01(String data01)
        {
            if(data01.Length != 8)
            {
                throw new Exception("Error de dato 8");
            }
            return Convert.ToByte(data01.Substring(0, 8), 2);
        }

        private static string FromBase10(String number, int target_base)
        {

            if (target_base < 2 || target_base > 36) return "";
            if (target_base == 10) return number.ToString();

            int n = target_base;
            BigInteger q = BigInteger.Parse(number);
            BigInteger r;
            string rtn = "";

            while (q >= n)
            {

                r = q % n;
                q = q / n;

                if (r < 10)
                    rtn = r.ToString() + rtn;
                else
                    rtn = Convert.ToChar((Int32)(r + 55)).ToString() + rtn;

            }

            if (q < 10)
                rtn = q.ToString() + rtn;
            else
                rtn = Convert.ToChar((Int32)(q + 55)).ToString() + rtn;

            return rtn;
        }

        private static String ToBase10(String number, int start_base)
        {
            if (start_base < 2 || start_base > 36) return "";
            if (start_base == 10) return number;

            char[] chrs = number.ToCharArray();
            int m = chrs.Length - 1;
            BigInteger n = start_base;
            BigInteger x;
            BigInteger rtn = 0;

            foreach (char c in chrs)
            {

                if (char.IsNumber(c))
                    x = int.Parse(c.ToString());
                else
                    x = Convert.ToInt32(c) - 55;

                rtn += x * BigInteger.Pow(n, m);

                m--;

            }

            return rtn.ToString();

        }


        private static BigInteger getBIFromByteArray(Byte[] bytes)
        {
            BigInteger result = 0;

            Int32 i = 0;
            foreach(Byte b in bytes.Reverse())
            {
                String s01 = getString01FromByte(b);

                foreach (Char c in s01.Reverse())
                {
                    if(c == '1')
                    {
                        result += BigInteger.Pow(2, i);
                    }
                    i++;
                }

            }

            result += BigInteger.Pow(2, i);

            return result;
        }

        private static String getString01FromByteArray(Byte[] bytes)
        {
            String[] stringArray = new String[bytes.Length];

            //List<Task> tasks = new List<Task>();
            for (Int32 i = 0; i < bytes.Length; i++)
            {
                Int32 j = i;

                //tasks.Add(Task.Run(() =>
                //{
                    String s = Convert.ToString(bytes[j], 2).PadLeft(8, '0');
                    stringArray[j] = s;
                //}));
            }

            //await Task.WhenAll(tasks);

            return String.Concat(stringArray);
        }

    }
}
