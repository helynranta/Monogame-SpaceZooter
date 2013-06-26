using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Windows.Devices.Input;
using Windows.UI.Xaml;
using System.ServiceModel.Dispatcher;
using System.ServiceModel;
using SpaceShooter;
using Microsoft.Xna.Framework.Audio;

//wednesday?
//Test comment
namespace SpaceShooter
{
    public class Game1 : Game
    {
        #region Variables
        public GraphicsDeviceManager _graphics; //graphics device
        public SpriteBatch _spriteBatch; //variable for spritebatch
        public Viewport _viewport; //variable for viewport
        public Random random = new Random();
        public Player player = new Player(); //create a new player
        public Model bullet, satelliteModel, ufoModel, heart;
        public BasicEffect basicEffect;
        public SpriteFont font; //font for debugging
        public Texture2D bulletTexture, healthBarTex, satelliteTexture, jsTexture, ufoTexture, heartTexture,
            shieldTexture, jsBackgroundTex, planet, healthBackground, achDone, achNotDone, heartParticle,
            shieldBar;
        public Enemy enemy;
        public Ufo ufo;
        public List<Enemy> enemyList = new List<Enemy>();
        public List<Ufo> ufoList = new List<Ufo>();
        public List<Bullet> bulletArray = new List<Bullet>(); //array for all bullet projectiles
        public Joystick joystick_right = new Joystick();
        public Joystick joystick_left = new Joystick();//create a joystick to move player
        public Matrix world = Matrix.CreateTranslation(new Vector3(0, 0, 0)); //world cordinates lol
        public Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 100), new Vector3(0, 0, 0), Vector3.UnitY); //creates look at view for camera
        public Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.1f, 100f); //calculate projection
        //find screen height and width
        public float SCREEN_HEIGHT, SCREEN_WIDTH;
        //find mouse cordinates in world, vectors for left-up and right down corners in world space
        public Vector3 mouseInWorld, upLeft, downRight;
        public Vector2 mousePosition; //vector for mouse position in screen-space
        public double time; //used to estimate time
        //using our particlesystem
        public ParticleEngine particleEngine;
        public List<Texture2D> particleTextures = new List<Texture2D>();
        public List<ParticleEngine> emitters = new List<ParticleEngine>();
        public List<HeartPickup> heartList = new List<HeartPickup>();
        public Model shieldModel;//simple uv unwrapped sphere
        public int enemiesKilled = 0;
        public float satelliteCreateDelay = 4f;
        public float ufoCreateDelay = 6f;
        public int combo;
        public float lastHitCombo, timeWhenDied, score, lastUfoSpawn, lastSatelliteSpawn;
        //music
        SoundEffectInstance musicInstance;
        public SoundEffect lazer, explosion, hit, end, pickup;
        //background Colors
        public Color background;
        public Color nextBackground;
        public bool isPressed;
        //variables for menus
        public int gameStage=0;//0==menu, 1==game, 2==gameOverScreen

        #endregion

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        //----------------------Initializing game------------------//
        protected override void Initialize()
        {
            //find out true width and height of viewport
            _viewport = _graphics.GraphicsDevice.Viewport;
            SCREEN_HEIGHT = _viewport.Height;
            SCREEN_WIDTH = _viewport.Width;
            //make sure that projection is using screen size correctly
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), SCREEN_WIDTH / SCREEN_HEIGHT, 0.1f, 1000f);
            IsMouseVisible = true; // show mouse
            joystick_right.Initialize(this, new Vector2(120, SCREEN_HEIGHT - 120)); //initialize joystick to right corner
            joystick_left.Initialize(this, new Vector2(SCREEN_WIDTH -120, SCREEN_HEIGHT-120)); //initialize joystick to right corner
            base.Initialize(); //init base of monogame
            lastSatelliteSpawn = (float)time;
            player.Initialize(this);
            _graphics.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            _graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            nextBackground = new Color(random.Next(150, 255), random.Next(150, 255), random.Next(150, 255));
            background = new Color(0,0,0);
            //create some achievements
        }
        //-------------------Loading Content-----------------------//
        protected override void LoadContent()
        {
            Content.RootDirectory = "Content";
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            player.model = Content.Load<Model>("spaceship");//Load player model from Content
            player.texture = Content.Load<Texture2D>("spaceship_diff");//load player texture from Content
            bullet = Content.Load<Model>("lazeh");
            bulletTexture = Content.Load<Texture2D>("lazeh_uv");
            //load all enemy content
            satelliteModel = Content.Load<Model>("datEnemy");
            satelliteTexture = Content.Load<Texture2D>("datEnemyUV");
            ufoModel = Content.Load<Model>("ufo");
            ufoTexture = Content.Load<Texture2D>("ufo_diff");
            //found this font here:
            //http://www.fontsquirrel.com/fonts/AnuDaw
            font = Content.Load<SpriteFont>("anudaw"); //load dummy font for debugging
            jsTexture = Content.Load<Texture2D>("joystick");//load joystick texture from Content
            jsBackgroundTex = Content.Load<Texture2D>("joystick_background");
            //create Basic effect
            basicEffect = new BasicEffect(GraphicsDevice)
            {
                TextureEnabled = true,
                VertexColorEnabled = true,
            };
            heart = Content.Load<Model>("heart");
            heartTexture = Content.Load<Texture2D>("diffusePink");
            //load particle staff
            //only one texture in this list atm...
            particleTextures.Add(Content.Load<Texture2D>("smoke2"));
            heartParticle = Content.Load<Texture2D>("heartParticle");

            shieldModel = Content.Load<Model>("shield");
            shieldTexture = Content.Load<Texture2D>("shieldTex");
            //load sound content
            SoundEffect music = Content.Load<SoundEffect>("soundtrack");
            musicInstance = music.CreateInstance();
            musicInstance.IsLooped = true;
            musicInstance.Play();
            lazer = Content.Load<SoundEffect>("AudioLaser");
            explosion = Content.Load<SoundEffect>("AudioExplosion");
            pickup = Content.Load<SoundEffect>("AudioPickup");
            end = Content.Load<SoundEffect>("AudioEnd");
            hit = Content.Load<SoundEffect>("AudioHit");
            //healthbar texture
            healthBarTex = Content.Load<Texture2D>("health");
            shieldBar = Content.Load<Texture2D>("shield_meter");
            planet = Content.Load<Texture2D>("planet");
            healthBackground = Content.Load<Texture2D>("healthBackground");
            achDone = Content.Load<Texture2D>("done");
            achNotDone = Content.Load<Texture2D>("undone");
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            // TODO: use it!
        }
        //---------------------main game loop----------------------//
        protected override void Update(GameTime gameTime)
        {
            //check if escape is pressed, then exit the game
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            //Game1.time is counting elapsed time...
            time += gameTime.ElapsedGameTime.TotalSeconds;
            //err... handle touch input?
            HandleInput();
            //randomize background color
            RandomBackground();
            //if we are in menu
            if (gameStage == 0)
            {
                if (player.isPressed)
                    gameStage = 1;
            }
            #region in-game
            //this one is for in-game
            else if (gameStage == 1)
            {
                //handle combo actions
                if (lastHitCombo + 2 < time)
                {
                    combo = 0;
                }
                //get screen width and height
                SCREEN_HEIGHT = _viewport.Height;
                SCREEN_WIDTH = _viewport.Width;
                //update player
                player.position.X -= joystick_right.dir.X * player.speed;
                player.position.Y += joystick_right.dir.Y * player.speed;
                //check for borders
                Vector3 var1 = _viewport.Unproject(new Vector3(0, 0, 0), projection, view, world);//these two first are finding what cordinates in world space are current (0,0)
                Vector3 var2 = _viewport.Unproject(new Vector3(0, 0, 100), projection, view, world);//meaning the left up corner
                Vector3 var3 = _viewport.Unproject(new Vector3(SCREEN_WIDTH, SCREEN_HEIGHT, 0), projection, view, world);//and these two are doing the same for right down corner
                Vector3 var4 = _viewport.Unproject(new Vector3(SCREEN_WIDTH, SCREEN_HEIGHT, 100), projection, view, world);
                upLeft = 1000 * (var1 - var2);//create Vector3 for up_left corner
                downRight = 1000 * (var3 - var4);//create Vector3 for down_right corner

                //clamp player position within borders
                player.position.X = MathHelper.Clamp(player.position.X, upLeft.X, downRight.X);
                player.position.Y = MathHelper.Clamp(player.position.Y, downRight.Y, upLeft.Y);
                HandleEnemies();
                player.Update();//position is not done, update it...

                if (emitters.Count > 0)
                {
                    for (int emitter = 0; emitter < emitters.Count; emitter++)
                    {
                        emitters[emitter].Update();
                        if (emitters[emitter].shouldDie)
                        {
                            emitters.RemoveAt(emitter);
                            emitter--;
                        }
                    }
                }
                updateBullets();
                if (heartList.Count > 0)
                {
                    for (int heart = 0; heart < heartList.Count; heart++)
                    {
                        heartList[heart].Update();
                        if (heartList[heart].shouldDie)
                        {
                            heartList.RemoveAt(heart);
                            heart--;
                            pickup.Play(0.8f,0,0);
                        }
                    }
                }
                //game is over, do some shit and goto gameStage 2
                if (player.health <= 0)
                {
                    gameStage = 2;
                    timeWhenDied = (float)time;
                }
            }
            #endregion
            #region game-over
            //this one is gameover screen
            else if (gameStage == 2)
            {

                if (emitters.Count > 0)
                {
                    for (int emitter = 0; emitter < emitters.Count; emitter++)
                    {
                        emitters[emitter].Update();
                        if (emitters[emitter].shouldDie)
                        {
                            emitters.RemoveAt(emitter);
                            emitter--;
                        }
                    }
                }
                //destory all existing hearts too
                List<Texture2D> hearts = new List<Texture2D>();
                hearts.Add(heartParticle);
                for (int h = 0; h < heartList.Count; h++)
                {
                    ParticleEngine heartParticles = new ParticleEngine(hearts, heartList[h].position, this);
                    emitters.Add(heartParticles);
                    heartList.RemoveAt(h);
                    h--;
                }
                //kill all satellites
                for (int e = 0; e < enemyList.Count; e++)
                {
                    particleEngine = new ParticleEngine(particleTextures, enemyList[e].position, this);
                    emitters.Add(particleEngine);
                    enemyList.RemoveAt(e);
                    e--;
                    enemiesKilled++;
                }//kill all UFOs
                for (int u = 0; u < ufoList.Count; u++)
                {
                    particleEngine = new ParticleEngine(particleTextures, ufoList[u].position, this);
                    emitters.Add(particleEngine);
                    ufoList.RemoveAt(u);
                    u--;
                    enemiesKilled++;
                }
                player.health = 100;
                satelliteCreateDelay = 4f;
                ufoCreateDelay = 6f;
                if (timeWhenDied + 5 <= time)
                {
                    gameStage = 0;
                }
                end.Play(0.1f, -.5f,-.5f);
            }
            #endregion
            base.Update(gameTime);
        }
        //-----------------------------------------//
        private void updateBullets()
        {
            if (bulletArray != null)
            {
                for(int b = 0; b < bulletArray.Count; b++)
                {
                    bulletArray[b].Update();
                    bulletArray[b].updateCollision();
                    if (bulletArray[b].shouldDie == true)
                    {
                        bulletArray.RemoveAt(b);
                        b--;
                    }
                }

            }
        }
        private void HandleEnemies()
        {
            //create some random enemies
            if (lastSatelliteSpawn + satelliteCreateDelay<= (float)time)
            {
                Enemy enemy = new Enemy(this, satelliteModel, satelliteTexture);
                enemyList.Add(enemy);
                lastSatelliteSpawn = (float)time;
                if (satelliteCreateDelay > 0.4f)
                    satelliteCreateDelay -= 0.1f;
            }
            if (lastUfoSpawn + ufoCreateDelay <= (float)time)
            {
                Ufo ufo = new Ufo(this, ufoModel, ufoTexture);
                ufoList.Add(ufo);
                lastUfoSpawn = (float)time;
                if (ufoCreateDelay > 0.4f)
                    ufoCreateDelay -= 0.1f;
            }
            //checksatellite updates
            for (int e = 0; e < enemyList.Count; e++ )
            {
                enemyList[e].Update();
                enemyList[e].UpdateCollision(enemyList, e);
                //check for death
                if (enemyList[e].shouldDie)
                {
                    particleEngine = new ParticleEngine(particleTextures, enemyList[e].position, this);
                    emitters.Add(particleEngine);
                    enemyList.RemoveAt(e);
                    e--;
                    enemiesKilled++;
                }
            }
            //check for ufo updates
            for (int u = 0; u < ufoList.Count; u++)
            {
                ufoList[u].Update();
                ufoList[u].UpdateCollision(ufoList, u);
                //check for death
                if (ufoList[u].shouldDie)
                {
                    particleEngine = new ParticleEngine(particleTextures, ufoList[u].position, this);
                    emitters.Add(particleEngine);
                    ufoList.RemoveAt(u);
                    u--;
                    enemiesKilled++;
                }
            }
        }
        //<summary>
        //handles input (dah... -.-)
        //TODO move joystick input to their own classes (done woot)
        //</summary>
        private void HandleInput()
        {
            MouseState currentMouseState = Mouse.GetState();//get mouse state
            mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);//mouse position
            TouchCollection touches = TouchPanel.GetState(); //touchstates blahblah
            //to the good part....
            foreach (var touch in touches)
            {
                //copy touch position to mouse position
                mousePosition = touch.Position;
                //project touch from 2d to 3d world
                Vector3 pos1 = _viewport.Unproject(new Vector3(mousePosition.X, mousePosition.Y, 100), projection, view, world);
                Vector3 pos2 = _viewport.Unproject(new Vector3(mousePosition.X, mousePosition.Y, 0), projection, view, world);
                mouseInWorld = 1000*(pos2 - pos1);
                mouseInWorld.Z = 0; //just clearing that touch plane is always z=0
                
                joystick_left.Update(mousePosition, touch);
                joystick_right.Update(mousePosition, touch);
                //<summary>
                //this one is pretty useless atm, it basicly checks if you touch player
                //without Calculating the players distance to touch, i use it to check for screen touches
                //</summary>
                #region screen touches
                //checking if there is already registered input for player, and that we are looking at is one that is registered for player input
                if (player.isPressed && touch.Id == player.touchID )
                {
                    //if touch state is still moving == not released
                    if (touch.State != TouchLocationState.Moved)
                        player.isPressed = false; //otherwise tell player its not anymore used
                }
                //if player is free atm
                else
                {
                    //check for player touch
                    if (touch.Id != joystick_right.touchID && touch.Id != joystick_left.touchID)
                    {
                        player.isPressed = true;
                        player.touchID = touch.Id; //register current touch id for player use
                    }
                }
                #endregion
            }
        }

        public void RandomBackground(){
            //check if Red value is close enough to randomize it again
            if (background.R == nextBackground.R)
            {
                nextBackground = new Color(random.Next(150,255), nextBackground.G, nextBackground.B);
            }
            else
            {//otherwise move it closer to target
                if (nextBackground.R < background.R)
                    background.R--;
                else
                    background.R++;
            }
            //do same for Green value
            if (background.G == nextBackground.G)
            {
                nextBackground = new Color(nextBackground.R, random.Next(150, 255), nextBackground.B);
            }
            else
            {
                if (nextBackground.G < background.G)
                    background.G--;
                else
                    background.G++;
            }
            //and blue
            if (background.B == nextBackground.B)
            {
                nextBackground = new Color(nextBackground.R, nextBackground.G, random.Next(150, 255));
            }
            else
            {
                if (nextBackground.B < background.B)
                    background.B--;
                else
                    background.B++;
            }
        }
        protected override void Draw(GameTime gameTime)
        {
            _graphics.GraphicsDevice.Clear(background);
            if (gameStage == 0)
            {
                _spriteBatch.Begin();
                //_spriteBatch.Draw(planet, new Rectangle(0, 0, (int)SCREEN_WIDTH, (int)SCREEN_HEIGHT), Color.White); 
                _spriteBatch.DrawString(font, "Awesome SpaceZooter Game", new Vector2(SCREEN_WIDTH / 2 - 275, SCREEN_HEIGHT / 2), Color.Black);
                _spriteBatch.DrawString(font, "tap to start game", new Vector2(SCREEN_WIDTH / 2 - 275, SCREEN_HEIGHT / 2 + 25), Color.Black);
                _spriteBatch.End();
            }
            if (gameStage == 1)
            {
                _spriteBatch.Begin();
                //_spriteBatch.Draw(planet, new Rectangle(0, 0, (int)SCREEN_WIDTH, (int)SCREEN_HEIGHT), Color.White); 
                _spriteBatch.DrawString(font, "Combo: " + combo + "x", new Vector2(50, 50), Color.Black);
                _spriteBatch.DrawString(font, "Score: " + score, new Vector2(50, 75), Color.Black);
                if (combo > 15)
                {
                    if(combo < 50){
                        _spriteBatch.DrawString(font, "get combo over 50 to use THREE (3)  FREKIN cannons! " + score, new Vector2(SCREEN_WIDTH / 2 - 280, 25), Color.Black);
                    }       
                    else
                        _spriteBatch.DrawString(font, "AWESOME! KEEP IT UP, CHAMP!" + score, new Vector2(SCREEN_WIDTH / 2 - 280, 25), Color.Black);
                }else
                    _spriteBatch.DrawString(font, "get combo over 15 to use two (2) cannons " + score, new Vector2(SCREEN_WIDTH / 2 - 280, 25), Color.Black);
                player.healthBar.Draw(_spriteBatch);
                //draw Joysticks
                joystick_right.Draw(_spriteBatch);
                joystick_left.Draw(_spriteBatch);
                _spriteBatch.End();
            }
            if (gameStage == 2)
            {
                _spriteBatch.Begin();
               // _spriteBatch.Draw(planet, new Rectangle(0,0, (int)SCREEN_WIDTH, (int)SCREEN_HEIGHT), Color.White);
                _spriteBatch.DrawString(font, "Game Over, n00b!", new Vector2(SCREEN_WIDTH / 2 - 275, SCREEN_HEIGHT / 2), Color.Black);
                _spriteBatch.DrawString(font, "your pathetic score was " + score, new Vector2(SCREEN_WIDTH / 2 - 280, SCREEN_HEIGHT / 2+25), Color.Black);
                _spriteBatch.End();
            }
            player.Draw(_spriteBatch, font);
            //draw hearts
            if (heartList.Count > 0)
            {
                foreach (HeartPickup h in heartList)
                {
                    h.Draw(this);
                }
            }
            //go and draw each enemy
            foreach (Enemy e in enemyList)
            {
                e.Draw();
            }
            foreach (Ufo u in ufoList)
            {
                u.Draw();
            }
            //draw particles in their own patch
            if (emitters.Count > 0)
            {
                foreach (ParticleEngine p in emitters)
                {
                    p.Draw(_spriteBatch);
                }
            }
            foreach (Bullet b in bulletArray)
            {
                if (b != null)
                    b.Draw();
            }
            base.Draw(gameTime);
        }

        public void DrawModel(Texture2D texture, Model thismodel, Vector3 position, Vector3 scale)
        {
            foreach (ModelMesh mesh in thismodel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.TextureEnabled = true;
                    effect.Texture = texture;
                    effect.World = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);
                    effect.View = view;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }
        }
        #region calculations
        //----calculate the direction of player lookin-------------//
        //based on some guys code in overstackflow, cant remember
        public float LookAt(Vector3 position, Vector3 aimSpot, float currentAngle, float turnSpeed)
        {
            float x = aimSpot.X - position.X;
            float y = aimSpot.Y - position.Y;
            float desiredAngle = (float)Math.Atan2(y, x);
            float difference = WrapAngle(desiredAngle - currentAngle);
            difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);
            return WrapAngle(currentAngle + difference);

        }
        private static float WrapAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }
        #endregion
    }
}
