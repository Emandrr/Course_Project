using Course_Project.Domain.Models.CustomIdModels;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_Project.Application.Utils
{
    public static class Generator
    {
        private static readonly SecureRandom _secureRandom = new SecureRandom();
        private static readonly CryptoApiRandomGenerator _randomGenerator = new CryptoApiRandomGenerator();
        private static string defaultRule = "no zeros";
        private static int bitmask = 0xF0;
        private static uint firstPartOf6Digit = 100000;
        private static uint secondPartOf6Digit = 900000;
        private static ulong firstPartOf9Digit = 100000000;
        private static ulong secondPartOf9Digit = 900000000;
        public static List<string> GenerateExample(List<CustomIdRule> CustomSetOfIds)
        {
            List<string> answer = new List<string>();
            for (int i = 0;i<CustomSetOfIds.Count();++i)
            {
                answer.Add(Generate(CustomSetOfIds[i]));
            }
            return answer;
        }
        public static string Generate(CustomIdRule idType) => idType.IdType switch
        {
            IdType.Text => GenerateText(idType.Rule),
            IdType.Rand20Bit => GenerateRand20Bit(idType.Rule),
            IdType.Rand32Bit => GenerateRand32Bit(idType.Rule),
            IdType.Rand6Digit => GenerateRand6Digit(idType.Rule),
            IdType.Rand9Digit => GenerateRand9Digit(idType.Rule),
            IdType.GUID => GenerateGUID().ToString(),
            IdType.DateTime => GenerateDateTime(idType.Rule),
            IdType.Sequence => GenerateRand20Bit(idType.Rule),
            _ => throw new ArgumentException($"Unknown IdType: {idType}")
        };
        public static string GenerateRand20Bit(string rule)
        {
            byte[] bytes = new byte[3];
            _randomGenerator.NextBytes(bytes);
            bytes[2] = (byte)(bytes[2] & bitmask); // bitmask of last 4 bytes 
            string res = BitConverter.ToString(bytes).Replace("-", "").Substring(0, 5);
            return (rule=="zeros"?res.PadLeft(5,'0'):res) + "-";
        }
        public static string GenerateRand32Bit(string rule)
        {
            byte[] bytes = new byte[4];
            _randomGenerator.NextBytes(bytes);
            string res = BitConverter.ToString(bytes).Replace("-", "");
            return (rule == "zeros" ? res.PadLeft(8, '0') : res) + "-";
        }
        public static string GenerateRand6Digit(string rule)
        {
            byte[] bytes = new byte[4];
            _randomGenerator.NextBytes(bytes);
            uint randomNumber = BitConverter.ToUInt32(bytes, 0);
            uint res = firstPartOf6Digit + (randomNumber % secondPartOf6Digit);
            return (rule == "zeros"? res.ToString("D6") : res.ToString()) + "-";

        }
        public static string GenerateRand9Digit(string rule)
        {
            byte[] bytes = new byte[8];
            _randomGenerator.NextBytes(bytes);
            ulong randomNumber = BitConverter.ToUInt64(bytes, 0);
            ulong res = firstPartOf9Digit + (randomNumber % secondPartOf9Digit);
            return (rule == "zeros" ? res.ToString("D9") : res.ToString())+"-";
        }
        public static string GenerateGUID()
        {
            return Guid.NewGuid().ToString();
        }
        public static string GenerateDateTime(string rule)
        {
            if (rule == defaultRule) return DateTime.UtcNow.ToString("yyyy")+"-";
            else return DateTime.Now.ToString(rule)+"_";
        }
        public static string GenerateText(string rule)
        {
            var result = new StringBuilder(10);
            for (int i = 0; i < 5; i++)
            {
                result.Append((char)_secureRandom.Next(0x0020, 0x007F + 1));
            }
            return rule == defaultRule? result.ToString()+"-": result.ToString() + rule;

        }
    }
}
