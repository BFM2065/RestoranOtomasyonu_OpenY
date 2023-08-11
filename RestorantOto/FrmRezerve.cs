
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.util;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MySql.Data;
using MySql.Data.MySqlClient;
using QRCoder;
using Document = iTextSharp.text.Document;

namespace RestorantOto
{
    public partial class FrmRezerve : Form
    {

        private int kat = 1;
        private int sira = 1;
        DbHelper db = new DbHelper();
        public FrmRezerve()
        {
            InitializeComponent();
        }

        void rezerv(int i)
        {
            FrmRezervBilgileri rezgir = new FrmRezervBilgileri();
            rezgir.masano = i.ToString();
            rezgir.ShowDialog();

        }
        void masarez(int i)// tarih kontrölü renk işlemleri 
        {

            rezerv(i);
            drawload();

        }
        private string GetMasaAdiByIndex(int index)
        {
            if (index >= 0 && index < masaButonlari.Count)
            {
                Button masaButonu = masaButonlari[index];
                return masaButonu.Text;
            }

            return string.Empty;
        }
        private string GetNumericPart(string text)
        {
            string numericPart = new string(text.Where(char.IsDigit).ToArray());
            return numericPart;
        }

        void labeltext()
        {
            switch (kat)
            {
                case 1:
                    label1.Text = "İç Alan " + sira.ToString();
                    break;
                case 2:
                    label1.Text = "Dış Alan " + sira.ToString();
                    break;
                case 3:
                    label1.Text = "Diğer Alan " + sira.ToString();
                    break;
            }
        }

