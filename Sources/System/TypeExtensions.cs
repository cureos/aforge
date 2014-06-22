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

using System.Linq;
using System.Reflection;

namespace System
{
    public static class TypeExtensions
    {
        public static FieldInfo[] GetFields(this Type type, BindingFlags bindingAttr)
        {
            return
                type.GetRuntimeFields()
                    .Where(fieldInfo => AreBindingFlagsMatching(fieldInfo, bindingAttr))
                    .ToArray();
        }

        private static bool AreBindingFlagsMatching(FieldInfo fieldInfo, BindingFlags bindingAttr)
        {
            var publicFlag = bindingAttr.HasFlag(BindingFlags.Public);
            var nonPublicFlag = bindingAttr.HasFlag(BindingFlags.NonPublic);
            if (publicFlag == nonPublicFlag) throw new ArgumentException("Binding must be set to either public or non-public.");

            var staticFlag = bindingAttr.HasFlag(BindingFlags.Static);
            var instanceFlag = bindingAttr.HasFlag(BindingFlags.Instance);
            if (staticFlag == instanceFlag) throw new ArgumentException("Binding must be set to either static or instance.");

            return ((fieldInfo.IsPublic && publicFlag) || (!fieldInfo.IsPublic && nonPublicFlag)) &&
                   ((fieldInfo.IsStatic && staticFlag) || (!fieldInfo.IsStatic && instanceFlag));
        }
    }
}