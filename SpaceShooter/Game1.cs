﻿using System;
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
        private Effect _blurEffect;
        public Random random = new Random();
        public SpriteFont font; //font for debugging
        public Player player = new Player(); //create a new player
        public Model bullet = new Model();
        public Texture2D bulletTexture;
        public Texture2D jsTexture;
        public Enemy enemy;
        public List<Enemy> enemyList = new List<Enemy>();
        public Joystick joystick_right = new Joystick();
        public Joystick joystick_left = new Joystick();//create a joystick to move player
        public Matrix world = Matrix.CreateTranslation(new Vector3(0, 0, 0)); //world cordinates lol
        public Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 100), new Vector3(0, 0, 0), Vector3.UnitY); //creates look at view for camera
        public Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.1f, 100f); //calculate projection
        //find screen height and width
        public float SCREEN_HEIGHT, SCREEN_WIDTH;
        public float enemyCreateDelay;
        //find mouse cordinates in world, vectors for left-up and right down corners in world space
        public Vector3 mouseInWorld, upLeft, downRight;
        public Vector2 mousePosition; //vector for mouse position in screen-space
        public double time; //used to estimate time
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
            player.Health = 100;
            base.Initialize(); //init base of monogame
            
        }
        //-------------------Loading Content-----------------------//
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            player.model = Content.Load<Model>("spaceship");//Load player model from Content
            player.texture = Content.Load<Texture2D>("spaceship_uv");//load player texture from Content
            bullet = Content.Load<Model>("lazeh");

            bulletTexture = Content.Load<Texture2D>("lazeh_uv");
            font = Content.Load<SpriteFont>("font"); //load dummy font for debugging
            jsTexture = Content.Load<Texture2D>("joystick");//load joystick texture from Content
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
            //get screen width and height
            SCREEN_HEIGHT = _viewport.Height;
            SCREEN_WIDTH = _viewport.Width;
            //update player
            player.position.X -= joystick_right.dir.X*player.speed;
            player.position.Y += joystick_right.dir.Y*player.speed;
            //check for borders
            Vector3 var1 = _viewport.Unproject(new Vector3(0,0,0), projection, view, world);//these two first are finding what cordinates in world space are current (0,0)
            Vector3 var2 = _viewport.Unproject(new Vector3(0, 0, 100), projection, view, world);//meaning the left up corner
            Vector3 var3 = _viewport.Unproject(new Vector3(SCREEN_WIDTH, SCREEN_HEIGHT, 0), projection, view, world);//and these two are doing the same for right down corner
            Vector3 var4 = _viewport.Unproject(new Vector3(SCREEN_WIDTH, SCREEN_HEIGHT, 100), projection, view, world);
            upLeft = 1000*(var1 - var2);//create Vector3 for up_left corner
            downRight = 1000 * (var3 - var4);//create Vector3 for down_right corner

            //clamp player position within borders
            player.position.X = MathHelper.Clamp(player.position.X, upLeft.X,downRight.X);
            player.position.Y = MathHelper.Clamp(player.position.Y, downRight.Y, upLeft.Y);
            HandleEnemies();
            player.Update(this);//position is not done, update it...
            base.Update(gameTime);
        }
        //-----------------------------------------//
        private void HandleEnemies()
        {
            //create some random enemies
            if (enemyCreateDelay + 0.5f <= (float)time)
            {
                Enemy enemy = new Enemy(this);
                enemyList.Add(enemy);
                enemyCreateDelay = (float)time;
            }
            //check enemy updates
            if (enemyList.Count >= 1)
            {
                for (int i = 0; i < enemyList.Count; i++)
                {
                    enemyList[i].Update();
                    enemyList[i].UpdateCollision(enemyList, i);
                }
            }
            //check for death
            foreach (Enemy e in enemyList)
            {
                if (e.shouldDie)
                {
                    enemyList.Remove(e);
                    break;
                }
            }
        }
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
                #region Right joystick touches
                joystick_right.normal = Vector2.Normalize(Vector2.Subtract(joystick_right.anchorPos, touch.Position)); //normal is vector of where joystick is pointing
                //check if user is moving joystick
                if (joystick_right.anchorPos != joystick_right.position)
                {
                    //count the direction
                    joystick_right.dir = Vector2.Normalize(Vector2.Subtract(joystick_right.anchorPos, joystick_right.position));
                }
                else
                {
                    //otherwise moving direction is Zero
                    joystick_right.dir = Vector2.Zero;
                }
                //check the overlapping touches for player and joystick, joystick always wins
                if (joystick_right.touchID == player.touchID)
                {
                    joystick_right.touchID = -1;
                    player.isPressed = false;
                    joystick_right.isPressed = false;
                }
                //if joystick is in use....
                //check if touch that we are checking now is indeed touch that is registered for joystick
                if (joystick_right.isPressed && touch.Id == joystick_right.touchID)
                {
                    //check if touchs state is still moving
                    if (touch.State != TouchLocationState.Released)
                    {
                        //handle joystick position. if movement isnt too far away from anchor
                        if ((float)Vector2.Subtract(mousePosition, joystick_right.anchorPos).Length() < 100f)
                            joystick_right.position = touch.Position;
                        //otherwise dont move it too far away
                        else
                            joystick_right.position = Vector2.Add(joystick_right.anchorPos, joystick_right.normal * -100f);
                    }
                    //if touch is registered for joystick, but has been released
                    else
                    {
                            joystick_right.isPressed = false; //joystick is not in use anymore, looking for new touchID
                            joystick_right.position = joystick_right.anchorPos; //return joystick to its anchor
                    }
                }
                //if joystick isn´t already in use...
                else
                {
                    if (!joystick_right.isPressed)
                    {
                        //check if current touch is close enough to joystic, doenst matter if its in use of player
                        if (Math.Abs(mousePosition.X - joystick_right.position.X) <= 30
                            && Math.Abs(mousePosition.Y - joystick_right.position.Y) <= 30)
                        {
                            joystick_right.isPressed = true;
                            joystick_right.touchID = touch.Id; //register this touch for joystick
                        }
                        //otherwise keep the joystick in anchor, if its not there already...
                        else
                            joystick_right.position = joystick_right.anchorPos;
                    }
                }
                #endregion
                #region Left joystick touches
                joystick_left.normal = Vector2.Normalize(Vector2.Subtract(joystick_left.anchorPos, touch.Position)); //normal is vector of where joystick is pointing
                //check if user is moving joystick
                if (joystick_left.anchorPos != joystick_left.position)
                {
                    //count the direction
                    joystick_left.dir = Vector2.Normalize(Vector2.Subtract(joystick_left.anchorPos, joystick_left.position));
                }
                else
                {
                    //otherwise moving direction is Zero
                    joystick_left.dir = Vector2.Zero;
                }
                //check the overlapping touches for player and joystick, joystick always wins
                if (joystick_left.touchID == player.touchID)
                {
                    joystick_left.touchID = -1;
                    player.isPressed = false;
                    joystick_left.isPressed = false;
                }
                //if joystick is in use....
                //check if touch that we are checking now is indeed touch that is registered for joystick
                if (joystick_left.isPressed && touch.Id == joystick_left.touchID)
                {
                    //check if touchs state is still moving
                    if (touch.State != TouchLocationState.Released)
                    {
                        //handle joystick position. if movement isnt too far away from anchor
                        if ((float)Vector2.Subtract(mousePosition, joystick_left.anchorPos).Length() < 100f)
                            joystick_left.position = touch.Position;
                        //otherwise dont move it too far away
                        else
                            joystick_left.position = Vector2.Add(joystick_left.anchorPos, joystick_left.normal * -100f);
                    }
                    //if touch is registered for joystick, but has been released
                    else
                    {
                        joystick_left.isPressed = false; //joystick is not in use anymore, looking for new touchID
                        joystick_left.position = joystick_left.anchorPos; //return joystick to its anchor
                    }
                }
                //if joystick isn´t already in use...
                else
                {
                    if (!joystick_left.isPressed)
                    {
                        //check if current touch is close enough to joystic, doenst matter if its in use of player
                        if (Math.Abs(mousePosition.X - joystick_left.position.X) <= 30
                            && Math.Abs(mousePosition.Y - joystick_left.position.Y) <= 30)
                        {
                            joystick_left.isPressed = true;
                            joystick_left.touchID = touch.Id; //register this touch for joystick
                        }
                        //otherwise keep the joystick in anchor, if its not there already...
                        else
                            joystick_left.position = joystick_left.anchorPos;
                    }
                }
                #endregion
                #region player touches
                //checking if there is already registered input for player, and that we are looking at is one that is registered for player input
                if (player.isPressed && touch.Id == player.touchID )
                {
                    //if touch state is still moving == not released
                    if (touch.State == TouchLocationState.Moved)
                        player.aimSpot = mouseInWorld; //move aimspot to current touch location in world space
                    else
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

        protected override void Draw(GameTime gameTime)
        {
            _graphics.GraphicsDevice.Clear(Color.White);
            _spriteBatch.Begin();
            _spriteBatch.DrawString(font, "mouse pos " + mousePosition, new Vector2(50, 25), Color.Black);
            _spriteBatch.DrawString(font, "mouse in world: " + mouseInWorld, new Vector2(50,50), Color.Black);
            _spriteBatch.DrawString(font, "player pos " + player.position, new Vector2(50, 75), Color.Black);
            _spriteBatch.DrawString(font, "joystick" + Vector2.Subtract(joystick_right.anchorPos, joystick_right.position).Length(), new Vector2(50, 100), Color.Black);
            _spriteBatch.DrawString(font, "joystick anchor" + joystick_right.anchorPos, new Vector2(50, 125), Color.Black);
            _spriteBatch.DrawString(font, "joystic pos" + joystick_right.position, new Vector2(50, 150), Color.Black);
            _spriteBatch.DrawString(font, "border0" + upLeft, new Vector2(50, 175), Color.Black);
            _spriteBatch.DrawString(font, "border1" + downRight, new Vector2(50, 200), Color.Black);
            if(player.bulletArray.Count >= 1)
                _spriteBatch.DrawString(font, "last Bullet position " + player.bulletArray[player.bulletArray.Count-1].position, new Vector2(50, 225), Color.Black);
            _spriteBatch.DrawString(font, "angle " + player.angle, new Vector2(50, 250), Color.Black);
            _spriteBatch.DrawString(font, "Player Health" + player.Health, new Vector2(50, 275), Color.Black);
            _spriteBatch.End();
            foreach (Enemy e in enemyList)
            {
                e.Draw();
            }
            base.Update(gameTime);
            player.Draw(_spriteBatch, font);
            joystick_right.Draw(_spriteBatch);
            joystick_left.Draw(_spriteBatch);
            base.Draw(gameTime);
        }
        //----calculate the direction of player lookin-------------//
        #region calculations
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
