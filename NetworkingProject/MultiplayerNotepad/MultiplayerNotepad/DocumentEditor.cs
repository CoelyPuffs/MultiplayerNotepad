using System;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using TCPClient;
using KeyboardInput;
using ApplicationProtocol;
using System.Drawing;
using RichTextBoxExtension;
using System.IO;
namespace MultiplayerNotepad
{
    public partial class DocumentEditor : Form
    {
        Client myClient;
        KeyboardInput<RichTextBox> keyboardinput;
        ApplicationProtocol.ApplicationProtocol AppProtocol;
        //cursorPosition currentCursor;

        public DocumentEditor(string serverIP, int serverPort)
        {
            InitializeComponent();
            RichTextBox.CheckForIllegalCrossThreadCalls = false;
            letterbox.AcceptsTab = true;
            keyboardinput = new KeyboardInput<RichTextBox>(letterbox, keypressed);
            AppProtocol = new ApplicationProtocol.ApplicationProtocol();
            myClient = new Client(serverIP, serverPort, new ClientProcessor(letterbox,AppProtocol),AppProtocol);
            BtnBold.Click += BtnBold_Click;
            BtnItalic.Click += BtnItalic_Click;
            BtnUnderline.Click += BtnUnderline_Click;
            BtnInc.Click += BtnInc_Click;
            BtnDec.Click += BtnDec_Click;
            BtnSave.Click += BtnSave_Click;
            BtnLeft.Click += BtnLeft_Click;
            BtnRight.Click += BtnRight_Click;
            BtnCenter.Click += BtnCenter_Click;

        }

        private void BtnCenter_Click(object sender, EventArgs e)
        {
            myClient.Send(AppProtocol.CreateChangeAlignmentMessage(letterbox.SelectionStart, letterbox.SelectionLength, HorizontalAlignment.Center));
        }

        private void BtnRight_Click(object sender, EventArgs e)
        {
            myClient.Send(AppProtocol.CreateChangeAlignmentMessage(letterbox.SelectionStart, letterbox.SelectionLength, HorizontalAlignment.Right));
        }

        private void BtnLeft_Click(object sender, EventArgs e)
        {
            myClient.Send(AppProtocol.CreateChangeAlignmentMessage(letterbox.SelectionStart, letterbox.SelectionLength, HorizontalAlignment.Left));
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            myClient.Send(AppProtocol.CreateSaveMessage());
        }

        private void BtnDec_Click(object sender, EventArgs e)
        {
            myClient.Send(AppProtocol.CreateChangeSizeMessage(letterbox.SelectionStart, letterbox.SelectionLength, -1));
        }

        private void BtnInc_Click(object sender, EventArgs e)
        {
            myClient.Send(AppProtocol.CreateChangeSizeMessage(letterbox.SelectionStart, letterbox.SelectionLength, 1));
        }

        private void BtnUnderline_Click(object sender, EventArgs e)
        {
            myClient.Send(AppProtocol.CreateToggleStyle(letterbox.SelectionStart, letterbox.SelectionLength, System.Drawing.FontStyle.Underline));
        }

        private void BtnItalic_Click(object sender, EventArgs e)
        {
            myClient.Send(AppProtocol.CreateToggleStyle(letterbox.SelectionStart, letterbox.SelectionLength, System.Drawing.FontStyle.Italic));
        }

        private void BtnBold_Click(object sender, EventArgs e)
        {
            myClient.Send(AppProtocol.CreateToggleStyle(letterbox.SelectionStart, letterbox.SelectionLength, System.Drawing.FontStyle.Bold));
        }

        [STAThreadAttribute]
        static void Main()
        {
            ConnectionForm login = new ConnectionForm();
            login.ShowDialog();
            string serverIP = login.srvrIP;
            int serverPort = login.srvrPort;
            Application.Run(new DocumentEditor(serverIP, serverPort));
        }

        private void fontButton_Click(object sender, EventArgs e)
        {
            fontDialog1.Font = letterbox.Font;
            if (fontDialog1.ShowDialog() != DialogResult.Cancel)
            {
                //letterbox.Font = fontDialog1.Font;
                byte[] data = AppProtocol.CreateChangeFontMessage(letterbox.SelectionStart, letterbox.SelectionLength, fontDialog1.Font.Size,fontDialog1.Font.Style, fontDialog1.Font.SystemFontName.Length, fontDialog1.Font.Name);
                myClient.Send(data);
            }
        }

        public void keypressed(KeyboardInput<RichTextBox> keyboardpressed, bool down)
        {
            if (!down) { return; }
            Console.WriteLine(keyboardpressed.getString());
            byte[] data = AppProtocol.CreateKeyInfoMessage<RichTextBox>(keyboardinput, letterbox.SelectionStart, letterbox.SelectionLength);
            myClient.Send(data);
        }

        public void formClosed(object sender, System.ComponentModel.CancelEventArgs e)
        {
            myClient.Close();
        }
    }

    public class ClientProcessor : DataProcessor
    {
        RichTextBox rtb;
        ApplicationProtocol.ApplicationProtocol AppProtocol;
        public ClientProcessor(RichTextBox box,ApplicationProtocol.ApplicationProtocol AppProtocol)
        {
            this.rtb = box;
            this.AppProtocol = AppProtocol;
        }
        
