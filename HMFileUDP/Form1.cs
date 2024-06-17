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

namespace HMFileUDP
{
    public partial class Form1 : Form
    {
        private UdpClient udpClient;
        private const int port = 51234;
        private string filePath;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                    lbl_Status.Text = $"Выбран файл: {filePath}";
                }
            }
        }

        private async void btnSendFile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                lbl_Status.Text = "Пожалуйста, выберите файл для отправки.";
                return;
            }

            udpClient = new UdpClient();
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

            try
            {
                // Отправляем имя файла
                string fileName = Path.GetFileName(filePath);
                byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);
                await udpClient.SendAsync(fileNameBytes, fileNameBytes.Length, remoteEndPoint);

                // Отправляем содержимое файла
                byte[] fileBytes = File.ReadAllBytes(filePath);
                await udpClient.SendAsync(fileBytes, fileBytes.Length, remoteEndPoint);

                lbl_Status.Text = "Файл отправлен.";
            }
            catch (Exception ex)
            {
                lbl_Status.Text = $"Ошибка: {ex.Message}";
            }
            finally
            {
                udpClient.Close();
            }
        }
    }
}
