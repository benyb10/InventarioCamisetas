namespace XWY.WebAPI.Business.DTOs
{
    public class ResponseDto<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; }

        public ResponseDto()
        {
            Errors = new List<string>();
        }

        public ResponseDto(T data, string message = "Operación exitosa")
        {
            Success = true;
            Message = message;
            Data = data;
            Errors = new List<string>();
        }

        public ResponseDto(string error)
        {
            Success = false;
            Message = "Error en la operación";
            Errors = new List<string> { error };
        }

        public ResponseDto(List<string> errors)
        {
            Success = false;
            Message = "Error en la operación";
            Errors = errors ?? new List<string>();
        }
    }

    public class PagedResponseDto<T>
    {
        public List<T> Items { get; set; }
        public int TotalRegistros { get; set; }
        public int PaginaActual { get; set; }
        public int RegistrosPorPagina { get; set; }
        public int TotalPaginas { get; set; }
        public bool TienePaginaAnterior { get; set; }
        public bool TienePaginaSiguiente { get; set; }

        public PagedResponseDto()
        {
            Items = new List<T>();
        }

        public PagedResponseDto(List<T> items, int totalRegistros, int paginaActual, int registrosPorPagina)
        {
            Items = items;
            TotalRegistros = totalRegistros;
            PaginaActual = paginaActual;
            RegistrosPorPagina = registrosPorPagina;
            TotalPaginas = (int)Math.Ceiling(totalRegistros / (double)registrosPorPagina);
            TienePaginaAnterior = paginaActual > 1;
            TienePaginaSiguiente = paginaActual < TotalPaginas;
        }
    }
}
