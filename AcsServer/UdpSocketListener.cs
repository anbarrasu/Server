using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using PacketFormat;
using System.Windows.Forms;

namespace AcsServer
{
    class UdpSocketListener
    {
        private bool receive;
        private UInt16 listening_port;
        private Thread rcv_thread;
        Socket listening_socket;
        Queue<RdrMsgFormat> tomain_msgQueue;
        AutoResetEvent QueueEvnt;

        public UdpSocketListener()
        {
            receive=true;
            listening_port = 33211;

            //open socket
            listening_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint ServerEndPoint = new IPEndPoint(IPAddress.Any, listening_port);
            listening_socket.Bind(ServerEndPoint);
            

           
        }

        public UdpSocketListener(ref Queue<RdrMsgFormat> tomainPktQueue,ref AutoResetEvent queueEvent)
        {
            receive = true;
            listening_port = 33211;

            //open socket
            listening_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //listening_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 1000);
            IPEndPoint ServerEndPoint = new IPEndPoint(IPAddress.Any, listening_port);
            listening_socket.Bind(ServerEndPoint);

            tomain_msgQueue = tomainPktQueue;
            QueueEvnt = queueEvent;
            startthd();
            
        }

        public bool jointhd()
        {
            rcv_thread.Join();
            return true;
        }

        public bool startthd()
        {
            try
            {
                //Create a new thread
                if (tomain_msgQueue != null)
                    rcv_thread = new Thread(thd_receiveUdpPkts);
                rcv_thread.IsBackground = true;
                rcv_thread.Start();
            }
            catch (Exception ex)
            {
                applog.logexcep("UdpSocketListenerClass: startthd()",ex.Message);
            }
        return true;
        }

        public bool isthreadAlive()
        {
            return rcv_thread.IsAlive;
        }

        public bool forceStopThread()
        {
            try
            {
                rcv_thread.Abort();
                return true;
            }
            catch(Exception ex)
            {
                applog.logexcep("forceStopThread",ex.Message);
                return false;   
            }
        }
        public UInt16 port
        {
            get
            {
                return listening_port;
            }
            set
            {
                listening_port = port;
            }
        }

        public bool stop_receiving
        {
            get
            {
                return !receive;
            }
            set
            {
                receive = !stop_receiving;
            }
        }

        void thd_receiveUdpPkts()
        {
            byte[] message = new byte[1024];
            int bytesRead;
            RdrMsgFormat rcvmsg = new RdrMsgFormat();
            //MessageBox.Show("In Receive thread");
            try
            {
                // Creates an IPEndPoint to capture the identity of the client when we'll use the Socket.ReceiveFrom Method
                IPEndPoint remote_endpoint = new IPEndPoint(IPAddress.Any, 0);   // IP and PORT pairing of the client

                EndPoint ep = (EndPoint)remote_endpoint;
                while (receive)
                {
                    bytesRead=listening_socket.ReceiveFrom(message, ref ep);
                    //Check for the ip address
                    if (bytesRead>0)
                    {
                        //MessageBox.Show(string.Format("No of bytes read: {0}",bytesRead));
                        rcvmsg = rcvmsg.GetPacketObject(message);

                        //Deserilise the received message
                        if (tomain_msgQueue != null)
                        {
                            //MessageBox.Show("Putting data in Queue");
                            tomain_msgQueue.Enqueue(rcvmsg);
                            QueueEvnt.Set();
                        }
                        
                    }
                    

                    //Send to a queue that would be read by
                }
                MessageBox.Show("Exiting Udp Receive thread");
                QueueEvnt.Set();
            }
            catch (Exception ex)
            {
                applog.logexcep("thd_receiveUdpPkts", ex.Message);
            }
        }

        //public void listener()
        //{
        //    Socket server_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //    IPEndPoint ServerEndPoint = new IPEndPoint(IPAddress.Any, listening_port);

        //    server_socket.Bind(ServerEndPoint);

        //    server_socket.BeginReceiveFrom(recBuffer, 0, recBuffer.Length,
        //    SocketFlags.None, ref bindEndPoint,
        //    new AsyncCallback(MessageReceivedCallback), (object)this);
        //}

        //void MessageReceivedCallback(IAsyncResult result)
        //{
        //    EndPoint remoteEndPoint = new IPEndPoint(0, 0);
        //    try
        //    {
        //        int bytesRead = receiveSocket.EndReceiveFrom(result,
        //            ref remoteEndPoint);
        //        player.FromBuffer(recBuffer, 0, Math.Min(recBuffer.Length,
        //            bytesRead));
        //        Console.WriteLine("ID:{0} X:{1} Y:{2}", player.playerID,
        //            player.locationX, player.locationY);
        //    }
        //    catch (SocketException e)
        //    {
        //        Console.WriteLine("Error: {0} {1}", e.ErrorCode, e.Message);
        //    }

        //    receiveSocket.BeginReceiveFrom(recBuffer, 0, recBuffer.Length,
        //        SocketFlags.None, ref bindEndPoint,
        //        new AsyncCallback(MessageReceivedCallback), (object)this);
        //}

        //create socket 
        //bind to the port
        //start listening


    }
}


