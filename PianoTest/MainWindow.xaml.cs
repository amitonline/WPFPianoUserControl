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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Melanchall.DryWetMidi.Multimedia;
using PianoUserControl;

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
            mIsLoaded = true;
            listMIDIOutDevices();
            listMIDIInDevices();
            initSynth();
        }

        private void initSynth()
        {
            string deviceName = null;
            if (mIsLoaded)
            {
                deviceName = cboMIDIOut.SelectedValue.ToString();
                synth.setMIDIOutputDevice(getOutputDeviceByName(deviceName));
                fillInstruments();
                synth.setMIDIInstrument((int) UserControl1.INSTRUMENTS.ELECTRIC_PIANO2);
            }
            if (mStart == mStop)
            {
                synth.setOctave(mStart);
            }
            else
            {
                synth.setMultipleOctaves(mStart, mStop);
            }

        }

        private void fillInstruments()
        {
            cboInstrument.Items.Clear();
            cboInstrument.SelectedValuePath = "Key";
            cboInstrument.DisplayMemberPath = "Value";
            int count = Enum.GetNames(typeof(UserControl1.INSTRUMENTS)).Length;
            for(int i=0; i < count; i++)
            {
                if (i == 0)
                    continue;
                cboInstrument.Items.Add(new KeyValuePair<int, string>(i, synth.mInstruments[i]));
            }
            cboInstrument.SelectedIndex = 0;
        }


        private void cboStart_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!mIsLoaded)
                return;
            if (cboStart.SelectedItem != null)
            {
                string val = ((ComboBoxItem)cboStart.SelectedItem).Content.ToString();
                mStart = int.Parse(val);

                if (mStart >= mStop)
                    mStop = mStart;
                else
                {
                    string val2 = ((ComboBoxItem)cboStop.SelectedItem).Content.ToString();
                    mStop = int.Parse(val2);
                }
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
                else
                {
                    string val2 = ((ComboBoxItem)cboStart.SelectedItem).Content.ToString();
                    mStart = int.Parse(val2);
                }
                initSynth();

            }
        }

        private void cboMIDIOut_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mIsLoaded)
            {
                string deviceName = cboMIDIOut.SelectedValue.ToString();
                synth.setMIDIOutputDevice(getOutputDeviceByName(deviceName));
            }
        }

        private void cboMIDIIn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mIsLoaded)
            {
                string deviceName = cboMIDIIn.SelectedValue.ToString();
                if (deviceName == "None")
                    synth.setMIDIInputDevice(null);
                else
                    synth.setMIDIInputDevice(getInputDeviceByName(deviceName));
            }
        }

        private void listMIDIOutDevices()
        {
            cboMIDIOut.Items.Clear();
            foreach (var outputDevice in OutputDevice.GetAll())
            {
                cboMIDIOut.Items.Add(outputDevice.Name);
            }
            cboMIDIOut.SelectedIndex = 0;
        }

        private void listMIDIInDevices()
        {
            cboMIDIIn.Items.Clear();
            cboMIDIIn.Items.Add("None");
            foreach (var inputDevice in Melanchall.DryWetMidi.Multimedia.InputDevice.GetAll())
            {
                cboMIDIIn.Items.Add(inputDevice.Name);
            }
            cboMIDIIn.SelectedIndex = 0;
        }

        private OutputDevice getOutputDeviceByName(string name)
        {
            OutputDevice retVal = null;
            foreach (var outputDevice in OutputDevice.GetAll())
            {
                if (outputDevice.Name == name) {
                    retVal = outputDevice;
                    break;
                }
            }
            return retVal;
        }

        private Melanchall.DryWetMidi.Multimedia.InputDevice getInputDeviceByName(string name)
        {
            Melanchall.DryWetMidi.Multimedia.InputDevice retVal = null;
            foreach (var outputDevice in Melanchall.DryWetMidi.Multimedia.InputDevice.GetAll())
            {
                if (outputDevice.Name == name)
                {
                    retVal = outputDevice;
                    break;
                }
            }
            return retVal;
        }

        private void cboInstrument_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!mIsLoaded)
                return;
            int test = Convert.ToInt16(cboInstrument.SelectedValue);
            if (test == 0)
                return;
            synth.setMIDIInstrument(Convert.ToInt16(cboInstrument.SelectedValue)-1);        }
    }

    
}
