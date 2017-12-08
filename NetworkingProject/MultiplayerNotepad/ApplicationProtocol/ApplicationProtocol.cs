using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using KeyboardInput;
using System.Windows.Forms;
namespace ApplicationProtocol
{

    /// <summary>
    /// Creates messages (byte[]) to be sent acorss the network
    /// </summary>
    public class ApplicationProtocol
    {
        /// <summary>
        /// Client has connected
        /// </summary>
        public const int Connect = 8;
        /// <summary>
        /// Client has disconnected / Server disconnected the client
        /// </summary>
        public const int Disconnect = 9;
        /// <summary>
        /// Server sends a new client the current document
        /// </summary>
        public const int UpdateNewClient = 2;
        /// <summary>
        /// Client - Informs server of a button press and the corresponding key should be upper case
        /// Server - informs all connected clients of another clients button press and that the key should be upper case
        /// </summary>
        public const int KeyInfoCapital = 1;
        /// <summary>
        /// Client - Informs server of a button press and the corresponding key should be lower case
        /// Server - informs all connected clients of another clients button press and that the key should be lower case
        /// </summary>
        public const int KeyInfoLowerCase = 0;
        public const int ToggleStyle = 3;
        public const int ChangeSize = 4;
        public const int ChangeFont = 5;
        public const int Save = 10;
        public const int ChangeAlignment = 11;

        public byte[] CreateConnectMessage()
        {
            return BitConverter.GetBytes(Connect);
        }

        public byte[] CreateDisconnectMessage()
        {
            return BitConverter.GetBytes(Disconnect);
        }

        public byte[] CreateKeyInfoMessage<T>(KeyboardInput<T> ki, int selectionStart, int selectionLength) where T : Control
        {
            int type = KeyInfoLowerCase;
            if (ki.isKeyDown(Keys.ShiftKey) || Control.IsKeyLocked(Keys.CapsLock))
            {
                type = KeyInfoCapital;
            }
            byte[] extras = BitConverter.GetBytes(type);
            byte[] cursorPosition = BitConverter.GetBytes(selectionStart);
            byte[] highlightLength = BitConverter.GetBytes(selectionLength);
            byte[] data = Encoding.ASCII.GetBytes(ki.getString());
            byte[] fullData = extras.Concat(cursorPosition.Concat(highlightLength.Concat(data).ToArray()).ToArray()).ToArray();

            return fullData;
        }

        public byte[] CreateUpdateNewClientMessage(string text)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);
            byte[] length = BitConverter.GetBytes(text.Length);
            byte[] type = BitConverter.GetBytes(2);
            return type.Concat(length).Concat(data).ToArray();
        }

        public byte[] CreateToggleStyle(int selectionStart, int selectionLength, FontStyle fontStyle)
        {
            byte[] type = BitConverter.GetBytes(ToggleStyle);
            byte[] cursorPosition = BitConverter.GetBytes(selectionStart);
            byte[] highlightLength = BitConverter.GetBytes(selectionLength);
            byte[] style = BitConverter.GetBytes((int)fontStyle);
            return type.Concat(cursorPosition.Concat(highlightLength).Concat(style)).ToArray();
        }

        public byte[] CreateChangeSizeMessage(int selectionStart, int selectionLength, float amount)
        {
            byte[] type = BitConverter.GetBytes(ChangeSize);
            byte[] cursorPosition = BitConverter.GetBytes(selectionStart);
            byte[] highlightLength = BitConverter.GetBytes(selectionLength);
            byte[] size = BitConverter.GetBytes(amount);
            return type.Concat(cursorPosition.Concat(highlightLength).Concat(size)).ToArray();
        }

        public byte[] CreateChangeFontMessage(int selectionStart, int selectionLength, float fontSize, FontStyle fontStyle, int nameLength, string fontName)
        {
            byte[] type = BitConverter.GetBytes(ChangeFont);
            byte[] cursorPosition = BitConverter.GetBytes(selectionStart);
            byte[] highlightLength = BitConverter.GetBytes(selectionLength);
            byte[] bsize = BitConverter.GetBytes(fontSize);
            byte[] style = BitConverter.GetBytes((int)fontStyle);
            byte[] bNameLength = BitConverter.GetBytes(fontName.Length);
            byte[] bFontName = Encoding.ASCII.GetBytes(fontName);
            return type.Concat(cursorPosition.Concat(highlightLength).Concat(bsize).Concat(style).Concat(bNameLength).Concat(bFontName)).ToArray();
        }

        public byte[] CreateSaveMessage()
        {
            byte[] type = BitConverter.GetBytes(Save);
            return type;
        }

        public byte[] CreateChangeAlignmentMessage(int selectionStart, int selectionLength, HorizontalAlignment alignment)
        {
            byte[] type = BitConverter.GetBytes(ChangeAlignment);
            byte[] cursorPosition = BitConverter.GetBytes(selectionStart);
            byte[] highlightLength = BitConverter.GetBytes(selectionLength);
            byte[] style = BitConverter.GetBytes((int)alignment);
            return type.Concat(cursorPosition.Concat(highlightLength).Concat(style)).ToArray();
        }

        public void DecodeKeyInfoMessage(byte[] data, out int type, out int selectionStart, out int selectionLength, out string text)
        {
            type = BitConverter.ToInt32(data,0);
            selectionStart = BitConverter.ToInt32(data, 4);//data[4];
            selectionLength = BitConverter.ToInt32(data, 8);//data[8];
            byte[] dataBytes = data.Skip(12).ToArray();//data.Skip(12).ToArray();
            string dataFromClient = Encoding.ASCII.GetString(dataBytes);
            text = dataFromClient.Split('\0')[0];
        }
        public string DecodeUpdateNewClient(byte[] data)
        {
            int length = BitConverter.ToInt32(data, 4);
            return Encoding.ASCII.GetString(data.Skip(8).Take(length).ToArray());
        }
        public void DecodeToggleStyle(byte[] data, out int selectionStart, out int selectionLength, out FontStyle fontStyle)
        {
            selectionStart = BitConverter.ToInt32(data, 4);
            selectionLength = BitConverter.ToInt32(data, 8);
            fontStyle = (FontStyle)BitConverter.ToInt32(data, 12);
        }
        public void DecodeChangeSize(byte[] data, out int selectionStart, out int selectionLength, out float amount)
        {
            selectionStart = BitConverter.ToInt32(data, 4);
            selectionLength = BitConverter.ToInt32(data, 8);
            amount = BitConverter.ToSingle(data, 12);
        }
        public void DecodeChangeFont(byte[] data, out int selectionStart, out int selectionLength, out float fontSize, out FontStyle fontStyle, out string fontName)
        {
            selectionStart = BitConverter.ToInt32(data, 4);
            selectionLength = BitConverter.ToInt32(data, 8);
            fontSize = BitConverter.ToSingle(data, 12);
            fontStyle = (FontStyle)BitConverter.ToInt32(data, 16);
            int length = BitConverter.ToInt32(data, 20);
            fontName = Encoding.ASCII.GetString(data.Skip(24).Take(length).ToArray());
        }

        public void DecodeChangeAlignment(byte[] data, out int selectionStart, out int selectionLength, out HorizontalAlignment alignment)
        {
            selectionStart = BitConverter.ToInt32(data, 4);
            selectionLength = BitConverter.ToInt32(data, 8);
            alignment = (HorizontalAlignment)BitConverter.ToInt32(data, 12);
        }

        public int GetMessageType(byte[] data)
        {
            return BitConverter.ToInt32(data, 0);
        }
    }
}
