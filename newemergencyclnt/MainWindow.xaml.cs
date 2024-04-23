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
using System.Net.Sockets;

using NAudio.Wave;
using System.IO;
using System.Runtime.InteropServices;
using NAudio.Wave.Compression;
using NAudio.CoreAudioApi;
using NAudio.Utils;
using System.Windows.Threading;

namespace emergencyclnt
{

    public partial class MainWindow : Window
    {

        public TcpClient clnt;
        public NetworkStream stream;

        public WaveIn wavesource = null;
        public WaveIn wavesource1 = null;
        public WaveFileWriter wavefile = null;
        public WaveFileReader sendfile = null;

        public MainWindow()
        {
            InitializeComponent();
            
            recogmic();
            //ConncectToServer();
            label.Content = "음성녹음입니다.";

        }
        public async Task recogmic()
        {
            int bytesPerSample = 2;

            wavesource = new WaveIn();
            wavesource.WaveFormat = new WaveFormat(16000, 1);
            wavesource.BufferMilliseconds = (int)((double)1024 / (16000 / 1000.0));
            wavesource.DeviceNumber = 0; // 마이크 장치 번호

            // 데이터가 들어오면 호출할 이벤트 핸들러
            wavesource.DataAvailable += (sender, e) =>
            {
                byte[] buffer = e.Buffer;
                double sum = 0;

                // 각 샘플을 16비트 signed로 변환하여 루프 돌며 절대값 합을 구합니다.
                for (int i = 0; i < e.BytesRecorded; i += bytesPerSample)
                {
                    short sample = (short)((buffer[i + 1] << 8) | buffer[i]);
                    sum += Math.Abs(sample);
                }

                // 루프 종료 후 평균값 계산
                double average = sum / (e.BytesRecorded / bytesPerSample);

                // 평균값을 데시벨로 변환
                double decibels = 20 * Math.Log10(average);

                Decibel.Content = decibels;
                if (decibels > 70)
                {
                    realtimerecord();
                }

            };
            try
            {
                wavesource.StartRecording();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        int ak = -1;
        List<WaveIn> wave = new List<WaveIn>();
        private async void realtimerecord()
        {

            //MessageBox.Show("음성녹음을 시작합니다.");
            await Task.Run(async () =>
            {
                //wavesource1 = new WaveIn();
                //wavesource1.WaveFormat = new WaveFormat(16000, 1);
                ak++;
                wavesource.DataAvailable += new EventHandler<WaveInEventArgs>(waveSource_DataAvailable);
                wavesource.RecordingStopped += new EventHandler<StoppedEventArgs>(waveSource_RecordingStopped);
                wave.Add(wavesource);
                //wavefile = new WaveFileWriter("C:\\Users\\AIOT1\\Desktop\\fuxkfuxkfuxk\\ak" + ak+".wav", wavesource.WaveFormat);
                wavefile = new WaveFileWriter("C:\\Users\\AIOT1\\Desktop\\fuxkfuxkfuxk\\ak" + ak + ".wav", wave[ak].WaveFormat);
                
                wave[ak].StartRecording();
                
                realtimerecordstop();
            });

        }
        int ss = 0;
        private async void realtimerecordstop()
        {
            
            await Task.Run(async () =>
            {
                Thread.Sleep(10000);
            });
            //wavesource.StopRecording();
            wave[ss].StopRecording();
            ss++;
            //MessageBox.Show("끝남");
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
            label.Content = "음성녹음을 시작합니다.";

            start.IsEnabled = false;
            stop.IsEnabled = true;

            wavesource = new WaveIn();
            wavesource.WaveFormat = new WaveFormat(16000, 1);

            wavesource.DataAvailable += new EventHandler<WaveInEventArgs>(waveSource_DataAvailable);
            wavesource.RecordingStopped += new EventHandler<StoppedEventArgs>(waveSource_RecordingStopped);

            wavefile = new WaveFileWriter("C:\\Users\\iot\\source\\repos\\WPFEmergency\\voice\\test.wav", wavesource.WaveFormat);

            wavesource.StartRecording();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // 레코딩 종료
            label.Content = "음성녹음을 종료합니다.";

            stop.IsEnabled = false;

            wavesource.StopRecording();
        }

        void waveSource_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (wavefile != null)
            {
                wavefile.Write(e.Buffer, 0, e.BytesRecorded);
                wavefile.Flush();
            }
        }

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
            string waveFilePath = "C:\\Users\\iot\\source\\repos\\WPFEmergency\\voice\\test.wav";

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
                while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, bytesRead);
                }
            }
        }
    }
}