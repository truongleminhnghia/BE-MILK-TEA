using Data_Access_Layer.Enum;


namespace Business_Logic_Layer.Models.Responses
{
    public class CategoryResponse
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; }
        public DateTime CreateAt { get; set; }
        public CategoryStatus CategoryStatus { get; set; }
        public CategoryType CategoryType { get; set; }


    }
}
