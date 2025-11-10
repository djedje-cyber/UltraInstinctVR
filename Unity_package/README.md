# UltraInstinctVR

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
- Import Zeno 3 into the gameObjects hierachy
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
- **Goku** represents the headset and simulates locomotion.
- **Vegeta** represents the hand and simulates manipulation.

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

## Implemented Oracles & Prefab Components

| Name      | Description |
|-----------|-------------|
| **Zeno**     | Manages and controls subcomponents; responsible for launching each component independently. |
| **Beerus**   | Scans the Unity scene and identifies interactable objects. The scan stops once no more objects are found. |
| **Goku**     | Attempts to teleport within the scene. The oracle passes if successful. |
| **Gohan**    | Attempts to teleport outside the scene. The oracle fails if teleportation is successful. |
| **Goten**    | Attempts to teleport into another GameObject. The oracle fails if successful. |
| **Maradona** | *(Reference to Maradona's "Hand of God" - [Video](https://www.youtube.com/watch?v=-ccNkksrfls))*<br>Tries to select GameObjects. The oracle passes if selection is successful. |
| **Broly**    | A cube that collides with interactable objects. The oracle passes if collisions occur. |
| **Vegeta**   | Selects, grabs, and moves interactable objects to position `(0, 0, 0)`. The oracle verifies movement via OpenXR hands. |
| **Karin**    | Reads logs and generates an HTML test report. |


## How to install and run UltraInstinctVR
- Import the package by drag and drop in the project tab
- Import the Zeno3 prefab into the gameObjects hierachy
- Assign the good gameobject to the Xareus scenario repsenting the oracle as petri-nets
   - open the xareus scenario
   - Assign the gameObject to both the sensor and effector
- Launch the unity scene and wait the test suite to be terminated



