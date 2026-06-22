namespace HauntedMansion.UI
{
    /// <summary>
    /// abstract user input
    /// </summary>
    public interface IInputProvider
    {
        int GetIntInput(int min, int max);
        void WaitForContinue();
    }
}