        public void drawload()//yüklendiği an tarih kontrolü boyama
        {

            db.baglantiopen();

            string queryButonSayisi = "Select count(ID) from masasayisi where AlanID='" + kat.ToString() + "' AND alanNo='" + sira.ToString() + "'";
            object sayi = db.ExecuteScalar(queryButonSayisi);
            try
            {

                for (int i = 0; i < Convert.ToInt32(sayi); i++)
                {

                    string masaadi = GetMasaAdiByIndex(i);
                    string masanum = GetNumericPart(masaadi);
                    string query = "Select RezervasyonTarihi from masa where MasaNumarası= '" + masanum + "'  AND RezervasyonTarihi > NOW() AND masa.Durum=1 ORDER BY RezervasyonTarihi LIMIT 1 ";//ORDER BY MasaID desc LIMIT 1
                    DateTime gelenTarih = Convert.ToDateTime(db.ExecuteScalar(query));
                    DateTime suankiTarih = DateTime.Now;
                    string gelenTarihStr = gelenTarih.ToString("yyyy-MM-dd HH:mm:ss");
                    string suankiTarihStr = suankiTarih.ToString("yyyy-MM-dd HH:mm:ss");
                    if (DateTime.Parse(suankiTarihStr) > DateTime.Parse(gelenTarihStr))
                    {
                        masaButonlari[i].BackColor = Color.Green;

                    }
                    else if (DateTime.Parse(suankiTarihStr) == DateTime.Parse(gelenTarihStr))
                    {

                        masaButonlari[i].BackColor = Color.Red;

                    }
                    else
                    {
                        masaButonlari[i].BackColor = Color.Red;

                    }
                }

                db.baglanticlose();
            }

            catch (Exception err)
            {
                MessageBox.Show("Hata drawload! " + err.Message, "Girdi Bulunamadı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            listele();
            labeltext();
        }

        public void listele()
        {
            string masaQuery = "SELECT masa.MasaNumarası, musteri.Ad, musteri.Soyad, musteri.Telefon, masa.RezervasyonTarihi, masa.KisiSayisi " +
                               "FROM masa " +
                               "JOIN musteri ON masa.MusteriID = musteri.MusteriID " +
                               "WHERE masa.RezervasyonTarihi >= CURDATE() AND HOUR(masa.RezervasyonTarihi) >= HOUR(CURTIME()) AND masa.Durum = 1";

            DataTable dataTable = db.ExecuteQuery(masaQuery);

            dataTable.Columns.Add("Alan Adı", typeof(string));

            foreach (DataRow row in dataTable.Rows)
            {
                int masaNumarasi = Convert.ToInt32(row["MasaNumarası"]);
                string alanAdi = db.GetAlanAdi(masaNumarasi);
                row["Alan Adı"] = alanAdi;
            }

            dataGridView1.DataSource = dataTable;
        }

        private void butonkonumu()
        {
            string selectQuery = "SELECT masasayisi.*, alanlar.kategori, alanlar.SıraNo FROM masasayisi JOIN alanlar ON masasayisi.AlanID = alanlar.kategori AND masasayisi.alanNo = alanlar.SıraNo WHERE masasayisi.AlanID=@kat AND masasayisi.alanNo=@sira";
            DataTable dataTable = db.ExecuteQuery(selectQuery, new MySqlParameter("@kat", kat), new MySqlParameter("@sira", sira));
            foreach (DataRow row in dataTable.Rows)
            {
                string masaAdi = row["AD"].ToString();
                int x = Convert.ToInt32(row["X"]);
                int y = Convert.ToInt32(row["y"]);
                int masaID = Convert.ToInt32(row["ID"]);
                DinamikButonEkle(masaAdi, x, y);
                // Butona masa ID'sini etiket olarak ekleyelim
                masaButonlari.Last().Tag = masaID;
            }

        }
        private void FrmRezerve_Load(object sender, EventArgs e)
        {
            ListeleAlanlar();
            butonkonumu();
            drawload();
        }

        private void btnlist_Click(object sender, EventArgs e)
        {
            listele();
            drawload();
        }

        private List<Button> masaButonlari = new List<Button>();

        private void DinamikButonEkle(string masaAdi, int x, int y)
        {
            Button yeniButon = new Button();

            yeniButon.Text = masaAdi;
            yeniButon.Location = new Point(x, y);
            yeniButon.Name = masaAdi;
            yeniButon.Width = 110;
            yeniButon.Height = 110;
            yeniButon.ContextMenuStrip = CreateContextMenu();
            yeniButon.Click += (sender, e) =>
            {
                string sayisalKisim = new string(masaAdi.Where(char.IsDigit).ToArray());
                int sayi = int.Parse(sayisalKisim);
                masarez(sayi);

            };
            

            panel1.Controls.Add(yeniButon);
            masaButonlari.Add(yeniButon);// burası her butonu tekrar tekrar listeye ekliyo sadece yeni oluşan butonları değil önceki butonlarda listeleniyor 
        }
        private ContextMenuStrip CreateContextMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem qrMenuItem = new ToolStripMenuItem("QR Oluştur");
            qrMenuItem.Image = Properties.Resources.QR_icon_svg;
            qrMenuItem.Click += QrMenuItem_Click;
            menu.Items.Add(qrMenuItem);
            return menu;
        }

        private void QrMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            ContextMenuStrip contextMenu = (ContextMenuStrip)menuItem.Owner;
            Button button = (Button)contextMenu.SourceControl;

            string masaAdi = button.Text;
            string masaID = new string(masaAdi.Where(char.IsDigit).ToArray());

            string adres = $"localhost:7236/Home/Menu?MasaID={masaID}";

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(adres, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(5); // QR kodunun boyutunu 5 ile çarptık

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "PDF Dosyaları|*.pdf";
            saveDialog.FileName = masaAdi;
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                using (FileStream fs = new FileStream(saveDialog.FileName, FileMode.Create))
                {
                    Document doc = new Document();
                    PdfWriter.GetInstance(doc, fs);
                    doc.Open();

                    iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(qrCodeImage, System.Drawing.Imaging.ImageFormat.Jpeg);
                    img.ScalePercent(50);
                    doc.Add(img);

                    doc.Close();
                }
            }
        }
        // Bir sonraki masa ID'sini almak için kullanılacak metod
        private int GetNextMasaID()
        {
            int nextID = 1;
            // MasaSayisi tablosundaki en yüksek ID değerini bulma
            string maxIDQuery = "SELECT MAX(ID) FROM masasayisi;";
            // Veritabanı bağlantısı ve sorguyu çalıştırma
            object result = db.ExecuteScalar(maxIDQuery);
            if (result != DBNull.Value)
            {
                nextID = Convert.ToInt32(result) + 1;
            }
            return nextID;
        }
        // Bir sonraki masa için y konumunu almak için kullanılacak metod
        private int GetNextYPosition()
        {
            int nextY = 40;
            int masaSayisi = masaButonlari.Count;
            int satir = (masaSayisi / 3) + 1;
            nextY = 40 + (satir - 1) * 165;
            return nextY;
        }
        private int GetNextXPosition()
        {
            int nextX = 45;
            int masaSayisi = masaButonlari.Count;
            if (masaSayisi % 3 == 0)
            {
                nextX = 45;
            }
            else if (masaSayisi % 3 == 1)
            {
                nextX = 381;
            }
            else if (masaSayisi % 3 == 2)
            {
                nextX = 717;
            }
            return nextX;
        }
        private void btnEkle_Click(object sender, EventArgs e)
        {
            panel1.AutoScrollPosition = new Point(0, 0);
            int yeniMasaID = GetNextMasaID();
            string yeniMasaAdi = "M" + yeniMasaID.ToString();
            int yeniMasaX = GetNextXPosition();
            int yeniMasaY = GetNextYPosition();
            // Hangi menü stripteysek ona göre kategori ve sıra numarasını belirle
            // Yeni masa butonunu oluştur ve kaydet
            DinamikButonEkle(yeniMasaAdi, yeniMasaX, yeniMasaY);
            // Yeni masa butonunu veritabanına kaydet
            string insertQuery = "INSERT INTO MasaSayisi (ID, AD, X, y, AlanID, alanNo) VALUES (@ID, @AD, @X, @Y, @AlanID, @AlanNo);";
            db.ExecuteNonQuery(insertQuery,
            new MySqlParameter("@ID", yeniMasaID),
            new MySqlParameter("@AD", yeniMasaAdi),
            new MySqlParameter("@X", yeniMasaX),
            new MySqlParameter("@Y", yeniMasaY),
            new MySqlParameter("@AlanID", kat),
            new MySqlParameter("@AlanNo", sira));
            drawload();
        }
        public void CallUpdateMasaAfterDelete(int masaId)
        {
            string connectionString = "Server=localhost;Database=restorantys;Uid=root;Pwd='1234Fm6789';";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand("UpdateMasaAfterDelete", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@masaId", masaId);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        private void BtnSil_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Masa Silmek İstediğinizden Emin misiniz?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    Button sonButon = masaButonlari.LastOrDefault();
                    string num = GetNumericPart(sonButon?.Text);
                    db.ExecuteNonQuery("DELETE FROM masasayisi WHERE ID=@ID",
                        new MySqlParameter("@ID", num));
                    CallUpdateMasaAfterDelete(Convert.ToInt32(num));
                    db.ExecuteNonQuery("UPDATE masa SET masa.Durum = 0 WHERE MasaNumarası = @Masa",
                        new MySqlParameter("@Masa", num));
                    masaButonlari.RemoveAt(masaButonlari.Count - 1);
                    Button silinenButon = panel1.Controls[panel1.Controls.Count - 1] as Button;
                    panel1.Controls.Remove(silinenButon);
                    panel1.Refresh();
                    drawload();
                }
                catch (Exception ex)
                {
                    // Hata durumunda yapılacak işlemler
                    // Örneğin, hata mesajını göstermek veya loglamak gibi
                    MessageBox.Show("Masa bulunamadı ! " + ex.Message, "Silinecek Masa bulunamadı !", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)//ALAN ekle
        {
            alanKaydet(1, "İç Alan");

        }
        void alanKaydet(int i, string a)
        {
            try
            {
                int kategori = i; // İç alan kategorisi
                // Kategoriye ait alan sayısını veritabanından sorgula
                string query = "SELECT COUNT(*) FROM alanlar WHERE kategori = @kategori;";
                int alanSayisi = Convert.ToInt32(db.ExecuteScalar(query, new MySqlParameter("@kategori", kategori)));
                string alanAdi = a + " " + (alanSayisi + 1);
                // Alanı veritabanına ekle
                string insertQuery = "INSERT INTO alanlar (AlanAdi, kategori, SıraNo) VALUES (@alanAdi, @kategori, @siraNo);";
                db.ExecuteNonQuery(insertQuery, new MySqlParameter("@alanAdi", alanAdi),
                    new MySqlParameter("@kategori", kategori),
                    new MySqlParameter("@siraNo", alanSayisi + 1));
                // Alanı menü strip içine ekle
                ToolStripMenuItem yeniAlanMenuItem = new ToolStripMenuItem();
                yeniAlanMenuItem.Text = alanAdi;
                yeniAlanMenuItem.Click += (sender, e) =>
                {
                    int seciliKategori = kategori; // Seçili kategori
                    string seciliAlanAdi = alanAdi; // Seçili alan adı
                    kat = kategori;
                    sira = (alanSayisi + 1);
                    // Seçili kategoriye ve alan adına göre masaları getirme işlemleri
                    GetMasalar(seciliKategori, seciliAlanAdi);
                };

                switch (kategori)
                {
                    case 1:
                        icalan1ToolStripMenuItem.DropDownItems.Add(yeniAlanMenuItem);
                        break;
                    case 2:
                        dışMekanToolStripMenuItem.DropDownItems.Add(yeniAlanMenuItem);
                        break;
                    case 3:
                        digerAlanlarToolStripMenuItem.DropDownItems.Add(yeniAlanMenuItem);
                        break;
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(a + " eklenirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void ClearSubMenuItems(ToolStripMenuItem item)
        {
            if (item.HasDropDownItems)
            {
                for (int i = item.DropDownItems.Count - 1; i >= 0; i--)
                {
                    ToolStripItem subItem = item.DropDownItems[i];
                    if (subItem is ToolStripMenuItem)
                    {
                        ClearSubMenuItems(subItem as ToolStripMenuItem);
                    }
                    item.DropDownItems.RemoveAt(i);
                    subItem.Dispose();
                }
            }
        }

        private void ListeleAlanlar()
        {
            ClearSubMenuItems(icalan1ToolStripMenuItem);
            ClearSubMenuItems(dışMekanToolStripMenuItem);
            ClearSubMenuItems(digerAlanlarToolStripMenuItem);

            // İç alanları listele
            ListeleAlanlarKategori(1, icalan1ToolStripMenuItem);

            // Dış alanları listele
            ListeleAlanlarKategori(2, dışMekanToolStripMenuItem);

            // Diğer alanları listele
            ListeleAlanlarKategori(3, digerAlanlarToolStripMenuItem);
        }
        private void ListeleAlanlarKategori(int kategori, ToolStripMenuItem menuStrip)
        {
            try
            {
                // Kategoriye ait alanları veritabanından sorgula
                string query = "SELECT AlanAdi FROM alanlar WHERE kategori = @kategori;";


                using (MySqlDataReader reader = db.ExecuteReader(query, new MySqlParameter("@kategori", kategori)))
                {
                    while (reader.Read())
                    {
                        string alanAdi = reader["AlanAdi"].ToString();
                        ToolStripMenuItem yeniAlanMenuItem = new ToolStripMenuItem();
                        yeniAlanMenuItem.Text = alanAdi;
                        yeniAlanMenuItem.Click += (sender, e) =>
                        {
                            int seciliKategori = kategori; // Seçili kategori
                            string seciliAlanAdi = alanAdi; // Seçili alan adı
                            kat = kategori;
                            string sayisalKisim = new string(alanAdi.Where(char.IsDigit).ToArray());
                            int sayi = int.Parse(sayisalKisim);
                            sira = sayi;
                            // Seçili kategoriye ve alan adına göre masaları getirme işlemleri
                            GetMasalar(seciliKategori, seciliAlanAdi);
                        };
                        menuStrip.DropDownItems.Add(yeniAlanMenuItem);
                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("Alanlar listelenirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e) //DIŞ ALAN Ekleme Buttonu
        {
            alanKaydet(2, "Dış Alan");
        }
        private void button4_Click(object sender, EventArgs e)//Diğer Alan Ekleme butonu
        {
            alanKaydet(3, "Diğer Alan");
        }
        private void GetMasalar(int kategori, string alanAdi)
        {
            try
            {
                string query = "SELECT masasayisi.ID, masasayisi.AD, masasayisi.X, masasayisi.y " +
                               "FROM masasayisi " +
                               "JOIN alanlar ON masasayisi.AlanID = alanlar.kategori AND masasayisi.alanNo = alanlar.SıraNo " +
                               "WHERE alanlar.kategori = @kategori AND alanlar.AlanAdi = @alanAdi;";

                List<int> masaIDListesi = new List<int>();


                using (MySqlDataReader reader = db.ExecuteReader(query, new MySqlParameter("@kategori", kategori), new MySqlParameter("@alanAdi", alanAdi)))
                {
                    while (reader.Read())
                    {
                        int masaID = Convert.ToInt32(reader["ID"]);
                        masaIDListesi.Add(masaID);
                    }
                }

                // Önceki butonları temizle
                foreach (Button buton in panel1.Controls.OfType<Button>().ToList())
                {
                    panel1.Controls.Remove(buton);
                    masaButonlari.Remove(buton);
                    buton.Dispose();
                }

                // Yeni masaları oluştur ve panel içine ekle
                foreach (int masaID in masaIDListesi)
                {

                    using (MySqlDataReader reader = db.ExecuteReader("SELECT AD, X, y FROM masasayisi WHERE ID = @masaID;", new MySqlParameter("@masaID", masaID)))
                    {
                        if (reader.Read())
                        {
                            string masaAdi = reader["AD"].ToString();
                            int x = Convert.ToInt32(reader["X"]);
                            int y = Convert.ToInt32(reader["y"]);
                            DinamikButonEkle(masaAdi, x, y);
                        }
                    }
                }


                drawload();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Masalar listelenirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SilButton_Click(object sender, EventArgs e, int kategori)
        {
            try
            {
                string kontrol = "SELECT kategori FROM alanlar WHERE kategori = @kat AND SıraNo = @sıra AND ID = (SELECT MAX(ID) FROM alanlar WHERE kategori = @kat AND SıraNo = @sıra)";


                db.baglantiopen();

                MySqlCommand Alnkont1 = new MySqlCommand(kontrol, db.baglantiopen());
                Alnkont1.Parameters.AddWithValue("@kat", kat);
                Alnkont1.Parameters.AddWithValue("@sıra", sira);
                object alnkategori = Alnkont1.ExecuteScalar();
                string kontrol2 = "SELECT SıraNo FROM alanlar WHERE kategori = @kat AND ID = (SELECT MAX(ID) FROM alanlar WHERE kategori = @kat)";
                MySqlCommand Alnkont2 = new MySqlCommand(kontrol2, db.baglantiopen());
                Alnkont2.Parameters.AddWithValue("@kat", kat);
                Alnkont2.Parameters.AddWithValue("@sıra", sira);
                object alnsıra = Alnkont2.ExecuteScalar();
                db.baglanticlose();

                if (Convert.ToInt32(alnkategori) == kategori && Convert.ToInt32(alnsıra) == sira)
                {
                    if (MessageBox.Show("Seçili alandaki tüm masalar ve alt alanlar silinecektir. Emin misiniz?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        while (masaButonlari.Count > 0)
                        {
                            Button sonButon = masaButonlari.LastOrDefault();
                            string num = GetNumericPart(sonButon?.Text);

                            db.baglantiopen();
                            string query2 = "DELETE FROM masasayisi WHERE ID=@ID";
                            MySqlCommand Sil = new MySqlCommand(query2, db.baglantiopen());
                            Sil.Parameters.AddWithValue("@ID", num);
                            Sil.ExecuteNonQuery();
                            CallUpdateMasaAfterDelete(Convert.ToInt32(num));
                            db.baglanticlose();
                            masaButonlari.RemoveAt(masaButonlari.Count - 1);
                            db.baglantiopen();
                            string queryMasa = "UPDATE masa SET masa.Durum = 0 WHERE MasaNumarası = @Masa";
                            MySqlCommand komutsil = new MySqlCommand(queryMasa, db.baglantiopen());
                            komutsil.Parameters.AddWithValue("@Masa", num);
                            komutsil.ExecuteNonQuery();
                            db.baglanticlose();
                            Button silinenButon = panel1.Controls[panel1.Controls.Count - 1] as Button;
                            panel1.Controls.Remove(silinenButon);
                            panel1.Refresh();
                        }

                        string queryalansil = "DELETE FROM alanlar WHERE kategori = @kat AND SıraNo=@sıra AND ID = (SELECT MAX(ID) FROM (SELECT ID FROM alanlar WHERE kategori = @kat) AS temp)";
                        db.baglantiopen();
                        MySqlCommand AlanSil = new MySqlCommand(queryalansil, db.baglantiopen());
                        AlanSil.Parameters.AddWithValue("@kat", kat);
                        AlanSil.Parameters.AddWithValue("@sıra", sira);
                        AlanSil.ExecuteNonQuery();
                        db.baglanticlose();


                        ListeleAlanlar();
                        drawload();
                    }

                    else
                    {
                        string message = "Lütfen Güvenli Silim İçin Silinecek Alana Gidiniz!!!!";
                        string title = "Uyarı";
                        MessageBoxButtons buttons = MessageBoxButtons.OK;
                        MessageBoxIcon icon = MessageBoxIcon.Warning;
                        MessageBox.Show(message, title, buttons, icon);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Alanlar silinirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void icsil_Click(object sender, EventArgs e)
        {
            SilButton_Click(sender, e, 1);
        }

        private void dsil_Click(object sender, EventArgs e)
        {
            SilButton_Click(sender, e, 2);
        }

        private void disil_Click(object sender, EventArgs e)
        {
            SilButton_Click(sender, e, 3);
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)//Çift  ttıklama olayında rezervee keranı gelsin diye bu var 
        {
            try
            {
                // DataGridView'da bir hücrenin çift tıklama olayını yakalayın.
                if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
                {
                    DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];
                    // Masanın numarasını alın (sıra numarasına göre 1. sütun olabilir, bu numaraya göre ayarlayın).
                    string masaNumarasi = selectedRow.Cells["MasaNumarası"].Value.ToString();
                    masarez(Convert.ToInt32(masaNumarasi));

                }
            }
            catch
            {

                MessageBox.Show("Boş Alan Sipariş Bulunamadı", "Sipariş Bulunamadı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

       
    }
}