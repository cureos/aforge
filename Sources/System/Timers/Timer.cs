//
// Portability Class Library
//
// Copyright © Cureos AB, 2014
// info at cureos dot com
//

using System.ComponentModel;

namespace System.Timers
{
    public delegate void ElapsedEventHandler(object sender, ElapsedEventArgs e);

    public sealed class Timer : IDisposable
    {
        #region EVENTS

        public event ElapsedEventHandler Elapsed;
        
        #endregion

        #region PROPERTIES

        public bool Enabled { get; set; }
        
        public double Interval { get; set; }

        public ISynchronizeInvoke SynchronizingObject { get; set; }

        #endregion

        #region METHODS

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
