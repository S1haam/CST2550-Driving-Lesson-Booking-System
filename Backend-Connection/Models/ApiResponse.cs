namespace Backend_Connection.Models
{
    //class can be passed on as any data type(lists, objects, strings, etc.)
    public class ApiResponse<T>
    {
        // object initializer syntax
        public ApiResponse() { }

        // allows you to quickly create a response object
  
        public ApiResponse(bool success, string message, T data = default)
        {
            Success = success;   // Set the success flag
            Message = message;   // Set the message
            Data = data == null ? default! : data; // Set the data (default = null if not provided)
        }

        //indicates whether the request was successful (true/false) 
        public bool Success { get; set; }

        //a message would show for users to read
        public string Message { get; set; }

        //shows the data being returned and its data type (could be a list, object, or null)
        public T? Data { get; set; }
    }
}