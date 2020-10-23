﻿using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.Psi;
using Microsoft.Psi.Media;
using OpenSense.Components;
using OpenSense.Components.OpenFace;
using OpenSense.DataStructure;

namespace OpenSense.GazeToDisplayConverter {

    public partial class PredictionWindow : Window {
        public PredictionWindow() {
            InitializeComponent();

            WindowStyle = WindowStyle.None;
            WindowState = WindowState.Maximized;
        }

        public IGazeToDisplayConverter Converter;

        public bool FlipX { get; set; }

        public bool FlipY { get; set; }

        public string WebcamSymbolicLink { get; set; } = string.Empty;

        public int WebcamWidth { get; set; } = 1920;

        public int WebcamHeight { get; set; } = 1080;

        public float WebcamFx { get; set; } = 500;

        public float WebcamFy { get; set; } = 500;

        public float WebcamCx { get; set; } = 1920 / 2f;

        public float WebcamCy { get; set; } = 1080 / 2f;

        private Pipeline pipeline;

        private void PredictionWindow_Input(object sender, EventArgs e) {
            switch (e) {
                case KeyEventArgs key:
                    switch (key.Key) {
                        case Key.Escape:
                        case Key.Enter:
                            Close();
                            break;
                    }
                    break;
                case MouseButtonEventArgs mouse:
                    Close();
                    break;
            }
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            InitializePipeline();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            pipeline?.Dispose();//TODO: get stuck here, why?
        }

        private void InitializePipeline() {
            pipeline = Pipeline.Create();
            var webcamConfig = MediaCaptureConfiguration.Default;
            webcamConfig.DeviceId = WebcamSymbolicLink;
            webcamConfig.Width = WebcamWidth;
            webcamConfig.Height = WebcamHeight;
            webcamConfig.Framerate = 30;
            webcamConfig.UseInSharedMode = true;
            var source = new MediaCapture(pipeline, webcamConfig);
            var flip = new FlipColorVideo(pipeline) { FlipHorizontal = FlipX, FlipVertical = FlipY };
            source.PipeTo(flip.In, DeliveryPolicy.LatestMessage);
            var openface = new OpenFace(pipeline) { CameraCalibFx = WebcamFx, CameraCalibFy = WebcamFy, CameraCalibCx = WebcamCx, CameraCalibCy = WebcamCy };
            flip.PipeTo(openface.In, DeliveryPolicy.LatestMessage);
            var dispFlip = new FlipColorVideo(pipeline) { FlipHorizontal = !FlipX, FlipVertical = FlipY };// mirror display
            source.PipeTo(dispFlip.In, DeliveryPolicy.LatestMessage);
            var joinedVideoFrame = openface.HeadPoseAndGazeOut.Join(dispFlip.Out, Reproducible.Exact<Shared<Microsoft.Psi.Imaging.Image>>(), (dataPoint, frame) => new Tuple<HeadPoseAndGaze, Shared<Microsoft.Psi.Imaging.Image>>(dataPoint, frame), DeliveryPolicy.LatestMessage, DeliveryPolicy.LatestMessage);
            joinedVideoFrame.Do(UpdateDisplay, DeliveryPolicy.LatestMessage);
            pipeline.RunAsync();
        }

        private void UpdateDisplay(Tuple<HeadPoseAndGaze, Shared<Microsoft.Psi.Imaging.Image>> dataPoint, Envelope e) {
            var (CoordinateX, CoordinateY) = Converter.Predict(dataPoint.Item1).DeepClone();
            try {
                Dispatcher.Invoke(() => {
                    var point = new Point(CoordinateX * PredictionCanvas.ActualWidth, CoordinateY * PredictionCanvas.ActualHeight);
                    EllipseGeometryPredictionCircle.Center = point;
                    var bitmap = dataPoint.Item2.Resource.ToBitmap();
                    using (var memory = new MemoryStream()) {
                        bitmap.Save(memory, ImageFormat.Png);
                        memory.Position = 0;
                        var bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = memory;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                        ImageBrushVideoFrame.ImageSource = bitmapImage;
                    }
                }, DispatcherPriority.Normal, CancellationToken.None, TimeSpan.FromMilliseconds(500));
            } catch (TimeoutException) {
            }
        }
    }
}