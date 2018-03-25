using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UnoSharp
{
    public static class CardFactory
    {
        public static void FastCreateCards(this Player player, int count)
        {
            var cards = new ConcurrentBag<Card>(player.Cards);
            ThreadPool.SetMinThreads(16, 16);
            ThreadPool.SetMaxThreads(64, 64);
            Parallel.ForEach(GenCards(count), new ParallelOptions { MaxDegreeOfParallelism = 64 }, card =>
            {
                cards.Add(card);
            });
            player.Cards = new List<Card>(cards);
            player.SendCardsMessage();
        }

        private static IEnumerable<Card> GenCards(int num)
        {
            for (var i = 0; i < num; i++)
            {
                yield return Card.CardsPool.PickOne();
            }
        }
    }
}