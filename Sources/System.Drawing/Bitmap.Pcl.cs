/*
 *  Copyright (c) 2013-2016, Cureos AB.
 *  All rights reserved.
 *  http://www.cureos.com
 *
 *	This file is part of Shim.Drawing.
 *
 *  Shim.Drawing is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  Shim.Drawing is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with Shim.Drawing.  If not, see <http://www.gnu.org/licenses/>.
 */

using System.IO;

namespace System.Drawing
{
    using System.Drawing.Imaging;

    public sealed partial class Bitmap
    {
        #region METHODS

        internal static Bitmap Create(Stream stream)
        {
            throw new NotImplementedException("PCL");
        }

        public override void Save(Stream stream, ImageFormat format)
        {
            throw new NotImplementedException("PCL");
        }

        public override void Save(string filename, ImageCodecInfo encoder, EncoderParameters encoderParams)
        {
            throw new NotImplementedException("PCL");
        }

        #endregion
    }
}
