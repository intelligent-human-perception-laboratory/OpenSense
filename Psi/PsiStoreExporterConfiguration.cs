﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Psi;
using Microsoft.Psi.Data;
using OpenSense.Component.Contract;

namespace OpenSense.Component.Psi {
    [Serializable]
    public class PsiStoreExporterConfiguration : PsiExporterConfiguration {

        private string storeName = string.Empty;

        public string StoreName {
            get => storeName;
            set => SetProperty(ref storeName, value);
        }

        private string rootPath;

        public string RootPath {
            get => rootPath;
            set => SetProperty(ref rootPath, value);
        }

        private bool createSubdirectory;

        public bool CreateSubdirectory {
            get => createSubdirectory;
            set => SetProperty(ref createSubdirectory, value);
        }

        public override IComponentMetadata GetMetadata() => new PsiStoreExporterMetadata();

        protected override Exporter CreateExporter(Pipeline pipeline, out object instance) {
            var exporter = PsiStore.Create(pipeline, StoreName, RootPath, CreateSubdirectory);
            instance = exporter;
            return exporter;
        }
    }
}
