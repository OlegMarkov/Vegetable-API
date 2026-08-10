namespace Vegetable.Entities
{
    public class Discount
    {
        public int Id { get; set; }

        public int Quantity { get; set; }

        public int TrialQuantity { get; set; }

        public int Percentage { get; set; }

        public bool IsEnabled { get; set; }
    }
}
