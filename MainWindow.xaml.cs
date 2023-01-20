using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace HittigProjectZweitesLehrjahr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Initializing the program, declaring primary variables and the dispatch timer for moving the ninja
        //Base bools for placing objects (Are they being placed right now and / or are they already placed)
        bool _ninjaPlaceBool;
        bool _wallPlaceBool;
        bool _yinyangPlaceBool;

        bool _yinyangSet = false;
        bool _ninjaSet = false;
        bool _ninjaWalking = false;

        DispatcherTimer dispatcherTimer;

        int _wallCounter = 0;
        int _maxWalkDistance = 50;
        int _globalWeight = 0;
        int _globalCounter = 0;

        GridFinder _ninjaPlace;
        GridFinder _yinyangPlace;
        List<GridFinder> _lastAdded;
        List<GridFinder> _temporaryAdded;

        List<GridFinder> _gridFinderList;
        List<GridFinder> _wayPoints;
        List<Point> _finalPath;

        Point _north;
        Point _south;
        Point _west;
        Point _east;
        
        //Preparing images for usage
        public MainWindow()
        {

            _lastAdded = new List<GridFinder>();
            _temporaryAdded = new List<GridFinder>();
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Tick += dispaterTimer_Tick;

            _finalPath = new List<Point>();

            _ninjaPlace = new GridFinder("Ninja", new Point(0, 0));
            _yinyangPlace = new GridFinder("YinYang", new Point(0, 0));
            _gridFinderList = new List<GridFinder>() { _ninjaPlace, _yinyangPlace,  };
            _wayPoints = new List<GridFinder>();

            for(int i = 0; i < 12; i++)
            {
                _gridFinderList.Add(new GridFinder("Wall", new Point(i, 0)));
                _gridFinderList.Add(new GridFinder("Wall", new Point(0, i)));
                _gridFinderList.Add(new GridFinder("Wall", new Point(i, 7)));
                _gridFinderList.Add(new GridFinder("Wall", new Point(11, i)));
            }

            ResetPlacingBools();
            InitializeComponent();
                        
            NinjaImage.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Bilder\\Ninja.png", UriKind.RelativeOrAbsolute));
            NinjaImage.Visibility = Visibility.Hidden;

            YinYangImage.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Bilder\\YinYang.png", UriKind.RelativeOrAbsolute));
            YinYangImage.Visibility = Visibility.Hidden;
        }

        private void dispaterTimer_Tick(object sender, EventArgs e)
        {
            if (_globalWeight >= 0)
            {
                Grid.SetColumn(NinjaImage, (int)_finalPath[_globalCounter].X);
                Grid.SetRow(NinjaImage, (int)_finalPath[_globalCounter].Y);
                _ninjaPlace.Place = new Point(_finalPath[_globalCounter].X, _finalPath[_globalCounter].Y);
                _globalWeight--;
                _globalCounter++;
            }
            else
            {
                _finalPath.Clear();
                _globalWeight = 0;
                _globalCounter = 0;
                _wayPoints.Clear();            
                dispatcherTimer.Stop();
            }
        }
        #endregion
        #region Managing moving and placing objects
        private void ResetPlacingBools()
        {
            _ninjaPlaceBool = false;
            _yinyangPlaceBool = false;
            _wallPlaceBool = false;
        }

        
        private void PlaceYingYang_Click(object sender, RoutedEventArgs e)
        {
            ResetPlacingBools();
            _yinyangPlaceBool = true;
        }

        private void PlaceWall_Click(object sender, RoutedEventArgs e)
        {
            ResetPlacingBools();
            _wallPlaceBool = true;
        }

        private void PlaceTheNinja_Click(object sender, RoutedEventArgs e)
        {
            ResetPlacingBools();
            _ninjaPlaceBool = true;
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ResetPlacingBools();
        }
        

        //Calculates spot on the grid with the mouse placement and returns the actual place on the grid
        private Point GridCalculator(Point mousePoint)
        {
            int y = (int)mousePoint.Y / 50;
            int x = (int)mousePoint.X / 57;
            Point finalPoint = new Point(x, y);
            return finalPoint;
        }
        //Checks what is in the targeted cell

        //var value = MyList.First(item => item.name == "foo").value;
        private string CheckCellContent(Point targetCell)
        {
            string cellContentName = "";
            foreach(GridFinder currentGridFinder in _gridFinderList)
            {
                if(currentGridFinder.Place == targetCell)
                {
                    cellContentName = currentGridFinder.Name;
                }
            }
            TestLabel.Content = cellContentName;
            return cellContentName;
        }

        //Places the selected object at the target grid cell
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point _mousePos = Mouse.GetPosition(MainGrid);
            Point _finalGridPoint = GridCalculator(_mousePos);


            if (_mousePos.X < 60 || _mousePos.Y < 60 || _mousePos.X > 600 || _mousePos.Y > 340 ||
                !_ninjaPlaceBool && !_wallPlaceBool && !_yinyangPlaceBool || CheckCellContent(_finalGridPoint) != "")
            { return; }


            if(_ninjaPlaceBool)
            {
                _ninjaPlaceBool = false;
                if(!_ninjaSet)
                {
                    NinjaImage.Visibility = Visibility.Visible;
                    _ninjaSet = true;
                }
                Grid.SetColumn(NinjaImage, (int)_finalGridPoint.X);
                Grid.SetRow(NinjaImage, (int)_finalGridPoint.Y);
                _gridFinderList[0].Place = _finalGridPoint;
                ResetPathfinding();
            }
            else if(_yinyangPlaceBool)
            {
                if(!_yinyangSet)
                {
                    _yinyangSet = true;
                    YinYangImage.Visibility = Visibility.Visible;
                }
                Grid.SetColumn(YinYangImage, (int)_finalGridPoint.X);
                Grid.SetRow(YinYangImage, (int)_finalGridPoint.Y);
                _gridFinderList[1].Place = _finalGridPoint;
                ResetPathfinding();
            }
            else
            {
                MainGrid.Children.Add(new Wall((int)_finalGridPoint.Y, (int)_finalGridPoint.X, MainGrid));
                _wallCounter++;
                _gridFinderList.Add(new GridFinder("Wall", _finalGridPoint));
            }
        }
        #endregion
        #region Clear And Reset Variables
        //Resets to start of the program
        private void ResetAll_Click(object sender, RoutedEventArgs e)
        {
            ResetPlacingBools();
            _ninjaSet = false;
            _yinyangSet = false;
            NinjaImage.Visibility = Visibility.Hidden;
            YinYangImage.Visibility = Visibility.Hidden;
            Grid.SetColumn(NinjaImage, 0);
            Grid.SetRow(NinjaImage, 0);
            Grid.SetColumn(YinYangImage, 0);
            Grid.SetRow(YinYangImage, 0);
            ResetPathfinding();

            int _removePosition = 7;

            for(int i = 0; i < _wallCounter; i++)
            {
                MainGrid.Children.RemoveAt(_removePosition);
                _gridFinderList.RemoveAt(2);
            }            
            _wallCounter = 0;
        }

        //Clear only the most fluctual Pathfinding variables
        private void ClearTemporaries()
        {
            _temporaryAdded.Clear();
            _lastAdded.Clear();
        }

        //Clear everything Pathfinding
        private void ResetPathfinding()
        {
            ResetPlacingBools();
            ClearTemporaries();
            _globalCounter = 0;
            _globalWeight = 0;
            _ninjaPlaceBool = false;
            _wallPlaceBool = false;
            _yinyangPlaceBool = false;
            _finalPath.Clear();
            _wayPoints.Clear();
        }
        #endregion
        #region Pathfinding
        //A-Star Pathfinding implementation, expanding circle around the goal with a counter, Walls and already set paths are
        //ignored - stops if it doesnt find a path after 50 steps or if it found the Ninja
        private bool WayFinder()
        {
            _wayPoints.Add(new GridFinder(_gridFinderList[1].Name, _gridFinderList[1].Place, 0));
            List<GridFinder> _lastAdded = new List<GridFinder>() { _wayPoints[0] };
            List<GridFinder> _temporaryAdded = new List<GridFinder>();

            for(int i = 1; i <= _maxWalkDistance; i++)
            {
                foreach(GridFinder lastStep in _lastAdded)
                {
                    _north = new Point(lastStep.Place.X, lastStep.Place.Y + 1);
                    _south = new Point(lastStep.Place.X, lastStep.Place.Y - 1);
                    _west = new Point(lastStep.Place.X - 1, lastStep.Place.Y);
                    _east = new Point(lastStep.Place.X + 1, lastStep.Place.Y);

                    if(_wayPoints.Any(GridFinder => GridFinder.Place == _north) == false && CheckCellContent(_north) != "Wall")
                    {
                        _wayPoints.Add(new GridFinder("Path", _north, i));
                        _temporaryAdded.Add(new GridFinder("Path", _north, i));
                        if (CheckCellContent(_north) == "Ninja")
                        {
                            ClearTemporaries();
                            return true;
                        }
                    }
                    if (_wayPoints.Any(GridFinder => GridFinder.Place == _south) == false && CheckCellContent(_south) != "Wall")
                    {
                        _wayPoints.Add(new GridFinder("Path", _south, i));
                        _temporaryAdded.Add(new GridFinder("Path", _south, i));
                        if (CheckCellContent(_south) == "Ninja")
                        {
                            ClearTemporaries();
                            return true;
                        }
                    }
                    if (_wayPoints.Any(GridFinder => GridFinder.Place == _west) == false && CheckCellContent(_west) != "Wall")
                    {
                        _wayPoints.Add(new GridFinder("Path", _west, i));
                        _temporaryAdded.Add(new GridFinder("Path", _west, i));
                        if (CheckCellContent(_west) == "Ninja")
                        {
                            ClearTemporaries();
                            return true;
                        }
                    }
                    if (_wayPoints.Any(GridFinder => GridFinder.Place == _east) == false && CheckCellContent(_east) != "Wall")
                    {
                        _wayPoints.Add(new GridFinder("Path", _east, i));
                        _temporaryAdded.Add(new GridFinder("Path", _east, i));
                        if (CheckCellContent(_east) == "Ninja")
                        {
                            ClearTemporaries();
                            return true;
                        }
                    }
                }
                _lastAdded.Clear();
                foreach(GridFinder item in _temporaryAdded)
                {
                    _lastAdded.Add(item);
                }
                _temporaryAdded.Clear();
            }
            ClearTemporaries();
            return false;
        }

        //Creates a List of waypoints which lie adjacent which the ninja can walk, ends with starting the dispatch timer
        private void NinjaWalk()
        {
            _globalWeight = _wayPoints[_wayPoints.Count() - 1].Weight;
            Point _lastPlace = _ninjaPlace.Place;

            while (_globalWeight >= 0)
            {
                bool foundCoordinates = false;
                int x = 0;
                int y = 0;
                while (!foundCoordinates)
                {
                    x = (int)_wayPoints.First(GridFinder => GridFinder.Weight == _globalWeight).Place.X;
                    y = (int)_wayPoints.First(GridFinder => GridFinder.Weight == _globalWeight).Place.Y;
                    if (x == _lastPlace.X + 1 && y == _lastPlace.Y + 1  || x == _lastPlace.X + 1 && y == _lastPlace.Y - 1   ||
                        x == _lastPlace.X - 1 && y == _lastPlace.Y + 1  || x == _lastPlace.X - 1 && y == _lastPlace.Y - 1   ||
                        x == _lastPlace.X + 1 && y == _lastPlace.Y      || x == _lastPlace.X && y == _lastPlace.Y + 1       ||
                        x == _lastPlace.X - 1 && y == _lastPlace.Y      || x == _lastPlace.X && y == _lastPlace.Y - 1       ||
                        x == _lastPlace.X && y == _lastPlace.Y)
                    {
                        foundCoordinates = true;
                    }
                    else
                    {
                        _wayPoints.Remove(_wayPoints.First(GridFinder => GridFinder.Weight == _globalWeight));
                        continue;
                    }
                }
                _lastPlace = new Point(x, y);
                _finalPath.Add(new Point(x, y));
                _globalWeight--;
            }
            _globalWeight = _wayPoints[_wayPoints.Count() - 1].Weight;
            dispatcherTimer.Start();
        }

        //Checks if a way has been found and if yes, sets the ninja on his way
        private void StartNinja_Click(object sender, RoutedEventArgs e)
        {
            if (WayFinder())
            {
                _ninjaWalking = true;
                NinjaWalk();
            }
        }
        #endregion
        //Stops the ninja in his tracks
        private void StopNinja_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            ResetPathfinding();
        }
    }
}