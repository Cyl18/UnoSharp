using Newbe.Mahua.MahuaEvents;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Newbe.Mahua;
using UnoSharp;

namespace Origind.Card.Uno.MahuaEvents
{
    /// <summary>
    /// 群消息接收事件
    /// </summary>
    public class GroupMessageReceivedMahuaEvent1
        : IGroupMessageReceivedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;
        private readonly TimerEvent _timerEvent;

        public GroupMessageReceivedMahuaEvent1(
            IMahuaApi mahuaApi, TimerEvent timerEvent)
        {
            _mahuaApi = mahuaApi;
            _timerEvent = timerEvent;
            _timerEvent.Start();
        }

        private static readonly object _locker = new object();

        public void ProcessGroupMessage(GroupMessageReceivedContext context)
        {
            lock (_locker)
            {
                var deskid = context.FromGroup;
                var playerid = context.FromQq;
                var message = context.Message;

                var desk = Desk.GetOrCreateDesk(deskid);
                desk.ParseMessage(playerid, message);
            }

            // 不要忘记在MahuaModule中注册
        }
    }
}