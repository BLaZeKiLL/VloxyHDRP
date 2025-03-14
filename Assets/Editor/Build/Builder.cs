﻿using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace CodeBlaze.Editor.Build {

    public static class Builder {

        private static readonly string Eol = Environment.NewLine;
        
        [MenuItem("Build/Package")]
        private static void Package() {
            AssetDatabase.ExportPackage("Packages/io.codeblaze.vloxyengine", "vloxyengine.unitypackage", ExportPackageOptions.Recurse);
        }
        
        #region Windows

        [MenuItem("Build/Windows/Debug")]
        private static void BuildWindowsDebug() {
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Standalone,new [] {
                "UNITY_POST_PROCESSING_STACK_V2", "VLOXY_LOGGING", "VLOXY_DEBUG"
            });
            
            var options = new BuildPlayerOptions {
                scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes),
                locationPathName = "Build/Windows/Mono-Debug/vloxy_mono_debug.exe",
                target = BuildTarget.StandaloneWindows64,
                targetGroup = BuildTargetGroup.Standalone,
                options = BuildOptions.Development | BuildOptions.AllowDebugging | BuildOptions.CompressWithLz4HC
            };

            var report = BuildPipeline.BuildPlayer(options);
            
            ReportSummary(report.summary);
        }

        [MenuItem("Build/Windows/Release")]
        private static void BuildWindowsRelease() {
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Standalone,new [] {
                "UNITY_POST_PROCESSING_STACK_V2", 
                "VLOXY_LOGGING",
            });
            
            var options = new BuildPlayerOptions {
                scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes),
                locationPathName = "Build/Windows/Mono-Release/vloxy_mono_release.exe",
                target = BuildTarget.StandaloneWindows64,
                targetGroup = BuildTargetGroup.Standalone,
                options = BuildOptions.CompressWithLz4HC
            };

            var report = BuildPipeline.BuildPlayer(options);
            
            ReportSummary(report.summary);
        }
        
        #endregion

        #region Mac

        [MenuItem("Build/Mac/Debug")]
        private static void BuildMacDebug() {
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Standalone,new [] {
                "UNITY_POST_PROCESSING_STACK_V2", "VLOXY_LOGGING", "VLOXY_DEBUG"
            });
            
            var options = new BuildPlayerOptions {
                scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes),
                locationPathName = "Build/Osx/Mono-Debug/vloxy_mono_debug",
                target = BuildTarget.StandaloneOSX,
                targetGroup = BuildTargetGroup.Standalone,
                options = BuildOptions.Development | BuildOptions.AllowDebugging | BuildOptions.CompressWithLz4HC
            };

            var report = BuildPipeline.BuildPlayer(options);
            
            ReportSummary(report.summary);
        }

        [MenuItem("Build/Mac/Release")]
        private static void BuildMacRelease() {
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Standalone,new [] {
                "UNITY_POST_PROCESSING_STACK_V2", 
                "VLOXY_LOGGING",
            });
            
            var options = new BuildPlayerOptions {
                scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes),
                locationPathName = "Build/Osx/Mono-Release/vloxy_mono_release",
                target = BuildTarget.StandaloneOSX,
                targetGroup = BuildTargetGroup.Standalone,
                options = BuildOptions.CompressWithLz4HC
            };

            var report = BuildPipeline.BuildPlayer(options);
            
            ReportSummary(report.summary);
        }
        
        #endregion

        #region Android

        [MenuItem("Build/Android/Debug")]
        private static void BuildAndroidDebug() {
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Standalone,new [] {
                "UNITY_POST_PROCESSING_STACK_V2", "VLOXY_LOGGING", "VLOXY_DEBUG"
            });
            
            var options = new BuildPlayerOptions {
                scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes),
                locationPathName = "Build/Android/Mono-Debug/vloxy_mono_debug.apk",
                target = BuildTarget.Android,
                targetGroup = BuildTargetGroup.Android,
                options = BuildOptions.Development | BuildOptions.AllowDebugging | BuildOptions.CompressWithLz4HC
            };

            var report = BuildPipeline.BuildPlayer(options);
            
            ReportSummary(report.summary);
        }

        [MenuItem("Build/Android/Release")]
        private static void BuildAndroidRelease() {
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Standalone,new [] {
                "UNITY_POST_PROCESSING_STACK_V2", 
                "VLOXY_LOGGING",
            });
            
            var options = new BuildPlayerOptions {
                scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes),
                locationPathName = "Build/Android/Mono-Release/vloxy_mono_release.apk",
                target = BuildTarget.Android,
                targetGroup = BuildTargetGroup.Android,
                options = BuildOptions.CompressWithLz4HC
            };

            var report = BuildPipeline.BuildPlayer(options);

            ReportSummary(report.summary);
        }
        
        #endregion

        private static void ReportSummary(BuildSummary summary)
        {
            Console.WriteLine(
                $"{Eol}" +
                $"###########################{Eol}" +
                $"#      Build results      #{Eol}" +
                $"###########################{Eol}" +
                $"{Eol}" +
                $"Duration: {summary.totalTime.ToString()}{Eol}" +
                $"Warnings: {summary.totalWarnings.ToString()}{Eol}" +
                $"Errors: {summary.totalErrors.ToString()}{Eol}" +
                $"Size: {summary.totalSize.ToString()} bytes{Eol}" +
                $"{Eol}"
            );
        }
    }

}