        public override void Process(Client client, byte[] msg)
        {
            int infoByte = AppProtocol.GetMessageType(msg);
            int start = rtb.SelectionStart;
            int selectLength = rtb.SelectionLength;
            if(infoByte == ApplicationProtocol.ApplicationProtocol.UpdateNewClient)
            {
                File.WriteAllText("Temp.txt",AppProtocol.DecodeUpdateNewClient(msg));
                rtb.LoadFile("Temp.txt");
                File.Delete("Temp.txt");
                return;
            }

            if (infoByte == ApplicationProtocol.ApplicationProtocol.ToggleStyle)
            {
                int selectionStart;
                int selectionLength;
                FontStyle style;
                AppProtocol.DecodeToggleStyle(msg, out selectionStart, out selectionLength, out style);
                rtb.SelectionStart = selectionStart;
                rtb.SelectionLength = selectionLength;
                rtb.ToggleFontStyle(style);
                rtb.SelectionStart = start;
                rtb.SelectionLength = selectLength;
                return;
            }

            if (infoByte == ApplicationProtocol.ApplicationProtocol.ChangeSize)
            {
                int selectionStart;
                int selectionLength;
                float amount;
                AppProtocol.DecodeChangeSize(msg, out selectionStart, out selectionLength, out amount);
                rtb.SelectionStart = selectionStart;
                rtb.SelectionLength = selectionLength;
                rtb.ChangeFontSize(amount);
                rtb.SelectionStart = start;
                rtb.SelectionLength = selectLength;
                return;
            }

            if (infoByte == ApplicationProtocol.ApplicationProtocol.ChangeFont)
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
                rtb.SelectionStart = start;
                rtb.SelectionLength = selectLength;
                return;
            }

            if(infoByte == ApplicationProtocol.ApplicationProtocol.ChangeAlignment)
            {
                int selectionStart;
                int selectionLength;
                HorizontalAlignment alignment;
                AppProtocol.DecodeChangeAlignment(msg, out selectionStart, out selectionLength, out alignment);
                rtb.SelectionStart = selectionStart;
                rtb.SelectionLength = selectionLength;
                rtb.SelectionAlignment = alignment;
                rtb.SelectionStart = start;
                rtb.SelectionLength = selectLength;
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
                data = dataFromClient.Substring(seperator + 1, dataFromClient.Length - seperator-1);

                if (cmd == "Paste")
                {
                    rtb.Select(selectionStart, selectionLength);
                    rtb.SelectedText = data;
                    if (rtb.SelectionStart<= start)
                    {
                        rtb.SelectionStart = start + data.Length-selectionLength;// + ((selectionStart < start) ? 1:0) ;
                        rtb.SelectionLength = 0;
                    }
                    return;
                }

                if (data != "None" && cmd == "key")
                {
                    rtb.Select(selectionStart, selectionLength);
                    if (data.Length == 1)
                    {
                        if (!cap) { data = data.ToLower(); }
                        rtb.SelectedText = data;
                        start += AdjustCursor(start, selectionStart, selectionLength);//start++;
                    }
                    else if (data == "Tab")
                    {
                        rtb.SelectedText = "\t";
                        start += AdjustCursor(start, selectionStart, selectionLength);//start++;
                    }
                    else if (data.Length == 2 && data[0] == 'D')
                    {
                        if (cap == false)
                        {
                            rtb.SelectedText = "" + data[1];
                            start+=AdjustCursor(start, selectionStart, selectionLength); //start++;
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
                            start+=AdjustCursor(start, selectionStart, selectionLength);//start++;

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
                        start+= AdjustCursor(start, selectionStart, selectionLength);//start++;
                    }
                    else if (data == "Space")
                    {
                        rtb.SelectedText = " ";
                        start += AdjustCursor(start, selectionStart, selectionLength);//start++;
                    }
                    else if (data[0] == 'N')
                    {
                        if (data.Length >= 6)
                        {
                            if (data.Substring(0, 6) == "NumPad")
                            {
                                rtb.SelectedText = data.Substring(6, 1);
                            }
                        }
                    }
                    else if (data == "Return")
                    {
                        rtb.SelectedText = "\n";
                        start+=AdjustCursor(start, selectionStart, selectionLength);//start++;
                    }
                    else if (data == "Back")
                    {
                        if (selectionStart > 0 && selectionLength == 0)
                        {
                            selectionStart -= 1;
                            selectionLength += 1;
                            rtb.Select(selectionStart, selectionLength);
                        }
                        rtb.SelectedText = "";
                        if (selectionStart < start)
                        {
                            start-=selectionLength;
                        }
                        selectLength = 0;
                    }
                    else if (data == "Delete")
                    {
                        if (selectionLength == 0)
                        {
                            selectionLength++;
                            rtb.Select(selectionStart, selectionLength);
                        }
                        rtb.SelectedText = "";
                        if (selectionStart < start)
                        {
                            start -= selectionLength;
                        }
                        selectLength = 0;
                    }
                }
                Console.WriteLine("From Client: " + dataFromClient);
            }
            if (start < 0) { start = 0; }
            if (selectLength < 0) { selectLength = 0; }
            rtb.SelectionStart = (start<=rtb.Text.Length)?start : rtb.Text.Length;
            rtb.SelectionLength = (selectLength <= rtb.Text.Length) ? selectLength : rtb.Text.Length ;
        }

        public int AdjustCursor(int currentPosition, int selectionStart, int selectionLength)
        {
            int selectionEnd = selectionStart + selectionLength;
            if(selectionStart <= currentPosition)
            {
                if (selectionLength == 0) { selectionLength = 1; }
                return selectionLength;
            }
            return 0;
        }
    }
}