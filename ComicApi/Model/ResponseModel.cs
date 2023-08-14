namespace ComicApi.Model
{
    public class ResponseModel
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public bool? Success { get; set; }
        public Object Data { get; set; }
    }
}
