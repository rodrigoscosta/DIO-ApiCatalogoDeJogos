using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCatalogoDeJogos.Exceptions
{
    public class GameJaCadastradoException : Exception
    {
        public GameJaCadastradoException()
            : base("Este jogo já está cadastrado")
        { }
    }
}
