using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.util;
using System.Windows.Forms;
namespace RestorantOto
{


    public partial class FrmGeriBildirim : Form
    {

        private int yildiz = 1;
        public FrmGeriBildirim()
        {
            InitializeComponent();
            SetSilikYazilar();
        }

        private void SetSilikYazilar()
        {
            txtAdi.Text = "ADI";
            txtAdi.ForeColor = Color.Gray;

            txtSya.Text = "SOYADI";
            txtSya.ForeColor = Color.Gray;

            txtTel.Text = "TELEFONU";
            txtTel.ForeColor = Color.Gray;

            texgeri.Text = "GERİ BİLDİRİM";
            texgeri.ForeColor = Color.Gray;
        }

        private void SilikYaziGirisi(TextBox textBox, string silikYazi)
        {
            if (textBox.Text == silikYazi)
            {
                textBox.Text = "";
                textBox.ForeColor = Color.Black;
            }
        }

        private void SilikYaziCikisi(TextBox textBox, string silikYazi)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = silikYazi;
                textBox.ForeColor = Color.Gray;
            }
        }

        private void txtAdi_Enter(object sender, EventArgs e)
        {
            SilikYaziGirisi(txtAdi, "ADI");
        }

        private void txtAdi_Leave(object sender, EventArgs e)
        {
            SilikYaziCikisi(txtAdi, "ADI");
        }

        private void txtSya_Enter(object sender, EventArgs e)
        {
            SilikYaziGirisi(txtSya, "SOYADI");
        }

        private void txtSya_Leave(object sender, EventArgs e)
        {
            SilikYaziCikisi(txtSya, "SOYADI");
        }

        private void txtTel_Enter(object sender, EventArgs e)
        {
            SilikYaziGirisi(txtTel, "TELEFONU");
        }

        private void txtTel_Leave(object sender, EventArgs e)
        {
            SilikYaziCikisi(txtTel, "TELEFONU");
        }

        private void texgeri_Enter(object sender, EventArgs e)
        {
            SilikYaziGirisi(texgeri, "GERİ BİLDİRİM");
        }

        private void texgeri_Leave(object sender, EventArgs e)
        {
            SilikYaziCikisi(texgeri, "GERİ BİLDİRİM");
        }
        private List<PictureBox> yildizButonlari = new List<PictureBox>();



        private void YildizButon_Click(object sender, EventArgs e)
        {
            PictureBox clickedButton = (PictureBox)sender;
            int yildizDegeri = yildizButonlari.IndexOf(clickedButton) + 1;

            for (int i = 0; i < yildizButonlari.Count; i++)
            {
                if (i < yildizDegeri)
                {
                    yildizButonlari[i].Image = Properties.Resources.doluY;
                    yildiz = i + 1;
                }
                else
                {
                    yildizButonlari[i].Image = Properties.Resources.bosY;
                }
            }
        }

        private void FrmGeriBildirim_Load(object sender, EventArgs e)
        {
            yildizButonlari.Add(pictureBox1);
            yildizButonlari.Add(pictureBox2);
            yildizButonlari.Add(pictureBox3);
            yildizButonlari.Add(pictureBox4);
            yildizButonlari.Add(pictureBox5);

            // Butonların tıklanma olayını aynı işleyiciye bağlayalım
            foreach (PictureBox yildizButon in yildizButonlari)
            {
                yildizButon.Click += YildizButon_Click;
            }
        }
        DbHelper db = new DbHelper();
        private void button1_Click(object sender, EventArgs e)
        {
            // Gerekli bilgileri alın
            string ad = txtAdi.Text.Trim();
            string soyad = txtSya.Text.Trim();
            string telefon = txtTel.Text.Trim();
            string aciklama = texgeri.Text.Trim();

            // Boş girdileri kontrol edin ve uyarı verin
            if (string.IsNullOrEmpty(ad) || ad == "ADI" || string.IsNullOrEmpty(soyad) || soyad == "SOYADI" || string.IsNullOrEmpty(telefon) || telefon == "TELEFONU" || string.IsNullOrEmpty(aciklama) || aciklama == "GERİ BİLDİRİM")
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Veritabanına kaydedin
            string query = "INSERT INTO musteri (Ad, Soyad, Telefon) VALUES (@ad, @soyad, @telefon); SELECT LAST_INSERT_ID();";
            object id = db.ExecuteScalar(query,
                new MySqlParameter("@ad", ad),
                new MySqlParameter("@soyad", soyad),
                new MySqlParameter("@telefon", telefon));

            if (id != null && int.TryParse(id.ToString(), out int musteriID))
            {
                string query2 = "INSERT INTO geribildirim (MusteriID, Aciklama, YildizPuan) VALUES (@ID, @acikla, @yildiz);";
                db.ExecuteNonQuery(query2,
                    new MySqlParameter("@ID", musteriID),
                    new MySqlParameter("@acikla", aciklama),
                    new MySqlParameter("@yildiz", yildiz));
            }

            MessageBox.Show("Geri Bildiriminiz alındı. Teşekkür ederiz!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // TextBoxları temizleyin
            texgeri.Clear();
            txtAdi.Clear();
            txtSya.Clear();
            txtTel.Clear();
        }


    }
}
