using ApiCatalogoDeJogos.Exceptions;
using ApiCatalogoDeJogos.InputModel;
using ApiCatalogoDeJogos.Services;
using ApiCatalogoDeJogos.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCatalogoDeJogos.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]

    public class JogosController : ControllerBase
    {
        private readonly IGameService _gameService;

        public JogosController(IGameService gameService)
        {
            _gameService = gameService;
        }

        /// <summary>
        /// Buscar todos os jogos de forma paginada
        /// </summary>
        /// <remarks>
        /// Não é possível retornar os jogos sem paginação
        /// </remarks>
        /// <param name="pagina">Indica qual página está sendo consultada. Mínimo 1</param>
        /// <param name="quantidade">Indica a quantidade de reistros por página. Mínimo 1 e máximo 50</param>
        /// <response code="200">Retorna a lista de jogos</response>
        /// <response code="204">Caso não haja jogos</response>

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameViewModel>>> Obter([FromQuery, Range(1, int.MaxValue)] int pagina = 1, [FromQuery, Range(1, 50)] int quantidade = 5)
        {
            var games = await _gameService.Obter(pagina, quantidade);

            if (games.Count() == 0)
                return NoContent();
            return Ok(games);
        }

        /// <summary>
        /// Buscar um jogo pelo seu Id
        /// </summary>
        /// <param name="idJogo">Id do jogo buscado</param>
        /// <response code="200">Retorna o jogo filtrado</response>
        /// <response code="204">Caso não haja jogo com este id</response>

        [HttpGet("{idGame:guid}")]
        public async Task<ActionResult<List<GameViewModel>>> Obter([FromRoute] Guid idGame)
        {
            var game = await _gameService.Obter(idGame);

            if (game == null)
                return NoContent();
            return Ok(game);
        }

        /// <summary>
        /// Inserir um jogo no catálogo
        /// </summary>
        /// <param name="jogoInputModel">Dados do jogo a ser inserido</param>
        /// <response code="200">Cao o jogo seja inserido com sucesso</response>
        /// <response code="422">Caso já exista um jogo com mesmo nome para a mesma produtora</response>

        [HttpPost]
        public async Task<ActionResult<GameViewModel>> InserirGame([FromBody] GameInputModel gameInputModel)
        {
            try
            {
                var game = await _gameService.Inserir(gameInputModel);
                return Ok(game);
            }
            catch (GameJaCadastradoException ex)
            {
                return UnprocessableEntity("Já existe um jogo com esse nome para esta produtora!");
            }
        }

        /// <summary>
        /// Atualizar um jogo no catálogo
        /// </summary>
        /// /// <param name="idJogo">Id do jogo a ser atualizado</param>
        /// <param name="jogoInputModel">Novos dados para atualizar o jogo indicado</param>
        /// <response code="200">Cao o jogo seja atualizado com sucesso</response>
        /// <response code="404">Caso não exista um jogo com este Id</response>

        [HttpPut("{idGame:guid}")]
        public async Task<ActionResult> AtualizarGame([FromRoute] Guid idGame, [FromRoute] GameInputModel gameInputModel)
        {
            try
            {
                await _gameService.Atualizar(idGame, gameInputModel);
                return Ok();
            }
            catch (GameNaoCadastradoException ex)
            {
                return NotFound("Este jogo não existe!");
            }
        }

        /// <summary>
        /// Atualizar o preço de um jogo
        /// </summary>
        /// /// <param name="idJogo">Id do jogo a ser atualizado</param>
        /// <param name="preco">Novo preço do jogo</param>
        /// <response code="200">Cao o preço seja atualizado com sucesso</response>
        /// <response code="404">Caso não exista um jogo com este Id</response>

        [HttpPatch("{idGame:guid}/preco/{preco:double}")]
        public async Task<ActionResult> AtualizarGame([FromRoute] Guid idGame, [FromRoute] double preco)
        {
            try
            {
                await _gameService.Atualizar(idGame, preco);
                return Ok();
            }
            catch (GameNaoCadastradoException ex)
            {
                return NotFound("Este jogo não existe!");
            }
        }

        /// <summary>
        /// Excluir um jogo
        /// </summary>
        /// /// <param name="idJogo">Id do jogo a ser excluído</param>
        /// <response code="200">Cao o preço seja atualizado com sucesso</response>
        /// <response code="404">Caso não exista um jogo com este Id</response>

        [HttpDelete("{idGame:guid}")]
        public async Task<ActionResult> DeletarGame([FromRoute] Guid idGame)
        {
            try
            {
                await _gameService.Remover(idGame);
                return Ok();
            }
            catch (GameNaoCadastradoException ex)
            {
                return NotFound("Este jogo não existe!");
            }
        }
    }
}
