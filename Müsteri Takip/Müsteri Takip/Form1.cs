using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Müsteri_Takip
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent(); // Formun bileşenlerini başlatır.
        }

        private void button1_Click(object sender, EventArgs e)
        {
            musteriekle musteriekle = new musteriekle(); // Yeni bir müşteri ekleme formunu oluşturur.
            musteriekle.ShowDialog(); // Oluşturulan müşteri ekleme formunu diyalog olarak açar.
        }

        private void button2_Click(object sender, EventArgs e)
        {
            musteriliste musteriliste = new musteriliste(); // Müşteri listesini görüntüleme formunu oluşturur.
            musteriliste.ShowDialog(); // Oluşturulan müşteri listeleme formunu diyalog olarak açar.
        }

        private void button4_Click(object sender, EventArgs e)
        {
            urunekle ürünekle = new urunekle(); // Yeni bir ürün ekleme formunu oluşturur.
            ürünekle.ShowDialog(); // Ürün ekleme formunu diyalog olarak açar.
        }

        private void button3_Click(object sender, EventArgs e)
        {
            urunliste urunliste = new urunliste(); // Ürün listesini görüntüleme formunu oluşturur.
            urunliste.ShowDialog(); // Ürün listeleme formunu diyalog olarak açar.
        }

        private void button5_Click(object sender, EventArgs e)
        {
            comboKategori kategorivemarkaekle = new comboKategori(); // Kategori ve marka ekleme formunu oluşturur.
            kategorivemarkaekle.ShowDialog(); // Kategori ve marka ekleme formunu diyalog olarak açar.
        }

        private void button6_Click(object sender, EventArgs e)
        {
            satis satis = new satis(); // Satış işlemleri formunu oluşturur.
            satis.ShowDialog(); // Satış formunu diyalog olarak açar.
        }
    }
}
