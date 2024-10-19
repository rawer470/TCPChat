using System.Net;
using System.Net.Sockets;
using System.Text;

internal class Program
{
    public static string userName = "";
    private static void Main(string[] args)
    {
        //CLIENT
        string ip = "127.0.0.1";
        int port = 7632;
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        Console.Write("Введите ваше Имя: ");
        userName = Console.ReadLine();

        socket.Connect(endPoint);
        Thread receiv = new Thread(Rec);
        receiv.Start(socket);

        while (true)
        {
            string mess = Console.ReadLine();
            var buffer = Encoding.Unicode.GetBytes($"[{userName}]: {mess}");
            socket.Send(buffer);
        }
    }

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
            }
            catch (Exception)
            {
                Console.WriteLine("Server не работает :(");
                break;
            }
        }
    }
}