<h1> UltraInstinctVR folder and file organization </h1> 



*UltraInstinctVR* is organized into several directories to ensure the correct operation of the framework.  
All folders are located in `/Assets/Scripts` and are structured into five main subdirectories:  
`/Interaction`, `/ScriptsReplays`, `/Sensors`, `/Effector`, and `/Utils`.

Each directory groups scripts of a similar nature, contributing to the reliable execution of *UltraInstinctVR*.

- **Interaction**: Contains scripts responsible for generating interactions within the VR environment.
- **ScriptsReplays**: Contains components that enable the replay of test sequences and help prevent flaky tests.
- **Sensors**: Contains the *Xareus Sensors*, which detect interactions triggered by test agents.
- **Effector**: Contains the *Xareus Effectors*, executed once a sensor has detected an interaction and responsible for activating the oracle mechanism.
- **Utils**: Contains utility scripts supporting non-functional requirements, such as automated test agent activation and test report generation for scientific analysis.
If you'd like a more compact version, a table, or GitHub-style formatting, I can generate it as well.




<h2> /Interaction folder</h2>

It contains sub folders which regroup interaction of the same type `/GameObject Movement`, `/Teleportation`, `/VRTest` 

- **GameObject Movement** : Scripts that simulates the movement of GameObject
- **Teleportation** :  Scripts that simulates teleportation 
- **VRTest** : Scripts that contains VRTest and other classes for the correct operation



Game object Movement contains : 
    
- **Colission.cs** : Scripts to simulates colission between game object

- **SelectAndMoveToOrigin.cs** : Simulates the selection and the grab of game object

Telportation contains :

- **ITeleportable.cs** : It's a interface to manage teleportation
- **SizeScene.cs** : Ensure teleportation outside the scene
- **TeleportAtObject.cs** : Ensure teleportation in interactable Object
- **TeleportPlayer.cs** : Ensure teleportation in random position


<h2>/ScriptsReplay</h2>

- **TeleportReplay.cs** : Allows to make teleportation according to a position file loaded in the componenent


<h2>/Sensors</h2>

- **SelectionSensors.cs** : Implement a Xareus sensor that detect the selection of interactable game object

- **TeleportationSensor.cs** : Implement a Xareus sensor that detect a teleportation of a game object in the scene



<h2>/Effectors</h2>


- **TeleportationEffector.cs** : Allows to triggers the Oracle mechanism and check if the teleportation has been done sucessfully
- **OutsideSceneEffector.cs** : Allows to triggers the Oracle mechanism and check if the teleportation outside the scene has been done 
- **InObjectSceneEffector.cs** : Allows to triggers the Oracle mechanism and check if the teleportation in a interactable object has been done
- **ObjectSelectionEffector.cs** :  Allows to triggers the Oracle mechanism and check if the selection of a game object has been done sucessfuly
- **ColissionEffector.cs** : Allows to triggers the Oracle mechanism and check if the colission between game object has been done
- **MoveObjectToOriginEffector.cs** : Allows to triggers the Oracle mechanism and check if the interactable object 


<h2>/Utils</h2>


- **ParentController.cs**: Manages each Test agent that are under the form of GameObject and ensures that each test agent are executed correctly one by one and independently  
- **ChildBehavior.cs** : Control the execution of a game object
- **GenerateReport.cs** : Read the logs and put in a log file in `Logs/game_logs.txt`
- **GetStartTime.cs** : Allows to get the execution time of the testing campaign
- **LogAnalyser.cs**: Generate the report in `.html` format 
- **LogPerformance.cs**: Generate a `.csv` file where each errors,time when it's triggers,the name of the error and his stackTrace are collected, it serves for sciences evaluation purposes the result are stored in `Logs/result`
