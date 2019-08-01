// OCVmatch.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include <opencv2/core.hpp>
#include <opencv2/highgui.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/imgcodecs.hpp>
#include <opencv2/videoio.hpp>
#include <opencv2/calib3d.hpp>

#include <iostream>
#include <stdio.h>

using namespace cv;
using namespace std;

extern "C" int __declspec(dllexport) __stdcall  Init()
{
	// Open console for DLL debugging, this only works on Windows
	FILE * pConsole;
	AllocConsole();
	freopen_s(&pConsole, "CONOUT$", "wb", stdout);

	return 0;
}

extern "C" void __declspec(dllexport) __stdcall ComputePNP(Vec3f *&op, Vec2f *&ip, unsigned char *&rv, unsigned char *&tv)
{
	Mat imagePoints;
	Mat objectPoints;
	Mat cameraMatrix;
	Mat distCoeffs;

	Mat rotation;
	Mat translation;

	
	imagePoints = Mat(4, 2, CV_32F, ip);
	objectPoints = Mat(4, 3, CV_32F, op);

	cout << "imagePoints = " << endl << " " << imagePoints << endl << endl;
	cout << "objectPoints = " << endl << " " << objectPoints << endl << endl;

	cameraMatrix = (Mat_<float>(3, 3) << 1514, 0, 1514/2, 0, 1514, 795/2, 0, 0, 1);
	
	solvePnP(objectPoints, imagePoints, cameraMatrix, distCoeffs, rotation, translation);

	Mat rotationMat;
	Rodrigues(rotation, rotationMat);
	cout << "rotationMat = " << endl << " " << rotationMat << endl << endl;

	memcpy(rv, rotationMat.data, rotationMat.total() * rotationMat.elemSize());
	memcpy(tv, translation.data, translation.total() * translation.elemSize());
}

