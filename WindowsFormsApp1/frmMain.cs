using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class frmMain : Form
    {

        Vorcyc.PowerLibrary.Windows.WinForms.ClickThroughExtender clickThroughExtender;

        Vorcyc.PowerLibrary.Windows.Shell.UserActivityHook hook = new Vorcyc.PowerLibrary.Windows.Shell.UserActivityHook();
        public frmMain()
        {
            InitializeComponent();

            hook.OnMouseActivity += Hook_OnMouseActivity;
            hook.Start();

            clickThroughExtender = new Vorcyc.PowerLibrary.Windows.WinForms.ClickThroughExtender(this.Handle)
            {
                WindowActiveAlpha = 0.8f,
                WindowTransparentAlpha = 0.3f,
                IsCanClickThrough = true
            };

        }

        private void Hook_OnMouseActivity(object sender, Vorcyc.PowerLibrary.Windows.Shell.MouseEventArgs e)
        {
            Console.WriteLine(e.Location);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Escape)
            {
                clickThroughExtender.IsCanClickThrough = false;
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.KeyCode == Keys.Escape)
            {
                clickThroughExtender.IsCanClickThrough = true;
            }
        }
    }
}
