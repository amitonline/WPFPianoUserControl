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

namespace PianoUserControl
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        MidiIn midiIn = null;
        int midiInIndex = -1;
        int midiOutIndex = -1;

       
        private double mParentWidth = 0.0;
        private double mParentHeight = 0.0;

        //note frequencies

        static int C4 = 60;
        static int CSHARP4 = 61;
        static int D4 = 62;
        static int DSHARP4 = 63;
        static int E4 = 64;
        static int F4 = 65;
        static int FSHARP4 = 66;
        static int G4 = 67;
        static int GSHARP4 = 68;
        static int A4 = 69;
        static int ASHARP4 = 70;
        static int B4 = 71;


        private const double BLACK_KEY_WIDTH_PERCENT = 30;
        private const double BLACK_KEY_HEIGHT_PERCENT = 60;

        private Rectangle[] arrBlackKeys = new Rectangle[10];

        public UserControl1()
        {
            InitializeComponent();
            initBlackKeys();
            listDevices();
        }

        public void initBlackKeys()
        {
            arrBlackKeys[0] = CSharp1;
            arrBlackKeys[1] = CSharp2;
            arrBlackKeys[2] = DSharp1;
            arrBlackKeys[3] = DSharp2;
            arrBlackKeys[4] = FSharp1;
            arrBlackKeys[5] = FSharp2;
            arrBlackKeys[6] = GSharp1;
            arrBlackKeys[7] = GSharp2;
            arrBlackKeys[8] = ASharp1;
            arrBlackKeys[9] = ASharp2;
        }

        public void sizeBlackKeys()
        {
            double whiteWidth = C.ActualWidth;
            double blackWidth = whiteWidth * (BLACK_KEY_WIDTH_PERCENT / 100);
            double whiteHeight = C.ActualHeight;
            double blackHeight = whiteHeight * (BLACK_KEY_HEIGHT_PERCENT / 100);

            foreach (Rectangle rect in arrBlackKeys)
            {
                rect.Width = blackWidth;
                rect.Height = blackHeight;
            }
        }

        void listDevices()
        {
            for (int i = 0; i < MidiIn.NumberOfDevices; i++)
            {
                midiInIndex = 0;
            }

            for (int i = 0; i < MidiOut.NumberOfDevices; i++)
            {
                if (i > 0)
                    midiOutIndex = 1;
            }

        }

        void playNote(int note)
        {
            if (midiOutIndex == -1)
                return;
            using (MidiOut midiOut = new MidiOut(midiOutIndex))
            {
                midiOut.Send(MidiMessage.StartNote(note, 127, 1).RawData);
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



        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            sizeBlackKeys();
        }

        private void C_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            highlightKey(C, false);
            playNote(C4);
        }

        private void C_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            unHighlightKey(C, false);
            stopNote(C4);
        }

        private void D_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            highlightKey(D, false);
            playNote(D4);
        }

        private void D_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            unHighlightKey(D, false);
            stopNote(D4);
        }

        public void highlightKey(Rectangle rect, bool isBlack)
        {
            if (!isBlack)
                rect.Fill = new SolidColorBrush(Colors.Yellow);
            else
                rect.Fill = new SolidColorBrush(Colors.DarkGray);
        }

        public void unHighlightKey(Rectangle rect, bool isBlack)
        {
            if (!isBlack)
                rect.Fill = new SolidColorBrush(Colors.Ivory);
            else
                rect.Fill = new SolidColorBrush(Colors.Black);
        }

        private void E_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            highlightKey(E, false);
            playNote(E4);
        }

        private void E_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            unHighlightKey(E, false);
            stopNote(E4);
        }

        private void F_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            highlightKey(F, false);
            playNote(F4);
        }

        private void F_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            unHighlightKey(F, false);
            stopNote(F4);
        }

        private void G_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            highlightKey(G, false);
            playNote(G4);
        }

        private void G_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            unHighlightKey(G, false);
            stopNote(G4);
        }

        private void A_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            highlightKey(A, false);
            playNote(A4);

        }

        private void A_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            unHighlightKey(A, false);
            stopNote(A4);
        }

        private void B_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            highlightKey(B, false);
            playNote(B4);
        }

        private void B_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            unHighlightKey(B, false);
            stopNote(B4);
        }

        private void CSharp1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            highlightKey(CSharp1, true);
            highlightKey(CSharp2, true);
            playNote(CSHARP4);
        }

        private void CSharp1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            unHighlightKey(CSharp1, true);
            unHighlightKey(CSharp2, true);
            stopNote(CSHARP4);
        }

        private void CSharp2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            highlightKey(CSharp1, true);
            highlightKey(CSharp2, true);
            playNote(CSHARP4);
        }

        private void CSharp2_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            unHighlightKey(CSharp1, true);
            unHighlightKey(CSharp2, true);
            stopNote(CSHARP4);
        }

        private void DSharp1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            highlightKey(DSharp1, true);
            highlightKey(DSharp2, true);
            playNote(DSHARP4);
        }

        private void DSharp1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            unHighlightKey(DSharp1, true);
            unHighlightKey(DSharp2, true);
            stopNote(DSHARP4);
        }

        private void FSharp1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            highlightKey(FSharp1, true);
            highlightKey(FSharp2, true);
            playNote(FSHARP4);
        }

        private void FSharp1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            unHighlightKey(FSharp1, true);
            unHighlightKey(FSharp2, true);
            stopNote(FSHARP4);
        }

        private void FSharp2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            highlightKey(FSharp1, true);
            highlightKey(FSharp2, true);
            playNote(FSHARP4);
        }

        private void FSharp2_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            unHighlightKey(FSharp1, true);
            unHighlightKey(FSharp2, true);
            stopNote(FSHARP4);
        }

        private void DSharp2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            highlightKey(DSharp1, true);
            highlightKey(DSharp2, true);
            playNote(DSHARP4);
        }

        private void DSharp2_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            unHighlightKey(DSharp1, true);
            unHighlightKey(DSharp2, true);
            stopNote(DSHARP4);
        }

        private void GSharp1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            highlightKey(GSharp1, true);
            highlightKey(GSharp2, true);
            playNote(GSHARP4);
        }

        private void GSharp1_MouseMove(object sender, MouseEventArgs e)
        {
            unHighlightKey(GSharp1, true);
            unHighlightKey(GSharp2, true);
        }

        private void GSharp2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            highlightKey(GSharp1, true);
            highlightKey(GSharp2, true);
            playNote(GSHARP4);
        }

        private void GSharp2_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            unHighlightKey(GSharp1, true);
            unHighlightKey(GSharp2, true);
            stopNote(GSHARP4);
        }

        private void ASharp1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            highlightKey(ASharp1, true);
            highlightKey(ASharp2, true);
            playNote(ASHARP4);
        }

        private void ASharp1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            unHighlightKey(ASharp1, true);
            unHighlightKey(ASharp2, true);
            stopNote(ASHARP4);
        }

        private void ASharp2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            highlightKey(ASharp1, true);
            highlightKey(ASharp2, true);
            playNote(ASHARP4);
        }

        private void ASharp2_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            unHighlightKey(ASharp1, true);
            unHighlightKey(ASharp2, true);
            stopNote(ASHARP4);
        }

        private void C_MouseLeave(object sender, MouseEventArgs e)
        {
            unHighlightKey(C, false);
            if (e.LeftButton == MouseButtonState.Pressed)
                stopNote(C4);
        }

        private void D_MouseLeave(object sender, MouseEventArgs e)
        {
            unHighlightKey(D, false);
            if (e.LeftButton == MouseButtonState.Pressed)
                stopNote(D4);

        }

        private void E_MouseLeave(object sender, MouseEventArgs e)
        {
            unHighlightKey(E, false);
            if (e.LeftButton == MouseButtonState.Pressed)
                stopNote(E4);

        }

        private void F_MouseLeave(object sender, MouseEventArgs e)
        {
            unHighlightKey(F, false);
            if (e.LeftButton == MouseButtonState.Pressed)
                stopNote(F4);

        }

        private void G_MouseLeave(object sender, MouseEventArgs e)
        {
            unHighlightKey(G, false);
            if (e.LeftButton == MouseButtonState.Pressed)
                stopNote(G4);

        }

        private void A_MouseLeave(object sender, MouseEventArgs e)
        {
            unHighlightKey(A, false);
            if (e.LeftButton == MouseButtonState.Pressed)
                stopNote(A4);

        }

        private void B_MouseLeave(object sender, MouseEventArgs e)
        {
            unHighlightKey(B, false);
            if (e.LeftButton == MouseButtonState.Pressed)
                stopNote(B4);

        }

        private void C_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                highlightKey(C, false);
                playNote(C4);
            }
        }

        private void D_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                highlightKey(D, false);
                playNote(D4);
            }
        }

        private void E_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                highlightKey(E, false);
                playNote(E4);
            }
        }

        private void F_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                highlightKey(F, false);
                playNote(F4);
            }
        }

        private void G_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                highlightKey(G, false);
                playNote(G4);
            }
        }

        private void A_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                highlightKey(A, false);
                playNote(A4);
            }
        }

        private void B_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                highlightKey(B, false);
                playNote(B4);
            }
        }

        private void CSharp1_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                highlightKey(CSharp1, true);
                highlightKey(CSharp2, true);
                playNote(CSHARP4);
            }
        }

        private void CSharp2_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                highlightKey(CSharp1, true);
                highlightKey(CSharp2, true);
                playNote(CSHARP4);
            }
        }

        private void CSharp1_MouseLeave(object sender, MouseEventArgs e)
        {
            unHighlightKey(CSharp1, true);
            unHighlightKey(CSharp2, true);
            stopNote(CSHARP4);
        }

        private void CSharp2_MouseLeave(object sender, MouseEventArgs e)
        {
            unHighlightKey(CSharp2, true);
            unHighlightKey(CSharp1, true);
            stopNote(CSHARP4);
        }

        private void DSharp1_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                highlightKey(DSharp1, true);
                highlightKey(DSharp2, true);

            }
        }

        private void DSharp2_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                highlightKey(DSharp1, true);
                highlightKey(DSharp2, true);
            }
        }

        private void FSharp1_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                highlightKey(FSharp1, true);
                highlightKey(FSharp2, true);
            }
        }

        private void FSharp2_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                highlightKey(FSharp1, true);
                highlightKey(FSharp2, true);
            }
        }

        private void GSharp1_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                highlightKey(GSharp1, true);
                highlightKey(GSharp2, true);
            }
        }

        private void GSharp2_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                highlightKey(GSharp1, true);
                highlightKey(GSharp2, true);
                playNote(DSHARP4);
            }
        }

        private void ASharp1_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                highlightKey(ASharp1, true);
                highlightKey(ASharp2, true);
                playNote(ASHARP4);
            }
        }

        private void ASharp2_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                highlightKey(ASharp1, true);
                highlightKey(ASharp2, true);
                playNote(ASHARP4);
            }
        }

        private void DSharp1_MouseLeave(object sender, MouseEventArgs e)
        {
            unHighlightKey(DSharp1, true);
            unHighlightKey(DSharp2, true);
            stopNote(DSHARP4);
        }

        private void DSharp2_MouseLeave(object sender, MouseEventArgs e)
        {
            unHighlightKey(DSharp1, true);
            unHighlightKey(DSharp2, true);
            stopNote(DSHARP4);
        }

        private void FSharp1_MouseLeave(object sender, MouseEventArgs e)
        {
            unHighlightKey(FSharp1, true);
            unHighlightKey(FSharp2, true);
            stopNote(FSHARP4);
        }

        private void FSharp2_MouseLeave(object sender, MouseEventArgs e)
        {
            unHighlightKey(FSharp1, true);
            unHighlightKey(FSharp2, true);
            stopNote(FSHARP4);
        }

        private void GSharp1_MouseLeave(object sender, MouseEventArgs e)
        {
            unHighlightKey(GSharp1, true);
            unHighlightKey(GSharp2, true);
            stopNote(GSHARP4);
        }

        private void GSharp2_MouseLeave(object sender, MouseEventArgs e)
        {
            unHighlightKey(GSharp1, true);
            unHighlightKey(GSharp2, true);
            stopNote(GSHARP4);
        }

        private void ASharp1_MouseLeave(object sender, MouseEventArgs e)
        {
            unHighlightKey(ASharp1, true);
            unHighlightKey(ASharp2, true);
            stopNote(ASHARP4);
        }

        private void ASharp2_MouseLeave(object sender, MouseEventArgs e)
        {
            unHighlightKey(ASharp1, true);
            unHighlightKey(ASharp2, true);
            stopNote(ASHARP4);
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Window parent = Window.GetWindow((DependencyObject)sender);
            if (parent != null)
            {
                mParentWidth = parent.ActualHeight;
                mParentHeight = parent.ActualHeight;
                sizeBlackKeys();
            }
        }

        
    }
}
