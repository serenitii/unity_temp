using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KPoker
{
    public enum PokerRank
    {
        _2 = 2,
        _3 = 3,
        _4 = 4,
        _5 = 5,
        _6 = 6,
        _7 = 7,
        _8 = 8,
        _9 = 9,
        _10 = 10,
        _J = 11,
        _Q = 12,
        _K = 13,
        _A = 14
    };

    public static class KPokerTools
    {
        public static int CardNumber(int number, Suit suit)
        {
            int stride = (int) suit - 1;

            return stride * 20 + number;
        }
    }
}