//
// Portability Class Library
//
// Copyright © Cureos AB, 2013
// info at cureos dot com
//

namespace System.Threading.Tasks
{
    public class ParallelLoopState
    {
        #region PROPERTIES

        public bool ShouldExitCurrentIteration
        {
            get { return false; }
        }

        #endregion

        #region METHODS

        public void Stop()
        {
        }

        #endregion
    }
}