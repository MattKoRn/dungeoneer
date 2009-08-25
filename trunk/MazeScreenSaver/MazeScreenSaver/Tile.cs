using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MazeScreenSaver.Properties;
using System.Drawing;

namespace MazeScreenSaver
{
    class Tile
    {
        Bitmap m_bmp;
        int m_TileX;
        int m_TileY;
        int m_TileWidth = 32;
        int m_TileHeight = 32;

            
        public Tile(Bitmap bmp, int tileX, int tileY)
        {
            m_bmp = bmp;
            m_TileX = tileX;
            m_TileY = tileY;
        }

        public void Draw(Graphics g, int x, int y)
        {
            g.DrawImage(m_bmp, y, x, new Rectangle(m_TileX * m_TileWidth, m_TileY * m_TileHeight,m_TileWidth, m_TileHeight),GraphicsUnit.Pixel);
        }
    }
}