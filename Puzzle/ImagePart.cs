using Microsoft.Xna.Framework;
using System;

namespace Puzzle
{

    class ImagePart
    {
        private Rectangle SourceRectangle;
        private Rectangle DestinationRectangle;

        private int  Width;
        private int Height;

        private float SPEED = 500;
        private Vector2 Velocity;

        public Vector2 Location { get; set; }
        public Vector2 NewLocation { get; set; }

        public int ID { get; set;  }

        public bool animate{ get; set; }

        public bool LockedOnMouse { get; set; }

        public ImagePart(int width, int height, int rows, int columns, Vector2 location, int id)
        {
            animate = false;
            ID = id;
            Location = location;
            Width = width;
            Height = height;
            SourceRectangle = new Rectangle(width * columns, height * rows, width, height);
            DestinationRectangle = new Rectangle((int)Location.X, (int)Location.Y, width, height);
        }

        public void UpdateDestinationAndLocation(Vector2 location)
        {
            NewLocation = location;

            calculateDistance();
            animate = true;
        }

        private void calculateDistance()
        {
            float componentX = NewLocation.X - Location.X;
            float componentY = NewLocation.Y - Location.Y;
            float distance = (float)Math.Sqrt(componentX * componentX + componentY * componentY);
            Velocity = new Vector2(componentX / distance * SPEED, componentY / distance * SPEED);
        }

        public void UpdateDestinationRectangle(Vector2 location)
        {
            DestinationRectangle = new Rectangle((int)location.X, (int)location.Y, Width, Height);
        }

        public void Update(GameTime gameTime)
        {
            if (animate)
            {
                float componentX = (float)(Location.X + Velocity.X * gameTime.ElapsedGameTime.TotalSeconds);
                float componentY = (float)(Location.Y + Velocity.Y * gameTime.ElapsedGameTime.TotalSeconds);

                float locationXDif = NewLocation.X - Location.X;
                float locationYDif = NewLocation.Y - Location.Y;
                if (Math.Abs(locationXDif) <= Width / 5 && Math.Abs(locationYDif) <= Height / 5)
                {
                    Location = NewLocation;
                    DestinationRectangle = new Rectangle((int)Location.X, (int)Location.Y, Width, Height);
                    animate = false;
                }
                else
                {
                    Location = new Vector2(componentX, componentY);
                    DestinationRectangle = new Rectangle((int)Location.X, (int)Location.Y, Width, Height);
                }
            }

        }

        public Rectangle GetSourceRectangle()
        {
            return SourceRectangle;     
        }

        public Rectangle GetDestinationRectangle()
        {
            return DestinationRectangle;
        }

    }
}
