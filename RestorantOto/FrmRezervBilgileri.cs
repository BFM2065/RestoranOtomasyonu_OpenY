using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace RestorantOto
{
    public partial class FrmRezervBilgileri : Form
    {
        public FrmRezervBilgileri()
        {

            InitializeComponent();

        }
        public MySqlConnection mysqlbaglan = new MySqlConnection("Server=localhost;Database=restorantys;Uid=root;Pwd='1234Fm6789';;AllowUserVariables=True;UseCompression=True");

        public string masano;


        void temizle()
        {
            txtAd.Clear();
            txtSya.Clear();
            txtTel.Clear();
            dtTar.ResetText();
            txtKisi.Clear();

        }
        void listele()
        {
            mysqlbaglan.Open();
            txtMasa.Text = masano;
            string query = "Select masa.MasaNumarası, musteri.Ad, musteri.Soyad,masa.RezervasyonTarihi,masa.MusteriID From masa JOIN musteri ON masa.MusteriID=musteri.MusteriID WHERE masa.RezervasyonTarihi >= CURDATE() AND MasaNumarası='" + masano.ToString() + "'AND masa.Durum=1";// ORDER BY MasaID desc LIMIT 1  AND HOUR(masa.RezervasyonTarihi) >= HOUR(CURTIME()) AND 

            MySqlCommand cmd = new MySqlCommand(query, mysqlbaglan);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

            DataTable table = new DataTable();
            adapter.Fill(table);

            dataGridView1.DataSource = table;
            mysqlbaglan.Close();
            temizle();
        }
        private void FrmRezervBilgileri_Load(object sender, EventArgs e)
        {
            listele();

        }

        private void button1_Click(object sender, EventArgs e)
        {

            try
            {

                string ad = txtAd.Text;
                string soyad = txtSya.Text;
                string telefon = txtTel.Text;
                int kisiSay = Convert.ToInt32(txtKisi.Text);

                if (string.IsNullOrEmpty(ad) || string.IsNullOrEmpty(soyad) || string.IsNullOrEmpty(telefon))
                {
                    MessageBox.Show("Lütfen boş alanları doldurun.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(txtKisi.Text, out kisiSay))
                {
                    MessageBox.Show("Geçersiz kişi sayısı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }



                // Rezervasyon tarihini ve saati alın
                DateTime secilenTarih = dtTar.Value;
                mysqlbaglan.Open();
                // Varolan rezervasyonları kontrol et
                MySqlCommand kontrolKomut = new MySqlCommand("SELECT RezervasyonTarihi FROM masa WHERE MasaNumarası = @masaNum AND RezervasyonTarihi BETWEEN @baslangicTarih AND @bitisTarih", mysqlbaglan);
                kontrolKomut.Parameters.AddWithValue("@masaNum", Convert.ToInt32(masano));
                kontrolKomut.Parameters.AddWithValue("@baslangicTarih", secilenTarih.AddMinutes(-30)); // Yeni rezervasyonun başlangıç tarihinden 30 dakika önce
                kontrolKomut.Parameters.AddWithValue("@bitisTarih", secilenTarih.AddMinutes(30)); // Yeni rezervasyonun başlangıç tarihinden 30 dakika sonra
                MySqlDataReader kontrolReader = kontrolKomut.ExecuteReader();

                if (kontrolReader.HasRows)
                {
                    // Varolan rezervasyonlar içinde aynı masa numarasına ve belirtilen süre içindeki başka bir rezervasyon var.
                    // Kullanıcıya uyarı ver ve işlemi engelle.
                    MessageBox.Show("Aynı masada belirtilen tarih ve saat aralığında başka bir rezervasyon bulunmaktadır. Lütfen başka bir saat seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    kontrolReader.Close();
                    mysqlbaglan.Close();
                    return;
                }
                kontrolReader.Close();


                MySqlCommand Musterikomut = new MySqlCommand("insert into  musteri  (Ad,Soyad,Telefon) values(@a,@b,@c)", mysqlbaglan);
                Musterikomut.Parameters.AddWithValue("@a", ad);
                Musterikomut.Parameters.AddWithValue("@b", soyad);
                Musterikomut.Parameters.AddWithValue("@c", telefon);
                Musterikomut.ExecuteNonQuery();

                MySqlCommand lastInsertedIDKomut = new MySqlCommand("SELECT LAST_INSERT_ID()", mysqlbaglan);
                var musteriID = lastInsertedIDKomut.ExecuteScalar();

                MySqlCommand masakomut = new MySqlCommand("insert into masa (MusteriID,RezervasyonTarihi,KisiSayisi,MasaNumarası) values(@d,@e,@f,@g)", mysqlbaglan);
                masakomut.Parameters.AddWithValue("@d", musteriID);
                masakomut.Parameters.AddWithValue("@e", secilenTarih);
                masakomut.Parameters.AddWithValue("@f", kisiSay);
                masakomut.Parameters.AddWithValue("@g", Convert.ToInt32(masano));
                masakomut.ExecuteNonQuery();
                mysqlbaglan.Close();
                MessageBox.Show("Rezarvasyon Başarıyla Yapıldı");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            mysqlbaglan.Close();
            listele();
        }


        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                string veri = selectedRow.Cells["MusteriID"].Value.ToString();
                string query = "SELECT musteri.Ad, musteri.Soyad, musteri.Telefon,masa.RezervasyonTarihi,masa.KisiSayisi FROM musteri JOIN masa ON musteri.MusteriID=masa.MusteriID WHERE musteri.MusteriID = @veri";
                mysqlbaglan.Open();
                MySqlCommand komut = new MySqlCommand(query, mysqlbaglan);
                komut.Parameters.AddWithValue("@veri", veri);
                MySqlDataReader read = komut.ExecuteReader();
                if (read.Read())
                {
                    string ad = read.GetString("Ad");
                    string soyad = read.GetString("Soyad");
                    string telefon = read.GetString("Telefon");//STR YAPILCAK
                    DateTime rezTarih = read.GetDateTime("RezervasyonTarihi");
                    int kisisayisi = read.GetInt32("KisiSayisi");

                    // TextBox'lara verileri yazdırma
                    txtAd.Text = ad;
                    txtSya.Text = soyad;
                    txtTel.Text = telefon;
                    dtTar.Value = rezTarih;
                    txtKisi.Text = kisisayisi.ToString();
                }
                mysqlbaglan.Close();


            }

        }

        private void btnGnc_Click(object sender, EventArgs e)
        {
            try
            {

                string ad = txtAd.Text;
                string soyad = txtSya.Text;
                string telefon = txtTel.Text;//STR YAPILCAK
                DateTime rezervasyonTarihi = dtTar.Value;
                int kisiSayisi = Convert.ToInt32(txtKisi.Text);
                string musteriID = dataGridView1.SelectedRows[0].Cells["MusteriID"].Value.ToString();
                string queryMusteri = "UPDATE musteri SET Ad = @ad, Soyad = @soyad, Telefon = @telefon WHERE MusteriID = @musteriID";
                string queryMasa = "UPDATE masa SET RezervasyonTarihi = @rezervasyonTarihi, KisiSayisi = @kisiSayisi WHERE MusteriID = @musteriID";

                mysqlbaglan.Open();

                using (MySqlCommand Musterikomut = new MySqlCommand(queryMusteri, mysqlbaglan))
                {
                    Musterikomut.Parameters.AddWithValue("@ad", ad);
                    Musterikomut.Parameters.AddWithValue("@soyad", soyad);
                    Musterikomut.Parameters.AddWithValue("@telefon", telefon);
                    Musterikomut.Parameters.AddWithValue("@musteriID", musteriID);
                    Musterikomut.ExecuteNonQuery();
                }

                using (MySqlCommand MasaKomut = new MySqlCommand(queryMasa, mysqlbaglan))
                {

                    MasaKomut.Parameters.AddWithValue("@rezervasyonTarihi", rezervasyonTarihi);
                    MasaKomut.Parameters.AddWithValue("@kisiSayisi", kisiSayisi);
                    MasaKomut.Parameters.AddWithValue("@musteriID", musteriID);
                    MasaKomut.ExecuteNonQuery();



                }
                mysqlbaglan.Close();
                if (string.IsNullOrEmpty(ad) || string.IsNullOrEmpty(soyad) || string.IsNullOrEmpty(telefon))
                {
                    MessageBox.Show("Lütfen boş alanları doldurun.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!int.TryParse(txtKisi.Text, out kisiSayisi))
                {
                    MessageBox.Show("Geçersiz kişi sayısı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



            listele();
        }

        private void button2_Click(object sender, EventArgs e)
        {


            try
            {
                string ad = txtAd.Text;
                string soyad = txtSya.Text;
                string telefon = txtTel.Text;
                if (string.IsNullOrEmpty(ad) || string.IsNullOrEmpty(soyad) || string.IsNullOrEmpty(telefon))
                {
                    MessageBox.Show("Lütfen Silmek İstediğiniz Kaydı Seçin! ", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string musteriID = dataGridView1.SelectedRows[0].Cells["MusteriID"].Value.ToString();
                string queryMasa = "UPDATE masa SET masa.Durum = 0  WHERE MusteriID = @musteriID";
                mysqlbaglan.Open();
                MySqlCommand komutsil = new MySqlCommand(queryMasa, mysqlbaglan);
                komutsil.Parameters.AddWithValue("@musteriID", musteriID);
                komutsil.ExecuteNonQuery();
                mysqlbaglan.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            listele();
        }
    }
}
