namespace ComicApi.Model
{
    public class ResponseModel
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public bool? Success { get; set; }
        public Object Data { get; set; }

        public static ResponseModel Ok(object obj = null, string message = "")
        {
            return new ResponseModel { Code = 0, Data = obj ?? new object(), Message = message, Success = true };
        }
    }
}
