using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Timer
{
    public class PCLTimer
    {
        private System.Threading.Timer _timer;

        private Action _action;

        public PCLTimer(Action action, TimeSpan dueTime, TimeSpan period)
        {
            _action = action;

            _timer = new System.Threading.Timer(PCLTimerCallback, null, dueTime, period);
        }

        private void PCLTimerCallback(object state)
        {
            _action.Invoke();
        }

        public bool Change(TimeSpan dueTime, TimeSpan period)
        {
            return _timer.Change(dueTime, period);
        }
    }
    //public class PCLTimer
    //{
    //    private System.Threading.Timer _timer;

    //    private Action _action;
    //    private TimeSpan _dueTime;
    //    private TimeSpan _period;

    //    public PCLTimer(Action action, TimeSpan dueTime, TimeSpan period)
    //    {
    //        _action = action;
    //        _dueTime = dueTime;
    //        _period = period;
    //    }

    //    private void PCLTimerCallback(object state)
    //    {
    //        _action.Invoke();
    //    }

    //    public bool Change(TimeSpan dueTime, TimeSpan period)
    //    {
    //        return _timer.Change(dueTime, period);
    //    }

    //    public void StopTimer()
    //    {
    //        _timer.Dispose();
    //    }

    //    public void StartTimer()
    //    {
    //        _timer = new System.Threading.Timer(PCLTimerCallback, null, _dueTime, _period);
    //    }
    //}
}
