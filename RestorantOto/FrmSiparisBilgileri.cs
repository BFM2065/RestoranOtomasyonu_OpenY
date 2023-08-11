using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace RestorantOto
{
    public partial class FrmSiparisBilgileri : Form
    {
        MySqlConnection baglan = new MySqlConnection("Server=localhost;Database=restorantys;Uid=root;" +
               "Pwd='1234Fm6789';;AllowUserVariables=True;UseCompression=True");

        public int siparisMasano;
        int sepetıd;
        public FrmSiparisBilgileri()
        {
            InitializeComponent();
        }

        private void FrmSiparisBilgileri_Load(object sender, EventArgs e)
        {
            textBox1.Text = siparisMasano.ToString();

            // Siparişe ait bilgileri al
            string masaNumarasi = string.Empty;
            int urunlerID = 0;
            int adet = 0;

            string siparisBilgileriQuery = "SELECT UrunlerID,Adet,SepetID FROM sepet WHERE MasaNumarası = @masano";
            using (MySqlCommand command = new MySqlCommand(siparisBilgileriQuery, baglan))
            {
                command.Parameters.AddWithValue("@masano", siparisMasano);
                baglan.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {

                        urunlerID = Convert.ToInt32(reader["UrunlerID"]);
                        adet = Convert.ToInt32(reader["Adet"]);
                        sepetıd = Convert.ToInt32(reader["SepetID"]);
                        textBox4.Text = adet.ToString();
                    }
                }
            }

            // Ürün adını al
            string urunAdi = string.Empty;
            string urunAdiQuery = "SELECT UrunAD FROM urunler WHERE UrunID = @urunID";
            using (MySqlCommand command = new MySqlCommand(urunAdiQuery, baglan))
            {
                command.Parameters.AddWithValue("@urunID", urunlerID);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        urunAdi = reader["UrunAD"].ToString();
                    }
                    textBox2.Text = urunAdi;
                }
            }

            // İçerikleri al ve label'lara yerleştir
            string iceriklerQuery = "SELECT icerik.IcerikAD FROM icerik " +
                        "JOIN sepeticerik ON icerik.IcerikID = sepeticerik.IcerikID " +
                        "WHERE sepeticerik.SepetID = @sepetID";

            using (MySqlCommand command = new MySqlCommand(iceriklerQuery, baglan))
            {
                command.Parameters.AddWithValue("@sepetID", sepetıd); // SepetID'ye göre içerikleri al
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    string icerikListesi = string.Empty;
                    while (reader.Read())
                    {
                        string icerikAdi = reader["IcerikAD"].ToString();
                        icerikListesi += icerikAdi + ", ";
                    }
                    icerikListesi = icerikListesi.TrimEnd(',', ' '); // Son virgülü ve boşluğu kaldır

                    textBox3.Text = icerikListesi;
                }
            }


            baglan.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TamamlaSiparis();
        }
        private void TamamlaSiparis()
        {
            // Masa üzerindeki siparişlerin durumunu güncelle
            string tamamlaQuery = "UPDATE sepet SET Durum = 0 WHERE MasaNumarası = @masano AND Durum = 1 ORDER BY SepetID ASC LIMIT 1";
            using (MySqlCommand command = new MySqlCommand(tamamlaQuery, baglan))
            {
                command.Parameters.AddWithValue("@masano", siparisMasano);
                baglan.Open();
                int affectedRows = command.ExecuteNonQuery();
                baglan.Close();

                if (affectedRows > 0)
                {
                    MessageBox.Show("Sipariş başarıyla tamamlandı!");
                }
                else
                {
                    MessageBox.Show("Sipariş tamamlanırken bir hata oluştu veya sipariş bulunamadı.");
                }
            }
        }


    }
}
