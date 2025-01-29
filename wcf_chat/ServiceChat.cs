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

        public int Connect(string name)
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

        public void Disconnect(int id)
        {
            var user = users.FirstOrDefault(i => i.ID == id);
            if (user!=null)
            {
                users.Remove(user);
                SendMsg(": " + user.Name + " покинув чат!",0);
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
            }
            answer += msg;
            BroadcastMessage(answer, id);
        }
    }
}