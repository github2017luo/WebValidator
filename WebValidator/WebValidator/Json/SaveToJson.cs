using Newtonsoft.Json;

namespace WebValidator.Json
{
    public class SaveToJson
    {
        public string Serialize(VisitedPagesDto dto)
        {
            return JsonConvert.SerializeObject(dto);
        }
    }
}