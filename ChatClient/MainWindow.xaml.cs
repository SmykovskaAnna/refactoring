using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
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
using ChatClient.ServiceChat;

namespace ChatClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window , IServiceChatCallback
    {
        bool isConnected = false;
        ServiceChatClient client;
        int ID;
        public ObservableCollection<User> OnlineUsers { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            OnlineUsers = new ObservableCollection<User>();
            OnlineUsersListBox.ItemsSource = OnlineUsers;
        }

        private void Windows_Loaded(object sender, RoutedEventArgs e)
        {
        }

        void ConnectUser()
        {
            if(!isConnected)
            {
                client = new ServiceChatClient(new System.ServiceModel.InstanceContext(this));
                ID = client.Connect(tbUserName.Text);
                tbUserName.IsEnabled = false;
                bConnDicon.Content = "Disconnect";
                isConnected = true;
            }
        }

        void DisconnectUser()
        {
            if (isConnected)
            {
                client.Disconnect(ID);
                client = null;
                tbUserName.IsEnabled = true;
                bConnDicon.Content = "Connect";
                isConnected = false;
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string userName = tbUserName.Text;
            User newUser = new User { UserName = userName, IsOnline = true };

            OnlineUsers.Add(newUser);

            OnlineUsersListBox.ItemsSource = OnlineUsers.Where(u => u.IsOnline);

            if (isConnected)
            {
                DisconnectUser();
            }
            else
            {
                ConnectUser();
            }
        }

        public void MsgCallback(string msg)
        {
            lbChat.Items.Add(msg);
            lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count-1]);
        }

        private void Windows_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DisconnectUser();
        }

        private void tbMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (client!=null)
                {
                    client.SendMsg(tbMessage.Text, ID);
                    tbMessage.Text = string.Empty;
                }
            }
        }
        public class User
        {
            public string UserName { get; set; }
            public bool IsOnline { get; set; }
        }
    }
}