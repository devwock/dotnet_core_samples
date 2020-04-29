using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkUtil
{
    public class StringUtil
    {
        /// <summary>
        /// 특정 숫자 간격마다 특정한 문자를 넣음.
        /// http://stackoverflow.com/a/4133475/2596404
        /// </summary>
        /// <param name="targetString">원문</param>
        /// <param name="separator">넣을 문자</param>
        /// <param name="partLength">문자를 넣을 숫자 간격</param>
        /// <returns></returns>
        public static string SplitInParts(string targetString, string separator, int partLength)
        {
            if (targetString == null)
            {
                throw new ArgumentException("String is null.");
            }
            if (partLength <= 0)
            {
                throw new ArgumentException("Part length has to be positive.", "partLength");
            }

            return string.Join(separator, SplitString(targetString, partLength));
        }

        public static IEnumerable<string> SplitString(string targetString, int partLength)
        {
            for (int i = 0; i < targetString.Length; i += partLength)
            {
                yield return targetString.Substring(i, Math.Min(partLength, targetString.Length - i));
            }
        }

        public static string ByteToString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("X2").ToUpper());
                if (i != bytes.Length - 1)
                {
                    sb.Append(":");
                }
            }
            return sb.ToString();
        }
    }
}