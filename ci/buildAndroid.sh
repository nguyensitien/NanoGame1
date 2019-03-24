#!/bin/bash
BUILD_METHOD="Assets.Editor.EditorBuild.PerformBuildAndroid"
OUT_DIR=$BUILD_ARTIFACTSTAGINGDIRECTORY
UNITY="/Applications/Unity/Unity.app/Contents/MacOS/Unity"
$UNITY -batchmode -quit -executeMethod $BUILD_METHOD -out $OUT_DIR -projectPath $BUILD_REPOSITORY_LOCALPATH -buildTarget Android -logFile
