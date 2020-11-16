1)Update Windows Updates
2)VSStudio
(Workloads)
universal windows platform development (Tick USB connectivity)
Desktop development with C++

3)Latest Unity

4)Windows SDK

5)GIT for windows

6)Import UNity foundation package

7)CHange Build to UWP

8) Reimport all assets

9) Restart PC 

10) DEBUG setting Cineast configuration file getfile path return ""

11) Open Unity and Build with Device ARM and set player setting mixed reality 16 Bit (Disable Graphic Jobs)

12) open Build with Visual studio 

13) Install required Tools recommend by visual studio

14) automatic windows enable developer mode windows 10

13) Select DEBUG/Release/Master and ARM

14) Enter PIN in Studio, PIN can be found hololens -> Settings -> Updates -> Developer -> Pair

15) Application will launch automatically Hololens

Cineast documentation:

Implement Cineast:
1) Add manifest.json with "https://github.com/vitrivr/UnityInterface.git#all-dev",
DoCineastRequest in Controller.cs creates a query object using the QueryBuilder from the CineastApi.
This query objects gets executed in the CineastWrapper which actually retrieves segments.
Segments contain all information on a specific media. 
The resultsegment can help to retrieve the final result data by using the ObjectRegistry.
With the help of ObjectRegistry we can map the final data to a multimedia object.