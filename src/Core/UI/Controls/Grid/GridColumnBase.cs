using MorseCode.CsJs.Common.Observable;

namespace MorseCode.CsJs.UI.Controls.Grid
{
    public abstract class GridColumnBase<T> : IGridColumn<T>
    {
        public string HeaderText { get; set; }

        public abstract IControl CreateControl(int rowIndex, IReadableObservableProperty<T> item);
    }
}