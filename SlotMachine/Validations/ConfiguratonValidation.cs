namespace SlotMachine.Validations
{
    public static class ConfiguratonValidation
    {

        public static bool DimentionIsValid(int length, int height)
        { 
            return length > 2 && height > 0;

        }

    }
}
