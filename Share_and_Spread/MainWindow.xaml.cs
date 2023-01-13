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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;
using System.Xml.Linq;

//using System.Text.Json;
//using System.Text.Json.Serialization;

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
        public string Title { get; set; }
        public string Phonenumber { get; set; }
        public string Address { get; set; }
        public string Complaint { get; set; }

    }

    public partial class MainWindow : Window
    {
        private int UserID;
        public IDGSocketClient client = new IDGSocketClient();
        public MainWindow()
        {
            InitializeComponent();
            Complaint_Window.Visibility = Visibility.Visible;
            Complaint_View_Window.Visibility = Visibility.Hidden;
            client.Connect("192.168.1.154", 8888);

        }

        private void View_Responses(object sender, RoutedEventArgs e)
        {
            Complaint_Window.Visibility = Visibility.Hidden;
            Complaint_View_Window.Visibility = Visibility.Visible;
        }

        private void Send_Complaint(object sender, RoutedEventArgs e)
        {
            var List = new DataList
            {
                Fullname = tb_fullname.Text,
                Email = tb_email.Text,
                Password = tb_password.Text,
                Title = tb_title.Text,
                Phonenumber = tb_phonenumber.Text,
                Address = tb_address.Text,
                Complaint = tb_Complaint.Text
            };
            MessageBox.Show(client.Send("Send|" + JsonConvert.SerializeObject(List)));
        }

        private void btn_ComplaintView_Click(object sender, RoutedEventArgs e)
        {
            Complaint_Window.Visibility = Visibility.Visible;
            Complaint_View_Window.Visibility = Visibility.Hidden;
        }

        private void lsb_Complaint_List_PreviewMouseDown(object sender, MouseEventArgs e) {
            var Boxitem = ItemsControl.ContainerFromElement(sender as ListBox, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (Boxitem != null)
            {
                tbc_List.SelectedIndex = 1;
                var TEMP = client.Send("ViewComplaint|"+ Boxitem.Content);

                if (TEMP == "Found nothing!")
                {
                    MessageBox.Show(TEMP);
                }
                else
                {
                    foreach (var item in TEMP.Split(new string[] { "|BRK|" }, StringSplitOptions.None))
                    {
                        
                        //lsb_View_Complaint.Items.Add(new ListViewItem(new string[] { item }));
                    }


                }
            }
        }

        private void btn_login_Click(object sender, RoutedEventArgs e)
        {
            lsb_Complaint_List.Items.Clear();
            var Email = tb_Login_Email.Text;
            var Password = tb_Login_Password.Text;


            JObject o = JObject.FromObject(new
            {
                Email = tb_Login_Email.Text,
                Password = tb_Login_Password.Text
            });
            var TEMP = client.Send("Login|" + o.ToString());
            if (TEMP.Split('|')[0] == "Logged in!")
            {
                MessageBox.Show("Logged in!");
                UserID = Int32.Parse(TEMP.Split('|')[1]);

                TEMP = client.Send("Retrieve|" + UserID.ToString());
                if (TEMP == "Found nothing!")
                {
                    MessageBox.Show(TEMP);
                }
                else
                {
                    foreach (var item in TEMP.Split(new string[] { "|BRK|" }, StringSplitOptions.None))
                    {
                        String[] itemsDepth = item.Split(new string[] { ";;;" }, StringSplitOptions.None);
                        // MessageBox.Show(itemsDepth[0]);
                        lsb_Complaint_List.Items.Add(itemsDepth[0] + ":" + itemsDepth[1]);
                    }

                    
                }

            }
            else
            {
                MessageBox.Show("Failed to login!");
            }
        }

        /////////////////////////////////////// SEPARATE FUNCTIONS ////////////////////////////////////////////////////////////////////////
    }


    public class IDGSocketClient
    {
        System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
        NetworkStream networkStream;
        Byte[] data;
        public void Connect(string ipAddress, int port)
        {
            try
            {
                clientSocket.ConnectAsync(ipAddress, port);
            }
            catch (Exception e)
            {
                MessageBox.Show("Failed to connect to the server");
                Console.WriteLine("Message - " + e.Message);
            }
        }
        public string Send(string message)
        {
            try
            {
                networkStream = clientSocket.GetStream();
                data = System.Text.Encoding.ASCII.GetBytes(message);
                networkStream.Write(data, 0, data.Length);
                networkStream.Flush();
                return Receive();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error-" + e.Message);
            }
            return "";
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
            Console.WriteLine("Received: {0}", responseData);
            return responseData;
        }
    }
}


