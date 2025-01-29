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
        List<ServerUser> users = new List<ServerUser>();
        int nextId = 1;

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
        }

        public void Disconnect(int id)
        {
            var user = users.FirstOrDefault(i => i.ID == id);
            if (user!=null)
            {
                users.Remove(user);
                SendMsg(string.Format(Messages.UserDisconnected, user.Name), 0);
            }
        }

        public void SendMsg(string msg, int id)
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
    }
}