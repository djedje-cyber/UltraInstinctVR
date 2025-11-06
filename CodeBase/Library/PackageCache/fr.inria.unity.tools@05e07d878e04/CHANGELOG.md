## [2.0.0]  
Renamed Hybrid Tools to Inria Tools for better long term usage
UnityThreadExecute.InvokeActionForNextExecutionStepsAndWait can now have a timeout to prevent blocking the Unity Editor
## [1.8.0]  
Fixed scene stats overlay issue when handling static batching
## [1.7.9]  
CommandLinePackageManager: Handling cases where the package is present but cannot be found on the repository
## [1.7.8]  
UnityThreadExecute methods can now handle executing actions in UnityEditor's thread.
UnityThreadExecute.InvokeActionInUnityThread will run the specified action in the next available unity step (including the editor) or immediatly if called from unity's thread
## [1.7.7]
UnityThreadExecute will now try create an instance of itself on application start.
## [1.7.6]  
Fixed: When using GenerateAssemblies, execution order of the scripts in assemblies would sometimes not correctly set
## [1.7.5]  
GenerateAssemblies now uses files to read/write a mapping between script classes and their assez ids. Due to Unity's constraint, it is now required to run GenerateAssemblies through the -replaceassembly command line option once and then run unity again with the -updateid command line option
## [1.7.4]  
Fixed : GenerateAssemblies would not correctly change assembly definition files referencing a replaced assembly when the assembly definition files enabled overrideReferences
## [1.7.3]  
Added EditorStats, a scene overlay that displays selected mesh metrics  
Added Stats UI, a UI that shows in-game performances stats  
## [1.7.2]  
Added InvokeActionForNextExecutionStepsAndWait methods to wait for an action to be executed in Unity's thread.
Added option on singletons to disable DontDestroyOnLoad when the singleton is created automatically  
## [1.7.1]  
Fixed : Assembly generation would not use the correct fileID to replace in existing assets files  
## [1.7.0]  
Fixed : GenerateAssemblies was broken in Unity 2022  
Minimum supported Unity version is now 2022.3  
## [1.6.2]  
Changed execution order of UnityThreadExecute so that actions are called in the order they are registered  
## [1.6.1]  
Fixed an issue when reloading scenes containing singleton objects. The objects would be considered being destroyed and throw an error instead of recreating the singleton  
## [1.6.0]  
Adding scene view stats overlay (for Unity 2021.3+)  
## [1.5.4]  
Fixed possible thread concurrency that would let running threads be reused before they were completed  
## [1.5.3]  
Removed legacy ExecuteInThread method  
ExecuteInThread can now take a failingCallback action parameter that will be called if the thread fails  
ExecuteInThread doesn't provide the full stacktrace that called the thread in order to reduce allocation and cpu usage. The full stacktrace will be provided if enabled in the Tools menu or if building the application in development mode  
ExecuteInTask can now exacute code in tasks. Note that this seems to consume more CPU time and memory allocation than threads  
SeparateThread was optimize for better memory performances (less allocations)  
## [1.5.2]  
Fixed replace assemblies not working correctly for packages in the packages folder  
## [1.5.1]  
Fixed AssetDatabase.GUIDFromAssetPath not available on some Unity versions  
## [1.5.0]  
Package renamed to fr.inria.unity.tools  
Added samples  
## [1.4.0]  
Added SerializableHashsSet and SerializableQueue  
## [1.3.4]  
Updating package manifest and package related files  
## [1.3.3]  
Adding GUID checker class  
## [1.3.2]  
Unity 2019 compatibility  
## [1.3.1]  
Fixed random issue resulting in replaced assemblies not having the correct GUID  
## [1.3.0]  
Added serialization utility classes  
## [1.2.5]  
Fixed package update when the updated package is Tools  
Fixing possible loop when replacing assemblies  
## [1.2.4]  
Fixed GenerateSolutionFile script for Unity >= 2020.1  
## [1.2.3]  
Specifying a package to be added in command line will ensure the last compatible version will be installed, even if the package is already present  
## [1.2.2]  
Fixed ChangeScriptExecutionOrder and ReplaceAssemblies script that were not working anymore due to the new unity asset database system  
## [1.2.1]  
CommandLinePackageManager can now handle package versions (usage : PackageName@version)  
## [1.2.0]  
Adding CommandLinePackageManager  