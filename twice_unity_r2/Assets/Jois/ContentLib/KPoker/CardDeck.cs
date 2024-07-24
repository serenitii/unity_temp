using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace KPoker
{
    public class CardDeck
    {
        IList<int> _cards;
        int _position;

        public CardDeck()
        {
            _cards = new List<int>(80);

            for (int s = 0; s < 4; ++s) {
                for (int i = 2; i < 2 + 13; ++i) {
                    int num = s * 20 + i;
                    _cards.Add(num);
                }
            }
            
            Debug.LogFormat("deck: {0}\n", string.Join(",", _cards));
        }

        public void Shuffle()
        {
            _position = 0;
            ShuffleArray(_cards);
            Debug.LogFormat("shuffled deck: {0}\n", string.Join(",", _cards));
        }
        
        public static void ShuffleArray<T>(IList<T> list)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                int random1 = UnityEngine.Random.Range(0, list.Count);
                int random2 = UnityEngine.Random.Range(0, list.Count);

                T tmp = list[random1];
                list[random1] = list[random2];
                list[random2] = tmp;
            }
        }

        public int DealCard()
        {
            Debug.Assert(_position < 52 - 1);
            return _cards[_position++];
        }

        public void SetTan(IList<int> tan)
        {
            _cards = tan;
        }
    }
}