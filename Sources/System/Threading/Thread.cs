//
// Shim.System
//
// Copyright © Cureos AB, 2013-2014
// info at cureos dot com
//

namespace System.Threading
{
    public delegate void ThreadStart();

    public sealed class Thread
    {
        #region CONSTRUCTORS

        public Thread(ThreadStart start)
        {
        }

        #endregion

        #region PROPERTIES

        public string Name { get; set; }

        #endregion

        #region METHODS

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Join()
        {
            throw new NotImplementedException();
        }

        public bool Join(int millisecondsTimeout)
        {
            throw new NotImplementedException();
        }

        public void Abort()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}