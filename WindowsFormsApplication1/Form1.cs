using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1 {
    public partial class Form1 : Form {
        TcpClient tcpClient=new TcpClient ();
        NetworkStream serverStream=default (NetworkStream);
        String readData=null;
        StreamWriter writer=null;
        StreamReader reader=null;

        public Form1 () {
            InitializeComponent ();
        }
        public Form1 (String nickname, String ip) {
            InitializeComponent ();
            connect (nickname, ip);
        }

        private void connect (String nickname, String ip) {
            tcpClient.Connect (ip, 5150);

            Thread chatThread=new Thread (new ThreadStart (getMessage));
            chatThread.Start ();

            serverStream=tcpClient.GetStream ();
            writer=new StreamWriter (serverStream);
            writer.WriteLine (nickname);
            writer.Flush ();
        }
        private void getMessage () {
            reader=new StreamReader (serverStream);
            String dataReceived=null;
            try {
                while (serverStream.CanRead) {
                    dataReceived=reader.ReadLine ();

                    switch (dataReceived.Substring(0,3)) {
                        case "MES":
                            readData=dataReceived.Substring (4, (dataReceived.Length-4));
                            msg ();
                            break;
                        case "FOR":
                            error.Text=dataReceived.Substring (4, (dataReceived.Length-4));
                            break;
                        case "NWU":
                            listBox1.Items.Clear ();
                            string[] userList=dataReceived.Substring (4, (dataReceived.Length-4)).Split (':');
                            foreach (String user in userList) {
                                listBox1.Items.Add (user);
                            }
                            break;
                        default:
                            error.Text="Error 404:Keyword not found";
                            break;
                    }
                }
            } catch (Exception ) {
                //Console.WriteLine (ex.ToString ());
            }
        }
        private void msg () {
            if (this.InvokeRequired)
                this.Invoke (new MethodInvoker (msg));
            else
                textbox.AppendText (readData+"\n");
        }

        private void send_KeyDown (object sender, KeyEventArgs e) {
            if (e.KeyData==Keys.Enter) {
                writer.WriteLine ("MES:"+send.Text);
                writer.Flush ();
                send.Text="";
            }
        }

        private void button2_Click (object sender, EventArgs e) {
            writer.WriteLine ("FOR:"+textBox1.Text+":"+textBox2.Text+":"+textBox3.Text);
            writer.Flush ();
        }
    }
}
