using System;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using TCPServer;
using KeyboardInput;
using ApplicationProtocol;
using RichTextBoxExtension;
using System.Drawing;
using System.IO;
namespace NotepadServer
{
    public partial class ServerForm : Form
    {
        Server myServer;
        ApplicationProtocol.ApplicationProtocol AppProtocol;
        public string FileName = "testing.txt";
        public ServerForm()
        {
            InitializeComponent();
            AppProtocol = new ApplicationProtocol.ApplicationProtocol();
            ServerCallback callback = new ServerCallback(FileName,serverBox,AppProtocol);
            myServer = new Server(6000, callback,AppProtocol);
            callback.server = myServer;
            try
            {
                if (File.Exists(FileName))
                {
                    serverBox.LoadFile(FileName);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            RichTextBox.CheckForIllegalCrossThreadCalls = false;
            this.FormClosing += ServerForm_FormClosing;
        }

        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }
    }

    public class ServerCallback : Callback
    {
        public TCPServer.Server server;
        RichTextBox rtb;
        string FileName;
        ApplicationProtocol.ApplicationProtocol AppProtocol;
        public ServerCallback(string fileName, RichTextBox rtb,ApplicationProtocol.ApplicationProtocol AppProtocol)
        {
            FileName = fileName;
            this.rtb = rtb;
            this.AppProtocol = AppProtocol;
        }

        public override void ClientConnected(Client client)
        {
            rtb.SaveFile(FileName);
            client.SendTo(AppProtocol.CreateUpdateNewClientMessage(File.ReadAllText(FileName)));
        }

