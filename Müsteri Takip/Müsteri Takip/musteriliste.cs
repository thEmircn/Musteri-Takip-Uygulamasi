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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Müsteri_Takip
{
    public partial class musteriliste : Form
    {
        public musteriliste()
        {
            InitializeComponent(); // Formun bileşenlerini başlatır.
        }

        // Veritabanı bağlantısı için gerekli nesne.
        OleDbConnection baglanti = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0; Data Source=musteritakip.mdb");
        DataSet daset = new DataSet(); // Verileri tutmak için bir DataSet oluşturulur.

        // Müşteri listesini yükleyen fonksiyon.
        private void liste()
        {
            baglanti.Open(); // Veritabanı bağlantısını açar.
            OleDbDataAdapter adtr = new OleDbDataAdapter("select * from musteri", baglanti); // Tüm müşteri kayıtlarını seçer.
            adtr.Fill(daset, "musteri"); // Veritabanından alınan veriler DataSet'e doldurulur.
            dataGridView1.DataSource = daset.Tables["musteri"]; // Veriler DataGridView'e bağlanır.
            baglanti.Close(); // Veritabanı bağlantısını kapatır.
        }

        // Güncelleme butonuna tıklanınca çalışan fonksiyon.
        private void btnGüncelle_Click(object sender, EventArgs e)
        {
            baglanti.Open(); // Veritabanı bağlantısını açar.
            OleDbCommand komut = new OleDbCommand("UPDATE musteri SET ad=@ad, soyad=@soyad, tel=@tel, mail=@mail, adres=@adres WHERE tc=@tc", baglanti);
            komut.Parameters.AddWithValue("@ad", txtAd.Text); // Ad alanını günceller.
            komut.Parameters.AddWithValue("@soyad", txtSoyad.Text); // Soyad alanını günceller.
            komut.Parameters.AddWithValue("@tel", txtTel.Text); // Telefon alanını günceller.
            komut.Parameters.AddWithValue("@mail", txtMail.Text); // Mail alanını günceller.
            komut.Parameters.AddWithValue("@adres", txtAdres.Text); // Adres alanını günceller.
            komut.Parameters.AddWithValue("@tc", txtTc.Text); // Hangi kaydın güncelleneceğini belirler (TC kimlik).

            komut.ExecuteNonQuery(); // Sorguyu çalıştırır ve güncelleme işlemini yapar.
            baglanti.Close(); // Veritabanı bağlantısını kapatır.

            // Güncellenen listeyi yeniler.
            daset.Tables["musteri"].Clear();
            liste();

            // Kullanıcıya işlem sonucu mesajı gösterir.
            MessageBox.Show("Müşteri Güncellemesi Tamamlandı.");

            // Formdaki tüm TextBox kontrollerini temizler.
            foreach (Control item in this.Controls)
            {
                if (item is TextBox)
                {
                    item.Text = ""; // TextBox içeriğini temizler.
                }
            }
        }

        // Form yüklendiğinde çalışan fonksiyon.
        private void musteriliste_Load(object sender, EventArgs e)
        {
            liste(); // Müşteri listesini yükler.
        }

        // DataGridView üzerindeki bir hücreye tıklanınca çalışan fonksiyon.
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Seçilen hücredeki müşteri bilgilerini ilgili TextBox'lara doldurur.
            txtTc.Text = dataGridView1.CurrentRow.Cells["tc"].Value.ToString();
            txtAd.Text = dataGridView1.CurrentRow.Cells["ad"].Value.ToString();
            txtSoyad.Text = dataGridView1.CurrentRow.Cells["soyad"].Value.ToString();
            txtTel.Text = dataGridView1.CurrentRow.Cells["tel"].Value.ToString();
            txtMail.Text = dataGridView1.CurrentRow.Cells["mail"].Value.ToString();
            txtAdres.Text = dataGridView1.CurrentRow.Cells["adres"].Value.ToString();
        }

        // Silme butonuna tıklanınca çalışan fonksiyon.
        private void btnsil_Click(object sender, EventArgs e)
        {
            baglanti.Open(); // Veritabanı bağlantısını açar.
            OleDbCommand komut = new OleDbCommand("delete from musteri where tc=@tc", baglanti);
            komut.Parameters.AddWithValue("@tc", dataGridView1.CurrentRow.Cells["tc"].Value.ToString()); // Seçilen müşteri TC'sine göre silme işlemi yapar.
            komut.ExecuteNonQuery(); // Sorguyu çalıştırır ve silme işlemini yapar.
            baglanti.Close(); // Veritabanı bağlantısını kapatır.

            // Güncellenen listeyi yeniler.
            daset.Tables["musteri"].Clear();
            liste();

            // Kullanıcıya işlem sonucu mesajı gösterir.
            MessageBox.Show("Kayıt Silindi.");
        }
    }
}
