# Asteroids

Game influenced by [**Asteroids**](https://en.wikipedia.org/wiki/Asteroids_(video_game))

This is made in **Unity 2022.3.14f1**

**Playing**

- **Editor** - Open a new project in the root of Asteroids, load the scene AsteroidsScene.unity and hit Play
- **Build** - Open the folder Build in the root of Asteroids, and run Asteroids.exe

**Explanation**:

The game will start as soon as you hit play, you can steer the rotation with 'A', 'D'. Forward with 'W'. Shoot with 'Space' and Quit with 'Esc'.
There is also controller support with rotation 'D-Pad Left' & 'D-Pad Right'. Forward with 'Cross'. Shoot with 'Right Trigger' and Quit with 'Start'.

This is an infinite runner, of Asteroids and Flying Saucers spawning in around the player. When cleared it will spawn in a new round of enemies.

----

### Design Principles:

I went in when making this with a memory efficient, modular idea with clear and concise classes all with their own responsibility.

Having a **ManagerSystem** that spawns all the managers involved that have ownership of the managers (_**Singleton Pattern**_).<br>
**GameManager** keep tracks of all the game logic in which we can hook up delegates to (_**Observer Pattern**_).

The Units in the game I really wanted to have a more modular approach with using **ScriptableObjects** to define their stats so we can easily create new ones, in which we can create **UnitDataPlayer** & **UnitDataEnemy**.

For memory I went in using the **Unity's Pooling System**, which is a modular system for hooking up different functions for each and every object. I also wanted to use some of the **Addressable Tools** to define enemy packs that wen can load in and out (_**as for this project we only have one pack that we load in at start**_).

For just the overall input I went in using **Unity's Input Actions System**, this has good support for setting different types of inputs and also we can hook these easy with functions.
<br>
<br>

**Challenges**:

It was pretty straightforward the project, I haven't delved in to the **Addessables System** that much so it was fun to learn more about that, if I have gotten more time I would like to have a more modular support on how I load and unload assets and really connect them to different enemy waves or levels.

For spawning in particles I wanted them to pool back in when they were done, so was struggling a little bit on how to do that since they didn't have any connection to it. So I decided when we spawned them in the **ObjectPoolManager** that we attach an script to them that will let them pool themselves when the particle is done, which I think worked well for that particular **Particle System**.
