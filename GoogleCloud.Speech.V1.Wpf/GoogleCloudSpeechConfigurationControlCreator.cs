﻿using System.Composition;
using System.Windows;
using OpenSense.Component.Contract;
using OpenSense.Component.GoogleCloud.Speech.V1;
using OpenSense.Wpf.Component.Contract;

namespace OpenSense.Wpf.Component.GoogleCloud.Speech.V1 {
    [Export(typeof(IConfigurationControlCreator))]
    public class GoogleCloudSpeechConfigurationControlCreator : IConfigurationControlCreator {

        public bool CanCreate(ComponentConfiguration configuration) => configuration is GoogleCloudSpeechConfiguration;

        public UIElement Create(ComponentConfiguration configuration) => new GoogleCloudSpeechConfigurationControl() { DataContext = configuration };
    }
}
