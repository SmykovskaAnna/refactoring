using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;


namespace wcf_chat
{

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ServiceChat : IServiceChat
    {
        private List<ServerUser> users = new List<ServerUser>();
        private int nextId = 1;
        private readonly object usersLock = new object();

        public static class Messages
        {
            public const string UserConnected = "{0} підключився до чату!";
            public const string UserDisconnected = "{0} покинув чат!";
        }

        public int Connect(string name)
        {
            ServerUser user = new ServerUser() {
                ID = nextId,
                Name = name,
                operationContext = OperationContext.Current
            };
            nextId++;
            SendMsg(string.Format(Messages.UserConnected, user.Name), 0);
            users.Add(user);  
            return user.ID;
            lock (usersLock)
            {
                ServerUser user = new ServerUser() {
                    ID = nextId,
                    Name = name,
                    operationContext = OperationContext.Current
                };
                nextId++;

                SendMsg(": " + user.Name + " підключився до чату!",0);
                users.Add(user);
                return user.ID;
            }
        }

        public void Disconnect(int id)
        {
            lock (usersLock)
            {
                users.Remove(user);
                SendMsg(string.Format(Messages.UserDisconnected, user.Name), 0);
                var user = users.FirstOrDefault(i => i.ID == id);
                if (user!=null)
                {
                    users.Remove(user);
                    SendMsg(": " + user.Name + " покинув чат!",0);
                }
            }
        }

        private void BroadcastMessage(string message, int senderId)
        {
            string timestamp = DateTime.Now.ToShortTimeString();
            string formattedMessage = $"{timestamp} {message}";

            foreach (var user in users)
            {
                user.operationContext.GetCallbackChannel<IServerChatCallback>().MsgCallback(formattedMessage);
            }
        }

        public void SendMsg(string msg, int id)
        {
            var user = users.FirstOrDefault(i => i.ID == id);
            string answer = DateTime.Now.ToShortTimeString();
            if (user != null)
            {
                answer += ": " + user.Name + " ";
            lock (usersLock)
            {
                foreach (var item in users)
                {
                    string answer = DateTime.Now.ToShortTimeString();

                    var user = users.FirstOrDefault(i => i.ID == id);
                    if (user != null)
                    {
                        answer += ": " + user.Name + " ";
                    }
                    answer += msg;
                    item.operationContext.GetCallbackChannel<IServerChatCallback>().MsgCallback(answer);
                }
            }
            answer += msg;
            BroadcastMessage(answer, id);
        }
    }
}