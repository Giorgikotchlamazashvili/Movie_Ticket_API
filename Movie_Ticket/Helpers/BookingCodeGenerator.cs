namespace Movie_Ticket.Helpers
{
    public class BookingCodeGenerator
    {
        public static string CodeGenerator()
        {
            string code = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();
            return code;
        }
    }
}
