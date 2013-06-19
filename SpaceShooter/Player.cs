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
    public class Player : Game
    {
        //---------------variables and shitah----------------------//
        #region variables
        protected Game1 game; //instancing the Game1
        public Model model; //player model
        public Texture2D texture; //player model texture
        public Vector3 position, aimSpot; //position vector and vector for position where to look at
        public Boolean isPressed = false; //check if player is pressing touchscreen, used in shooting and rotation
        public int touchID = -1; //ID for witch touch is used in player actions
        public float speed = 1f; //player speed
        public float turnSpeed = 0.2f; //speed in witch player can turn
        public float angle = 0.0f; //players current angle
        public BoundingBox physicsBody; //bounding box variables for player and dummy
        public double lastShot; //time after last time player shot
        public List<Bullet> bulletArray = new List<Bullet>(); //array for all bullet projectiles
        public float Health;
        #endregion
        //---------------main update for player--------------------//
        public void Update(Game1 g)
        {
            //instance game1
            game = g;
            //calculate speed
            speed =  Vector2.Subtract(game.joystick_right.anchorPos, game.joystick_right.position).Length()/100;
            //create boundingbox for player and randomcube //TESTS//
            physicsBody = new BoundingBox(new Vector3(position.X - 5, position.Y - 5, position.Y - 5), new Vector3(position.X + 5, position.Y + 5, position.Y + 5));
            //if shooting == touching but not moving joystick
            if (game.joystick_left.isPressed)
            {
                angle = game.LookAt(new Vector3(game.joystick_left.anchorPos.X, -game.joystick_left.anchorPos.Y, 0),
                    new Vector3(game.joystick_left.position.X, -game.joystick_left.position.Y, 0), angle, turnSpeed);
                if (game.time >= lastShot+0.3f)
                {
                    Shoot();
                    lastShot = game.time;
                }
            }

            updateBullets();
        }
        //-----------------calculate shooting action---------------//
        #region shooting and bullets
        public void Shoot()
        {
            if (0.5 >= (angle - game.LookAt(position, aimSpot, angle, turnSpeed)))
            {
                Bullet bullet = new Bullet(position, Vector3.Normalize(new Vector3(game.joystick_left.dir.X, -game.joystick_left.dir.Y, 0)));
                bullet.Initialize(game, angle);
                if (bullet != null)
                    bulletArray.Add(bullet);
            }
        }
        private void updateBullets()
        {
            if (bulletArray != null)
            {
                foreach (Bullet b in bulletArray)
                {
                    b.Update();
                    if (b.shouldDie == true)
                    {
                        bulletArray.Remove(b);
                        break;
                    }
                }

            }
        }
        #endregion
        //------------Drawing functions for player-----------------//
        #region drawing
        public void Draw(SpriteBatch _spriteBatch, SpriteFont font)
        {
            game.DrawModel(texture, model, position, angle);

            foreach (Bullet b in bulletArray)
            {
                if (b != null)
                    b.Draw();
            }
        }
        #endregion
    }
}
