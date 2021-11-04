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

namespace PianoTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int mStart = 4;
        private int mStop = 4;
        private bool mIsLoaded = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cboStart.Text = "4";
            cboStop.Text = "4";
            initSynth();
            mIsLoaded = true;
        }

        private void initSynth()
        {
            if (mStart == mStop)
            {
                synth.setOctave(mStart);
            } else
            {
                synth.setMultipleOctaves(mStart, mStop);
            }
            
        }

        private void cboStart_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!mIsLoaded)
                return;
            if (cboStart.SelectedItem != null)
            {
                string val = ((ComboBoxItem)cboStart.SelectedItem).Content.ToString();
                mStart = int.Parse(val);

                if (mStart > mStop)
                    mStop = mStart;
                initSynth();
                
            }
        }

        private void cboStop_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!mIsLoaded)
                return;
            if (cboStop.SelectedItem != null)
            {
                string val = ((ComboBoxItem)cboStop.SelectedItem).Content.ToString();
                mStop = int.Parse(val);

                if (mStart > mStop)
                    mStart = mStop;
                initSynth();
               
            }
        }
    }
}
