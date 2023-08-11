using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.AspNetCore.SignalR.Client;
using MySql.Data.MySqlClient;

namespace RestorantOto
{
    public partial class Form1 : Form
    {

        private HubConnection connection;
     
        public Form1()
        {

            InitializeComponent();

        
           connection = new HubConnectionBuilder()
                        .WithUrl("https://localhost:5001/mmHub", options => options.AccessTokenProvider = () => Task.FromResult(""))
                         .Build();                                                                                                //Token İÇİN MANUEL ELLE GİRDİM BU DEĞİŞİR TOKEN ÖMRÜ BİTİNCE 

                    // options => options.AccessTokenProvider = () => Task.FromResult("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYmYiOjE2OTE1ODM4MTUsImV4cCI6MTY5MTU4NDExNSwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NTAwMSIsImF1ZCI6ImxvY2FsaG9zdCJ9.q7rAvZazx-2af81HS0YrbPk0ldWSA1T2rThXeg0K9Tc")
                    connection.On<string>("ReceiveOrderNotification", message =>
                    {
                        // Sipariş bildirimi alındığında yapılacak işlemler
                        ShowOrderNotification(message);

                    });

                    ConnectToHub();


        }
              



       
        private void ShowOrderNotification(string message)
        {
            // Sipariş bildirimi alındığında yapılacak işlemler

            MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);


        }
      

        private async void ConnectToHub()
        {

            try
            {
                await connection.StartAsync();
                MessageBox.Show("SignalR Connection Established!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
       
        private void siparişlerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panel1.Controls.Clear();
            FrmSiparis rez = new FrmSiparis();
            rez.Dock = DockStyle.Fill;
            rez.TopLevel = false;
            panel1.Controls.Add(rez);
            rez.BringToFront();
            rez.Show();

        }

        private void müşteriGeriBildirimiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmGeriBildirim geri = new FrmGeriBildirim();
            geri.ShowDialog();
        }

        private void ödemeToolStripMenuItem_Click(object sender, EventArgs e)
        {

            panel1.Controls.Clear();
            FrmOdeme rez = new FrmOdeme();
            rez.Dock = DockStyle.Fill;
            rez.TopLevel = false;
            panel1.Controls.Add(rez);
            rez.BringToFront();
            rez.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void RVY_Click(object sender, EventArgs e)
        {
            panel1.Controls.Clear();
            FrmRezerve rez = new FrmRezerve();
            rez.Dock = DockStyle.Fill;
            rez.TopLevel = false;
            panel1.Controls.Add(rez);
            rez.BringToFront();
            rez.Show();

        }

        
    }
}

