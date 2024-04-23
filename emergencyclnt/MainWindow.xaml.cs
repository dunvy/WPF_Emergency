using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;

using NAudio.Wave;
using System.IO;
using System.Runtime.InteropServices;
using NAudio.Wave.Compression;
<<<<<<< Updated upstream
using System.Threading.Channels;
using NAudio.Utils;
using NAudio.CoreAudioApi;
=======
using NAudio.CoreAudioApi;
using NAudio.Wave.SampleProviders;
using System.Windows.Threading;

>>>>>>> Stashed changes

namespace emergencyclnt
{

    public partial class MainWindow : Window
    {

        public TcpClient clnt;
        public NetworkStream stream;

        public WaveIn micsource = null;
        public WaveIn wavesource = null;
        public WaveFileWriter wavefile = null;
        public WaveFileWriter wavefile1 = null;
        public WaveFileReader sendfile = null;

        public MainWindow()
        {
            InitializeComponent();
            //ConncectToServer();
            label.Content = "음성녹음입니다.";
<<<<<<< Updated upstream
            MMDeviceEnumerator en = new MMDeviceEnumerator();
            var devices = en.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);
=======
            recogmic();

>>>>>>> Stashed changes
        }
        public async Task recogmic()
        {
            MessageBox.Show("dl?");
            
            await Task.Run(async ()=>
            {
                MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
                MMDevice device = (MMDevice)enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia);
                while (true)
                {
                    double value = device.AudioMeterInformation.MasterPeakValue;
                    int deci = (int)Math.Floor(value * 79);
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        Decibel.Content = deci.ToString();
                    }));
                }
            });
            MessageBox.Show("dp?");
            
        }
        public async Task ConncectToServer() // 서버 연결 함수
        {
            try
            {
                clnt = new TcpClient();
                await clnt.ConnectAsync("10.10.20.120", 9190);
                stream = clnt.GetStream();
            }
            catch (Exception ex)
            {
                MessageBox.Show("서버연결 안됨");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // 레코딩 시작
            label.Content = "음성감지를 시작합니다.";

            start.IsEnabled = false;
            stop.IsEnabled = true;
            
            //wavesource = new WaveIn();
            //wavesource.WaveFormat = new WaveFormat(16000, 1);
            //wavesource.DataAvailable += new EventHandler<WaveInEventArgs>(waveSource_DataAvailable);
            //wavesource.RecordingStopped += new EventHandler<StoppedEventArgs>(waveSource_RecordingStopped);

            //wavefile = new WaveFileWriter("C:\\Users\\AIOT1\\Desktop\\fuxkfuxkfuxk\\test.wav", wavesource.WaveFormat);
            ////wavefile1 = new WaveFileWriter("C:\\Users\\AIOT1\\Desktop\\fuxkfuxkfuxk\\test1.wav", micsource.WaveFormat);

            //wavesource.StartRecording();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // 레코딩 종료
            label.Content = "음성녹음을 종료합니다.";

            stop.IsEnabled = false;
            //micsource.StopRecording();
            wavesource.StopRecording();
        }
        private void recordstart()
        {
            label.Content = "음성녹음을 종료합니다.";
            wavesource.StartRecording();
        }
        private void recordstop()
        {
            label.Content = "음성녹음을 종료합니다.";
            wavesource.StopRecording();
        }

        void waveSource_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (wavefile != null)
            {
                wavefile.Write(e.Buffer, 0, e.BytesRecorded);
                
                Decibel.Content = "q";
                wavefile.Flush();
                
            }
        }
        //void waveSource_DataAvailable1(object sender, WaveInEventArgs e)
        //{
        //    if (wavefile1 != null)
        //    {
        //        wavefile1.Write(e.Buffer, 0, e.BytesRecorded);
        //        wavefile1.Flush();
        //    }
        //}

        void waveSource_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (wavesource != null)
            {
                wavesource.Dispose();
                wavesource = null;
            }

            if (wavefile != null)
            {
                wavefile.Dispose();
                wavefile = null;
            }

            start.IsEnabled = true;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string waveFilePath = "C:\\Users\\AIOT1\\Desktop\\fuxkfuxkfuxk\\test0.wav";

            using(FileStream fs = new FileStream(waveFilePath, FileMode.Open, FileAccess.Read))
            {
                // 파일 사이즈 체크
                long size = fs.Length;

                // 파일 길이 전송
                byte[] sizeBytes = BitConverter.GetBytes(size);
                stream.Write(sizeBytes, 0, sizeBytes.Length);

                // 파일 전송
                byte[] buffer = new byte[size];
                int bytesRead;
                
                //while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                //{
                //    stream.Write(buffer, 0, bytesRead);
                //}
                //for (int i = 0; i < buffer.Length / 8; i++)
                //{
                    
                double sample = BitConverter.ToDouble(buffer, 4);
                double volume = Math.Abs(sample / 32768.0);
                double decibels = 20 * Math.Log10(volume);
                Decibel.Content = decibels.ToString();
                //}
            }
        }


        double dec;

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            //string waveFilePath = "C:\\Users\\iot\\source\\repos\\WPFEmergency\\voice\\test.wav";

            //WaveFileReader reader = new WaveFileReader(waveFilePath);

            ////WaveFormat.AverageBytesPerSecond: 평균 데이터 전송 속도(초당 바이트)
            //int bytesPerMillisecond = reader.WaveFormat.AverageBytesPerSecond / 1000;
            //TimeSpan time = new TimeSpan(0, 0, 1);

            //int bytesPerSecond = (int)time.TotalMilliseconds * bytesPerMillisecond;
            //byte[] oneSecondBuffer = new byte[bytesPerSecond];
            //int read = reader.Read(oneSecondBuffer, 0, bytesPerSecond);

            //short sample16Bit = BitConverter.ToInt16(oneSecondBuffer, 1);

            //double volume = Math.Abs(sample16Bit / 32768.0);
            //double decibels = 20 * Math.Log10(volume);




            // 마이크 입력 시작 (100ms마다 실행)
            var waveIn = new NAudio.Wave.WaveInEvent
            {
                DeviceNumber = 0,
                WaveFormat = new NAudio.Wave.WaveFormat(rate: 44100, bits: 16, channels: 1),
                BufferMilliseconds = 100
            };
            waveIn.DataAvailable += OnDataAvailable;
            waveIn.StartRecording();

            decibel.Content = dec;
            if(dec > 40)
            {
                decibelOk.Content = dec;
            }

            //// 오디오 입력장치 찾기
            //for(int i = -1; i<NAudio.Wave.WaveIn.DeviceCount; i++)
            //{
            //    var caps = NAudio.Wave.WaveIn.GetCapabilities(i);
            //    MessageBox.Show($"{i}: {caps.ProductName}");
            //}
            //int bytesPerSample = 2;

            //wavesource = new WaveIn();
            //wavesource.WaveFormat = new WaveFormat(16000, 1);
            //wavesource.BufferMilliseconds = (int)((double)1024 / (16000 / 1000.0));
            //wavesource.DeviceNumber = 0; // 마이크 장치 번호

            // 데이터가 들어오면 실행할 이벤트 핸들러
            //wavesource.DataAvailable += new EventHandler<WaveInEventArgs>(waveSource_DataAvailables);


            //// 데이터가 들어오면 호출할 이벤트 핸들러
            //wavesource.DataAvailable += (sender, e) =>
            //{
            //    byte[] buffer = e.Buffer;
            //    double sum = 0;

            //    // 각 샘플을 16비트 signed로 변환하여 루프 돌며 절대값 합을 구합니다.
            //    for (int i = 0; i < e.BytesRecorded; i += bytesPerSample)
            //    {
            //        short sample = (short)((buffer[i + 1] << 8) | buffer[i]);
            //        sum += Math.Abs(sample);
            //    }

            //    // 루프 종료 후 평균값 계산
            //    double average = sum / (e.BytesRecorded / bytesPerSample);

            //    // 평균값을 데시벨로 변환
            //    double decibels = 20 * Math.Log10(average);

            //    decibel.Content = decibels;
            //    if(decibels > 40)
            //    {
            //        decibelOk.Content = decibels;
            //    }

            //};
            //try
            //{
            //    wavesource.StartRecording();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.ToString());
            //}
        }


        private void OnDataAvailable(object sender, NAudio.Wave.WaveInEventArgs e)
        {
            int value;
            int bytesPerSample = 2;
            double sum = 0;

            for(int index = 0; index < e.BytesRecorded; index+= bytesPerSample)
            {
                value = BitConverter.ToInt32(e.Buffer, index);

                // 실시간 처리 함수
                // 그럼 여기에 데시벨 측정이 들어가야함
                sum += (value * value);
            }

            double rms = Math.Sqrt(sum / (e.BytesRecorded / 2));
            dec = 20 * Math.Log10(rms);
        }

        //void waveSource_DataAvailables(object sender, WaveInEventArgs e)
        //{
        //    byte[] buffer = e.Buffer;
        //    double sum = 0;
        //}
    }
}