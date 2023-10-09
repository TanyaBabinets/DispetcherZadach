using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DispetcherZadach
{
    public partial class Form1 : Form
    {
        Controller contr;
     
        public SynchronizationContext uiContext;
        public Form1()
        {
            InitializeComponent();
            uiContext = SynchronizationContext.Current;
            contr = new Controller();
          
            contr.ServerRecieveEvent += ServerRecieve;//подписка на событие\
          
            Task.Factory.StartNew(() => { contr.ConnectionToClient();  });
        }
        private void ServerRecieve(string message)
        {
            uiContext.Send(d => listBox1.Items.Add(message), null);
        }

    
       
    }
}
