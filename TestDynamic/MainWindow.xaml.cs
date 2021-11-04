using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NAudio.Midi;

namespace TestDynamic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        MidiIn midiIn = null;
        int midiInIndex = -1;
        int midiOutIndex = -1;

        //note frequencies
        private int mOctave = 4;    // default octave (octaves can be from 1 to 7)
        private static int MIN_OCTAVE = 1;
        private static int MAX_OCTAVE = 7;


        private int CNOTE = 60;
        private int CSHARPNOTE = 61;
        private int DNOTE = 62;
        private int DSHARPNOTE = 63;
        private int ENOTE = 64;
        private int FNOTE = 65;
        private int FSHARPNOTE = 66;
        private int GNOTE = 67;
        private int GSHARPNOTE = 68;
        private int ANOTE = 69;
        private int ASHARPNOTE = 70;
        private int BNOTE = 71;

        // note names
        private const string CKEY = "c";
        private const string DKEY = "d";
        private const string EKEY = "e";
        private const string FKEY = "f";
        private const string GKEY = "g";
        private const string AKEY = "a";
        private const string BKEY = "b";

        private const string CSHARPKEY1 = "csharp1";
        private const string CSHARPKEY2 = "csharp2";
        private const string DSHARPKEY1 = "dsharp1";
        private const string DSHARPKEY2 = "dsharp2";
        private const string FSHARPKEY1 = "fsharp1";
        private const string FSHARPKEY2 = "fsharp3";
        private const string GSHARPKEY1 = "gsharp1";
        private const string GSHARPKEY2 = "gsharp2";
        private const string ASHARPKEY1 = "asharp1";
        private const string ASHARPKEY2 = "asharp2";


        private const double BLACK_KEY_WIDTH_PERCENT = 30;
        private const double BLACK_KEY_HEIGHT_PERCENT = 60;

        private List<Rectangle> mRects = new List<Rectangle>();
        private List<Rectangle> mBlackRects = new List<Rectangle>();

        private Double mBlackHeight = 0.0;
        private Double mBlackWidth = 0.0;

        private int mNumOctaves = 1;        // by default show only 1 octave
        private int mColumns = 6;           // default is 6 columns for one octave
        private int mStartOctave = 4;       // default start and stop octave
        private int mStopOctave = 4;

        private bool mLoaded = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        #region audio handling *************************************************************************************

        private void setNotes()
        {
            int C4 = 60;
            int CSHARP4 = 61;
            int D4 = 62;
            int DSHARP4 = 63;
            int E4 = 64;
            int F4 = 65;
            int FSHARP4 = 66;
            int G4 = 67;
            int GSHARP4 = 68;
            int A4 = 69;
            int ASHARP4 = 70;
            int B4 = 71;

            int change = 0;
          

            CNOTE = C4 + change;
            CSHARPNOTE = CSHARP4 + change;
            DNOTE = D4 + change;
            DSHARPNOTE = DSHARP4 + change;
            ENOTE = E4 + change;
            FNOTE = F4 + change;
            FSHARPNOTE = FSHARP4 + change;
            GNOTE = G4 + change;
            GSHARPNOTE = GSHARP4 + change;
            ANOTE = A4 + change;
            ASHARPNOTE = ASHARP4 + change;
            BNOTE = B4 + change;

        }

        private int setNoteAsPerOctave(int note, int octave)
        {
            
            int change = 0;
            if (octave == 3)
                change = -12;
            else if (octave == 2)
                change = -24;
            else if (octave == 1)
                change = -36;
            else if (octave == 5)
                change = 12;
            else if (octave == 6)
                change = 24;
            else if (octave == 7)
                change = 36;

            return note + change;
          

        }
        public void setOctave(int o)
        {
            if (o < MIN_OCTAVE)
                o = MIN_OCTAVE;
            if (o > MAX_OCTAVE)
                o = MAX_OCTAVE;
            mOctave = o;
            mStartOctave = o;
            mStopOctave = o;
            mNumOctaves = 1;
            initUI();
            setNotes();
        }

        public void setMultipleOctaves(int start, int stop)
        {
            
            if (start < MIN_OCTAVE)
                start = MIN_OCTAVE;
            if (start > MAX_OCTAVE)
                start = MAX_OCTAVE;

            if (stop < MIN_OCTAVE)
                stop = MIN_OCTAVE;
            if (stop > MAX_OCTAVE)
                stop = MAX_OCTAVE;
            if (stop < start)
                stop = start;
            if (stop == start)
            {
                setOctave(start);
                return;
            }
            mOctave = start;
            mStartOctave = start;
            mStopOctave = stop;
            mNumOctaves = stop - start + 1;
            initUI();
            setNotes();
           
        }

        public int getOctave() { return mOctave; }

        void listDevices()
        {
            Console.WriteLine("Midi in devices");
            for (int i = 0; i < MidiIn.NumberOfDevices; i++)
            {
                midiInIndex = 0;
                Console.WriteLine(MidiIn.DeviceInfo(i).ProductName);
            }
            Console.WriteLine("Midi Out devices");
            for (int i = 0; i < MidiOut.NumberOfDevices; i++)
            {
                if (i > 0)
                    midiOutIndex = i;
                Console.WriteLine(MidiOut.DeviceInfo(i).ProductName);
            }

        }

        void playNote(int note)
        {
            if (midiOutIndex == -1)
                return;
            using (MidiOut midiOut = new MidiOut(midiOutIndex))
            {
                midiOut.Send(MidiMessage.StartNote(note, 127, 1).RawData);
                Console.WriteLine("play note" + note);
            }
        }
        void stopNote(int note)
        {
            if (midiOutIndex == -1)
                return;
            using (MidiOut midiOut = new MidiOut(midiOutIndex))
            {
                midiOut.Send(MidiMessage.StopNote(note, 127, 1).RawData);
            }
        }

        int noteNameToSound(string name)
        {
            int retVal = 0;

            if (name == CKEY)
                retVal = CNOTE;
            else if (name == DKEY)
                retVal = DNOTE;
            else if (name == EKEY)
                retVal = ENOTE;
            else if (name == FKEY)
                retVal = FNOTE;
            else if (name == GKEY)
                retVal = GNOTE;
            else if (name == AKEY)
                retVal = ANOTE;
            else if (name == BKEY)
                retVal = BNOTE;

            else if (name == CSHARPKEY1 || name == CSHARPKEY2)
                retVal = CSHARPNOTE;
            else if (name == DSHARPKEY1 || name == DSHARPKEY2)
                retVal = DSHARPNOTE;
            else if (name == FSHARPKEY1 || name == FSHARPKEY2)
                retVal = FSHARPNOTE;
            else if (name == GSHARPKEY1 || name == GSHARPKEY2)
                retVal = GSHARPNOTE;
            else if (name == ASHARPKEY1 || name == ASHARPKEY2)
                retVal = ASHARPNOTE;

            return retVal;
        }

        #endregion ****************************************************************************************

        #region UI handling *************************************************************************************
        private void initUI()
        {
            grd.Width = this.ActualWidth - 30;
            grd.Height = this.ActualHeight-40;
            grd.HorizontalAlignment = HorizontalAlignment.Left;
            grd.VerticalAlignment = VerticalAlignment.Top;
            grd.ShowGridLines = false;
            grd.Background = new SolidColorBrush(Colors.White);

            for (int i = 1; i <= mNumOctaves; i++)
            {
                for (int j = 0; j <= mColumns; j++)
                {
                    ColumnDefinition col = new ColumnDefinition();
                    col.Width = new GridLength(1, GridUnitType.Star);
                    grd.ColumnDefinitions.Add(col);
                }
            }
            RowDefinition row = new RowDefinition();
            row.Height = new GridLength(1, GridUnitType.Star);
            grd.RowDefinitions.Add(row);

            mBlackHeight = this.Height * (BLACK_KEY_HEIGHT_PERCENT/100);
            mBlackWidth = 100.0 * (BLACK_KEY_WIDTH_PERCENT / 100); // this width will actually be the percentage of a white key width
                                                                   // this gets set in resizeUI()

            int gridCol = 0;
            // white keys interspersed with black keys
            for (int octaves  = mStartOctave; octaves <= mStopOctave; octaves++)
            {
                for (int i = 0; i <= mColumns; i++)
                {
                    Rectangle rect = new Rectangle();
                    rect.RadiusX = 20;
                    rect.RadiusY = 20;
                    rect.Fill = new SolidColorBrush(Colors.Ivory);
                    rect.Height = Double.NaN;
                    rect.Margin = new Thickness(0, 0, 0, 0);
                    rect.StrokeThickness = 2;
                    rect.Stroke = new SolidColorBrush(Colors.Black);
                    rect.MouseLeftButtonDown += rect_MouseLeftButtonDown;
                    rect.MouseLeftButtonUp += rect_MouseLeftButtonUp;
                    rect.MouseEnter += rect_MouseEnter;
                    rect.MouseLeave += rect_MouseLeave;
                    rect.Tag = octaves;
                    // set key name
                    switch (i)
                    {
                        case 0:
                            rect.Name = CKEY;
                            break;
                        case 1:
                            rect.Name = DKEY;
                            break;
                        case 2:
                            rect.Name = EKEY;
                            break;
                        case 3:
                            rect.Name = FKEY;
                            break;
                        case 4:
                            rect.Name = GKEY;
                            break;
                        case 5:
                            rect.Name = AKEY;
                            break;
                        case 6:
                            rect.Name = BKEY;
                            break;
                    }
                    Grid.SetRow(rect, 0);
                    Grid.SetColumn(rect, gridCol);

                    grd.Children.Add(rect);
                    mRects.Add(rect);

                    if (i == 0)
                    {
                        // csharp 1
                        Rectangle rectb = new Rectangle();
                        rectb.Width = mBlackWidth;
                        rectb.Fill = new SolidColorBrush(Colors.Black);
                        rectb.Height = mBlackHeight;
                        rectb.Margin = new Thickness(0, 0, 0, 0);
                        rectb.StrokeThickness = 2;
                        rectb.Stroke = new SolidColorBrush(Colors.Black);
                        rectb.VerticalAlignment = VerticalAlignment.Top;
                        rectb.HorizontalAlignment = HorizontalAlignment.Right;
                        rectb.MouseLeftButtonDown += rect_MouseLeftButtonDown;
                        rectb.MouseLeftButtonUp += rect_MouseLeftButtonUp;
                        rectb.MouseEnter += rect_MouseEnter;
                        rectb.MouseLeave += rect_MouseLeave;
                        rectb.Name = CSHARPKEY1;
                        rectb.Tag = octaves;

                        Grid.SetRow(rectb, 0);
                        Grid.SetColumn(rectb, gridCol);
                        grd.Children.Add(rectb);
                        mBlackRects.Add(rectb);
                    }

                    else if (i == 1)
                    {
                        // csharp 2
                        Rectangle rectb = new Rectangle();

                        rectb.Width = mBlackWidth;
                        rectb.Fill = new SolidColorBrush(Colors.Black);
                        rectb.Height = mBlackHeight;
                        rectb.Margin = new Thickness(0, 0, 0, 0);
                        rectb.StrokeThickness = 2;
                        rectb.Stroke = new SolidColorBrush(Colors.Black);
                        rectb.VerticalAlignment = VerticalAlignment.Top;
                        rectb.HorizontalAlignment = HorizontalAlignment.Left;
                        rectb.MouseLeftButtonDown += rect_MouseLeftButtonDown;
                        rectb.MouseLeftButtonUp += rect_MouseLeftButtonUp;
                        rectb.MouseEnter += rect_MouseEnter;
                        rectb.MouseLeave += rect_MouseLeave;
                        rectb.Name = CSHARPKEY2;
                        rectb.Tag = octaves;

                        Grid.SetRow(rectb, 0);
                        Grid.SetColumn(rectb, gridCol);
                        grd.Children.Add(rectb);
                        mBlackRects.Add(rectb);

                        // dsharp 1
                        Rectangle rectc = new Rectangle();

                        rectc.Width = mBlackWidth;
                        rectc.Fill = new SolidColorBrush(Colors.Black);
                        rectc.Height = mBlackHeight;
                        rectc.Margin = new Thickness(0, 0, 0, 0);
                        rectc.StrokeThickness = 2;
                        rectc.Stroke = new SolidColorBrush(Colors.Black);
                        rectc.VerticalAlignment = VerticalAlignment.Top;
                        rectc.HorizontalAlignment = HorizontalAlignment.Right;
                        rectc.MouseLeftButtonDown += rect_MouseLeftButtonDown;
                        rectc.MouseLeftButtonUp += rect_MouseLeftButtonUp;
                        rectc.MouseEnter += rect_MouseEnter;
                        rectc.MouseLeave += rect_MouseLeave;
                        rectc.Name = DSHARPKEY1;
                        rectc.Tag = octaves;

                        Grid.SetRow(rectc, 0);
                        Grid.SetColumn(rectc, gridCol);
                        grd.Children.Add(rectc);
                        mBlackRects.Add(rectc);
                    }

                    else if (i == 2)
                    {
                        // dsharp 2
                        Rectangle rectb = new Rectangle();

                        rectb.Width = mBlackWidth;
                        rectb.Fill = new SolidColorBrush(Colors.Black);
                        rectb.Height = mBlackHeight;
                        rectb.Margin = new Thickness(0, 0, 0, 0);
                        rectb.StrokeThickness = 2;
                        rectb.Stroke = new SolidColorBrush(Colors.Black);
                        rectb.VerticalAlignment = VerticalAlignment.Top;
                        rectb.HorizontalAlignment = HorizontalAlignment.Left;
                        rectb.MouseLeftButtonDown += rect_MouseLeftButtonDown;
                        rectb.MouseLeftButtonUp += rect_MouseLeftButtonUp;
                        rectb.MouseEnter += rect_MouseEnter;
                        rectb.MouseLeave += rect_MouseLeave;
                        rectb.Name = DSHARPKEY2;
                        rectb.Tag = octaves;

                        Grid.SetRow(rectb, 0);
                        Grid.SetColumn(rectb, gridCol);
                        grd.Children.Add(rectb);
                        mBlackRects.Add(rectb);

                    }

                    else if (i == 3)
                    {
                        // fsharp 1
                        Rectangle rectb = new Rectangle();

                        rectb.Width = mBlackWidth;
                        rectb.Fill = new SolidColorBrush(Colors.Black);
                        rectb.Height = mBlackHeight;
                        rectb.Margin = new Thickness(0, 0, 0, 0);
                        rectb.StrokeThickness = 2;
                        rectb.Stroke = new SolidColorBrush(Colors.Black);
                        rectb.VerticalAlignment = VerticalAlignment.Top;
                        rectb.HorizontalAlignment = HorizontalAlignment.Right;
                        rectb.MouseLeftButtonDown += rect_MouseLeftButtonDown;
                        rectb.MouseLeftButtonUp += rect_MouseLeftButtonUp;
                        rectb.MouseEnter += rect_MouseEnter;
                        rectb.MouseLeave += rect_MouseLeave;
                        rectb.Name = FSHARPKEY1;
                        rectb.Tag = octaves;

                        Grid.SetRow(rectb, 0);
                        Grid.SetColumn(rectb, gridCol);
                        grd.Children.Add(rectb);
                        mBlackRects.Add(rectb);


                    }

                    else if (i == 4)
                    {
                        // fsharp 2
                        Rectangle rectb = new Rectangle();

                        rectb.Width = mBlackWidth;
                        rectb.Fill = new SolidColorBrush(Colors.Black);
                        rectb.Height = mBlackHeight;
                        rectb.Margin = new Thickness(0, 0, 0, 0);
                        rectb.StrokeThickness = 2;
                        rectb.Stroke = new SolidColorBrush(Colors.Black);
                        rectb.VerticalAlignment = VerticalAlignment.Top;
                        rectb.HorizontalAlignment = HorizontalAlignment.Left;
                        rectb.MouseLeftButtonDown += rect_MouseLeftButtonDown;
                        rectb.MouseLeftButtonUp += rect_MouseLeftButtonUp;
                        rectb.MouseEnter += rect_MouseEnter;
                        rectb.MouseLeave += rect_MouseLeave;
                        rectb.Name = FSHARPKEY2;
                        rectb.Tag = octaves;


                        Grid.SetRow(rectb, 0);
                        Grid.SetColumn(rectb, gridCol);
                        grd.Children.Add(rectb);
                        mBlackRects.Add(rectb);

                        //gsharp 1
                        Rectangle rectc = new Rectangle();

                        rectc.Width = mBlackWidth;
                        rectc.Fill = new SolidColorBrush(Colors.Black);
                        rectc.Height = mBlackHeight;
                        rectc.Margin = new Thickness(0, 0, 0, 0);
                        rectc.StrokeThickness = 2;
                        rectc.Stroke = new SolidColorBrush(Colors.Black);
                        rectc.VerticalAlignment = VerticalAlignment.Top;
                        rectc.HorizontalAlignment = HorizontalAlignment.Right;
                        rectc.MouseLeftButtonDown += rect_MouseLeftButtonDown;
                        rectc.MouseLeftButtonUp += rect_MouseLeftButtonUp;
                        rectc.MouseEnter += rect_MouseEnter;
                        rectc.MouseLeave += rect_MouseLeave;
                        rectc.Name = GSHARPKEY1;
                        rectc.Tag = octaves;

                        Grid.SetRow(rectc, 0);
                        Grid.SetColumn(rectc, gridCol);
                        grd.Children.Add(rectc);
                        mBlackRects.Add(rectc);

                    }

                    else if (i == 5)
                    {
                        // gsharp 2
                        Rectangle rectb = new Rectangle();

                        rectb.Width = mBlackWidth;
                        rectb.Fill = new SolidColorBrush(Colors.Black);
                        rectb.Height = mBlackHeight;
                        rectb.Margin = new Thickness(0, 0, 0, 0);
                        rectb.StrokeThickness = 2;
                        rectb.Stroke = new SolidColorBrush(Colors.Black);
                        rectb.VerticalAlignment = VerticalAlignment.Top;
                        rectb.HorizontalAlignment = HorizontalAlignment.Left;
                        rectb.MouseLeftButtonDown += rect_MouseLeftButtonDown;
                        rectb.MouseLeftButtonUp += rect_MouseLeftButtonUp;
                        rectb.MouseEnter += rect_MouseEnter;
                        rectb.MouseLeave += rect_MouseLeave;
                        rectb.Name = GSHARPKEY2;
                        rectb.Tag = octaves;

                        Grid.SetRow(rectb, 0);
                        Grid.SetColumn(rectb, gridCol);
                        grd.Children.Add(rectb);
                        mBlackRects.Add(rectb);

                        //asharp 1
                        Rectangle rectc = new Rectangle();

                        rectc.Width = mBlackWidth;
                        rectc.Fill = new SolidColorBrush(Colors.Black);
                        rectc.Height = mBlackHeight;
                        rectc.Margin = new Thickness(0, 0, 0, 0);
                        rectc.StrokeThickness = 2;
                        rectc.Stroke = new SolidColorBrush(Colors.Black);
                        rectc.VerticalAlignment = VerticalAlignment.Top;
                        rectc.HorizontalAlignment = HorizontalAlignment.Right;
                        rectc.MouseLeftButtonDown += rect_MouseLeftButtonDown;
                        rectc.MouseLeftButtonUp += rect_MouseLeftButtonUp;
                        rectc.MouseEnter += rect_MouseEnter;
                        rectc.MouseLeave += rect_MouseLeave;
                        rectc.Name = ASHARPKEY1;
                        rectc.Tag = octaves;

                        Grid.SetRow(rectc, 0);
                        Grid.SetColumn(rectc, gridCol);
                        grd.Children.Add(rectc);
                        mBlackRects.Add(rectc);

                    }

                    else if (i == 6)
                    {
                        // asharp 2
                        Rectangle rectb = new Rectangle();

                        rectb.Width = mBlackWidth;
                        rectb.Fill = new SolidColorBrush(Colors.Black);
                        rectb.Height = mBlackHeight;
                        rectb.Margin = new Thickness(0, 0, 0, 0);
                        rectb.StrokeThickness = 2;
                        rectb.Stroke = new SolidColorBrush(Colors.Black);
                        rectb.VerticalAlignment = VerticalAlignment.Top;
                        rectb.HorizontalAlignment = HorizontalAlignment.Left;
                        rectb.MouseLeftButtonDown += rect_MouseLeftButtonDown;
                        rectb.MouseLeftButtonUp += rect_MouseLeftButtonUp;
                        rectb.MouseEnter += rect_MouseEnter;
                        rectb.MouseLeave += rect_MouseLeave;
                        rectb.Name = ASHARPKEY2;
                        rectb.Tag = octaves;

                        Grid.SetRow(rectb, 0);
                        Grid.SetColumn(rectb, gridCol);
                        grd.Children.Add(rectb);
                        mBlackRects.Add(rectb);

                    } //   else if (i == 6)
                    gridCol++;
                } //    for (int i = 0; i <= mColumns; i++)
            } //    for (int x = 0; x < mNumOctaves; x++)

            mLoaded = true;
          
        }

        private bool isBlackRect(Rectangle r)
        {
            bool retVal = false;
            mBlackRects.ForEach((elem) =>
            {
                if (elem == r)
                {
                    retVal = true;
                }
            });
            return retVal;
        }

        private Rectangle getBlackKeyByName(string name, int octave)
        {
            foreach(Rectangle r1 in mBlackRects)
            {
                if (r1.Name == name && (int) r1.Tag == octave)
                {
                    return r1;
                }
            }
            return null;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            listDevices();
            setNotes();
            //setMultipleOctaves(2,5);
            setOctave(5);
           
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            resizeUI();
        }

        private void resizeUI()
        {
            if (!mLoaded)
                return;
            grd.Width = this.ActualWidth - 30;
            grd.Height = this.ActualHeight - 40;

            Rectangle rect = mRects[0];
            mBlackHeight = this.ActualHeight * (BLACK_KEY_HEIGHT_PERCENT / 100);
            mBlackWidth = rect.ActualWidth * (BLACK_KEY_WIDTH_PERCENT / 100);

            mBlackRects.ForEach((elem) =>
            {
                elem.Width = mBlackWidth;
                elem.Height = mBlackHeight;
            });
        }

        public void highlightKey(Rectangle rect, bool isBlack)
        {
            if (!isBlack)
                rect.Fill = new SolidColorBrush(Colors.Yellow);
            else
            {
                rect.Fill = new SolidColorBrush(Colors.DarkGray);
                Rectangle rectPair = null;
                if (rect.Name == CSHARPKEY1)
                    rectPair = getBlackKeyByName(CSHARPKEY2, (int) rect.Tag);
                else if (rect.Name == CSHARPKEY2)
                    rectPair = getBlackKeyByName(CSHARPKEY1, (int)rect.Tag);

                if (rect.Name == DSHARPKEY1)
                    rectPair = getBlackKeyByName(DSHARPKEY2, (int)rect.Tag);
                else if (rect.Name == DSHARPKEY2)
                    rectPair = getBlackKeyByName(DSHARPKEY1, (int)rect.Tag);

                if (rect.Name == FSHARPKEY1)
                    rectPair = getBlackKeyByName(FSHARPKEY2, (int)rect.Tag);
                else if (rect.Name == FSHARPKEY2)
                    rectPair = getBlackKeyByName(FSHARPKEY1, (int)rect.Tag);

                if (rect.Name == GSHARPKEY1)
                    rectPair = getBlackKeyByName(GSHARPKEY2, (int)rect.Tag);
                else if (rect.Name == GSHARPKEY2)
                    rectPair = getBlackKeyByName(GSHARPKEY1, (int)rect.Tag);

                if (rect.Name == ASHARPKEY1)
                    rectPair = getBlackKeyByName(ASHARPKEY2, (int)rect.Tag);
                else if (rect.Name == ASHARPKEY2)
                    rectPair = getBlackKeyByName(ASHARPKEY1, (int)rect.Tag);

                if (rectPair != null)
                    rectPair.Fill = new SolidColorBrush(Colors.DarkGray);
            }
        }

        public void unHighlightKey(Rectangle rect, bool isBlack)
        {
            if (!isBlack)
                rect.Fill = new SolidColorBrush(Colors.Ivory);
            else
            {
                rect.Fill = new SolidColorBrush(Colors.Black);

                Rectangle rectPair = null;
                if (rect.Name == CSHARPKEY1)
                    rectPair = getBlackKeyByName(CSHARPKEY2, (int)rect.Tag);
                else if (rect.Name == CSHARPKEY2)
                    rectPair = getBlackKeyByName(CSHARPKEY1, (int)rect.Tag);

                if (rect.Name == DSHARPKEY1)
                    rectPair = getBlackKeyByName(DSHARPKEY2, (int)rect.Tag);
                else if (rect.Name == DSHARPKEY2)
                    rectPair = getBlackKeyByName(DSHARPKEY1, (int)rect.Tag);

                if (rect.Name == FSHARPKEY1)
                    rectPair = getBlackKeyByName(FSHARPKEY2, (int)rect.Tag);
                else if (rect.Name == FSHARPKEY2)
                    rectPair = getBlackKeyByName(FSHARPKEY1, (int)rect.Tag);

                if (rect.Name == GSHARPKEY1)
                    rectPair = getBlackKeyByName(GSHARPKEY2, (int)rect.Tag);
                else if (rect.Name == GSHARPKEY2)
                    rectPair = getBlackKeyByName(GSHARPKEY1, (int)rect.Tag);

                if (rect.Name == ASHARPKEY1)
                    rectPair = getBlackKeyByName(ASHARPKEY2, (int)rect.Tag);
                else if (rect.Name == ASHARPKEY2)
                    rectPair = getBlackKeyByName(ASHARPKEY1, (int)rect.Tag);

                if (rectPair != null)
                    rectPair.Fill = new SolidColorBrush(Colors.Black);
            }
        }


        private void evtLeftButtonDown(Rectangle r)
        {
            Console.WriteLine("left button down");
            highlightKey(r, isBlackRect(r));
            playNote(setNoteAsPerOctave(noteNameToSound(r.Name), (int) r.Tag));
        }

        private void evtLeftButtonUp(Rectangle r)
        {
            Console.WriteLine("left button up");
            unHighlightKey(r, isBlackRect(r));
            stopNote(setNoteAsPerOctave(noteNameToSound(r.Name), (int)r.Tag));
        }

        private void evtMouseLeave(Rectangle r, MouseEventArgs e)
        {
            Console.WriteLine("mouse leave");
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                unHighlightKey(r, isBlackRect(r));
                stopNote(setNoteAsPerOctave(noteNameToSound(r.Name), (int)r.Tag));
            }
        }

        private void evtMouseEnter(Rectangle r, MouseEventArgs e)
        {
            Console.WriteLine("mouse enter");
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                highlightKey(r, isBlackRect(r));
                playNote(setNoteAsPerOctave(noteNameToSound(r.Name), (int)r.Tag));
            }

        }

        #endregion ****************************************************************************************

        #region event handling ***************************************************************************

        private void rect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            evtLeftButtonDown((Rectangle) sender);
        }

        private void rect_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            evtLeftButtonUp((Rectangle)sender);
        }

        private void rect_MouseLeave(object sender, MouseEventArgs e)
        {
            evtMouseLeave((Rectangle)sender, e);
        }

        private void rect_MouseEnter(object sender, MouseEventArgs e)
        {
            evtMouseEnter((Rectangle)sender, e);
        }

        #endregion ********************************************************

        private void grd_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void grd_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            resizeUI();
        }
    }
}
