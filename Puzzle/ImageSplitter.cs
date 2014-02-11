using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Puzzle
{
    class ImageSplitter
    {
        private Texture2D Image;

        private int Rows;
        private int Columns;
        private int TotalFrames;

        private List<ImagePart> imageParts;
        private List<int> numbersUsed;

        private Random randomGenerator;

        public ImageSplitter(Texture2D image, int rows, int columns)
        {
            Image = image;
            Rows = rows;
            Columns = columns;
            TotalFrames = rows * columns;
            imageParts = new List<ImagePart>();
            numbersUsed = new List<int>();
            randomGenerator = new Random();
        }

        public void SetGrid(int rowsAndColumns)
        {
            Rows = rowsAndColumns;
            Columns = rowsAndColumns;
            TotalFrames = Rows * Columns;
            numbersUsed = new List<int>();
            imageParts = new List<ImagePart>();

            SplitImage();
        }

        public void SplitImage()
        {
            int spreadX = 0;
            int spreadY = 0;
            int width = Image.Width / Columns;
            int height = Image.Height / Rows;

            for (int i = 0; i < Rows; i++)
            {
                spreadY = 0;
                if (i > 0)
                    spreadX += 1;

                for (int j = 0; j < Columns; j++)
                {
                    if (j > 0)
                        spreadY += 1;

                    int number = generateNumber();
                    int row = (int)((float)number / (float)Columns);
                    int column = number % Columns;

                    Vector2 location = new Vector2();

                    location.X = i * width + spreadX;
                    location.Y = j * height + spreadY;

                    imageParts.Add(new ImagePart(width, height, row, column, location, number));
                }
            }
             

        }

        public int generateNumber()
        {
            int number = randomGenerator.Next(0, TotalFrames);
            if (numbersUsed.Contains(number))
                number = generateNumber();
            else
                numbersUsed.Add(number);

            return number;
        }

        public List<ImagePart> getImageParts()
        {
            return imageParts;
        }
    }
}
