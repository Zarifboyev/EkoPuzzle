using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class frmEkoPuzzle : Form
    {
        int inNullSliceIndex, qadamlar = 0;
        List<Bitmap> lstOriginalRasmlarList = new List<Bitmap>(); // BitMap rasmlar listi olamiz
        System.Diagnostics.Stopwatch taymer = new System.Diagnostics.Stopwatch(); // Taymer Obyekti aniqlanib olindi

        public frmEkoPuzzle()
        {
            InitializeComponent(); // Komponentalar initsalizatsiya qilinsin
            // 9 ta rasm bo\'lagi va bitta bo'sh rasm yuklanadi      
            lstOriginalRasmlarList.AddRange(new Bitmap[] { Properties.Resources._1, Properties.Resources._2, Properties.Resources._3, Properties.Resources._4, Properties.Resources._5, Properties.Resources._6, Properties.Resources._7, Properties.Resources._8, Properties.Resources._9, Properties.Resources._null });
            lblQadamlarSoni.Text += qadamlar;
            lblQolganVaqt.Text = "00:00:00";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Shuffle();
        }

        void Shuffle()
        {
            do
            {
                int j;
                List<int> Indexes = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 9 });// 8-indeks ko'rsatilmagan - u rasmning oxirgi bo'lagi
                Random r = new Random();
                for (int i = 0; i < 9; i++)
                {
                    Indexes.Remove((j = Indexes[r.Next(0, Indexes.Count)]));
                    ((PictureBox)gbPuzzleBox.Controls[i]).Image = lstOriginalRasmlarList[j];
                    if (j == 9) inNullSliceIndex = i;// Bo'sh (qora fon) rasm indeksini saqlab olamiz
                }
                // 2% holatda random terilgan rasm originali bilan bir xil bo'lib chiqadi
            } while (GolibAniqlandimi()); // Agar Random terilgan puzzle original rasmni o'zi bo'lib chiqsa, jarayon takrorlansin
        }

        private void btnShuffle_Click(object sender, EventArgs e)
        {
            // Qaytadan boshlash Dialog oynasi
            DialogResult HaYokiYoq = new DialogResult();
            if (lblQolganVaqt.Text != "00:00:00")
            {
                HaYokiYoq = MessageBox.Show("QAYTA BOSHLAMOQCHIMSIZ ?", "Eko Puzzle", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }
            if (HaYokiYoq == DialogResult.Yes || lblQolganVaqt.Text == "00:00:00")
            {
                Shuffle();
                taymer.Reset();
                lblQolganVaqt.Text = "00:00:00";
                qadamlar = 0;
                lblQadamlarSoni.Text = "Qadamlar soni : 0";
            }
        }

        private void ChiqishdanOldinRuxsatSora(object sender, FormClosingEventArgs e)
        {
            DialogResult HaYokiYoq = MessageBox.Show("Haqiqatan Dasturdan ham chiqmoqchimisiz ?", "Eko Puzzle", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (sender as Button != btnQuit && HaYokiYoq == DialogResult.No) e.Cancel = true; // Amalni bekor qilish
            if (sender as Button == btnQuit && HaYokiYoq == DialogResult.Yes) Environment.Exit(0); // Dasturdan chiqib ketish
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            ChiqishdanOldinRuxsatSora(sender, e as FormClosingEventArgs);
        }


        /* <== TUIT & github.com/Zarifboyev 2024 ==>
         * Bu dasturda rasmlar quyidagicha joylashgan, deb faraz qilamiz
         *            * * *
         *              * *
         *            * * *   
         * Ko'rib turganingizdek, bo'sh rasm 4-o'rinda joylashgan, uning atrofidagi RasmQutilardan,
         * biri bosilganda undan tepada, pastda, chapdan va o'ngdan joylashgan RasmQutilarining Indekslarini olamiz
         * Va agar shulardan birida bo'sh rasm joylashgan bo'lsa => if (YonQoshniRasmlar.Contains(bosh_rasm_indeks)
         * Rasmlar o'rni almashtiriladi...
         * */
        private void SwitchPictureBox(object sender, EventArgs e)
        {
            if (lblQolganVaqt.Text == "00:00:00")
                taymer.Start();
            int inPictureBoxIndex = gbPuzzleBox.Controls.IndexOf(sender as Control);
            if (inNullSliceIndex != inPictureBoxIndex)
            {
                // Belgilangan Rasm qutisining yon tomonlari, tepa va pastidagi indekslari hisob chiqiladi
                List<int> YonQoshniRasmlar = new List<int>(new int[] { ((inPictureBoxIndex % 3 == 0) ? -1 : inPictureBoxIndex - 1), inPictureBoxIndex - 3, (inPictureBoxIndex % 3 == 2) ? -1 : inPictureBoxIndex + 1, inPictureBoxIndex + 3 });
                // Agar Qora (bo'sh rasm) shu indekslardan birida tursa uning pozitsiyasi o'zgaritiriladi
                if (YonQoshniRasmlar.Contains(inNullSliceIndex))
                {
                    // Bo'sh Rasm turgan qutining indeksiga Tanlanga Rasm qutisidagi rasm o'rnatiladi
                    ((PictureBox)gbPuzzleBox.Controls[inNullSliceIndex]).Image = ((PictureBox)gbPuzzleBox.Controls[inPictureBoxIndex]).Image;
                    // Tanlanga rasmga esa o'z-o'zidan bo'sh (qora) rasm o'rnatiladi
                    ((PictureBox)gbPuzzleBox.Controls[inPictureBoxIndex]).Image = lstOriginalRasmlarList[9];
                    // Qora rasm turgan pozitsiyasi yangilanadi
                    inNullSliceIndex = inPictureBoxIndex;
                    // Qadamlar soni bittaga oshiriladi
                    lblQadamlarSoni.Text = "Qadamlar Soni : " + (++qadamlar);
                    if (GolibAniqlandimi())
                    {
                        taymer.Stop();
                        (gbPuzzleBox.Controls[8] as PictureBox).Image = lstOriginalRasmlarList[8]; //Qora rasm o'rniga yashirilgan rasm bo'lagi joyiga qo'yib qo'yiladi
                        MessageBox.Show("Tabriklaymiz🎉...\n Sizning Gulingiz Juda ham Xursand! 🎉 \n⌚ Vaqt ⌚: " + taymer.Elapsed.ToString().Remove(8) + "\nJami Qadamlar soni ⌚: " + qadamlar, "Eko Puzzle");
                        qadamlar = 0;
                        lblQadamlarSoni.Text = "Qadamlar soni : 0";
                        lblQolganVaqt.Text = "00:00:00";
                        taymer.Reset();
                        Shuffle();
                    }
                }
            }
        }

        bool GolibAniqlandimi()
        {
            int i;
            for (i = 0; i < 8; i++)
            {
                if ((gbPuzzleBox.Controls[i] as PictureBox).Image != lstOriginalRasmlarList[i]) break;
            }
            if (i == 8) return true;
            else return false;
        }

        private void YangilashQolganVaqt(object sender, EventArgs e)
        {
            if (taymer.Elapsed.ToString() != "00:00:00")
                lblQolganVaqt.Text = taymer.Elapsed.ToString().Remove(8);
            if (taymer.Elapsed.ToString() == "00:00:00")
                btnPauza.Enabled = false;
            else
                btnPauza.Enabled = true;
            if (taymer.Elapsed.Minutes.ToString() == "1")
            {
                taymer.Reset();
                lblQadamlarSoni.Text = "Qadamlar soni : 0";
                lblQolganVaqt.Text = "00:00:00";
                qadamlar = 0;
                btnPauza.Enabled = false;
                MessageBox.Show("Vaqt tugadi\nQaytadan boshlash", "Eko Puzzle");
                Shuffle();
            }
        }

        private void PauzaYokiDavomEtish(object sender, EventArgs e)
        {
            if (btnPauza.Text == "Pauza")
            {
                taymer.Stop();
                gbPuzzleBox.Visible = false;
                btnPauza.Text = "Davom Etish";
            }
            else
            {
                taymer.Start();
                gbPuzzleBox.Visible = true;
                btnPauza.Text = "Pauza";
            }
        }
    }
}
