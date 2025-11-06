This package contains various utilities for Unity :  
  
UnityTCP : Unity compatible TCP communication  
UnityThreadExecute : helps executing some code in unity when the main code is in a thread  
SeparateThread : helps executing some code in a thread when the main code is in unity  
UnitySingleton : Define a singleton that can be added to the scene or created automatically on first usage  
Serialization : Help serializing data into files  
GraphDisplay : Display graphs of long values  
Timer : Various timer utilies for Higher resolution timers, sometimes with tradeoff with CPU usage  
SerializableDictionary/SerializableStack : Unity-serializable versions of Dictionary and Stack classes  
EditorStats : Scene overlay that displays selected mesh metrics  
Stats UI : In-game performances stats  
  
Command line actions (mainly for automated package creation in CI):  
  
ChangeScriptExecutionOrder : Change script execution order  
CommandLineBuilder : Trigger application build  
CommandLineDefineSymbol : Add/Remove symbols  
CommandLinePackageManager : Add/remove/Update packages  
ReplaceAssemblies : replaces assembly definition files by their assembly dll file counterpart  
GenerateSolutionFile : Generate visual studio Solution file  