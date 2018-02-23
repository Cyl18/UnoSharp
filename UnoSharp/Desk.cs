using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnoSharp.GameComponent;
using UnoSharp.GameStep;
using Timer = System.Timers.Timer;

namespace UnoSharp
{
    public class Desk : MessageSenderBase
    {
        private Dictionary<string, Player> _playersDictionary = new Dictionary<string, Player>();
        private readonly Timer _timer = new Timer(10*1000);
        
        public Card LastCard { get; set; } //TODO SET
        private GameStepBase _currentParser;
        public GamingState State { get; internal set; }

        public Desk(string deskId)
        {
            DeskId = deskId;
            _currentParser = new WaitingParser();
        }
        private static readonly Dictionary<string, Desk> Desks = new Dictionary<string, Desk>();

        public IEnumerable<Player> Players => _playersDictionary.Values;
        public List<Player> PlayerList => Players.ToList();
        public string DeskId { get; }
        public Player CurrentPlayer => PlayerList[_currentParser.CurrentIndex];
        public int OverlayCardNum { get; set; }
        public bool Reversed => _currentParser.Reversed;
        public Player LastSendPlayer { get; internal set; }
        public Card LastNonDrawFourCard { get; internal set; }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool AddPlayer(Player player)
        {
            if (Players.Contains(player)) {
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
            if (Players.Count() < 2) {
                AddMessage("喂伙计, 玩家人数不够!");
                return;
            }
            RandomizePlayers();
            LastCard = Card.Generate();
            _currentParser = new GamingParser();

            for (var index = 0; index < PlayerList.Count; index++)
            {
                var player = PlayerList[index];
                player.AddCardsAndSort(7);
                player.Index = index;
            }

            this.SendLastCardMessage();
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
            AddMessage($"{CurrentPlayer.AtCode}赢了!");
            Task.Run(() =>
            {
                Thread.Sleep(500);
                Desk.Desks.Remove(this.DeskId); 
            });
        }

        public void SendLastCardMessage()
        {
            AddMessageLine($"{this.RenderDesk().ToImageCode()}");
            AddMessage($"请{CurrentPlayer.AtCode}出牌.");
        }

        public void ParseMessage(string playerid, string message)
        {
            try
            {
                var player = GetPlayer(playerid);
                _currentParser.Parse(this, player, message);
            }
            catch (Exception e)
            {
                AddMessage($"抱歉我们在处理你的命令时发生了错误{e}");
            }
            
        }

        public void FinishDraw(Player player)
        {
            if (State  == GamingState.WaitingDrawFourOverlay || State == GamingState.WaitingDrawTwoOverlay || State == GamingState.Doubting)
            {
                State = GamingState.Gaming;
                AddMessage($"{player.AtCode}被加牌{OverlayCardNum}张.");

                player.AddCardsAndSort(OverlayCardNum);
                OverlayCardNum = 0;
                _currentParser.MoveNext(this);
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
