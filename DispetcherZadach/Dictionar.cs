using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DispetcherZadach
{
    internal class Dictionar
    {
        public Dictionary<int, string> dict { get; set; }

        public Dictionar() { }
        public Dictionary<int, string> CreateList()
        {
            dict = new Dictionary<int, string>();

            try
            {
                foreach (var item in Process.GetProcesses())
                {
                    if (item.MainWindowTitle != string.Empty) // показывает только основные процессы
                        dict.Add(item.Id, item.MainWindowTitle);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return dict;
        }

    }
}



