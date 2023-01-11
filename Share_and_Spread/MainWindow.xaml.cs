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
using System.Net;
using System.Net.Sockets;
using System.Windows.Markup;
using System.IO;
using System.Net.WebSockets;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Share_and_Spread
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public class DataList
    {
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordVer { get; set; }
        public string Phonenumber { get; set; }
        public string Address { get; set; }
        public string Complaint { get; set; }

    }

    public partial class MainWindow : Window
    {
        public IDGSocketClient client = new IDGSocketClient();
        public MainWindow()
        {
            InitializeComponent();
            
            client.Connect("localhost", 8888);
            
        }

        private void View_Responses(object sender, RoutedEventArgs e)
        {

        }
        
        private void Send_Complaint(object sender, RoutedEventArgs e)
        {
            var List = new DataList
            {
                Fullname = tb_fullname.Text,
                Email = tb_email.Text,
                Password = tb_password.Text,
                PasswordVer = tb_password_ver.Text,
                Phonenumber = tb_phonenumber.Text,
                Address = tb_address.Text,
                Complaint = tb_Complaint.Text
            };
            client.Send("Send|"+JsonSerializer.Serialize<DataList>(List));
            Console.Read();
        }

    }


    public class IDGSocketClient
    {
        System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
        NetworkStream networkStream;
        Byte[] data;
        public void Connect(string ipAddress, int port)
        {
            clientSocket.ConnectAsync(ipAddress, port);
        }
        public string Send(string message)
        {
            networkStream = clientSocket.GetStream();
            data = System.Text.Encoding.ASCII.GetBytes(message);
            networkStream.Write(data, 0, data.Length);
            networkStream.Flush();
            return Receive();
        }
        public void Close()
        {
            clientSocket.Close();
        }
        public string Receive()
        {
            networkStream = clientSocket.GetStream();

            data = new Byte[256];
            String responseData = String.Empty;
            Int32 bytes = networkStream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

            MessageBox.Show(responseData);
            Console.WriteLine("Received: {0}", responseData);
            return "nothing";
        }
    }
}


