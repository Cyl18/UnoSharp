using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnoSharp.GameComponent;
using UnoSharp.GameStep;
using UnoSharp.TimerEvents;
using Timer = System.Timers.Timer;

namespace UnoSharp
{
    public class Desk : MessageSenderBase
    {
        private Dictionary<string, Player> _playersDictionary = new Dictionary<string, Player>();
        private readonly Timer _timer = new Timer(1000);
        public List<TimerEvent> Events { get; } = new List<TimerEvent>();

        public Card LastCard
        {
            get => _lastCard;
            set
            {
                if (value.Color == CardColor.Special)
                    return;

                _lastCard = value;
            }
        } //TODO SET

        public Card LastNonDrawFourCard
        {
            get => _lastNonDrawFourCard;
            internal set
            {
                if (value.Color == CardColor.Special)
                    return;

                _lastNonDrawFourCard = value;
            }
        }

        internal GameStepBase CurrentParser;
        public GamingState State { get; internal set; }

        public Desk(string deskId)
        {
            DeskId = deskId;
            CurrentParser = new WaitingParser();
            _timer.Elapsed += Elapsed;
            _timer.Start();
        }

        private void Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var removes = new List<TimerEvent>();
            foreach (var timerEvent in Events)
            {
                timerEvent.Seconds--;
                if (timerEvent.Seconds == 0 && (timerEvent.Step == Step || timerEvent.Step == -1))
                {
                    removes.Add(timerEvent);
                    timerEvent.Action?.Invoke();
                }
            }

            foreach (var timerEvent in removes)
            {
                Events.Remove(timerEvent);
            }
        }

        private static readonly Dictionary<string, Desk> Desks = new Dictionary<string, Desk>();

        public IEnumerable<Player> Players => _playersDictionary.Values;
        public List<Player> PlayerList => Players.ToList();
        public string DeskId { get; }
        public Player CurrentPlayer => PlayerList[CurrentParser.CurrentIndex];
        public int OverlayCardNum { get; set; }
        public bool Reversed => CurrentParser.Reversed;
        public Player LastSendPlayer { get; internal set; }
        private readonly ICommandParser _standardParser = new StandardParser();
        private Card _lastNonDrawFourCard;
        private Card _lastCard;
        public int Step { get; internal set; }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool AddPlayer(Player player)
        {
            if (Players.Contains(player))
            {
                AddMessage($"已经加入: {player.ToAtCode()}");
                return false;
            }

            _playersDictionary.Add(player.PlayerId, player);
            AddMessageLine($"加入成功: {player.ToAtCode()}");
            AddMessage($"UNO当前玩家有: {string.Join(", ", Players.Select(p => p.ToAtCode()))}");
            return true;
        }

        public void RemovePlayer(Player player)
        {
            AddMessageLine($"移除成功: {player.ToAtCode()}");
            _playersDictionary.Remove(player.PlayerId);
            AddMessage($"UNO当前玩家有: {string.Join(", ", Players.Select(p => p.ToAtCode()))}");
        }

        public Player GetPlayer(string playerid)
        {
            return _playersDictionary.ContainsKey(playerid) ? _playersDictionary[playerid] : new Player(playerid, this);
        }

        public void RandomizePlayers()
        {
            var players = new List<Player>(PlayerList);
            players.Shuffle();
            _playersDictionary = players.ToDictionary(player => player.PlayerId);
        }

        public void StartGame()
        {
            if (Players.Count() < 2)
            {
                AddMessage("喂伙计, 玩家人数不够!");
                return;
            }
            RandomizePlayers();
            LastCard = Card.Generate(card => card.Color != CardColor.Special && card.Color != CardColor.Wild);
            CurrentParser = new GamingParser();

            for (var index = 0; index < PlayerList.Count; index++)
            {
                var player = PlayerList[index];
                player.AddCardsAndSort(7);
                player.Index = index;
                AddMessageLine(player.AtCode);
            }

            this.SendLastCardMessage();
            if (CurrentPlayer is FakePlayer)
            {
                Events.Add(new TimerEvent(() => { Samsara.DoAutoSubmitCard(this); }, 2, Step));
            }
        }

        public static Desk GetOrCreateDesk(string deskid)
        {
            if (Desks.ContainsKey(deskid))
                return Desks[deskid];

            var desk = new Desk(deskid);
            Desks.Add(deskid, desk);
            return desk;
        }

        public static List<Desk> GetDesks()
        {
            return Desks.Values.ToList();
        }

        public void FinishGame(Player player)
        {
            AddMessageLine($"{CurrentPlayer.AtCode}赢了!");
            PlayerList.ForEach(p => p.PublicCard = true);
            AddMessage(this.RenderDesk().ToImageCodeAndDispose());
            Task.Run(() =>
            {
                Thread.Sleep(500);
                Desks.Remove(this.DeskId);
            });
        }

        public void SendLastCardMessage()
        {
            if (Message?.EndsWith("出牌.") != true)
            {
                AddMessageLine($"{this.RenderDesk().ToImageCodeAndDispose()}");
                AddMessage($"请{CurrentPlayer.AtCode}出牌.");
                if (CurrentPlayer.AutoSubmitCard)
                {
                    Events.Add(new TimerEvent(() => { Samsara.DoAutoSubmitCard(this); }, 5, Step));
                }
                Events.Add(new TimerEvent(() => { AddMessage($"{CurrentPlayer.AtCode}你只剩15s时间出牌啦!"); }, 25, Step));
                Events.Add(new TimerEvent(() =>
                {
                    CurrentPlayer.AutoSubmitCard = true;
                    AddMessage($"{CurrentPlayer.AtCode}出牌超时");
                    Samsara.DoAutoSubmitCard(this);
                }, 40, Step));
            }
        }

        public readonly static object Locker = new object();

        public void ParseMessage(string playerid, string message)
        {
            lock (Locker)
            {
                try
                {
                    var player = GetPlayer(playerid);
                    CurrentParser.Parse(this, player, message);
                    _standardParser.Parse(this, player, message);
                }
                catch (Exception e)
                {
                    AddMessage($"抱歉我们在处理你的命令时发生了错误{e}");
                }
            }
        }

        //√BUG  after doubt boardcast cards
        //√BUG  doubt crash
        //√TODO public card anyone can use
        //√TODO last card
        //√TODO start who is p
        //√TODO end public card
        //√TODO current player
        //√TODO public card notify
        //√TODO auto submit card
        //√TODO config and set nick
        //√TODO special card
        //√TODO bot name
        //√TODO Java10
        //√TODO time limit
        //√TODO draw until success
        //√TODO after draw
        //√TODO follow
        //NO TODO continue game
        public void FinishDraw(Player player)
        {
            if (State == GamingState.WaitingDrawFourOverlay || State == GamingState.WaitingDrawTwoOverlay || State == GamingState.Doubting)
            {
                State = GamingState.Gaming;
                AddMessage($"{player.AtCode}被加牌{OverlayCardNum}张.");

                player.AddCardsAndSort(OverlayCardNum);
                OverlayCardNum = 0;
                CurrentParser.MoveNext(this);
                SendLastCardMessage();
            }
        }
    }

    public enum GamingState
    {
        Gaming,
        WaitingDrawTwoOverlay,
        WaitingDrawFourOverlay,
        Doubting
    }
}