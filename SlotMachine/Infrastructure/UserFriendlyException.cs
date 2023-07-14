namespace SlotMachine
{
    public class UserFriendlyException: Exception
    {
        public string FriendlyMessage { get; set; }
        public UserFriendlyException(string message)
        {
            this.FriendlyMessage = message;
        }
    }
}
