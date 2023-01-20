using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HittigProjectZweitesLehrjahr
{
    //Class for declaring content of gridcells
    internal class GridFinder
    {
        string _object = "";
        Point _place;
        int _weight;

        public GridFinder(string objectName, Point targetPoint, int weight)
        {
            _object = objectName;
            _place = targetPoint;
            _weight = weight;
        }

        public GridFinder(string objectName, Point targetPoint)
        {
            _object = objectName;
            _place = targetPoint;
        }

        public int Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }

        public string Name
        {
            get { return _object; }
            set { _object = value; }
        }
        public Point Place
        {
            get { return _place; }
            set { _place = value; }
        }
    }
}
