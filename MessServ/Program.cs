using System.Net;
using System.Net.Sockets;
using System.Text;

internal class Program
{
    // public static string userName = "";
    string ip = "127.0.0.1";
    int port = 7632;
    public static IPEndPoint endPoint;
    public static Socket socket;
    public static List<Socket> Clients = new List<Socket>();
    public static List<Thread> Threads = new List<Thread>();
    private static void Main(string[] args)
    {// Logic Server

        //Настройки подключения
        string ip = "127.0.0.1";
        int port = 7632;
        endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(endPoint);

        Thread listen = new Thread(ListenConnection);
        listen.Start();

        Console.ReadLine();
    }

    /// <summary>
    /// Метод который вызывается в другом потоке для прослушивания подключений
    /// </summary>
    public static void ListenConnection()
    {
        while (true)
        {
            socket.Listen(10);
            Console.WriteLine("Ожидание подключений...");
            Socket client = socket.Accept();
            byte[] buff = Encoding.Unicode.GetBytes("Пользователь подключился");
            BroadcastMess(buff);
            Thread thread = new Thread(Rec);
            thread.Start(client);
            Clients.Add(client);
            Threads.Add(thread);
        }
    }


    /// <summary>
    /// Метод для рассылки сообщений всем подключенным пользователям
    /// </summary>
    /// <param name="buff">сообщение для рассылки в байтах</param>
    public static void BroadcastMess(byte[] buff)
    {
        foreach (var client in Clients)
        {
            client.Send(buff);
        }
    }

    /// <summary>
    ///  Метод для рассылки сообщений всем подключенным пользователям
    /// </summary>
    /// <param name="buff">сообщение для рассылки в байтах</param>
    /// <param name="sender">сокет клиента Который отправил сообщение</param>
    public static void BroadcastMess(byte[] buff, Socket sender)
    {
        foreach (var client in Clients)
        {
            if (sender != client)//Проверяем что отправляем сообщение не тому человеку который его прислал :/
            {
                client.Send(buff);
            }
        }
    }

    /// <summary>
    /// Метод который вызываем в потоке для прослушивания сообщений от клиента
    /// </summary>
    /// <param name="obj"></param>
    public static void Rec(object obj)//socket client
    {
        Socket socket = obj as Socket;
        while (true)
        {
            try
            {
                var buff = new byte[4096];
                socket.Receive(buff);
                Console.WriteLine(Encoding.Unicode.GetString(buff));
                BroadcastMess(buff, socket);

            }
            catch (Exception)
            {
                Clients.Remove(socket);
                socket.Close();
                var buff = Encoding.Unicode.GetBytes("Клиент Отключится");
                BroadcastMess(buff);
                break;
            }
        }
    }
}