using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using SpaceShooter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Input;
namespace SpaceShooter
{
    class Player : Game
    {
        //---------------variables and shitah----------------------//
        #region variables
        protected Game1 game; //instancing the Game1
        public Model model; //player model
        public Texture2D texture; //player model texture
        public Vector3 position, aimSpot; //position vector and vector for position where to look at
        private Matrix RotationMatrix; //rotation matrix...
        public Boolean isPressed = false; //check if player is pressing touchscreen, used in shooting and rotation
        public int touchID = -1; //ID for witch touch is used in player actions
        public float speed = 1f; //player speed
        public float turnSpeed = 0.75f; //speed in witch player can turn
        float angle = 0.0f; //players current angle
        public BoundingBox physicsBody, randomCube; //bounding box variables for player and dummy
        protected Vector3 cubePos = new Vector3(0,20,0); //dummy box position //TESTS
        public int intersect; //turn 0 - 1 if dummy and player are touching. why is this not boolean...
        public double lastShot; //time after last time player shot
        private List<Bullet> bulletArray; //array for all bullet projectiles
        #endregion
        //---------------main update for player--------------------//
        public void Update(Game1 g)
        {
            //instance game1 and joystick
            game = g;
            //calculate speed
            speed =  Vector2.Subtract(game.joystick.anchorPos, game.joystick.position).Length()/100;
            //create boundingbox for player and randomcube //TESTS//
            physicsBody = new BoundingBox(new Vector3(position.X - 5, position.Y - 5, position.Y - 5), new Vector3(position.X + 5, position.Y + 5, position.Y + 5));
            randomCube = new BoundingBox(cubePos - new Vector3(10, 10, 10), cubePos + new Vector3(10, 10, 10));
            //if shooting == touching but not moving joystick
            if (isPressed)
            {
                angle = LookAt(position, aimSpot, angle, turnSpeed);
                if (game.time >= lastShot+1)
                {
                    Shoot();
                    lastShot = game.time;
                }
            }
            
            //check for collisions
            if (physicsBody.Intersects(randomCube))
            {
                //do something here
                intersect = 1;
            }
            else
            {
                //do something else here
                intersect = 0;
            }
            updateBullets();
        }
        //-----------------calculate shooting action---------------//
        #region shooting and bullets
        public void Shoot()
        {
            //create new projective of bullet
            //push it in bullet array
        }
        private void updateBullets()
        {
            //foreach(Bullet bullet in bulletArray)
            //{
            //    //somethin awesome here
            //}
        }
        #endregion
        //----calculate the direction of player lookin-------------//
        #region calculations
        protected float LookAt(Vector3 position, Vector3 aimSpot, float currentAngle, float turnSpeed)
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
        //------------Drawing functions for player-----------------//
        #region drawing
        public void Draw(SpriteBatch _spriteBatch, SpriteFont font)
        {
            _spriteBatch.Begin();
            _spriteBatch.End();
            DrawModel(texture); 
        }
        //draw model function
        public void DrawModel(Texture2D texture)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    RotationMatrix = RotationMatrix * Matrix.CreateFromAxisAngle(RotationMatrix.Up, MathHelper.ToRadians(1.0f));
                    Matrix[] absoluteBoneTransform = new Matrix[model.Bones.Count];
                    model.CopyAbsoluteBoneTransformsTo(absoluteBoneTransform);
                    effect.TextureEnabled = true;
                    effect.Texture = texture;
                    effect.World = Matrix.CreateRotationZ(angle + MathHelper.ToRadians(-90f)) * Matrix.CreateTranslation(position);
                    effect.View = game.view;
                    effect.Projection = game.projection;
                }

                mesh.Draw();
            }
        }
        #endregion
    }
}
