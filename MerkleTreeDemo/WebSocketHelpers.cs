using WebSocketSharp;

namespace MerkleTreeDemo
{
    public class MyListener { }

    public static partial class WebSocketHelpers
    {
        private static WebSocket ws;
        private static string response;

        public static void DropShape(string shapeName, int x, int y, string text = "")
        {
            Connect();
            ws.Send(string.Format("cmd=CmdDropShape&ShapeName={0}&X={1}&Y={2}&Text={3}", shapeName, x, y, text));
        }

        public static void DropShape(string shapeName, int x, int y, int w, int h, string text = "")
        {
            Connect();
            ws.Send(string.Format("cmd=CmdDropShape&ShapeName={0}&X={1}&Y={2}&Width={3}&Height={4}&Text={5}", shapeName, x, y, w, h, text));
        }

        public static void DropConnector(string shapeName, int x1, int y1, int x2, int y2)
        {
            Connect();
            ws.Send(string.Format("cmd=CmdDropConnector&ConnectorName={0}&X1={1}&Y1={2}&X2={3}&Y2={4}", shapeName, x1, y1, x2, y2));
        }

        private static void Connect()
        {
            if (ws == null || !ws.IsAlive)
            {
                ws = new WebSocket("ws://192.168.1.124:1100/flowsharp", new MyListener());

                ws.OnMessage += (sender, e) =>
                {
                    response = e.Data;
                };

                ws.Connect();
            }
        }
    }
}