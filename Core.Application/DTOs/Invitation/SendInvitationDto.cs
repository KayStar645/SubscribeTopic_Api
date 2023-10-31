namespace Core.Application.DTOs.Invitation
{
    public class SendInvitationDto
    {
        public string? Message { get; set; }

        public string? Status { get; set; }

        public int? GroupId { get; set; }

        public int? StudentJoinId { get; set; }
    }
}
