using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Müsteri_Takip
{
    public partial class comboKategori : Form
    {
        public comboKategori()
        {
            InitializeComponent();

        }

        OleDbConnection baglanti = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0; Data Source=musteritakip.mdb"); // Veritabanı bağlantısı.
        DataSet daset = new DataSet(); // Verileri geçici olarak tutmak için DataSet oluşturulur.
        int i = 0; // Kategori veya marka ekleme durumu için kontrol değişkeni.
        int sil = 0; // Silme işlemi için kontrol değişkeni.
        int guncelle = 0; // Güncelleme işlemi için kontrol değişkeni.

        public void sifirla()
        {
            button2.Enabled = false; // Sil butonunu devre dışı bırakır.
            button3.Enabled = false; // Güncelle butonunu devre dışı bırakır.
            button4.Visible = false; // Güncelleme onay butonunu gizler.
            button5.Visible = false; // Güncelleme iptal butonunu gizler.
        }

        private void kategorivemarkaekle_Load(object sender, EventArgs e)
        {
            baglanti.Open(); // Veritabanı bağlantısını açar.

            OleDbCommand komut = new OleDbCommand("select kategori from kategori", baglanti); // Kategori tablosundan tüm kategorileri çeker.
            OleDbDataReader reader = komut.ExecuteReader();
            while (reader.Read())
            {
                comboBox1.Items.Add(reader["kategori"].ToString()); // Kategorileri comboBox1'e ekler.
            }
            baglanti.Close(); // Veritabanı bağlantısını kapatır.

            radioButton1.Checked = true; // Varsayılan olarak kategori seçeneğini işaretler.
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (i == 0) // Kategori ekleme işlemi
            {
                baglanti.Open();
                OleDbCommand komut = new OleDbCommand("INSERT INTO kategori (kategori) VALUES (@kategori)", baglanti);
                komut.Parameters.AddWithValue("@kategori", comboBox1.Text); // Girilen kategori değerini alır.
                komut.ExecuteNonQuery(); // Kategoriyi veritabanına ekler.
                MessageBox.Show("Kategori veritabanına eklendi."); // Başarılı ekleme mesajı.
                baglanti.Close();
                kategoriliste(); // Kategori listesini günceller.
            }
            else if (i == 1) // Marka ekleme işlemi
            {
                baglanti.Open();
                OleDbCommand komut = new OleDbCommand("INSERT INTO marka(marka, kategori) VALUES(@marka, @kategori)", baglanti);
                komut.Parameters.AddWithValue("@marka", comboBox2.Text); // Girilen marka değerini alır.
                komut.Parameters.AddWithValue("@kategori", comboBox1.Text); // Seçilen kategori değerini alır.
                komut.ExecuteNonQuery(); // Markayı veritabanına ekler.
                MessageBox.Show("Marka veritabanına eklendi."); // Başarılı ekleme mesajı.
                baglanti.Close();
                markaliste(); // Marka listesini günceller.
            }

            if (radioButton1.Checked) // Eğer kategori seçeneği işaretliyse
            {
                KategoriTablosunuYukle(); // Kategori tablosunu günceller.
            }
            else if (radioButton2.Checked) // Eğer marka seçeneği işaretliyse
            {
                MarkaTablosunuYukle(); // Marka tablosunu günceller.
            }
        }
        public void markaliste()
        {
            comboBox2.Items.Clear(); // ComboBox2 içeriğini temizler.
            comboBox2.Text = ""; // ComboBox2'nin metin alanını temizler.

            baglanti.Open(); // Veritabanı bağlantısını açar.

            OleDbCommand komut = new OleDbCommand("SELECT marka FROM Marka WHERE kategori = @kategori", baglanti);
            komut.Parameters.AddWithValue("@kategori", comboBox1.Text); // Seçilen kategoriye göre markaları çeker.

            using (OleDbDataReader reader = komut.ExecuteReader())
            {
                while (reader.Read())
                {
                    comboBox2.Items.Add(reader["marka"].ToString()); // Markaları comboBox2'ye ekler.
                }
            }
            baglanti.Close(); // Veritabanı bağlantısını kapatır.
        }

        public void kategoriliste()
        {
            comboBox1.Items.Clear(); // ComboBox1 içeriğini temizler.
            comboBox1.Text = ""; // ComboBox1'in metin alanını temizler.

            baglanti.Open(); // Veritabanı bağlantısını açar.

            OleDbCommand komut = new OleDbCommand("select kategori from kategori", baglanti); // Tüm kategorileri çeker.
            OleDbDataReader reader = komut.ExecuteReader();
            while (reader.Read())
            {
                comboBox1.Items.Add(reader["kategori"].ToString()); // Kategorileri comboBox1'e ekler.
            }
            baglanti.Close(); // Veritabanı bağlantısını kapatır.
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            markaliste(); // Marka listesini günceller.
            bool varMi = false; // Kategori varlık kontrolü için değişken.
            baglanti.Open(); // Veritabanı bağlantısını açar.

            OleDbCommand komut = new OleDbCommand("SELECT COUNT(*) FROM kategori WHERE kategori = @kategori", baglanti);
            komut.Parameters.AddWithValue("@kategori", comboBox1.Text); // Girilen kategori değerini kontrol eder.

            varMi = (int)komut.ExecuteScalar() > 0; // Kategori varsa true, yoksa false döner.

            if (varMi) // Eğer kategori varsa
            {
                button1.Text = "Sadece Marka Ekle"; // Marka ekleme moduna geçiş yapar.
                button2.Enabled = true; // Sil butonunu aktif eder.
                button3.Enabled = true; // Güncelle butonunu aktif eder.
                i = 1; // Marka işlemi yapılacağını belirtir.
                comboBox2.Enabled = true; // ComboBox2'yi aktif eder.
                comboBox2.Items.Clear(); // ComboBox2 içeriğini temizler.

                komut = new OleDbCommand("SELECT marka FROM Marka WHERE kategori = @kategori", baglanti);
                komut.Parameters.AddWithValue("@kategori", comboBox1.Text); // Seçilen kategoriye göre markaları çeker.

                using (OleDbDataReader reader = komut.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        comboBox2.Items.Add(reader["marka"].ToString()); // Markaları comboBox2'ye ekler.
                    }
                }
            }
            else // Eğer kategori yoksa
            {
                button1.Text = "Sadece Kategori Ekle"; // Kategori ekleme moduna geçiş yapar.
                button2.Enabled = false; // Sil butonunu devre dışı bırakır.
                comboBox2.Items.Clear(); // ComboBox2 içeriğini temizler.
                comboBox2.Text = ""; // ComboBox2'nin metin alanını temizler.
                comboBox2.Enabled = false; // ComboBox2'yi devre dışı bırakır.
                button2.Enabled = false; // Sil butonunu devre dışı bırakır.
                button3.Enabled = false; // Güncelle butonunu devre dışı bırakır.
                i = 0; // Kategori işlemi yapılacağını belirtir.
            }
            baglanti.Close(); // Veritabanı bağlantısını kapatır.

            if (comboBox1.Text == null) // Eğer kategori alanı boşsa
            {
                button2.Enabled = false; // Sil butonunu devre dışı bırakır.
                button3.Enabled = false; // Güncelle butonunu devre dışı bırakır.
                sifirla(); // Formu varsayılan duruma getirir.
            }
        }





        private void comboBox2_TextChanged(object sender, EventArgs e)
        {
            baglanti.Open(); // Veritabanı bağlantısını açar.

            // Marka ve kategoriye göre marka sayısını kontrol eden sorgu
            OleDbCommand komut = new OleDbCommand("SELECT COUNT(*) FROM marka WHERE marka = @marka AND kategori = @kategori", baglanti);
            komut.Parameters.AddWithValue("@marka", comboBox2.Text); // Marka parametresini ekler.
            komut.Parameters.AddWithValue("@kategori", comboBox1.Text); // Kategori parametresini ekler.

            bool varMi = (int)komut.ExecuteScalar() > 0; // Marka var mı kontrolü.

            if (comboBox2.SelectedItem == null || comboBox2.SelectedIndex == -1) // Eğer marka seçilmemişse
            {
                if (varMi == false) // Marka yoksa
                {
                    button2.Text = "Sadece Kategori Sil"; // Kategori silme moduna geçiş.
                    button3.Text = "Sadece Kategoriyi Güncelle"; // Kategori güncelleme moduna geçiş.
                    button1.Enabled = true; // Kategori ekleme butonunu etkinleştirir.
                    sil = 0; // Silme işlemi kategori için ayarlanır.
                    guncelle = 0; // Güncelleme işlemi kategori için ayarlanır.
                }
                else // Marka varsa
                {
                    button2.Text = "Sadece Markayı Sil"; // Marka silme moduna geçiş.
                    button3.Text = "Sadece Markayı Güncelle"; // Marka güncelleme moduna geçiş.
                    button1.Enabled = false; // Ekleme butonunu devre dışı bırakır.
                    sil = 1; // Silme işlemi marka için ayarlanır.
                    guncelle = 1; // Güncelleme işlemi marka için ayarlanır.
                }
            }
            else // Marka seçiliyse
            {
                button2.Text = "Sadece Markayı Sil"; // Marka silme moduna geçiş.
                button3.Text = "Sadece Markayı Güncelle"; // Marka güncelleme moduna geçiş.
                button1.Enabled = false; // Ekleme butonunu devre dışı bırakır.

                sil = 1; // Silme işlemi marka için ayarlanır.
                guncelle = 1; // Güncelleme işlemi marka için ayarlanır.
            }

            baglanti.Close(); // Veritabanı bağlantısını kapatır.
        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (sil == 1) // Eğer silme işlemi marka içinse
            {
                baglanti.Open(); // Veritabanı bağlantısını açar.
                OleDbCommand komut = new OleDbCommand("DELETE FROM marka WHERE marka = @marka AND kategori = @kategori", baglanti);
                komut.Parameters.AddWithValue("@marka", comboBox2.Text); // Marka parametresini ekler.
                komut.Parameters.AddWithValue("@kategori", comboBox1.Text); // Kategori parametresini ekler.
                komut.ExecuteNonQuery(); // Silme işlemini gerçekleştirir.

                MessageBox.Show("Marka veritabanından Silindi."); // Silme işlemi sonrası kullanıcıya bilgi verir.
                baglanti.Close(); // Veritabanı bağlantısını kapatır.
                markaliste(); // Marka listesini günceller.

            }
            else if (sil == 0) // Eğer silme işlemi kategori içinse
            {
                baglanti.Open(); // Veritabanı bağlantısını açar.

                // İlk olarak marka tablosundan silme işlemi
                OleDbCommand komut = new OleDbCommand("DELETE FROM marka WHERE marka = @marka AND kategori = @kategori", baglanti);
                komut.Parameters.AddWithValue("@marka", comboBox2.Text); // Marka parametresini ekler.
                komut.Parameters.AddWithValue("@kategori", comboBox1.Text); // Kategori parametresini ekler.
                komut.ExecuteNonQuery(); // Silme işlemini gerçekleştirir.
                baglanti.Close(); // Veritabanı bağlantısını kapatır.

                baglanti.Open(); // Veritabanı bağlantısını tekrar açar.

                // Ardından kategori tablosundan silme işlemi
                komut = new OleDbCommand("DELETE FROM kategori WHERE kategori = @kategori", baglanti);
                komut.Parameters.AddWithValue("@kategori", comboBox1.Text); // Kategori parametresini ekler.
                komut.ExecuteNonQuery(); // Silme işlemini gerçekleştirir.

                baglanti.Close(); // Veritabanı bağlantısını kapatır.
                // Listeyi günceller
                markaliste(); // Marka listesini günceller.
                kategoriliste(); // Kategori listesini günceller.
            }

            if (radioButton1.Checked) // Eğer kategori seçiliyse
            {
                KategoriTablosunuYukle(); // Kategori tablosunu yükler.
            }
            else if (radioButton2.Checked) // Eğer marka seçiliyse
            {
                MarkaTablosunuYukle(); // Marka tablosunu yükler.
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button4.Visible = true; // Güncelleme onay butonunu görünür yapar.
            button5.Visible = true; // Güncelleme iptal butonunu görünür yapar.
            button4.Location = new System.Drawing.Point(140, 85); // Güncelleme onay butonunun konumunu ayarlar.
            button5.Location = new System.Drawing.Point(219, 85); // Güncelleme iptal butonunun konumunu ayarlar.
            button3.Visible = false; // Güncelleme butonunu gizler.
            if (guncelle == 1) // Eğer güncelleme işlemi marka içinse
            {
                textBox2.Visible = true; // Marka güncelleme için TextBox'ı görünür yapar.
            }
            else if (guncelle == 0) // Eğer güncelleme işlemi kategori içinse
            {
                textBox1.Visible = true; // Kategori güncelleme için TextBox'ı görünür yapar.
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (guncelle == 0) // Eğer güncelleme işlemi kategori içinse
            {
                // Marka tablosundaki kategori güncellemesi
                baglanti.Open(); // Veritabanı bağlantısını açar.
                OleDbCommand komut = new OleDbCommand("UPDATE marka SET kategori = @kategori WHERE kategori = @eskiKategori", baglanti);
                komut.Parameters.AddWithValue("@kategori", textBox1.Text); // Yeni kategori değeri.
                komut.Parameters.AddWithValue("@eskiKategori", comboBox1.Text); // Eski kategori değeri.
                komut.ExecuteNonQuery(); // Güncelleme işlemini gerçekleştirir.
                baglanti.Close(); // Veritabanı bağlantısını kapatır.

                // Kategori tablosundaki kategori güncellemesi
                baglanti.Open(); // Veritabanı bağlantısını tekrar açar.
                komut = new OleDbCommand("UPDATE kategori SET kategori = @kategori WHERE kategori = @eskiKategori", baglanti);
                komut.Parameters.AddWithValue("@kategori", textBox1.Text); // Yeni kategori değeri.
                komut.Parameters.AddWithValue("@eskiKategori", comboBox1.Text); // Eski kategori değeri.
                komut.ExecuteNonQuery(); // Güncelleme işlemini gerçekleştirir.
                baglanti.Close(); // Veritabanı bağlantısını kapatır.
            }
            else if (guncelle == 1) // Eğer güncelleme işlemi marka içinse
            {
                // Marka tablosundaki marka güncellemesi
                baglanti.Open(); // Veritabanı bağlantısını açar.
                OleDbCommand komut = new OleDbCommand("UPDATE marka SET marka = @marka WHERE marka = @eskimarka", baglanti);
                komut.Parameters.AddWithValue("@marka", textBox2.Text); // Yeni marka değeri.
                komut.Parameters.AddWithValue("@eskimarka", comboBox2.Text); // Eski marka değeri.
                komut.ExecuteNonQuery(); // Güncelleme işlemini gerçekleştirir.
                baglanti.Close(); // Veritabanı bağlantısını kapatır.
            }

            // Kategori ve marka listelerini günceller
            kategoriliste(); // Kategori listesini günceller.
            markaliste(); // Marka listesini günceller.

            // Butonların görünürlüğü ve konumları
            button3.Visible = true; // Güncelleme butonunu görünür yapar.
            button3.Enabled = false; // Güncelleme butonunu devre dışı bırakır.
            button4.Visible = false; // Güncelleme onay butonunu gizler.
            button5.Visible = false; // Güncelleme iptal butonunu gizler.
            button4.Location = new System.Drawing.Point(196, 84); // Güncelleme onay butonunun varsayılan konumu.
            button5.Location = new System.Drawing.Point(275, 84); // Güncelleme iptal butonunun varsayılan konumu.

            // TextBox'ları gizle
            textBox1.Visible = false; // Kategori güncelleme TextBox'ını gizler.
            textBox2.Visible = false; // Marka güncelleme TextBox'ını gizler.

            if (radioButton1.Checked) // Eğer kategori seçiliyse
            {
                KategoriTablosunuYukle(); // Kategori tablosunu yükler.
            }
            else if (radioButton2.Checked) // Eğer marka seçiliyse
            {
                MarkaTablosunuYukle(); // Marka tablosunu yükler.
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button4.Visible = false; // Güncelleme onay butonunu gizler.
            button5.Visible = false; // Güncelleme iptal butonunu gizler.
            button4.Location = new System.Drawing.Point(196, 84); // Güncelleme onay butonunun varsayılan konumu.
            button5.Location = new System.Drawing.Point(275, 84); // Güncelleme iptal butonunun varsayılan konumu.
            button3.Visible = true; // Güncelleme butonunu görünür yapar.

            textBox1.Visible = false; // Kategori güncelleme TextBox'ını gizler.
            textBox2.Visible = false; // Marka güncelleme TextBox'ını gizler.
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked) // Eğer kategori seçeneği işaretliyse
            {
                KategoriTablosunuYukle(); // Kategori tablosunu yükler.
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked) // Eğer marka seçeneği işaretliyse
            {
                MarkaTablosunuYukle(); // Marka tablosunu yükler.
            }
        }




        // Kategori tablosunu DataGridView'e yükleme fonksiyonu
        private void KategoriTablosunuYukle()
        {
            baglanti.Open(); // Veritabanı bağlantısını açar.
            OleDbDataAdapter da = new OleDbDataAdapter("SELECT kategori FROM kategori", baglanti); // Kategori tablosundaki tüm kategorileri çeker.
            DataTable dt = new DataTable(); // Kategoriler için geçici bir tablo oluşturur.
            da.Fill(dt); // Verileri tabloya doldurur.
            dataGridView1.DataSource = dt; // Tabloyu DataGridView'e bağlar.
            baglanti.Close(); // Veritabanı bağlantısını kapatır.
            AyarlariGuncelle(); // DataGridView ayarlarını günceller.
        }

        // Marka tablosunu DataGridView'e yükleme fonksiyonu
        private void MarkaTablosunuYukle()
        {
            baglanti.Open(); // Veritabanı bağlantısını açar.
            OleDbDataAdapter da = new OleDbDataAdapter("SELECT marka, kategori FROM marka", baglanti); // Marka tablosundaki tüm marka ve kategori verilerini çeker.
            DataTable dt = new DataTable(); // Markalar için geçici bir tablo oluşturur.
            da.Fill(dt); // Verileri tabloya doldurur.
            dataGridView1.DataSource = dt; // Tabloyu DataGridView'e bağlar.
            baglanti.Close(); // Veritabanı bağlantısını kapatır.
            AyarlariGuncelle(); // DataGridView ayarlarını günceller.
        }

        private void AyarlariGuncelle()
        {
            if (dataGridView1.Rows.Count > 0) // Eğer DataGridView'de satır varsa
            {
                // Hücre sayısına göre DataGridView genişliğini ayarla
                int toplamHücreSayisi = dataGridView1.Columns.Count; // Toplam sütun sayısını alır.

                if (toplamHücreSayisi == 1) // Eğer tek sütun varsa
                {
                    dataGridView1.Width = 143; // Tek hücre için genişlik ayarı.
                }
                else if (toplamHücreSayisi == 2) // Eğer iki sütun varsa
                {
                    dataGridView1.Width = 243; // İki hücre için genişlik ayarı.
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Eğer bir hücreye tıklandıysa
            if (e.RowIndex >= 0) // Geçerli bir satır seçildiyse
            {
                // Seçim yapılan satırdaki hücre verilerini al
                string kategori = dataGridView1.Rows[e.RowIndex].Cells["kategori"].Value.ToString(); // Kategori bilgisini alır.

                if (radioButton1.Checked) // Eğer kategori seçiliyse
                {
                    comboBox1.Text = kategori; // Kategoriyi comboBox1'e yükler.
                }
                else if (radioButton2.Checked) // Eğer marka seçiliyse
                {
                    string marka = dataGridView1.Rows[e.RowIndex].Cells["marka"].Value.ToString(); // Marka bilgisini alır.
                    comboBox2.Text = marka; // Markayı comboBox2'ye yükler.

                    // Ayrıca ilgili kategoriyi comboBox1'e yükler
                    comboBox1.Text = dataGridView1.Rows[e.RowIndex].Cells["kategori"].Value.ToString(); // Kategoriyi de alır.
                }
            }
        }
    }
}
