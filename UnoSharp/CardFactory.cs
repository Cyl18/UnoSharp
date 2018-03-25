using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
            Parallel.For((long)0, count, new ParallelOptions { MaxDegreeOfParallelism = 64 }, (i) =>
            {
                cards.Add(GenCards());
            });
            player.Cards = cards.ToList();
            player.SendCardsMessage();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Card GenCards()
        {
            return Card.CardsPool.PickOne();
        }
    }
}