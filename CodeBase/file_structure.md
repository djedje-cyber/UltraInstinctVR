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

- **ITeleportable.cs** : 
- **SizeScene.cs** : Ensure teleportation outside the scene
- **TeleportAtObject.cs** : Ensure teleportation in interactable Object
- **TeleportPlayer.cs** : Ensure teleportation in random position




