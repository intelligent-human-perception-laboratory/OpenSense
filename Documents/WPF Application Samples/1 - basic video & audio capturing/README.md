# Basic video & audio capturing

## Description

This sample contains a basic pipeline that capture both video and audio from a camera and visualizing the results.
This sample is for introducing basic UI usage for building and exectuing a pipeline.

## How to use

+ Open a `Pipeline Editor` window.
+ Click the `File` -> `Open` menu button to load the `sample.pipe.json` configuration for a pipeline.
+ Click the `Microsoft.Psi.Media.MediaCapture` item on the left.
+ Select the camera you want to use on the right.
+ Click the `Execute` -> `Execute` menu button to open a `Pipeline Executer` window for this pipeline.
+ Click the `Execute` -> `Run` to run the pipeline.
+ Wait for several seconds for pipeline initializing, then you can see the results.
+ To stop the execution, click the `Execute` -> `Stop` menu button or simply close the window. A message box will poped out after the pipeline is stopped.

## Steps to replicate

+ Open a `Pipeline Editor` window, now there is an empty pipeline.
+ Click the `Add` button on the left to open a window listing all available components.
+ Double click the item with name `Microsoft.Psi.Media.MediaCapture` to add an instance of that component into the pipeline. The newly added instance is selected by default, its available settings are shown on the right side of the window.
+ Check `Capture Audio` checkbox to enable it producing audio output. Change other settings as needed. Since the ID of devices are different among machines, when you save a configuration on one machine and then load it on another machine, you need to adjust the selected device accordingly. *Settings shown are usually directly mapped to settings of the underlying /psi components, if you find the explanation of a setting is missing, you can verify it inside the underlying component, we are trying to find a good way to show this kind of information.*
+ Click the `Add` button on the left again, and add a instance of `OpenSense.Component.Imaging.Visualizer.ColorVideoVisualizer`. Select this instance on the left if it is not selected.
+ Click another `Add` button in the middle of the window to open a window listing floating inputs (inputs that are not connected). Some components have inputs are not manditory requiring a connection.
+ Double click the item with name `In` to prepare for connecting that input. Select that input in `Inputs` if it is not selected, all available outputs with a matched data type are shown in the middle right of the window.
+ Select `Video` or `Out` output from `MediaCapture` on the right to connect. The red `In` will become black. For `MediaCapture`, `Video` and `Out` are identical outputs.
+ Add a new instance of the component with name `OpenSense.Component.Audio.Visualizer.AudioVisualizer`.
+ Connect the `In` input of `AudioVisualizer` to the `Audio` output of `MediaCapture`.
+ Click the `File` -> `Save as` menu button to save this pipeline configuration.

## Tips

+ Use `Microsoft.Psi.Audio.AudioCapture` to capture audio only.
