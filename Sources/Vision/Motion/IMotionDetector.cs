namespace AForge.Vision.Motion
{
    using System;
    using AForge.Imaging;

    public interface IMotionDetector
    {
        /// <summary>
        /// Motion level value, [0, 1].
        /// </summary>
        /// 
        /// <remarks><para>Amount of changes in the last processed frame. For example, if value of
        /// this property equals to 0.1, then it means that last processed frame has 10% of changes
        /// (however it is up to specific implementation to decide how to compare specified frame).</para>
        /// </remarks>
        /// 
        double MotionLevel { get; }

        UnmanagedImage MotionFrame { get; }

        /// <summary>
        /// Process new video frame.
        /// </summary>
        /// 
        /// <remarks><para>Process new frame from video source and detect motion in it.</para></remarks>
        /// 
        void ProcessFrame( UnmanagedImage videoFrame );

        /// <summary>
        /// Reset detector's to initial state.
        /// </summary>
        /// 
        /// <remarks><para>Resets internal state and variables of motion detection algorithm.
        /// Usually this is required to do before processing new video source, but
        /// may be also done at any time to start motion detection from scratch.</para>
        /// </remarks>
        /// 
        void Reset( );
    }
}
