using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Windows.Forms;

namespace AudioRecording
    {
        public partial class AudioRecording : Form
        {
            private string outputFileName;
            private WasapiLoopbackCapture capture;
            private WaveFileWriter writer;
            private bool isRecording = false;

            public AudioRecording()
            {
                InitializeComponent();
                LoadDevices();
            }

            private void LoadDevices()
            {
                var enumerator = new MMDeviceEnumerator();
                var defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                OutputDeviceComboBox.Items.Add(defaultDevice);
                OutputDeviceComboBox.SelectedIndex = 0;
            }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void StartButton_Click(object sender, EventArgs e)
            {
                if (!isRecording)
                {
                    var dialog = new SaveFileDialog();
                    dialog.Filter = "Wave files | *.wav";
                    dialog.FileName = "AudioRecord.wav";
                    if (dialog.ShowDialog() != DialogResult.OK)
                        return;

                    outputFileName = dialog.FileName;

                    var device = (MMDevice)OutputDeviceComboBox.SelectedItem;
                    capture = new WasapiLoopbackCapture(device);
                    writer = new WaveFileWriter(outputFileName, capture.WaveFormat);
                    capture.DataAvailable += (s, args) =>
                    {
                        if (isRecording)
                            writer.Write(args.Buffer, 0, args.BytesRecorded);
                    };
                    capture.RecordingStopped += (s, args) =>
                    {
                        writer.Dispose();
                        capture.Dispose();
                        StartButton.Enabled = true;
                        StopButton.Enabled = false;
                        PauseButton.Enabled = false;
                        isRecording = false;
                        MessageBox.Show("Recording stopped.");
                    };
                    capture.StartRecording();
                    StartButton.Enabled = false;
                    StopButton.Enabled = true;
                    PauseButton.Enabled = true;
                    isRecording = true;
                }
                else
                {
                    if (isRecording)
                    {
                        isRecording = false;
                        PauseButton.Text = "Resume";
                    }
                    else
                    {
                        isRecording = true;
                        PauseButton.Text = "Pause";
                    }
                }
            }

            private void PauseButton_Click(object sender, EventArgs e)
            {
                if (isRecording)
                {
                    isRecording = false;
                    PauseButton.Text = "Resume";
                }
                else
                {
                    isRecording = true;
                    PauseButton.Text = "Pause";
                }
            }

            private void StopButton_Click(object sender, EventArgs e)
            {
                if (isRecording)
                {
                    capture?.StopRecording();
                }
            }

             private void OutputDeviceComboBox_SelectedIndexChanged(object sender, EventArgs e)
             {

             }

             private void Form1_Load(object sender, EventArgs e)
             {

              }
    }
}

