namespace OnlineLearning.DTO
{
    public class UpdatePaymentStatusDto
    {
        public int PaymentId { get; set; } 
        public string  Status { get; set; } // Success / Failed
    }
}
