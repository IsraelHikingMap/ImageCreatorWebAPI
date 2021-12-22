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
docker run -p 12345:80 imagecreatorwebapi
```

Now surf to localhost:12345/swagger/ to get a simple UI to interact with the elevation service
Example of the most simple object to be sent in the post request:
```json
{
  "routes": [
  ],
  "northEast": {
    "lat": 32.5,
    "lng": 35.5,
    "alt": 0
  },
  "southWest": {
    "lat": 32,
    "lng": 35,
    "alt": 0
  },
  "baseLayer": { "key": "IHM", "address": "" },
  "overlays": [
    
  ]
}
```
