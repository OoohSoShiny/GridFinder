using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace HittigProjectZweitesLehrjahr
{
    //Wallclass for creating and placing walls and adding them as children to the maingrid.  
    public class Wall : Image
    {        
        public Point WallPlacement { get; set; }
        public Wall(int _yPosition, int _xPosition, Grid _mainGrid)
        {
            this.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Bilder\\Wall.png", UriKind.RelativeOrAbsolute));
            WallPlacement = new Point(_xPosition, _yPosition);
            Grid.SetColumn(this, _xPosition);
            Grid.SetRow(this, _yPosition);
        }
    }
}
