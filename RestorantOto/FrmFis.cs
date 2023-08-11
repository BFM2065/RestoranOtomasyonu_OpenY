using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RestorantOto
{
    public partial class FrmFis : Form
    {
        public FrmFis()
        {
            InitializeComponent();
        }
        public string masano;
        DbHelper db = new DbHelper();
        decimal toplamTutar;
        decimal odenenTutar;
        decimal kalan;
        private decimal deger;
        decimal tutucu;
        int puan;
        private void FrmFis_Load(object sender, EventArgs e)
        {
            textBox1.Text = masano;

            try//TUTAR YAZDIRMA
            {
                // Sorgu oluşturma
                string query1 = "SELECT SUM(Tutar) " +
                               "FROM odeme WHERE MasaNumarası = @masano " +
                               "GROUP BY MasaNumarası";

                // MySqlCommand ve MySqlParameter oluşturma
                MySqlCommand command1 = new MySqlCommand(query1, db.baglantiopen());
                command1.Parameters.AddWithValue("@masano", masano); // Değiştirmeniz gereken yer

                // Sorguyu çalıştırma ve sonucu alıp TextBox 2'ye yazma
                object result = command1.ExecuteScalar();
                if (result != null)
                {
                    toplamTutar = Convert.ToDecimal(result);
                    textBox3.Text = toplamTutar.ToString();
                }

                // Bağlantı kapatma işlemi
                db.baglanticlose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            MySqlConnection connection = db.baglantiopen();

            // Sorgu oluşturma
            string query = "SELECT urunler.urunAD ,sepet.Adet " +
                           "FROM sepet " +
                           "JOIN urunler ON sepet.urunlerID = urunler.urunID " +
                           "WHERE sepet.MasaNumarası = @masano ";


            // MySqlCommand ve MySqlParameter oluşturma
            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@masano", masano); // Değiştirmeniz gereken yer

            // Sorguyu çalıştırma ve sonucu alıp TextBox 2'ye yazma
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                List<string> urunAdlari = new List<string>();

                while (reader.Read())
                {
                    string urunAdi = reader["urunAD"].ToString();
                    urunAdi = urunAdi + "(" + reader["Adet"].ToString() + ")";
                    urunAdlari.Add(urunAdi);
                }

                string birlesikUrunAdlari = string.Join(", ", urunAdlari);
                textBox2.Text = birlesikUrunAdlari;
            }

            // Bağlantı kapatma işlemi
            db.baglanticlose();
            tutucu = toplamTutar;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtTam.Checked)
            {
                if (radioButton1.Checked)
                {
                    // Label ve TextBox'ı görünür yap
                    label5.Visible = true;
                    textBox4.Visible = true;
                    button2.Visible = true;
                    button3.Visible = true;
                    button4.Visible = true;
                    button5.Visible = true;
                    button6.Visible = true;
                    button7.Visible = true;
                    button8.Visible = true;
                }
                else
                {
                    // Diğer radio butonları seçiliyken Label ve TextBox'ı gizle
                    label5.Visible = false;
                    textBox4.Visible = false;
                    button2.Visible = false;
                    button3.Visible = false;
                    button4.Visible = false;
                    button5.Visible = false;
                    button6.Visible = false;
                    button7.Visible = false;
                    button8.Visible = false;
                }
            }
            else
            {
                label5.Visible = true;
                textBox4.Visible = true;
                button2.Visible = true;
                button3.Visible = true;
                button4.Visible = true;
                button5.Visible = true;
                button6.Visible = true;
                button7.Visible = true;
                button8.Visible = true;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            

            if (rbtTam.Checked)
            {

                if (radioButton1.Checked)
                {
                    // Nakit ödeme işlemi
                    NakitOdeme();
                    fiscikar();

                }
                else if (radioButton2.Checked)
                {
                    // Kredi kartı ödeme işlemi
                    KrediKartiOdeme();
                    fiscikar();
                }
                else if (radioButton3.Checked)
                {
                    // Ticket ödeme işlemi
                    TicketOdeme();
                    fiscikar();
                }
                else
                {
                    MessageBox.Show("Lütfen bir ödeme yöntemi seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }


            }
            else if (rbtpar.Checked)
            {
                if (toplamTutar >= Convert.ToDecimal(textBox4.Text))
                {

                    kalan = Convert.ToDecimal(textBox3.Text) - Convert.ToDecimal(textBox4.Text);
                }
                else
                {
                    kalan = Convert.ToDecimal("0,00");
                }
                textBox3.Text = kalan.ToString();
                MessageBox.Show("Ödeme Tamamlandı\n Ödenen Miktar: " + textBox4.Text + " \n Kalan Tutar:  " + kalan.ToString(), "Ödeme Tamamlandı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (kalan == 0 || kalan < 0)
                {
                    string gquery = "UPDATE sepet SET ODurum=0 WHERE MasaNumarası=@masano";
                    db.ExecuteNonQuery(gquery, new MySqlParameter("@masano", masano));

                }
                else
                {
                    string getMaxTutarQuery = "SELECT MAX(Tutar) AS MaxTutar FROM odeme WHERE MasaNumarası = @masano";
                    string updateTutarQuery = "UPDATE odeme SET Tutar = Tutar - @ode WHERE MasaNumarası = @masano AND Tutar = @maxTutar";
                   object xtutar= db.ExecuteScalar(getMaxTutarQuery, new MySqlParameter("@masano", masano));
                    db.ExecuteNonQuery(updateTutarQuery, new MySqlParameter("@ode", Convert.ToDecimal(textBox4.Text)), new MySqlParameter("@masano", masano), new MySqlParameter("@maxTutar", Convert.ToDecimal(xtutar)));

                }
                fiscikar();
                toplamTutar = kalan;
                if (decimal.TryParse(textBox4.Text, out decimal tutar))
                {
                    puan = Convert.ToInt32(tutar) / 10;
                }

                textBox4.Text = "0";
                deger = 0;
                odemelerListesi.Clear();
            }
            else
            {
                MessageBox.Show("Lütfen Bir Ödeme Türü Seçiniz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if (radioButton4.Checked)
            {
                FrmPuanE p = new FrmPuanE();
                p.puan = puan;
                p.ShowDialog();
            }
        }
        private void fiscikar()
        {
            DialogResult result = MessageBox.Show("Fişi yazdırmak ister misiniz?", "Yazdırma Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // PrintPreviewDialog ile yazdırma panelini gösterelim
                PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
                printPreviewDialog.Document = printDocument1;
                printPreviewDialog.ShowDialog();
            }
        }

        private List<decimal> odemelerListesi = new List<decimal>();

        private void BtnTutar_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            object buttonText = button.Tag;

            // Dönüştürme işlemi öncesi doğrulama
            if (decimal.TryParse(buttonText.ToString(), out decimal odemeMiktari))
            {
               
                    deger += odemeMiktari;
                    odemelerListesi.Add(odemeMiktari); // Ödemeyi listeye ekle
                    textBox4.Text = deger.ToString();
               
               
            }
            else
            {
                MessageBox.Show("Geçerli bir tutar girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnGeriAl_Click(object sender, EventArgs e)
        {
            if (odemelerListesi.Count > 0)
            {
                decimal sonOdeme = odemelerListesi[odemelerListesi.Count - 1]; // Son ödemeyi al
                deger -= sonOdeme; // Ödemeyi geri al
                textBox4.Text = deger.ToString();
                odemelerListesi.RemoveAt(odemelerListesi.Count - 1); // Son ödemeyi listeden çıkar
            }
            else
            {
                MessageBox.Show("Geri alınacak ödeme yok.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void NakitOdeme()
        {
            if (!decimal.TryParse(textBox4.Text, out decimal odenenTutar))
            {
                MessageBox.Show("Lütfen geçerli bir tutar girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (decimal.TryParse(textBox3.Text, out decimal toplamTutar))
            {
                if (odenenTutar >= toplamTutar)
                {
                    decimal paraUstu = odenenTutar - toplamTutar;
                    MessageBox.Show("Ödeme Tamamlandı\nPara Üstü: " + paraUstu.ToString(), "Ödeme Tamamlandı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    string gquery = "UPDATE sepet SET ODurum=0 WHERE MasaNumarası=@masano";
                    db.ExecuteNonQuery(gquery, new MySqlParameter("@masano", masano));
                }
                else
                {
                    MessageBox.Show("Ödenen tutar yeterli değil.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Toplam tutar geçerli değil.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void KrediKartiOdeme()
        {
            // Kredi kartı ödeme işlemi burada gerçekleştirilebilir.
            MessageBox.Show("Kredi Kartı ile Ödeme Yapıldı.", "Ödeme Tamamlandı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            string gquery = "UPDATE sepet SET ODurum=0 WHERE MasaNumarası=@masano";
            db.ExecuteNonQuery(gquery, new MySqlParameter("@masano", masano));
        }

        private void TicketOdeme()
        {
            // Ticket ödeme işlemi burada gerçekleştirilebilir.
            MessageBox.Show("Ticket ile Ödeme Yapıldı.", "Ödeme Tamamlandı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            string gquery = "UPDATE sepet SET ODurum=0 WHERE MasaNumarası=@masano";
            db.ExecuteNonQuery(gquery, new MySqlParameter("@masano", masano));
        }


        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            string masaNo = masano;

            // Fiş düzenini belirlemek için gerekli bilgileri alalım
            if (textBox4.Text == "0" || string.IsNullOrWhiteSpace(textBox4.Text))
            {
                odenenTutar = toplamTutar; // Ticket veya kredi kartı seçeneği seçildiğinde ödenecek tutar toplam tutara eşit olmalı

            }
            else
            {
                if (!decimal.TryParse(textBox4.Text, out odenenTutar))
                {
                    MessageBox.Show("Geçerli bir tutar girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            string dec = (odenenTutar - toplamTutar).ToString();
            string odemeYontemi = radioButton1.Checked ? "Nakit" : radioButton2.Checked ? "Kredi Kartı" : radioButton3.Checked ? "Ticket" : "";
            string paraustu = odenenTutar > toplamTutar ? dec : odenenTutar <= toplamTutar ? "0,00" : "";
            // Yazı fontu ve fırçası oluşturun
            Font myFont = new Font("Calibri", 18);
            SolidBrush sbrush = new SolidBrush(Color.Black);
            string siparisler = textBox2.Text.Replace(",", ",\n");
            // Fiş içeriğini oluşturun
            string fisMetni = "Masa Numarası: " + masaNo + "\n" +
                              "--------------------------------------------\n" +
                              "Siparişler: " + siparisler + "\n";
            if (rbtTam.Checked)
            {
                // Toplam tutar, ödenen tutar ve para üstü
                fisMetni += "--------------------------------------------\n" +
                            "Toplam Tutar: \t" + tutucu + "\n" +
                            "Ödenen Tutar: \t" + odenenTutar.ToString() + "\n" +
                            "Para Üstü: \t" + paraustu + "\n" +
                            "--------------------------------------------\n" +
                            "Ödeme Yöntemi: \t" + odemeYontemi + "\n" +
                            "--------------------------------------------\n";
            }
            else
            {
                fisMetni += "--------------------------------------------\n" +
                       "Toplam Tutar: \t" + tutucu + "\n" +
                       "Kalan Tutar: \t" + kalan + "\n" +
                       "Toplam Ödene Tutar:  " + (tutucu - kalan).ToString() + "\n" +
                       "Ödenen Tutar: \t" + odenenTutar.ToString() + "\n" +
                       "Para Üstü: \t" + paraustu + "\n" +
                       "--------------------------------------------\n" +
                       "Ödeme Yöntemi: \t" + odemeYontemi + "\n" +
                       "--------------------------------------------\n";
            }

            // Fişi yazdırma
            e.Graphics.DrawString(fisMetni, myFont, sbrush, 60, 60);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox4.Text = textBox3.Text;
        }
    }
}





