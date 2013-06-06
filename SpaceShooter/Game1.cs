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
namespace SpaceShooter
{
    public class Game1 : Game
    {
        #region Variables
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        Viewport _viewport;
        SpriteFont font;

        Player player = new Player();
        Joystick joystick = new Joystick();

        public Matrix world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        public Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 100), new Vector3(0, 0, 0), Vector3.UnitY);
        public Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.1f, 100f);
        //find screen height and width
        public float SCREEN_HEIGHT;
        public float SCREEN_WIDTH;
        //find mouse cordinates in world
        private Vector3 mouseInWorld = new Vector3();
        public Vector2 mousePosition;
        #endregion
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            _viewport = _graphics.GraphicsDevice.Viewport;
            SCREEN_HEIGHT = _viewport.Height;
            SCREEN_WIDTH = _viewport.Width;
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), SCREEN_WIDTH / SCREEN_HEIGHT, 0.1f, 1000f);
            IsMouseVisible = true;

            TouchPanel.EnabledGestures =
                GestureType.Hold |
                GestureType.Tap |
                GestureType.DoubleTap |
                GestureType.FreeDrag |
                GestureType.Flick |
                GestureType.Pinch;

            joystick.Initialize(SCREEN_HEIGHT, SCREEN_WIDTH);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            player.model = Content.Load<Model>("spaceship");
            player.texture = Content.Load<Texture2D>("spaceship_uv");
            font = Content.Load<SpriteFont>("font");
            joystick.jsTexture = Content.Load<Texture2D>("joystick");
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            //err... handle touch input?
            HandleInput();
            //get screen width and height
            SCREEN_HEIGHT = _viewport.Height;
            SCREEN_WIDTH = _viewport.Width;
            //update player
            player.position.X -= joystick.dir.X*player.speed;
            player.position.Y += joystick.dir.Y*player.speed;
            player.Update(SCREEN_WIDTH, SCREEN_HEIGHT, world, view, projection, mouseInWorld, joystick);
            joystick.Update(SCREEN_HEIGHT, SCREEN_WIDTH, mousePosition);
            
            base.Update(gameTime);

        }

        private void HandleInput()
        {
            MouseState currentMouseState = Mouse.GetState();
            mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);
            
            TouchCollection touches = TouchPanel.GetState();

            foreach (var touch in touches)
            {
                mousePosition = touch.Position;
                Vector3 pos1 = _viewport.Unproject(new Vector3(mousePosition.X, mousePosition.Y, 100), projection, view, world);
                Vector3 pos2 = _viewport.Unproject(new Vector3(mousePosition.X, mousePosition.Y, 0), projection, view, world);
                mouseInWorld = 1000*(pos2 - pos1);
                mouseInWorld.Z = 0;
                #region player touches
                if (player.isPressed)
                {
                    if (touch.Id == player.touchID && touch.State == TouchLocationState.Moved)
                    {
                        //player.position = mouseInWorld;
                    }
                    else
                    {
                        player.isPressed = false;
                    }
                }
                else
                {
                    //check for player touch
                    if (Math.Abs(mouseInWorld.X - player.position.X) <= 2
                        && Math.Abs(mouseInWorld.Y - player.position.Y) <= 2)
                    {
                        if (touch.Id != joystick.touchID)
                        {
                            player.isPressed = true;
                            player.touchID = touch.Id;
                        }
                    }
                }
                #endregion
                #region joystick touches
                if (joystick.isPressed)
                {
                    if (touch.Id == joystick.touchID && touch.State == TouchLocationState.Moved)
                    {
                        //handle joystick position
                        if ((float)Vector2.Subtract(mousePosition, joystick.anchorPos).Length() < 100f)
                            joystick.position = mousePosition;
                        else
                        {
                            joystick.position = Vector2.Add(joystick.anchorPos, joystick.normaali * -100f);
                        }
                    }
                    else
                    {
                        joystick.isPressed = false;
                        joystick.position = joystick.anchorPos;
                    }
                }
                else
                {
                   //check for joystick touch
                    if (Math.Abs(mousePosition.X - joystick.position.X) <= 20
                        && Math.Abs(mousePosition.Y - joystick.position.Y) <= 20)
                    {
                        if (touch.Id != player.touchID)
                        {
                            joystick.isPressed = true;
                            joystick.touchID = touch.Id;
                        }
                    }
                    else
                    {
                        joystick.position = joystick.anchorPos;
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
            //_spriteBatch.DrawString(font, "joystick" + Vector2.Subtract(joystick.anchorPos, joystick.position).Length(), new Vector2(50, 100), Color.Black);
            _spriteBatch.DrawString(font, "joystick anchor" + joystick.anchorPos, new Vector2(50, 125), Color.Black);
            _spriteBatch.DrawString(font, "joystic pos" + joystick.position, new Vector2(50, 150), Color.Black);
            
            _spriteBatch.End();
            joystick.Draw(_spriteBatch);
            player.Draw(_spriteBatch, font);
            base.Draw(gameTime);
 
        }
       
    }
}
