// AForge Vision Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
//

namespace AForge.Vision.Motion
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    using AForge.Imaging;

    /// <summary>
    /// Motion detection wrapper class, which performs motion detection and processing.
    /// </summary>
    ///
    /// <remarks><para>The class serves as a wrapper class for
    /// <see cref="IMotionDetector">motion detection</see> and
    /// <see cref="IMotionProcessing">motion processing</see> algorithms, allowing to call them with
    /// single call. Unlike motion detection and motion processing interfaces, the class also
    /// provides additional methods for convenience, so the algorithms could be applied not
    /// only to <see cref="AForge.Imaging.UnmanagedImage"/>, but to .NET's <see cref="Bitmap"/> class
    /// as well.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create motion detector
    /// MotionDetector detector = new MotionDetector(
    ///     new SimpleBackgroundModelingDetector( ),
    ///     new MotionAreaHighlighting( ) );
    /// 
    /// // continuously feed video frames to motion detector
    /// while ( ... )
    /// {
    ///     // process new video frame and check motion level
    ///     if ( detector.ProcessFrame( videoFrame ) > 0.15 )
    ///     {
    ///         // ring alarm or do somethng else
    ///     }
    /// }
    /// </code>
    /// </remarks>
    ///
    public class MotionDetector
    {
        private IMotionDetector   detector;
        private IMotionProcessing processor;

        /// <summary>
        /// Motion detection algorithm to apply to each video frame.
        /// </summary>
        ///
        /// <remarks><para>The property sets motion detection algorithm, which is used by
        /// <see cref="ProcessFrame(UnmanagedImage)"/> method in order to calculate
        /// <see cref="IMotionDetector.MotionLevel">motion level</see> and
        /// <see cref="IMotionDetector.MotionFrame">motion frame</see>.
        /// </para></remarks>
        ///
        public IMotionDetector MotionDetectionAlgorthm
        {
            get { return detector; }
            set { detector = null; }
        }

        /// <summary>
        /// Motion processing algorithm to apply to each video frame after
        /// motion detection is done.
        /// </summary>
        /// 
        /// <remarks><para>The property sets motion processing algorithm, which is used by
        /// <see cref="ProcessFrame(UnmanagedImage)"/> method after motion detection in order to do further
        /// post processing of motion frames. The aim of further post processing depends on
        /// actual implementation of the specified motion processing algorithm - it can be
        /// highlighting of motion area, objects counting, etc.
        /// </para></remarks>
        /// 
        public IMotionProcessing MotionProcessingAlgorithm
        {
            get { return processor; }
            set { processor = null; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MotionDetector"/> class.
        /// </summary>
        /// 
        /// <param name="detector">Motion detection algorithm to apply to each video frame.</param>
        /// 
        public MotionDetector( IMotionDetector detector ) : this( detector, null ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MotionDetector"/> class.
        /// </summary>
        /// 
        /// <param name="detector">Motion detection algorithm to apply to each video frame.</param>
        /// <param name="processor">Motion processing algorithm to apply to each video frame after
        /// motion detection is done.</param>
        /// 
        public MotionDetector( IMotionDetector detector, IMotionProcessing processor )
        {
            this.detector  = detector;
            this.processor = processor;
        }

        /// <summary>
        /// Process new video frame.
        /// </summary>
        /// 
        /// <param name="videoFrame">Video frame to process (detect motion in).</param>
        /// 
        /// <returns>Returns amount of motion, which is provided <see cref="IMotionDetector.MotionLevel"/>
        /// property of the <see cref="MotionDetectionAlgorthm">motion detection algorithm in use</see>.</returns>
        /// 
        /// <remarks><para>See <see cref="ProcessFrame(UnmanagedImage)"/> for additional details.</para>
        /// </remarks>
        /// 
        public double ProcessFrame( Bitmap videoFrame )
        {
            double motionLevel = 0;

            BitmapData videoData = videoFrame.LockBits(
                new Rectangle( 0, 0, videoFrame.Width, videoFrame.Height ),
                ImageLockMode.ReadWrite, videoFrame.PixelFormat );

            try
            {
                motionLevel = ProcessFrame( new UnmanagedImage( videoData ) );
            }
            finally
            {
                videoFrame.UnlockBits( videoData );
            }

            return motionLevel;
        }

        /// <summary>
        /// Process new video frame.
        /// </summary>
        /// 
        /// <param name="videoFrame">Video frame to process (detect motion in).</param>
        /// 
        /// <returns>Returns amount of motion, which is provided <see cref="IMotionDetector.MotionLevel"/>
        /// property of the <see cref="MotionDetectionAlgorthm">motion detection algorithm in use</see>.</returns>
        /// 
        /// <remarks><para>See <see cref="ProcessFrame(UnmanagedImage)"/> for additional details.</para>
        /// </remarks>
        ///
        public double ProcessFrame( BitmapData videoFrame )
        {
            return ProcessFrame( new UnmanagedImage( videoFrame ) );
        }

        /// <summary>
        /// Process new video frame.
        /// </summary>
        /// 
        /// <param name="videoFrame">Video frame to process (detect motion in).</param>
        /// 
        /// <returns>Returns amount of motion, which is provided <see cref="IMotionDetector.MotionLevel"/>
        /// property of the <see cref="MotionDetectionAlgorthm">motion detection algorithm in use</see>.</returns>
        /// 
        /// <remarks><para>The method first of all applies motion detection algorithm to the specified video
        /// frame to calculate <see cref="IMotionDetector.MotionLevel">motion level</see> and
        /// <see cref="IMotionDetector.MotionFrame">motion frame</see>. After this it applies motion processing algorithm
        /// (if it was set) to do further post processing, like highlighting motion areas, counting moving
        /// objects, etc.</para>
        /// </remarks>
        /// 
        public double ProcessFrame( UnmanagedImage videoFrame )
        {
            // call motion detection
            detector.ProcessFrame( videoFrame );
            // call motion post processing
            if ( ( processor != null ) && ( detector.MotionFrame != null ) )
            {
                processor.ProcessFrame( videoFrame, detector.MotionFrame );
            }

            return detector.MotionLevel;
        }

        /// <summary>
        /// Reset motion detector to initial state.
        /// </summary>
        /// 
        /// <remarks><para>The method resets motion detection and motion processing algotithms by calling
        /// their <see cref="IMotionDetector.Reset"/> and <see cref="IMotionProcessing.Reset"/> methods.</para>
        /// </remarks>
        /// 
        public void Reset( )
        {
            detector.Reset( );
            if ( processor != null )
            {
                processor.Reset( );
            }
        }
    }
}
