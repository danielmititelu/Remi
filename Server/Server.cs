﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server {
    class Server {
        public static List<User> clientsList=new List<User>();
        public static List<Room> roomList=new List<Room>();

        public Server() {
            TcpClient client=null;
            String nickname=null;
            StreamReader read;
            TcpListener server=new TcpListener(IPAddress.Any, 5150);
            Console.WriteLine("Chat server started");
            server.Start();
            while(true) {
                if(server.Pending()) {
                    client=server.AcceptTcpClient();
                    read=new StreamReader(client.GetStream());
                    nickname=read.ReadLine();
                    if(clientsList.Exists(c=> c.Nickname==nickname)) {
                        StreamWriter write=null;
                        write=new StreamWriter(client.GetStream());
                        write.WriteLine("NICKNAME_TAKEN:"+nickname);
                        write.Flush();
                        Console.WriteLine("Client "+nickname+" already exists");
                    } else {
                        clientsList.Add(new User(nickname, client));
                        Thread chatThread=new Thread(() => ConnectToChat(client, nickname));
                        chatThread.Start();
                        MessageSender.MsgtoClient(nickname, "WELCOME:"+nickname, clientsList);
                        Console.WriteLine("A new client has connected to chat "+nickname);
                        MessageSender.AllUsers("NEW_USER_IN_CHAT", clientsList,false);
                        MessageSender.AllRooms(roomList,clientsList);
                    }
                }
            }
        }

        private void ConnectToChat(TcpClient client, string nickname) {
            new HandleClient(client, nickname);
        }

        static void Main(string[] args) {
            new Server();
        }
    }
}
