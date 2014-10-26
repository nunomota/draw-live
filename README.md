![Alt text](/Assets/Resources/Splash.png?raw=true)

draw-live is a cross-platform drawing streamer, developed with Unity3D.

#How does it work?

Currently it is meant to be used as a streamer by Android phones and tablets. 
You open up the application on your preferred device and select "Host session". After that, you will be able to connect any other device (be it PC, another Android device, etc...) by filling in the IP of the streaming device and clicking "Join session".

#Instructions

To get this working on Unity3D, you just have to follow a few simple steps:

	- Open a new Unity project and name it whatever you want;
	- Clone this repository and copy the contents inside "Assets" into your Project's Assets folder;
	- Go to "Assets/Scene" and open it;
	- Go to "Assets/Scripts" and drag: 'Main.cs' to the GameObject "Scripts"; 'draw.cs' to the "Main Camera";
	- Finnaly, go to "Assets/Resources" and drag 'skybox' to: Plane -> Mesh Renderer -> Materials -> Element 0;

After all this, you are good to go ;)
