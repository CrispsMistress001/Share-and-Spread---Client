﻿using System;
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

namespace Share_and_Spread
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public IDGSocketClient client = new IDGSocketClient();
        public MainWindow()
        {
            InitializeComponent();
            
            client.Connect("fe80::6f9e:9169:a7a7:338b%12", 8888);
            
        }

        private void View_Responses(object sender, RoutedEventArgs e)
        {

        }

        private void Send_Complaint(object sender, RoutedEventArgs e)
        {
            client.Send("Hello");
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
            clientSocket.Connect(ipAddress, port);
            networkStream = clientSocket.GetStream();

        }
        public void Send(string message)
        {
            data = System.Text.Encoding.ASCII.GetBytes(message);
            networkStream.Write(data, 0, data.Length);
        }
        public void Close()
        {
            clientSocket.Close();
        }
        public string Receive(string message)
        {
            Console.WriteLine("Sent: {0}", message);
            data = new Byte[256];
            String responseData = String.Empty;
            Int32 bytes = networkStream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

            Console.WriteLine("Received: {0}", responseData);
            return "nothing";
        }
    }
}

