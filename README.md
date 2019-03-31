# Demonstrating OpenCV integration with Unity

Uses OpenCV with Unity to align a 3D plane with four 2D points selected from the camera feed. This code uses the Vuforia ARcamera object to get the camera feed, but this Vuforia integration is largely unnecessary and this program could easily be adapted to not rely on Vuforia.

This program translates and rotates the quad to fit the 2D points rather than moving the camera to match the real camera pose, but this could be done using the inverse of the transformation matrix.

The C++ component needs to be compiled as .DLL and placed in assets/plugins directory of the Unity project. The camera matrix used by SolvePNP is currently hardcoded to use the Height/Width of the Unity Editor viewport.

The GameObject "Ground" is a quad with the expected proportions of your real surface.
