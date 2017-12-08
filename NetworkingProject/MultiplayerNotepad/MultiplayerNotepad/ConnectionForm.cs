using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiplayerNotepad
{
    public partial class ConnectionForm : Form
    {
        public string srvrIP;
        public int srvrPort;
        public ConnectionForm()
        {
            InitializeComponent();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if (serverIP.Text != string.Empty && serverPort.Text != string.Empty)
            {
                srvrIP = serverIP.Text;
                srvrPort = Convert.ToInt32(serverPort.Text);
                this.Close();
            }
        }
    }
}
