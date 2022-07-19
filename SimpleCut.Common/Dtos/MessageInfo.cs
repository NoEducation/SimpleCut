namespace SimpleCut.Common.Dtos
{
    public class MessageInfo
    {
        public List<string> Tos { get; set; }
        public List<(byte[], string)>? Attachments { get; set; } = null;
        public string Title { get; set; }
        public string Message { get; set; }

        public MessageInfo(List<string> tos, List<(byte[], string)>? attachments, string title, string message)
        {
            Tos = tos;
            Attachments = attachments;
            Title = title;
            Message = message;
        }

    }
}
