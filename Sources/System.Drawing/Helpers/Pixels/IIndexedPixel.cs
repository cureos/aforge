using System;

namespace ImagePixelEnumerator.Helpers.Pixels
{
    internal interface IIndexedPixel
    {
        // index methods
        Byte GetIndex(Int32 offset);
        void SetIndex(Int32 offset, Byte value);
    }
}
