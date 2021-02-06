using System.Windows.Forms;

namespace HanSon
{
    interface IKeyHandler
    {
        void KeyDown(ref KeyEventArgs e);
        void KeyUp(ref KeyEventArgs e);
    }
}