        public override void Process(Client client, byte[] msg)
        {
            int infoByte = AppProtocol.GetMessageType(msg);
            server.Broadcast(client, msg, true);
            if (infoByte == ApplicationProtocol.ApplicationProtocol.Save)
            {
                rtb.SaveFile(FileName);
                return;
            }
            if(infoByte == ApplicationProtocol.ApplicationProtocol.ToggleStyle)
            {
                int selectionStart;
                int selectionLength;
                FontStyle style;
                AppProtocol.DecodeToggleStyle(msg, out selectionStart, out selectionLength, out style);
                //int oldStart = rtb.SelectionStart;
                //int oldLength = rtb.SelectionLength;
                rtb.SelectionStart = selectionStart;
                rtb.SelectionLength = selectionLength;
                rtb.ToggleFontStyle(style);
                //rtb.SelectionStart = oldStart;
                //rtb.SelectionLength = oldLength;
                return;
            }

            if(infoByte == ApplicationProtocol.ApplicationProtocol.ChangeSize)
            {
                int selectionStart;
                int selectionLength;
                float amount;
                AppProtocol.DecodeChangeSize(msg, out selectionStart, out selectionLength, out amount);
                rtb.SelectionStart = selectionStart;
                rtb.SelectionLength = selectionLength;
                rtb.ChangeFontSize(amount);
                return;
            }

            if(infoByte == ApplicationProtocol.ApplicationProtocol.ChangeFont)
            {
                int selectionStart;
                int selectionLength;
                float size;
                FontStyle style;
                string name;
                AppProtocol.DecodeChangeFont(msg, out selectionStart, out selectionLength, out size, out style, out name);
                rtb.SelectionStart = selectionStart;
                rtb.SelectionLength = selectionLength;
                rtb.SelectionFont = new Font(new FontFamily(name), size);
                rtb.SelectionFont = new Font(rtb.SelectionFont, style);
            }

            if (infoByte == ApplicationProtocol.ApplicationProtocol.ChangeAlignment)
            {
                int selectionStart;
                int selectionLength;
                HorizontalAlignment alignment;
                AppProtocol.DecodeChangeAlignment(msg, out selectionStart, out selectionLength, out alignment);
                rtb.SelectionStart = selectionStart;
                rtb.SelectionLength = selectionLength;
                rtb.SelectionAlignment = alignment;
                return;
            }

            bool cap = false;
            if (infoByte == ApplicationProtocol.ApplicationProtocol.KeyInfoCapital)
            {
                cap = true;
                infoByte = 0;
            }
            if (infoByte == ApplicationProtocol.ApplicationProtocol.KeyInfoLowerCase)
            {
                int selectionStart;
                int selectionLength;
                string dataFromClient;
                AppProtocol.DecodeKeyInfoMessage(msg, out infoByte, out selectionStart, out selectionLength, out dataFromClient);
                if (dataFromClient == "") { return; }
                dataFromClient = dataFromClient.Split('\0')[0];
                int seperator = dataFromClient.IndexOf(':');
                string cmd;
                string data;
                if (seperator == -1)
                {
                    if (dataFromClient == "Undo")
                    {
                        rtb.Undo();
                    }
                    else if (dataFromClient == "Redo")
                    {
                        rtb.Redo();
                    }
                    return;
                }

                cmd = dataFromClient.Substring(0, seperator);
                data = dataFromClient.Substring(seperator+1, dataFromClient.Length-seperator-1);

                if (cmd == "Paste")
                {
                    rtb.SelectedText = data;
                }

                if (data != "None" && cmd == "key")
                {
                    rtb.Select(selectionStart, selectionLength);
                    if (data.Length == 1)
                    {
                        if (!cap) { data = data.ToLower(); }
                        rtb.SelectedText = data;
                    }
                    else if (data == "Tab")
                    {
                        rtb.SelectedText = "\t";
                    }
                    else if (data.Length == 2 && data[0] == 'D')
                    {
                        if (cap == false)
                        {
                            rtb.SelectedText = "" + data[1];
                        }
                        else
                        {
                            if (data[1] == '1') { rtb.SelectedText = "!"; }
                            if (data[1] == '2') { rtb.SelectedText = "@"; }
                            if (data[1] == '3') { rtb.SelectedText = "#"; }
                            if (data[1] == '4') { rtb.SelectedText = "$"; }
                            if (data[1] == '5') { rtb.SelectedText = "%"; }
                            if (data[1] == '6') { rtb.SelectedText = "^"; }
                            if (data[1] == '7') { rtb.SelectedText = "&"; }
                            if (data[1] == '8') { rtb.SelectedText = "*"; }
                            if (data[1] == '9') { rtb.SelectedText = "("; }
                            if (data[1] == '0') { rtb.SelectedText = ")"; }

                        }
                    }
                    else if (data.Substring(0, 3) == "Oem")
                    {
                        if (data == "OemOpenBrackets") { rtb.SelectedText = (!cap) ? "[" : "{"; }
                        if (data == "Oem6") { rtb.SelectedText = (!cap) ? "]" : "}"; }
                        if (data == "Oem1") { rtb.SelectedText = (!cap) ? ";" : ":"; }
                        if (data == "Oem7") { rtb.SelectedText = (!cap) ? "'" : "\""; }
                        if (data == "Oemcomma") { rtb.SelectedText = (!cap) ? "," : "<"; }
                        if (data == "OemPeriod") { rtb.SelectedText = (!cap) ? "." : ">"; }
                        if (data == "OemQuestion") { rtb.SelectedText = (!cap) ? "/" : "?"; }
                        if (data == "Oem5") { rtb.SelectedText = (!cap) ? "\\" : "|"; }
                        if (data == "OemMinus") { rtb.SelectedText = (!cap) ? "-" : "_"; }
                        if (data == "Oemplus") { rtb.SelectedText = (!cap) ? "=" : "+"; }
                        if (data == "Oemtilde") { rtb.SelectedText = (!cap) ? "`" : "~"; }
                    }
                    else if (data[0]=='N')
                    {
                        if (data.Length >= 6)
                        {
                            if (data.Substring(0, 6) == "NumPad")
                            {
                                rtb.SelectedText = data.Substring(6, 1);
                            }
                        }
                    }
                    else if (data == "Space")
                    {
                        rtb.SelectedText = " ";
                    }
                    else if (data == "Return")
                    {
                        rtb.SelectedText = "\n";
                    }
                    else if (data == "Back")
                    {
                        if (selectionStart > 0 && selectionLength == 0)
                        {
                            rtb.Select(selectionStart - 1, selectionLength + 1);
                        }
                        rtb.SelectedText = "";
                    }
                    else if (data == "Delete")
                    {
                        if (selectionLength == 0)
                        {
                            rtb.Select(selectionStart, selectionLength + 1);
                        }
                        rtb.SelectedText = "";
                    }
                }
                Console.WriteLine("From Client: " + dataFromClient);
            }
        }
    }
}
