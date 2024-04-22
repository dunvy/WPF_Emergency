using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics; //TCP
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net; //TCP
using System.Net.Sockets; //TCP
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


namespace emergencyServer
{
    public partial class MainWindow : Window
    {
        TcpListener server = null;

        public MainWindow()
        {
            InitializeComponent();

            //서버 코드 작성
            string bindIP = "10.10.20.103";
            const int bindPort = 9090;

            try
            {
                /*IPEndPoint localAdr = new IPEndPoint(IPAddress.Parse(bindIP), bindPort);*/
                IPEndPoint localAdr = new IPEndPoint(IPAddress.Parse(bindIP), bindPort);//주소 정보 설정

                server = new TcpListener(localAdr); //TCPListener 객체 생성

                server.Start(); //서버 오픈

                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    NetworkStream stream = client.GetStream();
                }
            }
            catch (SocketException err) //소켓 오류 날때 예외처리
            {
                MessageBox.Show(err.ToString());
            }
        }
    }
}