using System.Collections.Generic;
using UnityEngine;

namespace KPoker
{
    public static class KPokerStrUtils
    {
        public static readonly string[] units = {"", "만", "억", "조", "경"};
        public static readonly string[] units_R = {"경", "조", "억", "만", ""};
        public const string kZeroString = "0";

        public static string ToMoneyString(long value)
        {
            if (value == 0)
                return kZeroString;

            long curr_val = value < 0 ? System.Math.Abs(value) : value;

            long unit_5 = curr_val / 10000000000000000; // 경
            curr_val -= unit_5 * 10000000000000000;

            long unit_4 = curr_val / 1000000000000; // 조
            curr_val -= unit_4 * 1000000000000;

            long unit_3 = curr_val / 100000000; // 억
            curr_val -= unit_3 * 100000000;

            long unit_2 = curr_val / 10000; // 만
            curr_val -= unit_2 * 10000;

            long unit_1 = curr_val; // 일

            string str_unit_1 = string.Empty;
            string str_unit_2 = string.Empty;
            string str_unit_3 = string.Empty;
            string str_unit_4 = string.Empty;

            if (unit_4 > 0)
                str_unit_4 = unit_4 + "조";
            if (unit_3 > 0)
                str_unit_3 = unit_3 + "억";
            if (unit_2 > 0)
                str_unit_2 = unit_2 + "만";
            if (unit_1 > 0)
                str_unit_1 = unit_1.ToString();

            string result = str_unit_4 + str_unit_3 + str_unit_2 + str_unit_1;
            if (value < 0)
                return "-" + result;
            else
                return result;
        }

        public static void TestMoneyString()
        {
            Debug.LogFormat("test {0} \n", KPokerStrUtils.ToMoneyString(91000200031000));
            Debug.LogFormat("test {0} \n", KPokerStrUtils.ToMoneyString(200031000));
            Debug.LogFormat("test {0} \n", KPokerStrUtils.ToMoneyString(9121000));
            Debug.LogFormat("test {0} \n", KPokerStrUtils.ToMoneyString(21000));
            Debug.LogFormat("test {0} \n", KPokerStrUtils.ToMoneyString(5));
        }

        static Dictionary<CardStrength, string> _handMap = new Dictionary<CardStrength, string>
        {
            {CardStrength.Unknown, ""},
            {CardStrength.HighCard, "탑"},
            {CardStrength.OnePair, "원페어"},
            {CardStrength.TwoPair, "투페어"},
            {CardStrength.Triple, "트리플"},
            {CardStrength.Straight, "스트레이트"},
            {CardStrength.BackStraight, "백스트레이트"},
            {CardStrength.Mountain, "마운틴"},
            {CardStrength.Flush, "플러시"},
            {CardStrength.FullHouse, "풀하우스"},
            {CardStrength.FourCard, "포카드"},
            {CardStrength.StraightFlush, "스트레이트플러시"},
            {CardStrength.BackStraightFlush, "백스트레이트플러시"},
            {CardStrength.RoyalFlush, "로얄플러시"}
        };

        public static string ToString(CardStrength hand)
        {
            /*
             * public enum CardStrength : byte
    {
     Unknown = 0,
     HighCard = 1,
     OnePair = 2,
     TwoPair = 3,
     Triple = 4,
     Straight = 5,
     BackStraight = 6,
     Mountain = 7,
     Flush = 8,
     FullHouse = 9,
     FourCard = 10,
     StraightFlush = 11,
     BackStraightFlush = 12,
     RoyalFlush = 13,
    };
             */

            return "";
        }
    }
}