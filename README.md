# Image Creator
A .net core web api to create images from a specific data structure related to routes.
See `DataContainer` class to understand the relevant data structure.
For example this image:
![image](https://user-images.githubusercontent.com/3269297/147157323-609d38d3-59bd-4b90-90f4-43931fb6161f.png)

What it does it take a few tiles from Israel Hiking Map and combines them to a background image.
On top of that it draws the routes and the markers.
It can also receive the required width and height of the image.
Try it out on dockerhub: `israelhikingmap/imagecreatorwebapi`
Or build it from sources:
```
cd src
docker build . -t imagecreatorwebapi
```
