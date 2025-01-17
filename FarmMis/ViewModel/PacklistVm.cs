namespace FarmMis.ViewModel
{
    public class PacklistVm
    {
        public DateTime Date { get; set; }
    }

    public class CustomerProduct
    {
        public string Barcode { get; set; }
        public int ClientId { get; set; }
        public int BranchId { get; set; }
        public DateTime Date { get; set; }
    }
}
