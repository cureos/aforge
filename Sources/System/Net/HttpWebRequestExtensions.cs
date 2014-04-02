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
#if !PORTABLE && !NETFX_CORE && !WINDOWS_PHONE
            request.ConnectionGroupName = groupName;
#endif
        }

        public static void SetProxy(this HttpWebRequest request, IWebProxy proxy)
        {
#if !PORTABLE && !NETFX_CORE && !WINDOWS_PHONE
            request.Proxy = proxy;
#endif
        }

        public static void SetTimeout(this HttpWebRequest request, int timeout)
        {
#if !PORTABLE && !NETFX_CORE && !WINDOWS_PHONE
            request.Timeout = timeout;
#endif
        }

        public static void SetUserAgent(this HttpWebRequest request, string userAgent)
        {
#if !PORTABLE && !NETFX_CORE && !WINDOWS_PHONE
            request.UserAgent = userAgent;
#endif
        }
    }
}