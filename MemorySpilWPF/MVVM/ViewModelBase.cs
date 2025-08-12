using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

internal class ViewModelBase : INotifyPropertyChanged
{
    //Interface fra "PropertyChange"
    public event PropertyChangedEventHandler? PropertyChanged;

    //Metode til at aktivere dette event
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
