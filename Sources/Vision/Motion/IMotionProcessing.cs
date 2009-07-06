namespace AForge.Vision.Motion
{
    using System;
    using AForge.Imaging;

    public interface IMotionProcessing
    {
        void ProcessFrame( UnmanagedImage videoFrame, UnmanagedImage motionFrame );

        void Reset( );
    }
}
