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
        public float health;
        public HealthBar healthBar;
        private int gunStage;
        private int machinegunSides = 0;
        private bool shield = false;
        public float shieldHealth = 0;
        private float angleS;

        public Vector3 gunPosition;
        #endregion
        //---------------main update for player--------------------//
        public void Initialize(Game1 g)
        {
            //instance game1
            game = g;
            //ceate a healthbar for player
            health = 100;
            healthBar = new HealthBar(health, new Vector2(game.SCREEN_WIDTH / 2 - (game.healthBarTex.Width * health) / 4, game.SCREEN_HEIGHT - 50), game, game.healthBarTex);
            position = new Vector3(0,5,0);
        }
        public void Update()
        {
            if (health > 100)
            {
                shield = true;
                health--;
                shieldHealth++;
                if (shieldHealth > 100)
                    shieldHealth = 100;
            }
            else if(health < 100)
            {
                if (shieldHealth > 0)
                {
                    shieldHealth--;
                    health++;
                }
                else
                {
                    shield = false;
                }
            }

            //calculate speed
            speed =  Vector2.Subtract(game.joystick_right.anchorPos, game.joystick_right.position).Length()/100;
            //create boundingbox for player and randomcube //TESTS//
            physicsBody = new BoundingBox(new Vector3(position.X - 5, position.Y - 5, position.Y - 5), new Vector3(position.X + 5, position.Y + 5, position.Y + 5));
            //if shooting == touching but not moving joystick
            if (game.joystick_left.isPressed)
            {
                angle = game.LookAt(new Vector3(game.joystick_left.anchorPos.X, -game.joystick_left.anchorPos.Y, 0),
                                    new Vector3(game.joystick_left.position.X, -game.joystick_left.position.Y, 0), angle, turnSpeed);
                Shoot();
            }
            else if (game.mouseClicking)
            {
                angle = game.LookAt(position,game.mouseInWorld, angle, turnSpeed);
                Shoot();
            }
            position.Z = 0;
            healthBar.Update(health);
            if (game.combo < 15){
                gunStage = 1;
            }else
            {
                if (game.combo > 50)
                    gunStage = 3;
                else
                    gunStage = 2;
            }

        }
        //-----------------calculate shooting action---------------//
        #region shooting and bullets
        public void Shoot()
        {
            //if cannon
            if (gunStage == 1)
            {
                if (game.time >= lastShot + 0.3f)
                {
                    if (0.5 >= (angle - game.LookAt(position, aimSpot, angle, turnSpeed)) && (game.joystick_left.position-game.joystick_left.anchorPos).Length() > 0.8f)
                    {
                        Bullet bullet = new Bullet(position, Vector3.Normalize(new Vector3(game.joystick_left.dir.X, -game.joystick_left.dir.Y, 0)), false, true);
                        bullet.Initialize(game, angle);
                        if (bullet != null)
                            game.bulletArray.Add(bullet);
                        game.lazer.Play(0.1f, 0, 0);
                        lastShot = game.time;
                    }
                    else if (game.mouseClicking)
                    {
                        Bullet bullet = new Bullet(position, Vector3.Normalize(position - game.mouseInWorld), false, true);
                        bullet.Initialize(game, angle);
                        if (bullet != null)
                            game.bulletArray.Add(bullet);
                        game.lazer.Play(0.1f, 0, 0);
                        lastShot = game.time;
                    }
                }
            }//else we have machinegun
            else if (gunStage == 2)
            {
                if (game.time >= lastShot + 0.1f)
                {   //cannot use aimspot, messes up if not moving
                    gunPosition = new Vector3(1.5f,1.5f,0);
                    if(machinegunSides == 0){
                        gunPosition = position - Vector3.Transform(gunPosition, Matrix.CreateRotationZ(angle - 2*MathHelper.ToRadians(45)));
                        machinegunSides = 1;
                    }
                    else{
                        gunPosition = position - Vector3.Transform(gunPosition, Matrix.CreateRotationZ(angle + MathHelper.ToRadians(45)));
                        machinegunSides = 0;
                    }
                    Bullet bullet = new Bullet(gunPosition, Vector3.Normalize(new Vector3(game.joystick_left.dir.X, -game.joystick_left.dir.Y, 0)), false, true);
                    bullet.Initialize(game, angle);
                    if (bullet != null)
                        game.bulletArray.Add(bullet);
                    game.lazer.Play(0.1f, 0, 0);
                    lastShot = game.time;
                }
            }//else we have 3 cannons, awesome....
            else if (gunStage == 3)
            {
                if (game.time >= lastShot + .06f)
                {   //cannot use aimspot, messes up if not moving
                    gunPosition = new Vector3(1.5f, 1.5f, 0);
                    if (machinegunSides == 0)
                    {
                        gunPosition = position - Vector3.Transform(gunPosition, Matrix.CreateRotationZ(angle - 2 * MathHelper.ToRadians(45)));
                        machinegunSides = 1;
                    }
                    else if(machinegunSides == 1)
                    {
                        gunPosition = position - Vector3.Transform(gunPosition, Matrix.CreateRotationZ(angle + MathHelper.ToRadians(45)));
                        machinegunSides = 2;
                    }
                    else
                    {
                        gunPosition = position;
                        machinegunSides = 0;
                    }
                    Bullet bullet = new Bullet(gunPosition, Vector3.Normalize(new Vector3(game.joystick_left.dir.X, -game.joystick_left.dir.Y, 0)), false, true);
                    bullet.Initialize(game, angle);
                    if (bullet != null)
                        game.bulletArray.Add(bullet);
                    game.lazer.Play(0.1f, 0, 0);
                    lastShot = game.time;
                }
            }
        }
        #endregion
        //------------Drawing function for player-----------------//
        #region drawing
        public void Draw(SpriteBatch _spriteBatch, SpriteFont font)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.TextureEnabled = true;
                    effect.Texture = texture;
                    effect.World = Matrix.CreateScale(1, .9f, 1) * Matrix.CreateRotationZ(angle) * Matrix.CreateTranslation(position);
                    effect.View = game.view;
                    effect.Projection = game.projection;
                }
                mesh.Draw();
            }
            if (shield == true)
            {
                foreach (ModelMesh mesh in game.shieldModel.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.TextureEnabled = true;
                        effect.Texture = game.shieldTexture;
                        angleS += .05f * (float)game.random.NextDouble();
                        effect.World = Matrix.CreateRotationX(angleS) * Matrix.CreateRotationZ(angleS) * Matrix.CreateRotationY(angleS) * Matrix.CreateScale(4, 4, 4);
                        effect.World *= Matrix.CreateTranslation(position);
                        effect.View = game.view;
                        effect.Projection = game.projection;
                    }
                    mesh.Draw();
                }
            }
        }
        #endregion
    }
}
