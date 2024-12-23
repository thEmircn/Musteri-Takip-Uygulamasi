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
    public partial class urunliste : Form
    {
        public urunliste()
        {
            InitializeComponent(); // Formun bileşenlerini başlatır.
        }

        // Veritabanı bağlantısı
        OleDbConnection baglanti = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0; Data Source=musteritakip.mdb");
        DataSet daset = new DataSet(); // Verileri geçici olarak tutmak için DataSet oluşturulur.

        // Barkod numarasına göre ürün arama işlemi
        private void txtBarkodNoAra_TextChanged(object sender, EventArgs e)
        {
            DataTable tablo = new DataTable(); // Arama sonuçlarını tutacak geçici tablo.
            baglanti.Open(); // Veritabanı bağlantısını açar.

            // Barkod numarasına göre filtreli ürün sorgusu
            OleDbDataAdapter adtr = new OleDbDataAdapter("SELECT * FROM urun WHERE barkod LIKE '%" + txtBarkodNoAra.Text + "%'", baglanti);
            adtr.Fill(tablo); // Sorgu sonuçlarını tabloya doldurur.
            dataGridView1.DataSource = tablo; // Tabloyu DataGridView'e bağlar.

            baglanti.Close(); // Veritabanı bağlantısını kapatır.
        }

        // Tüm ürünleri listeleyen fonksiyon
        private void liste()
        {
            baglanti.Open(); // Veritabanı bağlantısını açar.
            dataGridView1.Rows.Clear(); // DataGridView üzerindeki mevcut satırları temizler.

            // Tüm ürünleri veritabanından çeker
            OleDbDataAdapter adtr = new OleDbDataAdapter("SELECT * FROM urun", baglanti);
            adtr.Fill(daset, "urun"); // Veriler DataSet'e doldurulur.
            dataGridView1.DataSource = daset.Tables["urun"]; // Veriler DataGridView'e bağlanır.

            baglanti.Close(); // Veritabanı bağlantısını kapatır.
        }

        // Form yüklendiğinde çalışan fonksiyon
        private void urunliste_Load(object sender, EventArgs e)
        {
            liste(); // Tüm ürünleri listeleme işlemini başlatır.
        }
    }
}
