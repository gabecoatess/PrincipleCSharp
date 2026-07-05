using Silk.NET.Input;

namespace Principle.Input;

public class WindowInputContext(IInputContext inputContext)
{
    public event EventHandler<Key>? KeyPressed;

    private IInputContext _inputContext = inputContext;

    public void RegisterInputs()
    {
        for (int i = 0; i < _inputContext.Keyboards.Count; i++)
        {
            _inputContext.Keyboards[i].KeyDown += HandleKeyDown;
        }
    }

    private void HandleKeyDown(IKeyboard keyboard, Key key, int keyCode)
    {
        KeyPressed?.Invoke(this, key);
    }
}