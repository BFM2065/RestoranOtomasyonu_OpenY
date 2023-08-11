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
using System.Threading;
using Microsoft.AspNetCore.SignalR.Client;

namespace RestorantOto
{
    public partial class FrmSiparis : Form
    {


        private int lastId; // Son ID değeri
        private System.Threading.Timer timer;
      
        public FrmSiparis()
        {
            InitializeComponent();
            lastId = GetMaxId();
            //Timer oluşturarak belirli aralıklarla sorgu yapın
            timer = new System.Threading.Timer(CheckForUpdates, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }

       


        private int GetMaxId()
        {
            // Veritabanı bağlantı dizesi
            string connectionString = "Server=localhost;Database=restorantys;Uid=root;Pwd=1234Fm6789;";

            // Sorgu
            string query = "SELECT MAX(SepetID) FROM sepet";

            // Veritabanına bağlanın
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand(query, connection);
                    var result = command.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        return Convert.ToInt32(result);
                    }
                }
                catch (Exception ex)
                {
                    // Hata durumunu işleyin
                    MessageBox.Show("Veritabanı hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return 0;
        }

        private void CheckForUpdates(object state)
        {
            int currentMaxId = GetMaxId();

            if (currentMaxId > lastId)
            {
                // Veritabanında yeni bir sipariş var, güncellemeleri yapın
                lastId = currentMaxId;

                // Invoke kullanarak ana iş parçacığına geçiş yapın
                Invoke(new Action(() =>
                {
                    listele();
                    siparisLoad();
                }));
            }
        }

        private void FrmSiparis_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Form kapatıldığında timer'ı durdurma
            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }


        DbHelper db = new DbHelper();
        private List<Button> masaButonlari = new List<Button>();
        private int kat = 1;
        private int sira = 1;

        private string GetNumericPart(string text)
        {
            string numericPart = new string(text.Where(char.IsDigit).ToArray());
            return numericPart;
        }


