using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Müsteri_Takip
{
    public partial class satis : Form
    {
        public satis()
        {
            InitializeComponent(); // Form bileşenlerini başlatır.
        }

        // Veritabanı bağlantısı
        OleDbConnection baglanti = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0; Data Source=musteritakip.mdb");
        DataSet daset = new DataSet(); // Verileri tutmak için bir DataSet oluşturulur.

        // Sepet listesini veritabanından çeken fonksiyon
        private void sepetListele()
        {
            baglanti.Open(); // Veritabanı bağlantısını açar.
            OleDbDataAdapter adtr = new OleDbDataAdapter("SELECT * FROM satis", baglanti); // Tüm satış kayıtlarını çeker.
            adtr.Fill(daset, "satis"); // Satış verilerini DataSet'e ekler.
            sepetListe.DataSource = daset.Tables["satis"]; // Veriler DataGridView'e bağlanır.
            baglanti.Close(); // Veritabanı bağlantısını kapatır.
        }

        // TC kimlik alanı değiştikçe müşteri bilgilerini otomatik doldurur.
        private void txtTc_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTc.Text)) // TC alanı boşsa
            {
                txtAdSoyad.Clear();
                txtTel.Clear();
            }
            else
            {
                baglanti.Open();
                OleDbCommand komut = new OleDbCommand("SELECT * FROM musteri WHERE tc LIKE @tc", baglanti);
                komut.Parameters.AddWithValue("@tc", txtTc.Text + "%"); // TC'ye göre müşteri arar.
                OleDbDataReader read = komut.ExecuteReader();
                if (read.Read()) // Eğer müşteri bulunursa
                {
                    txtAdSoyad.Text = read["ad"].ToString() + " " + read["soyad"].ToString(); // Ad ve soyad alanını doldurur.
                    txtTel.Text = read["tel"].ToString(); // Telefon alanını doldurur.
                }
                baglanti.Close();
            }
        }

        // Sepetteki toplam fiyatı hesaplar.
        private void hesapla()
        {
            baglanti.Open();
            OleDbCommand komut = new OleDbCommand("SELECT SUM(toplamfiyat) FROM satis", baglanti);
            var result = komut.ExecuteScalar(); // Toplam fiyatı hesaplar.
            lblGenelToplam.Text = (result != DBNull.Value ? result.ToString() : "0") + " TL "; // Toplam fiyatı gösterir.
            baglanti.Close();
        }

        // Barkod alanı değiştikçe ürün bilgilerini doldurur.
        private void txtBarkodNo_TextChanged(object sender, EventArgs e)
        {
            temizle(); // Alanları sıfırlar.
            if (!string.IsNullOrEmpty(txtBarkodNo.Text)) // Barkod alanı boş değilse
            {
                baglanti.Open();
                OleDbCommand komut = new OleDbCommand("SELECT * FROM urun WHERE barkod LIKE @barkod", baglanti);
                komut.Parameters.AddWithValue("@barkod", txtBarkodNo.Text + "%"); // Barkoda göre ürün arar.
                OleDbDataReader read = komut.ExecuteReader();
                if (read.Read()) // Ürün bulunursa
                {
                    txtUrunAdi.Text = read["adi"].ToString(); // Ürün adını doldurur.
                    txtSatisFiyati.Text = read["satis"].ToString(); // Satış fiyatını doldurur.
                }
                baglanti.Close();
            }
            hesapla(); // Genel toplamı günceller.
        }

        // Alanları temizler.
        private void temizle()
        {
            txtUrunAdi.Clear();
            txtSatisFiyati.Clear();
            txtToplamFiyat.Clear();
            txtMiktari.Text = "1"; // Miktar varsayılan olarak 1.
        }

        // Sepete ürün ekler.
        private void btnEkle_Click(object sender, EventArgs e)
        {
            try
            {
                // Boş alan kontrolü
                if (string.IsNullOrEmpty(txtTc.Text) || string.IsNullOrEmpty(txtAdSoyad.Text) ||
                    string.IsNullOrEmpty(txtBarkodNo.Text) || string.IsNullOrEmpty(txtUrunAdi.Text))
                {
                    MessageBox.Show("Tüm alanları doldurmalısınız.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Veri türü kontrolü
                int miktar;
                if (!int.TryParse(txtMiktari.Text, out miktar)) // Miktar sayısal olmalı.
                {
                    MessageBox.Show("Miktar sayısal bir değer olmalıdır.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                double satisFiyati, toplamFiyat;
                if (!double.TryParse(txtSatisFiyati.Text, out satisFiyati) || !double.TryParse(txtToplamFiyat.Text, out toplamFiyat))
                {
                    MessageBox.Show("Satış fiyatı ve toplam fiyat sayısal bir değer olmalıdır.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Ürünü veritabanına ekler.
                baglanti.Open();
                OleDbCommand komut = new OleDbCommand("INSERT INTO satis (tc, adsoyad, telefon, barkod, adi, miktari, satis, toplamfiyat, tarih) " +
                    "VALUES (@tc, @adsoyad, @telefon, @barkod, @adi, @miktari, @satis, @toplamfiyat, @tarih)", baglanti);
                komut.Parameters.AddWithValue("@tc", txtTc.Text);
                komut.Parameters.AddWithValue("@adsoyad", txtAdSoyad.Text);
                komut.Parameters.AddWithValue("@telefon", txtTel.Text);
                komut.Parameters.AddWithValue("@barkod", txtBarkodNo.Text);
                komut.Parameters.AddWithValue("@adi", txtUrunAdi.Text);
                komut.Parameters.AddWithValue("@miktari", miktar);
                komut.Parameters.AddWithValue("@satis", satisFiyati);
                komut.Parameters.AddWithValue("@toplamfiyat", toplamFiyat);
                komut.Parameters.AddWithValue("@tarih", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                komut.ExecuteNonQuery();
                baglanti.Close();

                // Listeyi ve toplamı günceller.
                daset.Tables["satis"].Clear();
                sepetListele();
                temizle();
                hesapla();
                MessageBox.Show("Kayıt başarıyla eklendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                baglanti.Close();
            }
        }

        // Satış miktarı değiştikçe toplam fiyatı hesaplar.
        private void txtMiktari_TextChanged(object sender, EventArgs e)
        {
            try
            {
                double miktar = double.Parse(txtMiktari.Text); // Miktarı alır.
                double satisFiyati = double.Parse(txtSatisFiyati.Text); // Satış fiyatını alır.
                txtToplamFiyat.Text = (miktar * satisFiyati).ToString("F2"); // Toplam fiyatı hesaplar.
            }
            catch
            {
                txtToplamFiyat.Text = "0.00"; // Hata durumunda varsayılan değer.
            }
        }

        // Satış işlemini tamamlar.
        private void btnSatisYap_Click(object sender, EventArgs e)
        {
            try
            {
                if (sepetListe.Rows.Count <= 0) // Sepet boşsa
                {
                    MessageBox.Show("Sepet boş, satış yapmadan önce ürün ekleyin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                baglanti.Open();

                foreach (DataGridViewRow row in sepetListe.Rows) // Sepetteki her ürünü işler.
                {
                    if (row.Cells["barkod"].Value != null)
                    {
                        // Satışı ve stok güncellemelerini yapar.
                        string barkod = row.Cells["barkod"].Value.ToString();
                        int miktari = Convert.ToInt32(row.Cells["miktari"].Value);
                        OleDbCommand komut2 = new OleDbCommand("UPDATE urun SET stok = stok - @stok WHERE barkod = @barkod", baglanti);
                        komut2.Parameters.AddWithValue("@stok", miktari);
                        komut2.Parameters.AddWithValue("@barkod", barkod);
                        komut2.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Satış işlemi başarıyla tamamlandı.", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Satış tablosunu temizler.
                OleDbCommand temizleKomut = new OleDbCommand("DELETE FROM satis", baglanti);
                temizleKomut.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                baglanti.Close();
                sepetListele();
                hesapla();
            }
        }

        private void satisSayfasi_Load(object sender, EventArgs e)
        {
            sepetListele();
        }

        // Sepetten ürün siler.
        private void btnSil_Click(object sender, EventArgs e)
        {
            if (sepetListe.CurrentRow != null) // Bir satır seçiliyse
            {
                string barkod = sepetListe.CurrentRow.Cells["barkod"].Value.ToString(); // Seçili barkodu alır.
                if (MessageBox.Show($"Barkod: {barkod} olan kaydı silmek istediğinizden emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    try
                    {
                        baglanti.Open();
                        OleDbCommand komut = new OleDbCommand("DELETE FROM satis WHERE barkod = @barkod", baglanti);
                        komut.Parameters.AddWithValue("@barkod", barkod);
                        komut.ExecuteNonQuery();
                        baglanti.Close();

                        daset.Tables["satis"].Clear();
                        sepetListele();
                        MessageBox.Show("Seçilen kayıt başarıyla silindi!", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Hata: " + ex.Message);
                        baglanti.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Silmek için bir kayıt seçmelisiniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
