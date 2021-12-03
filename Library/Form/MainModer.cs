using Library.Class;
using System.Windows.Forms;

namespace Library
{
    public partial class MainModer : Form
    {
        public MainModer()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            Hide();
            new Moderating().Show();
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            Hide();
            new ModerationUsers().Show();
        }

        private void MainModer_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void MainModer_SizeChanged(object sender, System.EventArgs e)
        {
            WindowState = FormWindowState.Normal;
        }
    }
}