        void rezerv(int i)
        {
            FrmSiparisBilgileri sip = new FrmSiparisBilgileri();
            sip.siparisMasano = i;
            sip.ShowDialog();
            butonkonumu();
            listele();
            siparisLoad();


        }
        public void listele()
        {
            try
            {

                string query = "SELECT sepet.MasaNumarası AS 'Masa Numarası', urunler.urunAD AS 'Sipariş Adı', terminal.TAd AS 'Terminal Adı' " +
                               "FROM sepet " +
                               "JOIN urunler ON sepet.urunlerID = urunler.urunID " +
                               "JOIN terminal ON sepet.TerminalID = terminal.TerminalID " +
                               "WHERE sepet.Durum = 1";

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

        private void btnlist_Click(object sender, EventArgs e)
        {
            listele();
        }
        private void butonkonumu()
        {
            try
            {
                string selectQuery = "SELECT masasayisi.* " +
                                     "FROM masasayisi " +
                                     "JOIN sepet ON masasayisi.ID = sepet.MasaNumarası " +
                                     "WHERE sepet.Durum = 1  ;";

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
                int butonArasiMesafe = 220; // Butonlar arasındaki mesaf

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



        public void siparisLoad()
        {
            try
            {

                string queryButonSayisi = "SELECT COUNT(ID) FROM masasayisi WHERE AlanID = @kat AND alanNo = @sıra";
                MySqlCommand d2 = new MySqlCommand(queryButonSayisi, db.baglantiopen());
                d2.Parameters.AddWithValue("@kat", kat);
                d2.Parameters.AddWithValue("@sıra", sira);
                object sayi = d2.ExecuteScalar();

                for (int i = 0; i < Convert.ToInt32(sayi); i++)
                {
                    string query1 = "SELECT MasaNumarası FROM sepet WHERE Durum = 1";
                    foreach (Button masa in masaButonlari)
                    {
                        masa.BackColor = DefaultBackColor;
                    }

                    MySqlCommand command = new MySqlCommand(query1, db.baglantiopen());
                    MySqlDataReader reader = command.ExecuteReader();

                    List<int> siparisBekleyenMasalar = new List<int>();

                    // Sipariş durumu 1 olan masaları bir listede saklayın
                    while (reader.Read())
                    {
                        int masaNumarasi = Convert.ToInt32(reader["MasaNumarası"]);
                        siparisBekleyenMasalar.Add(masaNumarasi);
                    }

                    reader.Close();

                    foreach (Button masa in masaButonlari)
                    {
                        int masaNumarasi = Convert.ToInt32(masa.Tag);

                        if (siparisBekleyenMasalar.Contains(masaNumarasi))
                        {
                            if (masaNumarasi == siparisBekleyenMasalar[0])
                            {
                                masa.BackColor = Color.Yellow;
                            }
                            else
                            {
                                masa.BackColor = Color.Red;
                            }
                        }
                    }
                }

            }
            catch (Exception err)
            {
                MessageBox.Show("Hata siparisLoad! " + err.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            listele();
            //labeltext();
        }
        private List<string> masaAdlari = new List<string>();
        private void FrmSiparis_Load(object sender, EventArgs e)
        {
            butonkonumu();
            listele();
            // ListeleAlanlar();
            siparisLoad();


        }
        private void dataGridView1_CellDoubleClick_2(object sender, DataGridViewCellEventArgs e)
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
                    siparisLoad();

                }
            }
            catch
            {

                MessageBox.Show("Boş Alan Sipariş Bulunamadı", "Sipariş Bulunamadı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
    }


}
//void labeltext()
//{
//    switch (kat)
//    {
//        case 1:
//            label1.Text = "İç Alan " + sira.ToString();
//            break;
//        case 2:
//            label1.Text = "Dış Alan " + sira.ToString();
//            break;
//        case 3:
//            label1.Text = "Diğer Alan " + sira.ToString();
//            break;
//    }
//}
//private void ListeleAlanlar()
//{ 
//    // İç alanları listele
//    ListeleAlanlarKategori(1, icMekanoolStripMenuItem);

//    // Dış alanları listele
//    ListeleAlanlarKategori(2, disMekanToolStripMenuItem);

//    // Diğer alanları listele
//    ListeleAlanlarKategori(3, digerMekanlarToolStripMenuItem);

//}
//private void ListeleAlanlarKategori(int kategori, ToolStripMenuItem menuStrip)
//{
//    try
//    {

//            string query = "SELECT AlanAdi FROM alanlar WHERE kategori = @kategori;";
//            MySqlCommand command = new MySqlCommand(query, db.baglantiopen());
//            command.Parameters.AddWithValue("@kategori", kategori);
//            MySqlDataReader reader = command.ExecuteReader();

//            // Alanları menü strip içine ekle
//            while (reader.Read())
//            {
//                string alanAdi = reader["AlanAdi"].ToString();
//                ToolStripMenuItem yeniAlanMenuItem = new ToolStripMenuItem();
//                yeniAlanMenuItem.Text = alanAdi;
//                yeniAlanMenuItem.Click += (sender, e) =>
//                {
//                    int seciliKategori = kategori; // Seçili kategori
//                    string seciliAlanAdi = alanAdi; // Seçili alan adı
//                    kat = kategori;
//                    string sayisalKisim = new string(alanAdi.Where(char.IsDigit).ToArray());
//                    int sayi = int.Parse(sayisalKisim);
//                    sira = sayi;
//                    // Seçili kategoriye ve alan adına göre masaları getirme işlemleri
//                    GetMasalar(seciliKategori, seciliAlanAdi);
//                    siparisLoad();
//                };
//                menuStrip.DropDownItems.Add(yeniAlanMenuItem);
//            }

//            reader.Close();
//            db.baglanticlose();

//    }
//    catch (Exception ex)
//    {
//        MessageBox.Show("Hata ListeleAlanlarKategori! " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
//    }
//}

//private void GetMasalar(int kategori, string alanAdi)
//{
//    try
//    {
//        string query = "SELECT masasayisi.ID, masasayisi.AD, masasayisi.X, masasayisi.y " +
//                       "FROM masasayisi " +
//                       "JOIN alanlar ON masasayisi.AlanID = alanlar.kategori AND masasayisi.alanNo = alanlar.SıraNo " +
//                       "WHERE alanlar.kategori = @kategori AND alanlar.AlanAdi = @alanAdi;";

//        List<int> masaIDListesi = new List<int>();


//        using (MySqlDataReader reader = db.ExecuteReader(query, new MySqlParameter("@kategori", kategori), new MySqlParameter("@alanAdi", alanAdi)))
//        {
//            while (reader.Read())
//            {
//                int masaID = Convert.ToInt32(reader["ID"]);
//                masaIDListesi.Add(masaID);
//            }
//        }


//        // Önceki butonları temizle
//        foreach (Button buton in panel1.Controls.OfType<Button>().ToList())
//        {
//            panel1.Controls.Remove(buton);
//            masaButonlari.Remove(buton);
//            buton.Dispose();
//        }

//        // Yeni masaları oluştur ve panel içine ekle
//        foreach (int masaID in masaIDListesi)
//        {

//            using (MySqlDataReader reader = db.ExecuteReader("SELECT AD, X, y FROM masasayisi WHERE ID = @masaID;", new MySqlParameter("@masaID", masaID)))
//            {
//                if (reader.Read())
//                {
//                    string masaAdi = reader["AD"].ToString();
//                    int x = Convert.ToInt32(reader["X"]);
//                    int y = Convert.ToInt32(reader["y"]);
//                    DinamikButonEkle(masaAdi, x, y);
//                }
//            }
//        }


//        siparisLoad();
//    }
//    catch (Exception ex)
//    {
//        MessageBox.Show("Masalar listelenirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
//    }
//}
//private void butonkonumu()
//{
//    try
//    {

//            string selectQuery = "SELECT masasayisi.*, alanlar.kategori, alanlar.SıraNo " +
//                                 "FROM masasayisi " +
//                                 "JOIN alanlar ON masasayisi.AlanID = alanlar.kategori AND masasayisi.alanNo = alanlar.SıraNo " +
//                                 "WHERE masasayisi.AlanID = @kat AND masasayisi.alanNo = @sira";

//            MySqlCommand selectCommand = new MySqlCommand(selectQuery, db.baglantiopen());
//            selectCommand.Parameters.AddWithValue("@kat", kat);
//            selectCommand.Parameters.AddWithValue("@sira", sira);
//            MySqlDataReader reader = selectCommand.ExecuteReader();

//            while (reader.Read())
//            {
//                string masaAdi = reader["AD"].ToString();
//                int x = Convert.ToInt32(reader["X"]);
//                int y = Convert.ToInt32(reader["y"]);
//                int masaID = Convert.ToInt32(reader["ID"]);

//                DinamikButonEkle(masaAdi, x, y);
//                // Butona masa ID'sini etiket olarak ekleyelim
//                masaButonlari.Last().Tag = masaID;
//            }

//            reader.Close();
//            db.baglanticlose();

//    }
//    catch (Exception ex)
//    {
//        MessageBox.Show("Hata butonkonumu! " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
//    }
//}


