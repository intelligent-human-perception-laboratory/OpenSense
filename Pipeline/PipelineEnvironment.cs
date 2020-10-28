﻿using System;
using System.Collections.Generic;
using System.Linq;
using OpenSense.Component.Contract;
using PsiPipeline = Microsoft.Psi.Pipeline;

namespace OpenSense.Pipeline {
    public class PipelineEnvironment {

        public readonly PsiPipeline Pipeline;

        public readonly IReadOnlyList<ComponentEnvironment> Instances;

        public PipelineEnvironment(PipelineConfiguration configuration) {
            if (configuration is null) {
                throw new ArgumentNullException(nameof(configuration));
            }
            Pipeline = PsiPipeline.Create(configuration.Name, configuration.DeliveryPolicy);
            var instEnvs = new List<ComponentEnvironment>();
            Instances = instEnvs;
            var pending = new List<ComponentConfiguration>(configuration.Instances);
            while (true) {
                //find one with all dependencies ready
                var compConfig = pending.Where(c => c.Inputs.All(i => instEnvs.Select(e => e.Configuration.Id).Contains(i.RemoteId))).FirstOrDefault();
                if (compConfig is null) {//if not found
                    if (pending.Count > 0) {
                        throw new Exception($"{pending.Count} components cannot be instantiated because their data flow dependencies are not fulfilled.");
                    }
                    break;
                }
                pending.Remove(compConfig);
                var instance = compConfig.Instantiate(Pipeline, instEnvs);
                var env = new ComponentEnvironment(compConfig, instance);
                instEnvs.Add(env);
            }
        }
    }
}
