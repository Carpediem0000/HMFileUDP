using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace HMFileUDPServer
{
    public partial class Form1 : Form
    {
        private UdpClient udpClient;
        private const int port = 51234;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            udpClient = new UdpClient(port);
            Task.Run(() => ReceiveFile());
            lbl_Status.Text = "Сервер запущен...";
        }

        private async Task ReceiveFile()
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, port);

            try
            {
                while (true)
                {
                    // Получаем имя файла
                    var result = await udpClient.ReceiveAsync();
                    string fileName = Encoding.UTF8.GetString(result.Buffer);

                    // Получаем содержимое файла
                    result = await udpClient.ReceiveAsync();
                    File.WriteAllBytes(fileName, result.Buffer);

                    this.Invoke(new Action(() =>
                    {
                        lbl_Status.Text = $"Файл получен и сохранен как {fileName}";
                    }));
                }
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    lbl_Status.Text = $"Ошибка: {ex.Message}";
                }));
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            udpClient?.Close();
        }
    }
}
