using Newtonsoft.Json;
using RestSharp;

public class ImgBBService
{
    private readonly string _apiKey;

    public ImgBBService(string apiKey)
    {
        _apiKey = apiKey;
    }

    public async Task<string> UploadImageAsync(Stream imageStream, string fileName)
    {
        try
        {
            // Configurar el cliente RestSharp
            var client = new RestClient("https://api.imgbb.com/1/upload");

            // Crear una solicitud POST
            var request = new RestRequest { Method = Method.Post };

            // Convertir la imagen a bytes
            byte[] fileBytes;
            using (var memoryStream = new MemoryStream())
            {
                await imageStream.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            // Agregar la imagen a la solicitud
            request.AddFile("image", fileBytes, fileName); 

            // Enviar la solicitud al servicio ImgBB
            var response = await client.ExecuteAsync(request);

            // Verificar si la solicitud fue exitosa
            if (response.IsSuccessful)
            {
                // Deserializar la respuesta para obtener la URL de la imagen
                var deserializedResponse = JsonConvert.DeserializeObject<ImgBBResponse>(response.Content);
                return deserializedResponse?.Data?.Url;
            }

            // Manejar errores si la solicitud no fue exitosa
            var errorResponse = new ImgBBErrorResponse
            {
                ErrorMessage = $"Error al cargar la imagen: {response.ErrorMessage}"
            };

            return JsonConvert.SerializeObject(errorResponse);
        }
        catch (Exception ex)
        {
            // Manejar excepciones inesperadas
            var errorResponse = new ImgBBErrorResponse
            {
                ErrorMessage = $"Error inesperado al cargar la imagen: {ex.ToString()}"
            };

            return JsonConvert.SerializeObject(errorResponse);
        }
    }
}
public class ImgBBResponse
    {
        public ImgBBData Data { get; set; }
    }

    public class ImgBBData
    {
        public string Url { get; set; }
    }

    public class ImgBBErrorResponse
    {
        public string ErrorMessage { get; set; }
    }


