# Box Collider

Basic 3D game in monogame implementing following 3D concepts

* Z up coordinate system -> `+x` (right), `+z` (up) ,`+y` (towards viewer)


> ℹ️ **Note:** never determine x, y, z as left right up down as they can vary depending on the camera and different conventions are used as per user convenience


* buffers -> vertex buffer, index buffer
* coordinate spaces -> local space, global space (world matrix), camera space (view matrix), projection space (projection matrix)

<br/>

**complete chain**
```
World -> Where is the object?
View -> Where is it relative to the camera?
Projection -> How does the camera see/project it?
```

* Local vertices -> vertices relative to object's 
origin which is already decided ; either by 
modelling software or by you

* Transformations -> scale, rotation, translations
* non indexed rendering -> picking 3 vertices linearly at a time and forming triangle


* Ensure -> texture format is set to color and generate minimaps is enabled in `mgcb editor`