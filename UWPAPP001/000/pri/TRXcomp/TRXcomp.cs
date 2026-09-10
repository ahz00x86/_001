using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRXcomp
{
    public class TRXf()
    {
        public Int64 Offset { get; set; }

        public Int64 Step { get; set; }

        public Boolean Value { get; set; }

    }


    public class TRXc
    {
        public void trxComp(String fPath)
        {
            Byte[] fBytes = File.ReadAllBytes(fPath);
            LargeBoolArray fBools = BytesToLargeBoolArray(fBytes);

            Int64 fInitBools = fBools.LongCount();
            
            //double sqWH = Math.Sqrt(fBools.Length);

            //for (Int32 w = Convert.ToInt32(sqWH); w > 7; w--)
            //{
            //    Boolean[,] TRX = getMatrix(fBools, w, fBools.Length);
            //}
            List<TRXf> fList = new List<TRXf>();
            Boolean reel = false;
            Boolean nnn = true;
            while (fBools.LongCount() > fInitBools / 10)
            {
                reel = false;
                for (Int64 step = 2; step < fBools.LongCount() / 10; step++)
                {
                    for (Int64 offset = 0; offset < step; offset++)
                    {
                        Boolean notPatt = false;
                        Boolean antV = false;
                        Boolean first = true;
                        //for (Int64 sW = offset; sW < fBools.LongCount(); sW += step)
                        Int64 iOffs = 0;
                        Int64 iterStep = 0;

                        LargeBoolArray boolsCompT = new LargeBoolArray();

                        for(Int64 i = 0; i < fBools.LongCount() ; i++)
                        {
                            if (iOffs < offset)
                            {
                                boolsCompT.Add(fBools.Get(i));
                                iOffs++;
                                continue;
                            }
                            
                            if (first)
                            {
                                first = false;
                                antV = fBools.Get(i);
                                continue;
                            }
                                                        
                            if(iterStep < step)
                            {
                                boolsCompT.Add(fBools.Get(i));
                                iterStep++;
                                continue;
                            }

                            if (antV != fBools.Get(i))
                            {
                                notPatt = true;
                                break;
                            }
                        }

                        if (!notPatt)
                        {
                            nnn = false;

                            //List<Boolean> boolsComp = new List<Boolean>();

                            ////offset
                            //for (Int64 i = 0; i < offset; i++)
                            //{
                            //    boolsComp.Add(fBools[i]);
                            //}

                            ////steps
                            //Int32 flag = 1;
                            //for (Int64 i = offset + 1; i < fBools.LongCount(); i++)
                            //{
                            //    if (flag < step)
                            //    {
                            //        boolsComp.Add(fBools[i]);
                            //        flag++;
                            //    }
                            //    else
                            //    {
                            //        flag = 1;
                            //    }
                            //}

                            fList.Add(new TRXf() { Offset = offset, Step = step, Value = antV });

                            fBools = boolsCompT;
                            reel = true;
                        }

                        if (reel)
                        {
                            break;
                        }
                    }

                    if (reel)
                    {
                        break;
                    }
                }

                if(nnn)
                {
                    break;
                }
            }

            Console.WriteLine("END");
            Console.ReadKey();
        }

        private bool[,] getMatrix(Boolean[] data, Int32 with, Int32 total)
        {
            Boolean[,] trx = new Boolean[with, total / with];

            Int32 n = 0;
            for(Int32 w = 0; w < data.Length; w += with)
            {
                Boolean[] wBools = new Boolean[with];

                for (Int32 s = 0; s < with; s++)
                {
                    wBools[s] = data[w + s];
                }

                trx.SetValue(wBools, n);
                n++;

                if(n > total / with)
                {
                    break;
                }

            }

            return trx;
            
        }

        public static bool[] BytesToBools(byte[] byteArray)
        {
            bool[] boolArray = new bool[byteArray.Length * 8];
            int index = 0;
            foreach (byte b in byteArray)
            {
                for (int i = 7; i >= 0; i--)
                {
                    boolArray[index++] = (b & (1 << i)) != 0;
                }
            }
            return boolArray;
        }

        public static List<bool> BytesToBoolsL(byte[] byteArray)
        {
            List<bool> boolList = new List<bool>();
            foreach (byte b in byteArray)
            {
                for (int i = 7; i >= 0; i--)
                {
                    boolList.Add((b & (1 << i)) != 0);
                }
            }
            return boolList;
        }

        public static BitArray BytesToBitArray(byte[] byteArray)
        {
            BitArray bitArray = new BitArray(byteArray.Length * 8);
            int index = 0;
            foreach (byte b in byteArray)
            {
                for (int i = 7; i >= 0; i--)
                {
                    bitArray.Set(index++, (b & (1 << i)) != 0);
                }
            }
            return bitArray;
        }

        public static LargeBoolArray BytesToLargeBoolArray(byte[] byteArray)
        {
            LargeBoolArray boolArray = new LargeBoolArray();
            foreach (byte b in byteArray)
            {
                for (int i = 7; i >= 0; i--)
                {
                    boolArray.Add((b & (1 << i)) != 0);
                }
            }
            return boolArray;
        }
    }

    
}

public class LargeBoolArray
{
    private const int MaxListSize = int.MaxValue / 8; // Dividimos para evitar el límite de int32
    private List<List<bool>> segments = new List<List<bool>>();

    public LargeBoolArray()
    {
        segments.Add(new List<bool>());
    }

    public void Add(bool value)
    {
        List<bool> lastSegment = segments[segments.Count - 1];
        if (lastSegment.Count >= MaxListSize)
        {
            lastSegment = new List<bool>();
            segments.Add(lastSegment);
        }
        lastSegment.Add(value);
    }

    public bool Get(long index)
    {
        int segmentIndex = (int)(index / MaxListSize);
        int indexInSegment = (int)(index % MaxListSize);
        return segments[segmentIndex][indexInSegment];
    }

    public long LongCount()
    {
        long count = 0;
        foreach (var segment in segments)
        {
            count += segment.Count;
        }
        return count;
    }
}
