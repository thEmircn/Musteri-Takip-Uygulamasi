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
    public partial class musteriekle : Form
    {
        public musteriekle()
        {
            InitializeComponent(); // Formun bileşenlerini başlatır.
        }

        // Access veritabanı bağlantısı
        OleDbConnection baglanti = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0; Data Source=musteritakip.mdb");

        private void button1_Click(object sender, EventArgs e)
        {
            baglanti.Open(); // Veritabanı bağlantısını açar.

            // Müşteri ekleme sorgusu oluşturulur.
            OleDbCommand komut = new OleDbCommand("INSERT INTO musteri (tc, ad, soyad, tel, mail, adres) VALUES (@tc, @ad, @soyad, @tel, @mail, @adres)", baglanti);

            // Parametrelerin atanması
            komut.Parameters.AddWithValue("@tc", txtTc.Text); // TC kimlik numarası
            komut.Parameters.AddWithValue("@ad", txtAd.Text); // Müşteri adı
            komut.Parameters.AddWithValue("@soyad", txtSoyad.Text); // Müşteri soyadı
            komut.Parameters.AddWithValue("@tel", txtTel.Text); // Telefon numarası
            komut.Parameters.AddWithValue("@mail", txtMail.Text); // E-posta adresi
            komut.Parameters.AddWithValue("@adres", txtAdres.Text); // Adres bilgisi

            // Sorguyu çalıştırır (veritabanına kaydeder).
            komut.ExecuteNonQuery();

            baglanti.Close(); // Veritabanı bağlantısını kapatır.

            // Kullanıcıya işlemin başarılı olduğunu bildiren bir mesaj gösterir.
            MessageBox.Show("Müşteri Kaydı Tamamlandı.");

            // Formdaki tüm TextBox kontrollerini temizler.
            foreach (Control item in this.Controls) // Form üzerindeki kontroller arasında gezinir.
            {
                if (item is TextBox) // Eğer kontrol bir TextBox ise
                {
                    item.Text = ""; // TextBox'ın içeriğini temizler.
                }
            }
        }
    }
}
