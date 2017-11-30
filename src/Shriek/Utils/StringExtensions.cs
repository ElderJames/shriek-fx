using System;
using System.Text;

namespace Shriek.Utils
{
    /// <summary>
    /// 字符串通用功能
    /// </summary>
    public static class StringExtensions
    {
        private static readonly Random _rnd = new Random();

        private static readonly char[] _arrChar =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            'a', 'b', 'd', 'c', 'e', 'f', 'g', 'h', 'i', 'j',
            'k', 'l', 'm', 'n', 'p', 'r', 'q', 's', 't', 'u',
            'v', 'w', 'z', 'y', 'x',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J',
            'K', 'L', 'M', 'N', 'Q', 'P', 'R', 'T', 'S', 'V',
            'U', 'W', 'X', 'Y', 'Z'
        };

        /// <summary>
        /// 生成随机串
        /// </summary>
        /// <returns></returns>
        public static string RandomStr(int length = 8)
        {
            var num = new StringBuilder(length);
            for (var i = 0; i < length; i++)
            {
                num.Append(_arrChar[_rnd.Next(0, 59)]);
            }
            return num.ToString();
        }

        /// <summary>
        /// 随机数字
        /// </summary>
        /// <returns></returns>
        public static string RandomNum(int length = 4)
        {
            var num = new StringBuilder(length);
            for (var i = 0; i < length; i++)
            {
                num.Append(_rnd.Next(0, 9));
            }
            return num.ToString();
        }

        // 排除【0，O】I 4 这类
        private const string _arrCodeStr = "12356789ABCDEFGHJKLMNPQRSTUVWXYZ";

        /// <summary>
        /// 数字转化为短码
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string ToCode(this long num)
        {
            const long codeTemp = 0x1F;
            var code = new StringBuilder(13);

            while (num > 0)
            {
                var index = num & codeTemp;
                code.Append(_arrCodeStr[(int)index]);

                num >>= 5;
            }
            return code.ToString();
        }

        /// <summary>
        /// 根据短码反推数字
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static long ToCodeNum(this string code)
        {
            if (string.IsNullOrEmpty(code))
                return 0;
            var count = code.Length;
            if (count > 13)
                throw new ArgumentOutOfRangeException("code", "the code is not from [ToCode] method !");

            long value = 0;
            for (var i = count - 1; i >= 0; i--)
            {
                var num = _arrCodeStr.IndexOf(code[i]);
                if (num < 0)
                    throw new ArgumentOutOfRangeException("code", "the code is not from [ToCode] method !");

                value <<= 5;
                if (i == 12)
                    value = value ^ (num & 0x0F); // 最高位只有四位
                else
                    value = value ^ num;
            }
            return value;
        }
    }
}