#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;
#endregion

namespace Puzzle
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        ImageSplitter imageSplitter;

        ImagePart lockedPart;
        bool locked = false;
        MouseState mouseState;
        KeyboardState keyboardState;

        Texture2D image;
        Texture2D background;

        private int rowsAndColumns = 3;

        private int minBlock = 2;
        private int maxBlock = 46;

        private Texture2D[] images;
        private int imagesIterator = -1;

        private bool upPressed = false;
        private bool downPressed = false;
        private bool leftPressed = false;
        private bool rightPressed = false;

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1290;
            graphics.PreferredBackBufferHeight = 810;
            graphics.GraphicsDevice.DepthStencilState.DepthBufferEnable = true;
            graphics.IsFullScreen = false;

            this.IsMouseVisible = true;

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            imageSplitter = new ImageSplitter(image, rowsAndColumns, rowsAndColumns);
            imageSplitter.SplitImage();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            string[] filePaths = Directory.GetFiles(Environment.CurrentDirectory + "\\" + Content.RootDirectory);
            images = new Texture2D[filePaths.Length - 1];
            for (int i = 0; i < filePaths.Length; i++)
            {
                if (Path.GetFileName(filePaths[i]) != "background.jpg")
                {
                    imagesIterator++;
                    images[imagesIterator] = Content.Load<Texture2D>(Path.GetFileName(filePaths[i]));
                }
            }

            background = Content.Load<Texture2D>("background.jpg");
            image = images[0];

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Left))
                leftPressed = true;
            else if(leftPressed && keyboardState.IsKeyUp(Keys.Left))
            { 
                if(rowsAndColumns > minBlock)
                {
                    rowsAndColumns--;
                    imageSplitter.SetGrid(rowsAndColumns);
                }

                leftPressed = false;
            }

            if (keyboardState.IsKeyDown(Keys.Right))
                rightPressed = true;
            else if(rightPressed && keyboardState.IsKeyUp(Keys.Right))
            {
                if (rowsAndColumns < maxBlock)
                {
                    rowsAndColumns++;
                    imageSplitter.SetGrid(rowsAndColumns);  
                }

                rightPressed = false;
            }

            if (keyboardState.IsKeyDown(Keys.Up))
                downPressed = true;
            else if (downPressed && keyboardState.IsKeyUp(Keys.Up))
            {
                if (imagesIterator < images.Length -1)
                {
                    imagesIterator++;
                    image = images[imagesIterator];
                }

                downPressed = false;
            }

            if (keyboardState.IsKeyDown(Keys.Down))
                upPressed = true;
            else if (upPressed && keyboardState.IsKeyUp(Keys.Down))
            {
                if (imagesIterator > 0)
                {
                    imagesIterator--;
                    image = images[imagesIterator];
                }

                upPressed = false;
            }

            mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (!locked)
                {
                    foreach (ImagePart imagePart in imageSplitter.getImageParts())
                    {
                        Rectangle bBox = imagePart.GetDestinationRectangle();
                        if (bBox.Contains(mouseState.X, mouseState.Y) && !imagePart.animate)
                        {
                            lockedPart = imagePart;
                            lockedPart.LockedOnMouse = true;
                            locked = true;
                            break;
                        }
                    }
                }
            }
            else if (mouseState.LeftButton == ButtonState.Released && locked)
            {
                foreach (ImagePart imagePart in imageSplitter.getImageParts())
                {
                    if (!imagePart.animate && lockedPart.ID != imagePart.ID && imagePart.GetDestinationRectangle().Contains(mouseState.X, mouseState.Y)
                        && lockedPart.GetDestinationRectangle().Intersects(imagePart.GetDestinationRectangle())
                        )
                    {
                        Vector2 temp = lockedPart.Location;
                        lockedPart.UpdateDestinationAndLocation(imagePart.Location);
                        imagePart.UpdateDestinationAndLocation(temp);
                        locked = false;
                        break;
                    }

                    if (locked)
                    {
                        lockedPart.UpdateDestinationAndLocation(lockedPart.Location);                        
                        locked = false;
                    }
                }

                lockedPart.LockedOnMouse = false;
            }

            if (locked)
            {
                Rectangle bBox = lockedPart.GetDestinationRectangle();
                lockedPart.UpdateDestinationRectangle(new Vector2(mouseState.X - bBox.Width / 2, mouseState.Y - bBox.Height / 2));
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
            foreach (ImagePart imagePart in imageSplitter.getImageParts())
            {
                imagePart.Update(gameTime);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(ClearOptions.DepthBuffer | ClearOptions.Target,
            Color.Black, 1.0f, 0);
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            spriteBatch.Draw(background, new Rectangle(0, 0, 1280, 800), new Rectangle(0, 0, 1280, 800), Color.White * 0.5f, 0, Vector2.Zero, SpriteEffects.None, 0);

            foreach (ImagePart imagePart in imageSplitter.getImageParts())
            {
                if(!imagePart.animate && !imagePart.LockedOnMouse)
                    spriteBatch.Draw(image, imagePart.GetDestinationRectangle(), imagePart.GetSourceRectangle(), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);
                else
                    spriteBatch.Draw(image, imagePart.GetDestinationRectangle(), imagePart.GetSourceRectangle(), Color.White, 0, Vector2.Zero, SpriteEffects.None, 1);
            }

            // TODO: Add your drawing code here
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
