/*
 *  Copyright (c) 2013-2014, Cureos AB.
 *  All rights reserved.
 *  http://www.cureos.com
 *
 *	This file is part of Shim.NET.
 *
 *  Shim.NET is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  Shim.NET is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with Shim.NET.  If not, see <http://www.gnu.org/licenses/>.
 */

namespace System.Net
{
    public static class HttpWebRequestExtensions
    {
        public static void SetConnectionGroupName(this HttpWebRequest request, string groupName)
        {
#if DOTNET
            request.ConnectionGroupName = groupName;
#endif
        }

        public static void SetProxy(this HttpWebRequest request, IWebProxy proxy)
        {
#if DOTNET
            request.Proxy = proxy;
#endif
        }

        public static void SetTimeout(this HttpWebRequest request, int timeout)
        {
#if DOTNET
            request.Timeout = timeout;
#endif
        }

        public static void SetUserAgent(this HttpWebRequest request, string userAgent)
        {
#if DOTNET
            request.UserAgent = userAgent;
#endif
        }
    }
}