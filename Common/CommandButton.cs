using System.Windows.Input;

namespace cashregister.Common
{
    public class CommandButton
    {
        public string Label { get; set; } = string.Empty;
        public ICommand Command { get; set; } = default!;
    }
}
