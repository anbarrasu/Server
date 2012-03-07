using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using PacketFormat;
using System.Windows.Forms;
using Access_Control_System;

namespace AcsServer
{
    class AcsServerMain
    {
        private UdpSocketListener udpserver;
        private bool vStopthread;
        private Queue<RdrMsgFormat> fromclientQueue;
        private Thread thdloop;
        AutoResetEvent QueueEvnt;
        dataBaseM dbM;

        public AcsServerMain()
        {
            vStopthread = false;
            fromclientQueue = new Queue<RdrMsgFormat>();
            QueueEvnt = new AutoResetEvent(false);
            thdloop = new Thread(loop);
            dbM = new dataBaseM();
            thdloop.Start();
        }

        public void stop()
        {

            vStopthread = true;
            //udpserver.forceStopThread();
        }

        public void loop()
        {
            //try
            //{
                //MessageBox.Show("In Main thread");
                udpserver = new UdpSocketListener(ref fromclientQueue,ref QueueEvnt);
                RdrMsgFormat rcvdvalue;

                while (udpserver.isthreadAlive() && vStopthread != true)
                {
                    QueueEvnt.WaitOne(1000,false);
                    while (fromclientQueue.Count > 0)
                    {
                        rcvdvalue = fromclientQueue.Dequeue();

                        dbM.addattendance(rcvdvalue.cardid,rcvdvalue.doorno,Convert.ToInt32( rcvdvalue.inout));
                        //Put it in database 
                        //MessageBox.Show(rcvdvalue.cardid.ToString());
                    }
                }

                udpserver.stop_receiving = true;
                //udpserver.jointhd();
                MessageBox.Show("Acs Main thread exiting after Join");
                //Inform the UI thread of the death of the udpserver and exit
            //}
           
            //catch(Exception ex)
            //{
            //    applog.logexcep("AcsServerMain: loop()",ex.Message);

            //}
        }


    }
}
