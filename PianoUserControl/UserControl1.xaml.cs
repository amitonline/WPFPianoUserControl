using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.MusicTheory;

namespace PianoUserControl
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        OutputDevice midiOUTDevice = null;
        Melanchall.DryWetMidi.Multimedia.InputDevice midiINDevice = null;
        
        //note frequencies
        private int mOctave = 4;    // default octave (octaves can be from 1 to 7)
        private static int MIN_OCTAVE = 1;
        private static int MAX_OCTAVE = 7;


        //default freqs assuming this is the 4th octave
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
        private const string DSHARPKEY1 = "dsharp1";
        private const string FSHARPKEY1 = "fsharp1";
        private const string GSHARPKEY1 = "gsharp1";
        private const string ASHARPKEY1 = "asharp1";

        private const string CSHARPKEY1A = "csharp1a";
        private const string DSHARPKEY1A = "dsharp1a";
        private const string FSHARPKEY1A = "fsharp1a";
        private const string GSHARPKEY1A = "gsharp1a";
        private const string ASHARPKEY1A = "asharp1a";

        //white keys sizes
        private const double WHITE_KEY_WIDTH_PERCENT = 14.28;
        private const double WHITE_KEY_HEIGHT_PERCENT = 95;

        //black keys sizes
        private const double BLACK_KEY_WIDTH_PERCENT = 10;
        private const double BLACK_KEY_HEIGHT_PERCENT = 60;
        private const double BLACK_KEY_OFFSET_PERCENT = 65; // offset percent of black from its preceding white key

        //ivory keys sizes
        private const double IVORY_KEY_WIDTH_PERCENT = 8;
        private const double IVORY_KEY_HEIGHT_PERCENT = 55;
        private const double IVORY_KEY_OFFSET_PERCENT = 10; // offset percent of black from its parent black key


        private const double KEY_TOP = 5;//y pos for all keys
        private const double WHITE_KEY_HORIZ_SPACING = 0;//space between white keys

        private List<Rectangle> mRects = new List<Rectangle>(); // white keys
        private List<Rectangle> mBlackRects = new List<Rectangle>(); // black keys
        private List<Rectangle> mIvoryRects = new List<Rectangle>(); // inner rects for black keys

        private Double mWhiteWidth = 50;
        private Double mWhiteHeight = 250;
        private Double mBlackHeight = 180;
        private Double mBlackWidth = 40.0;
        private int mBlackKeyOffset = 30;
        private Double mIvoryHeight = 180;
        private Double mIvoryWidth = 40.0;
        private int mIvoryKeyOffset = 30;

        private int mNumOctaves = 1;        // by default show only 1 octave
        private int mColumns = 7;          // default is 6 columns for one octave
        private int mStartOctave = 4;       // default start and stop octave
        private int mStopOctave = 4;

        public enum INSTRUMENTS
        {
            NONE, ACOUSTIC_GRAND_PIANO, BRIGHT_PIANO,ELECTRIC_GRAND_PIANO,HONKYTONK_PIANO,ELECTRTC_PIANO1,ELECTRIC_PIANO2,
            HARPSICHORD,CLAVINET,CELESTA,GLOCKENSPIEL,MUSICBOX,VIBRAPHONE,MARIMBA,XYLOPHONE,TUBULAR_BELLS,DULCIMER
        }
        public string[] mInstruments = { "None", "Acoustic Grand Piano", "Bright Piano", "Electric Grand Piano",
                                         "HonkyTonk Piano", "Electric Piano 1", "Electric Piano 2", "Harpsichord",
                                         "Clavinet", "Celesta", "Glockenspiel", "MusicBox", "Vibraphone", "Marimba",
                                         "Xylophone", "Tubular Bells", "Dulcimer"};
        private bool mLoaded = false;

        public UserControl1()
        {
            InitializeComponent();
           
        }

        #region audio handling *************************************************************************************
        
        /// <summary>
        /// Set default note frequencies
        /// </summary>
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

        /// <summary>
        /// Change the note frequency as per the current octave
        /// </summary>
        /// <param name="note"></param>
        /// <param name="octave"></param>
        /// <returns></returns>
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

        /// <summary>
        /// set current octave. This is used to set a single octave (start  = stop)
        /// </summary>
        /// <param name="o"></param>
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

        /// <summary>
        /// Set multiple octaves where start < stop
        /// </summary>
        /// <param name="start"></param>
        /// <param name="stop"></param>
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

        public void setMIDIOutputDevice(OutputDevice dev)
        {
            if (midiOUTDevice != null)
                midiOUTDevice.Dispose();

            midiOUTDevice = dev;
            midiOUTDevice.PrepareForEventsSending();
           
        }

        public void setMIDIInputDevice(Melanchall.DryWetMidi.Multimedia.InputDevice dev)
        {
            if (midiINDevice != null)
                midiINDevice.Dispose();
            if (dev != null)
            {
                midiINDevice = dev;
                midiINDevice.EventReceived += onMIDIEventReceived;
                midiINDevice.StartEventsListening();
            }
        }

        /// <summary>
        /// Do a program change event 
        /// </summary>
        /// <param name="inst">1 to 89 for general MIDI instrument</param>
        public void setMIDIInstrument(int inst)
        {
            if (midiOUTDevice == null)
                return;

            ProgramChangeEvent evt = new ProgramChangeEvent((SevenBitNumber)inst);
            midiOUTDevice.SendEvent(evt);
        }
         
        /// <summary>
        /// Send midi command to play a note frequency for a fixed duration
        /// </summary>
        /// <param name="note"></param>
        /// <param name="velocity">default 127 if none passed</param>
        void playNote(int note, int velocity=127)
        {
            if (midiOUTDevice == null)
                return;

            NoteOnEvent evt = new NoteOnEvent((SevenBitNumber) note,(SevenBitNumber) velocity);
            midiOUTDevice.SendEvent(evt);
            Debug.WriteLine("play note" + note);
        }

        /// <summary>
        /// Send midi command to stop playing a note 
        /// </summary>
        /// <param name="note"></param>
        void stopNote(int note)
        {
            if (midiOUTDevice == null)
                return;
            NoteOffEvent evt = new NoteOffEvent((SevenBitNumber)note, (SevenBitNumber)10);
            midiOUTDevice.SendEvent(evt);
             
        }

        /// <summary>
        /// Map key names to key sounds
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
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

            else if (name == CSHARPKEY1 || name == CSHARPKEY1A)
                retVal = CSHARPNOTE;
            else if (name == DSHARPKEY1 || name == DSHARPKEY1A)
                retVal = DSHARPNOTE;
            else if (name == FSHARPKEY1 || name == FSHARPKEY1A)
                retVal = FSHARPNOTE;
            else if (name == GSHARPKEY1 || name == GSHARPKEY1A)
                retVal = GSHARPNOTE;
            else if (name == ASHARPKEY1 || name == ASHARPKEY1A)
                retVal = ASHARPNOTE;

            return retVal;
        }

        /// <summary>
        /// Get MIDI events from MIDI keyboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onMIDIEventReceived(object sender, MidiEventReceivedEventArgs e)
        {
            var midiDevice = (MidiDevice)sender;
            Debug.WriteLine($"Event received from '{midiDevice.Name}' at {DateTime.Now}: {e.Event}");
            if (e.Event.EventType == MidiEventType.NoteOn)
            {
                NoteOnEvent evt = (NoteOnEvent)e.Event;
                playNote(evt.NoteNumber, evt.Velocity);
            } else if (e.Event.EventType == MidiEventType.NoteOff) {
                NoteOffEvent evt = (NoteOffEvent)e.Event;
                stopNote(evt.NoteNumber);
            }

        }

        #endregion ****************************************************************************************

        #region UI handling *************************************************************************************


        /// <summary>
        /// Create rectangle for black keys
        /// </summary>
        /// <param name="name">rect name</param>
        /// <param name="tag">rect tag</param>
        /// <returns>rectb</returns>
        private Rectangle createBlackKey(string name, object tag)
        {
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
            rectb.Name = name;
            rectb.Tag = tag;
            Panel.SetZIndex(rectb, 2);

            //set placement 
            rectb = positionBlackKeys(name, tag, rectb);
            return rectb;
        }

        /// <summary>
        /// Create inner rectangle for black keys
        /// </summary>
        /// <param name="name">rect name</param>
        /// <param name="tag">rect tag</param>
        /// <param name="parent">parent black rectangle</param>
        /// <returns>rectb</returns>
        private Rectangle createIvoryKey(string name, object tag, Rectangle parent)
        {
            Rectangle rectb = new Rectangle();
            rectb.Width = mIvoryWidth;
            rectb.Fill = new SolidColorBrush(Colors.Ivory);
            rectb.Height = mIvoryHeight;
            rectb.Margin = new Thickness(0, 0, 0, 0);
            rectb.StrokeThickness = 2;
            rectb.Stroke = new SolidColorBrush(Colors.Black);
            rectb.VerticalAlignment = VerticalAlignment.Top;
            rectb.HorizontalAlignment = HorizontalAlignment.Right;
            rectb.MouseLeftButtonDown += rect_MouseLeftButtonDown;
            rectb.MouseLeftButtonUp += rect_MouseLeftButtonUp;
            rectb.MouseEnter += rect_MouseEnter;
            rectb.MouseLeave += rect_MouseLeave;
            rectb.Name = name;
            rectb.Tag = tag;
            rectb.Fill = createGradientForIvoryKey();
            Panel.SetZIndex(rectb, 3);

            //set placement 
            rectb = positionIvoryKeys(name, tag, rectb, parent);
            return rectb;
        }


        /// <summary>
        /// Position black keys based on the key name
        /// </summary>
        /// <param name="name">key name</param>
        /// <param name="tag">octave info</param>
        /// <param name="rectb">black rectangle</param>
        /// <returns>rectb</returns>
        private Rectangle positionBlackKeys(string name, object tag, Rectangle rectb)
        {
            Rectangle w = null;
            if (name == CSHARPKEY1)
            {
                w = getWhiteKeyByName(CKEY, (int)tag);
            }
            else if (name == DSHARPKEY1)
            {
                w = getWhiteKeyByName(DKEY, (int)tag);
            }
            else if (name == FSHARPKEY1)
            {
                w = getWhiteKeyByName(FKEY, (int)tag);
            }
            else if (name == GSHARPKEY1)
            {
                w = getWhiteKeyByName(GKEY, (int)tag);
            }
            else if (name == ASHARPKEY1)
            {
                w = getWhiteKeyByName(AKEY, (int)tag);
            }
            Canvas.SetTop(rectb, KEY_TOP);
            double offsetPos = (BLACK_KEY_OFFSET_PERCENT / 100) * w.Width;
            Canvas.SetLeft(rectb, Canvas.GetLeft(w) + offsetPos);

            return rectb;
        }

        /// <summary>
        /// Position ivory keys based on the key name
        /// </summary>
        /// <param name="name">key name</param>
        /// <param name="tag">octave info</param>
        /// <param name="rectb">ivory rectangle</param>
        /// <param name="parent">parent black key</param>
        /// <returns>rectb</returns>
        private Rectangle positionIvoryKeys(string name, object tag, Rectangle rectb, Rectangle parent)
        {
           
            Canvas.SetTop(rectb, KEY_TOP);
            double offsetPos = (IVORY_KEY_OFFSET_PERCENT / 100) * parent.Width;
            Canvas.SetLeft(rectb, Canvas.GetLeft(parent) + offsetPos);
            return rectb;
        }

        /// <summary>
        /// Init the keyboard rects and the Grid constraints
        /// </summary>
        private void initUI()
        {
            mLoaded = false;
            //remove any keys present in data
            canvas.Children.Clear();
            mRects.Clear();
            mBlackRects.Clear();
            mIvoryRects.Clear();

            canvas.Width = this.ActualWidth - 5;
            canvas.Height = this.ActualHeight - 5;
            canvas.HorizontalAlignment = HorizontalAlignment.Left;
            canvas.VerticalAlignment = VerticalAlignment.Top;

            mWhiteHeight = canvas.ActualHeight * (WHITE_KEY_HEIGHT_PERCENT / 100);
            mWhiteWidth= canvas.ActualWidth * (WHITE_KEY_WIDTH_PERCENT / 100);

            mBlackHeight = canvas.ActualHeight * (BLACK_KEY_HEIGHT_PERCENT / 100);
            mBlackWidth = canvas.ActualWidth * (BLACK_KEY_WIDTH_PERCENT / 100); // this width will actually be the percentage of a white key width
                                                                                // this gets set in resizeUI()
            mIvoryHeight = canvas.ActualHeight * (IVORY_KEY_HEIGHT_PERCENT / 100);
            mIvoryWidth = canvas.ActualWidth * (IVORY_KEY_WIDTH_PERCENT / 100);

            //adjust for number of octave
            mWhiteWidth = (mWhiteWidth / (mStopOctave - mStartOctave+1));
            mBlackWidth = (mBlackWidth / (mStopOctave - mStartOctave+1));
            mIvoryWidth = (mIvoryWidth / (mStopOctave - mStartOctave + 1));


            double currWhiteKeyPos = 0; // track horizontal pos for white keys


            // white keys interspersed with black keys
            for (int octaves = mStartOctave; octaves <= mStopOctave; octaves++)
            {
                for (int i = 0; i < mColumns; i++)
                {
                    Rectangle rect = new Rectangle();
                    rect.RadiusX = 10;
                    rect.RadiusY = 10;
                    rect.Fill = new SolidColorBrush(Colors.Ivory);
                    rect.Height = mWhiteHeight;
                    rect.Width = mWhiteWidth;
                    Canvas.SetTop(rect, KEY_TOP);
                    Canvas.SetLeft(rect, currWhiteKeyPos);
                    rect.Margin = new Thickness(0, 0, 0, 0);
                    rect.StrokeThickness = 2;
                    rect.Stroke = new SolidColorBrush(Colors.Black);
                    rect.MouseLeftButtonDown += rect_MouseLeftButtonDown;
                    rect.MouseLeftButtonUp += rect_MouseLeftButtonUp;
                    rect.MouseEnter += rect_MouseEnter;
                    rect.MouseLeave += rect_MouseLeave;
                    if (octaves == mStartOctave && i == 0)
                    {
                        rect.SizeChanged += rect_SizeChanged;
                    }

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
                    canvas.Children.Add(rect);
                    mRects.Add(rect);

               

                    //show black keys
                    if (i == 0)
                    {
                        Rectangle rectb = createBlackKey(CSHARPKEY1, octaves);
                        mBlackRects.Add(rectb);
                        canvas.Children.Add(rectb);
                        Rectangle rectInner = createIvoryKey(CSHARPKEY1A, octaves, rectb);
                        canvas.Children.Add(rectInner);
                        mIvoryRects.Add(rectInner);
                    }

                    else if (i == 1)
                    {
                        // dsharp 1
                        Rectangle rectc = createBlackKey(DSHARPKEY1, octaves);
                        mBlackRects.Add(rectc);
                        canvas.Children.Add(rectc);
                        Rectangle rectInner = createIvoryKey(DSHARPKEY1A, octaves, rectc);
                        canvas.Children.Add(rectInner);
                        mIvoryRects.Add(rectInner);
                    }

                    else if (i == 3)
                    {
                        // fsharp 1
                        Rectangle rectb = createBlackKey(FSHARPKEY1, octaves);
                        mBlackRects.Add(rectb);
                        canvas.Children.Add(rectb);
                        Rectangle rectInner = createIvoryKey(FSHARPKEY1A, octaves, rectb);
                        canvas.Children.Add(rectInner);
                        mIvoryRects.Add(rectInner);
                    }

                    else if (i == 4)
                    {
                        //gsharp 1
                        Rectangle rectc = createBlackKey(GSHARPKEY1, octaves);
                        mBlackRects.Add(rectc);
                        canvas.Children.Add(rectc);
                        Rectangle rectInner = createIvoryKey(GSHARPKEY1A, octaves, rectc);
                        canvas.Children.Add(rectInner);
                        mIvoryRects.Add(rectInner);
                    }

                    else if (i == 5)
                    {
                        //asharp 1
                        Rectangle rectc = createBlackKey(ASHARPKEY1, octaves);
                        mBlackRects.Add(rectc);
                        canvas.Children.Add(rectc);
                        Rectangle rectInner = createIvoryKey(ASHARPKEY1A, octaves, rectc);
                        canvas.Children.Add(rectInner);
                        mIvoryRects.Add(rectInner);
                    }

                } //    for (int i = 0; i <= mColumns; i++)
            } //    for (int x = 0; x < mNumOctaves; x++)

            mLoaded = true;

        }

        /// <summary>
        /// get  white rectangle(key) by name and octave
        /// </summary>
        /// <param name="name"></param>
        /// <param name="octave"></param>
        /// <returns></returns>
        private Rectangle getWhiteKeyByName(string name, int octave)
        {
            foreach (Rectangle r1 in mRects)
            {
                if (r1.Name == name && (int)r1.Tag == octave)
                {
                    return r1;
                }
            }
            return null;
        }

        /// <summary>
        /// Check if a rectangle is Black key
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Check if a rectangle is Ivory key
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private bool isIvoryRect(Rectangle r)
        {
            bool retVal = false;
            mIvoryRects.ForEach((elem) =>
            {
                if (elem == r)
                {
                    retVal = true;
                }
            });
            return retVal;
        }

        /// <summary>
        /// get  black rectangle(key) by name and octave
        /// </summary>
        /// <param name="name"></param>
        /// <param name="octave"></param>
        /// <returns></returns>
        private Rectangle getBlackKeyByName(string name, int octave)
        {
            foreach (Rectangle r1 in mBlackRects)
            {
                if (r1.Name == name && (int)r1.Tag == octave)
                {
                    return r1;
                }
            }
            return null;
        }

        /// <summary>
        /// get ivory rectangle(key) by name and octave
        /// </summary>
        /// <param name="name"></param>
        /// <param name="octave"></param>
        /// <returns></returns>
        private Rectangle getIvoryKeyByName(string name, int octave)
        {
            foreach (Rectangle r1 in mIvoryRects)
            {
                if (r1.Name == name && (int)r1.Tag == octave)
                {
                    return r1;
                }
            }
            return null;
        }

        /// <summary>
        /// get parent of ivory rectangle
        /// </summary>
        /// <param name="name"></param>
        /// <param name="octave"></param>
        /// <returns></returns>
        private Rectangle getIvoryKeyParent(string name, int octave)
        {
            Rectangle r = null;

            switch (name) {
                case CSHARPKEY1A:
                    r = getBlackKeyByName(CSHARPKEY1, octave);
                    break;
                case DSHARPKEY1A:
                    r = getBlackKeyByName(DSHARPKEY1, octave);
                    break;
                case FSHARPKEY1A:
                    r = getBlackKeyByName(FSHARPKEY1, octave);
                    break;
                case GSHARPKEY1A:
                    r = getBlackKeyByName(GSHARPKEY1, octave);
                    break;
                case ASHARPKEY1A:
                    r = getBlackKeyByName(ASHARPKEY1, octave);
                    break;
            }
            return r;
        }


        private LinearGradientBrush createGradientForIvoryKey(bool highlightFlag=false)
        {
            Color startColor = Color.FromRgb(0, 0, 0);
            Color stopColor = Colors.DarkGray;
            if (highlightFlag)
            {
                startColor = Colors.DarkGray;
                stopColor = Color.FromRgb(0, 0, 0);
            }
            Point startPoint = new Point(0, 0);
            Point endPoint = new Point(1,1);
            GradientStop gradient1 = new GradientStop(startColor, 0.0);
            GradientStop gradient2 = new GradientStop(stopColor, 1.0);
            LinearGradientBrush brush = new LinearGradientBrush();
            brush.GradientStops.Add(gradient1); brush.GradientStops.Add(gradient2);
            brush.StartPoint = startPoint; brush.EndPoint = endPoint;

            return brush;
        }


        /// <summary>
        /// Force the redraw of the keys when the container resizes
        /// </summary>
        public void resizeUI()
        {
            Debug.WriteLine("resizeUI() start");
            if (!mLoaded)
                return;
            canvas.Width = this.ActualWidth - 30;
            canvas.Height = this.ActualHeight - 40;

            Rectangle rect = mRects[0];
            mWhiteHeight = canvas.Height * (WHITE_KEY_HEIGHT_PERCENT / 100);
            mWhiteWidth = canvas.Width * (WHITE_KEY_WIDTH_PERCENT / 100);

            mBlackHeight = canvas.Height * (BLACK_KEY_HEIGHT_PERCENT / 100);
            mBlackWidth = canvas.Width * (BLACK_KEY_WIDTH_PERCENT / 100);

            mIvoryHeight = canvas.ActualHeight * (IVORY_KEY_HEIGHT_PERCENT / 100);
            mIvoryWidth = canvas.ActualWidth * (IVORY_KEY_WIDTH_PERCENT / 100);


            //adjust for number of octave
            mWhiteWidth = (mWhiteWidth / (mStopOctave - mStartOctave + 1));
            mBlackWidth = (mBlackWidth / (mStopOctave - mStartOctave + 1));
            mIvoryWidth = (mIvoryWidth / (mStopOctave - mStartOctave + 1));


            double currWhiteKeyPos = 0;
            mRects.ForEach((elem) =>
            {
                elem.Width = mWhiteWidth;
                elem.Height = mWhiteHeight;
                Canvas.SetTop(elem, KEY_TOP);
                Canvas.SetLeft(elem, currWhiteKeyPos);
                currWhiteKeyPos += WHITE_KEY_HORIZ_SPACING + mWhiteWidth;
                Debug.WriteLine("white rect=" + elem.Name +", tag=" + Convert.ToString(elem.Tag));
            });

            mBlackRects.ForEach((elem) =>
            {
                elem.Width = mBlackWidth;
                elem.Height = mBlackHeight;
                positionBlackKeys(elem.Name, elem.Tag, elem);
            });

            mIvoryRects.ForEach((elem) =>
            {
                elem.Width = mIvoryWidth;
                elem.Height = mIvoryHeight;
                positionIvoryKeys(elem.Name, elem.Tag, elem, getIvoryKeyParent(elem.Name, Convert.ToInt16(elem.Tag)));
            });
            Debug.WriteLine("resizeUI() end");

        }

        /// <summary>
        /// Highlight key as it is pressed
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="isBlack"></param>
        /// <param name="isIvory"></param>
        public void highlightKey(Rectangle rect, bool isBlack, bool isIvory)
        {
            if (!isBlack && !isIvory)
                rect.Fill = new SolidColorBrush(Color.FromRgb(243,243,243));
            else
            {
                if (isBlack)
                {
                    rect.Fill = new SolidColorBrush(Colors.DarkGray);
                } else if (isIvory)
                {
                    rect.Fill = createGradientForIvoryKey(true);
                }
            }
        }

        /// <summary>
        /// Remove highlight from key as it is stopped being pressed
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="isBlack"></param>
        /// <param name="isIvory"></param>
        public void unHighlightKey(Rectangle rect, bool isBlack, bool isIvory)
        {
            if (!isBlack && !isIvory)
                rect.Fill = new SolidColorBrush(Colors.Ivory);
            else
            {
                if (isBlack)
                {
                    rect.Fill = new SolidColorBrush(Colors.Black);
                }
                else if (isIvory)
                {
                    rect.Fill = createGradientForIvoryKey(false);
                }

            }
        }


        /// <summary>
        /// Handle left button clicked event for a rectangle
        /// </summary>
        /// <param name="r"></param>
        private void evtLeftButtonDown(Rectangle r)
        {
            Console.WriteLine("left button down");
            highlightKey(r, isBlackRect(r), isIvoryRect(r));
            playNote(setNoteAsPerOctave(noteNameToSound(r.Name), (int)r.Tag));

        }

        /// <summary>
        /// Handle left button up event for a rectangle
        /// </summary>
        /// <param name="r"></param>
        private void evtLeftButtonUp(Rectangle r)
        {
            Console.WriteLine("left button up");
            unHighlightKey(r, isBlackRect(r), isIvoryRect(r));
            stopNote(setNoteAsPerOctave(noteNameToSound(r.Name), (int)r.Tag));
        }

        /// <summary>
        /// Handle mouse leave event for a rectangle
        /// </summary>
        /// <param name="r"></param>
        /// <param name="e"></param>
        private void evtMouseLeave(Rectangle r, MouseEventArgs e)
        {
            Console.WriteLine("mouse leave");
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                unHighlightKey(r, isBlackRect(r), isIvoryRect(r));
                stopNote(setNoteAsPerOctave(noteNameToSound(r.Name), (int)r.Tag));
            }
        }

        /// <summary>
        /// Handle mouse entered event for a rectangle
        /// </summary>
        /// <param name="r"></param>
        /// <param name="e"></param>
        private void evtMouseEnter(Rectangle r, MouseEventArgs e)
        {
            Console.WriteLine("mouse enter");
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                highlightKey(r, isBlackRect(r), isIvoryRect(r));
                playNote(setNoteAsPerOctave(noteNameToSound(r.Name), (int)r.Tag));
            }

        }

        #endregion ****************************************************************************************

        #region event handling ***************************************************************************

        private void rect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            evtLeftButtonDown((Rectangle)sender);
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

        private void rect_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            resizeUI();
        }

        #endregion ********************************************************

        private void grd_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void grd_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            resizeUI();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            setNotes();
           
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            resizeUI();
        }

        private void grdLoaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
