namespace SlotMachine.Validations
{
    public static class ConfiguratonValidation
    {

        public static bool DimentionIsValid(int length, int height)
        { 
            return length > 0 && height > 0;

        }

    }
}
