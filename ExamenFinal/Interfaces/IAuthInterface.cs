using Contrato.DTO;

namespace ExamenFinal.Interfaces
{
    public interface IAuthInterface
    {
        void Registrar(LoginDTO dto);
        LoginRespuestaDTO? Login(LoginDTO dto);
    }
}