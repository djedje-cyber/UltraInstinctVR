# UltraInstinctVR


## Using UltraInstinctVR

### Installation
To use **UltraInstinctVR**, import the Unity package located at: `/Unity_package/UltraInstinctVR_V1.0.6.unitypackage`

### Contributing
If you want to contribute via a Pull Request:  

1. Clone the repository.  
2. Open Unity and import the `CodeBase` folder from the cloned repository.  
3. Make your changes and submit a Pull Request. 

Important note : We will need a Xareus license to install the framework.
For more information about the license : [Xareus quickstart](https://xareus.insa-rennes.fr/tutorials/quickstart.html)

## What is UltraInstinctVR?

**UltraInstinctVR** is a Unity VR interaction testing framework that simulates interactions in a VR application and verifies that actions (oracles) are correctly executed throughout the scenario engine **Xareus**.

---

## What is Xareus?

**Xareus** is a set of tools that helps creators develop XR applications easily and efficiently. It aims to put domain experts at the center of the development process by providing higher abstraction levels than traditional code when needed.  
It is compatible with any C# application and can be delivered as a Unity package for easier management.

**References**: [Xareus Official Site](https://xareus.insa-rennes.fr/?tabs=air)

---


## How to install and run UltraInstinctVR?
- Import the package
- Import TestManager into the gameObjects hierachy
- Assign the good gameobject to the Xareus scenario repsenting the oracle as petri-nets
- Launch the unity scene and wait the test suite to be terminated

---



## What is an Interaction in VR?

An interaction in VR refers to how users engage with the virtual environment, such as selecting, grabbing, or moving objects or characters.

There are three main types of interaction in VR:

- **Selection**: Selecting game objects in the scene. This typically precedes interaction with the object.
- **Locomotion**: Moving within the scene, either via teleportation or physical movement tracked by the headset.
- **Manipulation**: Interacting with objects by performing actions like grabbing, rotating, or activating them.

**More info**: [FutureLearn: Construct a VR Experience](https://www.futurelearn.com/info/courses/construct-a-virtual-reality-experience/0/steps/96390)

---

## How Are Test Cases Defined in UltraInstinctVR?

To define test cases in VR using UltraInstinctVR, several components are needed to simulate interactions at runtime.

### Main Structure

- A **parent GameObject** is used to launch each test case independently.
- Each test case includes a **GameObject** representing a part of the VR player.

For example:
- **Simple Teleportation** represents the headset and simulates locomotion.
- **Move Object To Origin** represents the hand and simulates manipulation.

Each GameObject includes:
- OpenXR components (used by Unity to trigger interactions)
- A MonoBehaviour script that simulates a specific interaction (e.g., teleportation)
- A **Xareus scenario** to verify the oracle

---

## How Are Oracles Defined with Xareus?

Unlike traditional assertions at the end of test cases, oracles in VR are continuously evaluated using **Petri-nets**.

### Oracle Structure

- **Initial state**: Represents the start of the Unity application.
- **Transition**: A sensor continuously listens for specific actions (e.g., teleportation).
- **Effector**: Checks whether the detected action was properly executed.
  - If it fails, an error is logged.
  - If successful, a passing log is created.
- **Final state**: Indicates the end of the check before looping back to the initial state.

---

## How to Write a New Test Case

### 1. Create a GameObject
- Create a child GameObject under the parent test manager.
- Add necessary OpenXR components (e.g., for hand tracking or headset simulation).
- Write an automated interaction using a MonoBehaviour script.
- Attach the script to the GameObject.

### 2. Create an Oracle Using Xareus
- Add all components needed for Xareus to function.
- Create:
  - Sensors in the `/Sensor` folder
  - Effectors in the `/Effector` folder
- Open the Xareus Editor:
  - Create a new scenario and design the Petri-net structure.
  - Assign the initial and final states.
  - Link sensors and effectors to transitions.

---

## Implemented Oracles linked to the Prefab components and the scenario file

| Name      | Description |Scenario file
|-----------|-------------|------------------------------------------------------------------------------------------| 
| **Test Manager**     | Manages and controls subcomponents; responsible for launching each component independently. | / |
| **ScanInteractableObjec**   | Scans the Unity scene and identifies interactable objects. The scan stops once no more objects are found. | / |
| **Simple Teleportation**     | Attempts to teleport within the scene. The oracle passes if successful. | Oracle_Teleportation.xml |
| **Teleportation Outside scene**    | Attempts to teleport outside the scene. The oracle fails if teleportation is successful. | Oracle_OutSceneTeleportation.xml |
| **Teleport in interactable Object**    | Attempts to teleport into another GameObject. The oracle fails if successful. | Oracle_InObjectScene.xml |
| **SelectGameObject** | Tries to select GameObjects. The oracle passes if selection is successful. | Oracle_Selection.xml |
| **Object colission**    | A cube that collides with interactable objects. The oracle passes if collisions occur. | Oracle_Colission.xml
| **Move Object To Origin**   | Selects, grabs, and moves interactable objects to position `(0, 0, 0)`. The oracle verifies movement via OpenXR hands. | Oracle_MoveObjectToOrigin |
| **Rapport Generator**    | Reads logs and generates an HTML test report. | / |


## How to install and run UltraInstinctVR
- Import the package by drag and drop in the project tab
- Import the `TestManager` prefab into the gameObjects hierachy
- Assign the good `TestManager` to the Xareus scenario repsenting the oracle as petri-nets
   - open the xareus scenario
   - Assign the gameObject to both the sensor and effector
- Launch the unity scene and wait the test suite to be terminated



## UltraInstinctVR version 


| Name    | Purpose  
|---------|------------ 
|`UltraInstinctVR_V1.0.6.unitypackage`| it's the version for Unity6 |  
|`UltraInstinct_V2022.3_experimentVersionV1.2.unitypackage` | it's the version for benchmarking purposes against other automated VR testing tool for Unity 2022.X |

|`UltraInstinct_experimentVersion_6V1.0.unitypackage` | it's the version for benchmarking purposes against other automated VR testing tool for Unity6 |

