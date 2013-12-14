//
// Portability Class Library
//
// Copyright © Cureos AB, 2013
// info at cureos dot com
//

namespace System.Diagnostics
{
    public static class Trace
    {
        #region METHODS

        public static void WriteLine(string format, params object[] args)
        {
            Debug.WriteLine(format, args);
        }

        public static void TraceWarning(string message)
        {
            Debug.WriteLine(message);
        }

        public static void Assert(bool condition)
        {
            Debug.Assert(condition);
        }

        public static void TraceInformation(string format, params object[] args)
        {
            Debug.WriteLine(format, args);
        }
        
        #endregion
    }
}