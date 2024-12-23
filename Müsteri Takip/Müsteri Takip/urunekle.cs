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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace Müsteri_Takip
{
    public partial class urunekle : Form
    {
        public urunekle()
        {
            InitializeComponent();
        }
        OleDbConnection baglanti = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0; Data Source=musteritakip.mdb");

        private void btnYeniEkle_Click(object sender, EventArgs e)
        {
            // Barkod numarası kontrol ediliyor
            baglanti.Open();
            OleDbCommand komutKontrol = new OleDbCommand("select * from urun where barkod=@barkodno", baglanti);
            komutKontrol.Parameters.AddWithValue("@barkodno", txtBarkodNo.Text);
            OleDbDataReader read = komutKontrol.ExecuteReader();

            if (read.Read()) // Barkod numarası veritabanında varsa
            {
                // Güncelleme işlemi
                baglanti.Close(); // Mevcut bağlantıyı kapat ve tekrar aç
                baglanti.Open();
                OleDbCommand komutGuncelle = new OleDbCommand("update urun set adi=@adi, kategori=@kategori, marka=@marka, stok=@stok, alis=@alis, satis=@satis where barkod=@barkodno", baglanti);
                komutGuncelle.Parameters.AddWithValue("@adi", txtUrunAdi.Text);
                komutGuncelle.Parameters.AddWithValue("@kategori", comboKategori.Text);
                komutGuncelle.Parameters.AddWithValue("@marka", comboMarka.Text);
                komutGuncelle.Parameters.AddWithValue("@stok", int.Parse(txtMiktari.Text));
                komutGuncelle.Parameters.AddWithValue("@alis", double.Parse(txtAlisFiyati.Text));
                komutGuncelle.Parameters.AddWithValue("@satis", double.Parse(txtSatisFiyati.Text));
                komutGuncelle.Parameters.AddWithValue("@barkodno", txtBarkodNo.Text);
                komutGuncelle.ExecuteNonQuery();
                baglanti.Close();

                MessageBox.Show("Ürün bilgileri güncellendi.");
            }
            else // Barkod numarası veritabanında yoksa yeni ürün ekle
            {
                baglanti.Close(); // Bağlantıyı kapat ve tekrar aç
                baglanti.Open();
                OleDbCommand komutYeniEkle = new OleDbCommand("insert into urun(barkod, adi, kategori, marka, stok, alis, satis, tarih) values(@barkod, @adi, @kategori, @marka, @stok, @alis, @satis, @tarih)", baglanti);
                komutYeniEkle.Parameters.AddWithValue("@barkod", txtBarkodNo.Text);
                komutYeniEkle.Parameters.AddWithValue("@adi", txtUrunAdi.Text);
                komutYeniEkle.Parameters.AddWithValue("@kategori", comboKategori.Text);
                komutYeniEkle.Parameters.AddWithValue("@marka", comboMarka.Text);
                komutYeniEkle.Parameters.AddWithValue("@stok", int.Parse(txtMiktari.Text));
                komutYeniEkle.Parameters.AddWithValue("@alis", double.Parse(txtAlisFiyati.Text));
                komutYeniEkle.Parameters.AddWithValue("@satis", double.Parse(txtSatisFiyati.Text));
                komutYeniEkle.Parameters.AddWithValue("@tarih", DateTime.Now.ToString());
                komutYeniEkle.ExecuteNonQuery();
                baglanti.Close();

                MessageBox.Show("Yeni ürün başarıyla eklendi.");
            }

            // Formu temizleme işlemi
            foreach (Control item in this.Controls)
            {
                if (item is TextBox)
                {
                    item.Text = "";
                }
                if (item is ComboBox)
                {
                    item.Text = "";
                }
            }
        }

        private void txtBarkodNo_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBarkodNo.Text)) // Barkod no boş değilse
            {
                // Barkod kontrolü ve verileri doldurma işlemi
                baglanti.Open();
                OleDbCommand komutKontrol = new OleDbCommand("select * from urun where barkod=@barkodno", baglanti);
                komutKontrol.Parameters.AddWithValue("@barkodno", txtBarkodNo.Text);
                OleDbDataReader read = komutKontrol.ExecuteReader();

                if (read.Read()) // Eğer barkod numarası veritabanında varsa
                {
                    // Ürün bilgilerini form alanlarına doldur
                    txtUrunAdi.Text = read["adi"].ToString();
                    comboKategori.Text = read["kategori"].ToString();
                    comboMarka.Text = read["marka"].ToString();
                    txtMiktari.Text = read["stok"].ToString();
                    txtAlisFiyati.Text = read["alis"].ToString();
                    txtSatisFiyati.Text = read["satis"].ToString();
                    btnYeniEkle.Text = "Güncelle";
                }
                else
                {
                    // Barkod numarası veritabanında yoksa alanları temizle
                    txtUrunAdi.Text = "";
                    comboKategori.Text = "";
                    comboMarka.Text = "";
                    txtMiktari.Text = "";
                    txtAlisFiyati.Text = "";
                    txtSatisFiyati.Text = "";
                    btnYeniEkle.Text = "Ekle";
                }
                baglanti.Close();
            }
        }
    }
}
