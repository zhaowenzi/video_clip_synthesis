using System;
using System.Collections.Generic;
using System.Text;

namespace 素材合成.Helper
{
    public class CmdHelper
    {
        public static byte[] StringToByte(string byteData)
        {
            string source = byteData.Replace(" ", "");
            List<byte> temp = new List<byte>();
            for (int i = 0; i < source.Length; i += 2)
            {
                byte[] tempByte = BitConverter.GetBytes(To16_10_ff(source.Substring(i, 2)));
                temp.Add(tempByte[0]);
            }
            return temp.ToArray();
        }
        public static string ByteToString(byte[] array)
        {
            string str = "";
            for (int i = 0; i < array.Length; i++)
            {
                str += To10_16_Length_2(array[i]);
            }
            return str.ToUpper();
        }
        public static string ASCStringToString(byte[] array)
        {
            string str = "";
            str =  Encoding.ASCII.GetString(array);
            return str.ToUpper();
        }

        public static string StringToBit(string byteData)
        {
            string resultBit = "";
            byteData = byteData.Replace(" ", "");
            for (int i = 0; i < byteData.Length; i += 2)
            {
                byte[] tempByte = BitConverter.GetBytes(To16_10_ff(byteData.Substring(i, 2)));
                string tempBit = Convert.ToString(BitConverter.ToInt16(tempByte, 0), 2);
                while (tempBit.Length < 8)
                {
                    tempBit = "0" + tempBit;
                }
                resultBit += tempBit;
            }
            return resultBit;
        }
        public static string ByteToBit(byte[] byteData)
        {
            string resultBit = "";
            for (int i = 0; i < byteData.Length; i++)
            {
                string tempBit = Convert.ToString(byteData[i], 2);
                while (tempBit.Length < 8)
                {
                    tempBit = "0" + tempBit;
                }
                resultBit += tempBit;
            }
            return resultBit;
        }
        public static string BitTo16(string bit)
        {
            string result = "";
            for (int i = 0; i < bit.Length; i += 8)
            {
                string tempBit = bit.Substring(i, 8);
                int num = Convert.ToInt32(tempBit, 2);
                result += To10_16_Length_2(num);
            }

            return result.ToUpper();
        }

        public static string To10_16_Length_4(int num)
        {
            string result = Convert.ToString(num, 16);
            while (result.Length < 4)
            {
                result = "0" + result;
            }
            return result;
        }

        public static string To10_16_Length_2(int num)
        {
            string result = Convert.ToString(num, 16);
            while (result.Length < 2)
            {
                result = "0" + result;
            }
            return result.Substring(result.Length - 2, 2);
        }
        public static string To10_16_Length_1(int num)
        {
            string result = Convert.ToString(num, 16);
            return result;
        }
        public static short To16_10_ff(string num)
        {
            if (num == "00" || num == "0")
            {
                return 0;
            }
            return Convert.ToInt16(num, 16);
        }
        public static int To16_10_ffff(string num)
        {
            if (num == "00" || num == "0")
            {
                return 0;
            }
            return Convert.ToInt32(num, 16);
        }
        public static long To16_10_ffffff(string num)
        {
            if (num == "00" || num == "0")
            {
                return 0;
            }
            return Convert.ToInt64(num, 16);
        }


    }
}
