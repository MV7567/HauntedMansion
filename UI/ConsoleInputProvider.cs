namespace HauntedMansion.UI
{
    // implementation of input for the console
    public class ConsoleInputProvider : IInputProvider
    {
        public int GetIntInput(int min, int max)
        {
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int choice) && choice >= min && choice <= max)
                    return choice;
                    
                Console.Write($"Invalid choice (must be {min}-{max}). Try again: ");
            }
        }

        public void WaitForContinue()
        {
            Console.ReadLine();
        }
    }
}