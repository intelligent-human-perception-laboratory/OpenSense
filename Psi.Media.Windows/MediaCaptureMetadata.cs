﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.Psi;
using Microsoft.Psi.Media;
using OpenSense.Component.Contract;

namespace OpenSense.Component.Psi.Media {
    [Export(typeof(IComponentMetadata))]
    public class MediaCaptureMetadata : IComponentMetadata {

        public string Name => typeof(MediaCapture).FullName;

        public string Description => "Component that captures and streams video and audio from a camera.";

        public IReadOnlyList<IPortMetadata> Ports => new[] { 
            new StaticPortMetadata(typeof(MediaCapture).GetProperty(nameof(MediaCapture.Video))),
            new StaticPortMetadata(typeof(MediaCapture).GetProperty(nameof(MediaCapture.Audio))),
        };

        public ComponentConfiguration CreateConfiguration() => new MediaCaptureConfiguration();

        public IProducer<T> GetOutputProducer<T>(object instance, PortConfiguration portConfiguration) => this.GetOutputProducerOfStaticPort<T>(instance, portConfiguration);
    }
}
