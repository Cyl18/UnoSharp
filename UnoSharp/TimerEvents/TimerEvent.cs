using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnoSharp.TimerEvents
{
    public class TimerEvent
    {
        public Action Action { get; }
        public int Seconds { get; internal set; }
        public int Step { get; }

        public TimerEvent(Action action, int seconds, int step)
        {
            Action = action;
            Seconds = seconds;
            Step = step;
        }
    }
}
