using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpaceShooter;
using Microsoft.Xna.Framework.Input.Touch;
//<summary>
//joystick class
//TODO handle joystick touches here, as a listener or something.
//</summary>
namespace SpaceShooter
{
    //JOYSTICK
    public class Joystick
    {
        public Game1 game;
        public Texture2D jsTexture;
        public Texture2D jsBackgroundTex;
        public Vector2 anchorPos;
        public Vector2 position;
        public Vector2 normal;
        public Vector2 dir;
        public int touchID;
        public Boolean isPressed = false;


        public void Initialize(Game1 g, Vector2 pos)
        {
            game = g;
            position = pos;
            anchorPos = pos;
        }
        public void Update(Vector2 mousePosition, TouchLocation touch)
        {
            #region Right joystick touches
            normal = Vector2.Normalize(Vector2.Subtract(anchorPos, touch.Position)); //normal is vector of where joystick is pointing
            //check if user is moving joystick
            if (anchorPos != position)
            {
                //count the direction
                dir = Vector2.Normalize(Vector2.Subtract(anchorPos, position));
            }
            else
            {
                //otherwise moving direction is Zero
                dir = Vector2.Zero;
            }
            //check the overlapping touches for player and joystick, joystick always wins
            if (touchID == game.player.touchID)
            {
                touchID = -1;
                game.player.isPressed = false;
                isPressed = false;
            }
            //if joystick is in use....
            //check if touch that we are checking now is indeed touch that is registered for joystick
            if (isPressed && touch.Id == touchID)
            {
                //check if touchs state is still moving
                if (touch.State != TouchLocationState.Released)
                {
                    //handle joystick position. if movement isnt too far away from anchor
                    if ((float)Vector2.Subtract(mousePosition, anchorPos).Length() < 100f)
                        position = touch.Position;
                    //otherwise dont move it too far away
                    else
                        position = Vector2.Add(anchorPos, normal * -100f);
                }
                //if touch is registered for joystick, but has been released
                else
                {
                    isPressed = false; //joystick is not in use anymore, looking for new touchID
                    position = anchorPos; //return joystick to its anchor
                }
            }
            //if joystick isn´t already in use...
            else
            {
                if (!isPressed)
                {
                    //check if current touch is close enough to joystic, doenst matter if its in use of player
                    if (Math.Abs(mousePosition.X - position.X) <= 80
                        && Math.Abs(mousePosition.Y - position.Y) <= 80)
                    {
                        isPressed = true;
                        touchID = touch.Id; //register this touch for joystick
                    }
                    //otherwise keep the joystick in anchor, if its not there already...
                    else
                        position = anchorPos;
                }
            }
            #endregion
        }
        public void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(game.jsBackgroundTex, new Vector2(anchorPos.X - 100, anchorPos.Y - 100), Color.White);
            _spriteBatch.Draw(game.jsTexture, new Vector2(position.X-50, position.Y-50), Color.White);
        }
    }
}
