using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

namespace DispetcherZadach
{
    internal class Controller
    {
       
        public delegate void ServerHandler(string message);

        public event ServerHandler ServerRecieveEvent;///собітие на которое подпишеться вьюшка!!!!!!!!!!!!!!!!!!!!!!
        public Controller()
        { }
       
        public async void ConnectionToClient()
        {
            await Task.Run(() =>
            {
                try
                {
                   // MessageBox.Show("Server is waiting for connection");
                    TcpListener listener = new TcpListener(IPAddress.Any, 49152);
                    listener.Start();

                    while (true)
                    {
                        TcpClient client = listener.AcceptTcpClient();                  
                        Task.Factory.StartNew(() => { ReadSendMessage(client); });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Server say: " + ex.Message);
                }
            });

        }

        // метод Читаем сообщение от клиента 
        public async void ReadSendMessage(TcpClient client)
        {
            try
            {
                NetworkStream netstream = client.GetStream();
                byte[] arr = new byte[client.ReceiveBufferSize];

                int len = netstream.Read(arr, 0, client.ReceiveBufferSize);
                MemoryStream memoryStream = new MemoryStream(arr);//temp buffer
                BinaryFormatter formatter = new BinaryFormatter();
                List<string>actions = (List<string>)formatter.Deserialize(memoryStream);
                Action(actions[0], actions[1]);  // в параметрах действие клиента и индекс процесса выбранного

                memoryStream.Close();
                memoryStream = new MemoryStream(); //create new memory for list of process to send client
                Dictionar dictionar = new Dictionar(); // словарь со всеми процессами 
               
                formatter.Serialize(memoryStream, dictionar.CreateList());  // возвращает словарь с процессами и сериализует его в memoryStream    
               
                arr= memoryStream.ToArray();    //массив байтов
                memoryStream.Close();
                netstream.Write(arr, 0, arr.Length);//передаем клиенту
               
                netstream.Close();
                client.Close(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Server say: " + ex.Message);
            }


        }

        public void Action(string act, string indexProc)
        {
            if (act == "2")
            {
                Process process = Process.GetProcessById(int.Parse(indexProc));
                if (process.MainWindowTitle != "DispetcherZadach") 
                process.Kill();
                ServerRecieveEvent("Stopped the process");
            }
        }


        // метод Отправить список процессов клиенту
        public async void SendProc(TcpClient client)
        {
            await Task.Run(() =>
            {
                try
                {
                    NetworkStream netstream = client.GetStream();
                    MemoryStream stream = new MemoryStream();
                    BinaryFormatter formatter = new BinaryFormatter();
                    Dictionar list = new Dictionar();
                    formatter.Serialize(stream, list.CreateList());
                    byte[] arr = stream.ToArray();
                    stream.Close();
                    netstream.Write(arr, 0, arr.Length);
                    netstream.Close();
                }
                catch (Exception ex)
                {
                   
                    MessageBox.Show("Server say: " + ex.Message);
                }
            });
        }
    }
}