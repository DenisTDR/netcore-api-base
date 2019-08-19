
namespace API.Base.Web.Base.Models.ViewModels
{
    public class DataResponseModel
    {
        public DataResponseModel(object data = null)
        {
            Data = data;
        }

        public object Data { get; set; }
    }
}