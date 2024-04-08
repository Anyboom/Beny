using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beny.Interfaces
{
    public interface IDialogService
    {
        void ShowDialog<TView, TViewModel>();
        void ShowDialog<TView, TViewModel>(Action<TViewModel> func);
        void ShowDialog<TView>();
        void Show<TView>();
        void Show<TView, TViewModel>();
        void Show<TView, TViewModel>(Action<TViewModel> func);
    }
}
