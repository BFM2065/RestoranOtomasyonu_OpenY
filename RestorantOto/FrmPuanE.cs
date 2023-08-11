using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
namespace RestorantOto
{
    public partial class FrmPuanE : Form
    {
        DbHelper db = new DbHelper();
    
        public int puan;
        public FrmPuanE()
        {
            InitializeComponent();
        }
   
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string query1 = "SELECT MusteriID FROM musteri WHERE Telefon = @Tel AND Daimi=1";
                object Mid = db.ExecuteScalar(query1, new MySqlParameter("@Tel", txtTel.Text));

                if (Mid != null)
                {
                    // MusteriID bulundu, puanı güncelleyebiliriz
                    string query2 = "UPDATE musteri SET Puan = Puan + @P WHERE MusteriID = @ID";
                    db.ExecuteNonQuery(query2, new MySqlParameter("@P",puan), new MySqlParameter("@ID", Convert.ToInt32(Mid)));

                    // Başarı mesajı göster
                    MessageBox.Show("Puan eklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // MusteriID bulunamadı, hata mesajı göster
                    MessageBox.Show("Müşteri bulunamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                // Hata oluştu, hata mesajı göster
                MessageBox.Show("Hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



        }
        

     
        private void txtTel_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

            // Metnin uzunluğunu kontrol et ve 11 hane geçmesini engelle
            if (txtTel.Text.Length >= 11 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtTel_Enter(object sender, EventArgs e)
        {
            txtTel.ForeColor = Color.Black;
            txtTel.Clear();
        }

        private void FrmPuanE_Load(object sender, EventArgs e)
        {

        }
    }
}
