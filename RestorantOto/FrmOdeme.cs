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
    public partial class FrmOdeme : Form
    {
        public FrmOdeme()
        {
            InitializeComponent();
        }

        DbHelper db = new DbHelper();
        private List<Button> masaButonlari = new List<Button>();
        void rezerv(int i)
        {
            FrmFis rezgir = new FrmFis();
            rezgir.masano = i.ToString();
            rezgir.ShowDialog();
            listele();
            butonkonumu();

        }
        private string GetNumericPart(string text)
        {
            string numericPart = new string(text.Where(char.IsDigit).ToArray());
            return numericPart;
        }

        // Yeni bir liste oluşturarak eklenen masaların ID'lerini tutalım.
        private void butonkonumu()
        {
            try
            {
                string selectQuery = "SELECT masasayisi.* " +
                                     "FROM masasayisi " +
                                     "JOIN sepet ON masasayisi.ID = sepet.MasaNumarası " +
                                     "WHERE sepet.ODurum = 1  ;";

                MySqlCommand selectCommand = new MySqlCommand(selectQuery, db.baglantiopen());
                MySqlDataReader reader = selectCommand.ExecuteReader();

                List<int> masaIDListesi = new List<int>();

                while (reader.Read())
                {
                    int masaID = Convert.ToInt32(reader["ID"]);
                    masaIDListesi.Add(masaID);
                }

                reader.Close();
                db.baglanticlose();

                // Önceki butonları temizle
                foreach (Button buton in panel1.Controls.OfType<Button>().ToList())
                {
                    panel1.Controls.Remove(buton);
                    masaButonlari.Remove(buton);
                    buton.Dispose();
                }

                masaAdlari.Clear(); // mevcut butonların adlarını temizle

                int satirBaslangicX = 45; // İlk satırın başlangıç X koordinatı
                int satirY = 10; // İlk satırın Y koordinatı
                int butonGenislik = 110; // Butonun genişliği
                int butonYukseklik = 110; // Butonun yüksekliği
                int butonArasiMesafe = 220; // Butonlar arasındaki mesafe

                int butonSayisi = 0;

                foreach (int masaID in masaIDListesi)
                {
                    using (MySqlCommand command = new MySqlCommand("SELECT AD FROM masasayisi WHERE ID = @masaID GROUP BY masasayisi.ID LIMIT 1 ;", db.baglantiopen()))
                    {
                        command.Parameters.AddWithValue("@masaID", masaID);
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            string masaAdi = result.ToString();

                            // Aynı masanumarasına sahip buton varsa yeni buton oluşturma
                            if (!masaAdlari.Contains(masaAdi))
                            {
                                masaAdlari.Add(masaAdi); // Yeni buton eklendiğinde adını listeye ekleyin
                                int x = satirBaslangicX + (butonSayisi % 3) * (butonGenislik + butonArasiMesafe);
                                int y = satirY + (butonSayisi / 3) * (butonYukseklik + butonArasiMesafe);

                                DinamikButonEkle(masaAdi, x, y);
                                butonSayisi++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata butonkonumu! " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private List<string> masaAdlari = new List<string>();
        private void DinamikButonEkle(string masaAdi, int x, int y)
        {

            Button yeniButon = new Button();
            yeniButon.Text = masaAdi;
            yeniButon.Location = new Point(x, y);
            yeniButon.Name = masaAdi;
            yeniButon.Width = 110;
            yeniButon.Height = 110;
            string masatag = GetNumericPart(masaAdi);
            yeniButon.Tag = Convert.ToInt32(masatag);
            yeniButon.BackColor = Color.Red;
            yeniButon.Click += (sender, e) =>
            {
                //Butonların Özellikleri yazılcak click event
                rezerv(Convert.ToInt32(masatag));

            };
            panel1.Controls.Add(yeniButon);
            masaButonlari.Add(yeniButon);// burası her butonu tekrar tekrar listeye ekliyo sadece yeni oluşan butonları değil önceki butonlarda listeleniyor 
            masaAdlari.Add(masaAdi);
        }
        void listele()
        {
            try
            {

                string query = "SELECT sepet.MasaNumarası AS 'Masa Numarası',sepet.Tutar, urunler.urunAD AS 'Sipariş Adı',sepet.Adet ,terminal.TAd AS 'Terminal Adı' " +
                               "FROM sepet " +
                               "JOIN urunler ON sepet.urunlerID = urunler.urunID " +
                               "JOIN terminal ON sepet.TerminalID = terminal.TerminalID " +
                               "WHERE sepet.ODurum = 1";

                DataTable dataTable = db.ExecuteQuery(query);

                // Yeni bir sütun ekleyerek "Alan Adı" başlığını oluşturun
                dataTable.Columns.Add("Alan Adı", typeof(string));

                // Her bir masa numarası için alan adını çekerek yeni sütuna ekleyin
                foreach (DataRow row in dataTable.Rows)
                {
                    int masaNumarasi = Convert.ToInt32(row["Masa Numarası"]);
                    // masasayisi tablosundan alanID ve AlanNo'ya göre alan adını çekin
                    string alanAdi = db.GetAlanAdi(masaNumarasi);
                    // Yeni sütuna alan adını ekleyin
                    row["Alan Adı"] = alanAdi;
                }

                // DataGridView'e verileri doldurun
                dataGridView1.DataSource = dataTable;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata listele! " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FrmOdeme_Load(object sender, EventArgs e)
        {
            butonkonumu();

            listele();

        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // DataGridView'da bir hücrenin çift tıklama olayını yakalayın.
                if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
                {
                    DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];
                    // Masanın numarasını alın (sıra numarasına göre 1. sütun olabilir, bu numaraya göre ayarlayın).
                    string masaNumarasi = selectedRow.Cells["Masa Numarası"].Value.ToString();
                    rezerv(Convert.ToInt32(masaNumarasi));

                }
            }
            catch
            {

                MessageBox.Show("Boş Alan Ödeme Bulunamadı", "Ödeme Bulunamadı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
