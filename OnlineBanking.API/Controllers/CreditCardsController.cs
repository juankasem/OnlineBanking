using OnlineBanking.Application.Features.CreditCards.Activate;
using OnlineBanking.Application.Features.CreditCards.Create;
using OnlineBanking.Application.Features.CreditCards.Deactivate;
using OnlineBanking.Application.Features.CreditCards.GetAll;
using OnlineBanking.Application.Features.CreditCards.GetByCustomer;
using OnlineBanking.Application.Features.CreditCards.GetById;
using OnlineBanking.Application.Features.CreditCards.Update;

namespace OnlineBanking.API.Controllers;

public class CreditCardsController : BaseApiController
{
    // GET api/v1/credit-cards/all
    [HttpGet(ApiRoutes.FastTransactions.GetByIBAN)]
    [ProducesResponseType(typeof(PagedList<CreditCardDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<PagedList<CreditCardDto>>> GetAllCreditCards([FromQuery] CreditCardParams creditCardParams,
                                                                                CancellationToken cancellationToken = default)
    {
        var query = new GetAllCreditCardsRequest() { CreditCardParams = creditCardParams };

        var result = await _mediator.Send(query);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok(result.Payload);
    }

    // GET api/v1/credit-cards/TR12345678 
    [HttpGet(ApiRoutes.CreditCards.GetByIBAN)]
    [ProducesResponseType(typeof(IReadOnlyList<CreditCardDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyList<CreditCardDto>>> GetCustomerCreditCards([FromRoute] string customerNo,
                                                                                        CancellationToken cancellationToken = default)
    {
        var query = new GetCustomerCreditCardsRequest() { CustomerNo = customerNo };

        var result = await _mediator.Send(query);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok(result.Payload);
    }

    // GET api/v1/credit-cards/TR12345678 
    [HttpGet(ApiRoutes.CreditCards.IdRoute)]
    [ValidateGuid]
    [ProducesResponseType(typeof(CreditCardDetailsResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CreditCardDetailsResponse>> GetCreditCardById([FromRoute] string id,
                                                                                CancellationToken cancellationToken = default)
    {
        var query = new GetCreditCardByIdRequest() { Id = Guid.Parse(id) };

        var result = await _mediator.Send(query);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok(result.Payload);
    }

    // POST api/v1/credit-cards
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> CreateCreditCard([FromBody] CreateCreditCardRequest request,
                                                        CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<CreateCreditCardCommand>(request);

        var result = await _mediator.Send(command);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok();
    }

    // PUT api/v1/credit-cards/TR12345678
    [HttpPut(ApiRoutes.CreditCards.IdRoute)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateCreditCard([FromBody] UpdateCreditCardRequest request,
                                                        CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<UpdateCreditCardCommand>(request);

        var result = await _mediator.Send(command);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok();
    }


    // PUT api/v1/credit-cards/activate/TR12345678
    [HttpPut(ApiRoutes.CreditCards.Activate)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> ActivateCreditCard([FromRoute] string creditCardNo,
                                                        CancellationToken cancellationToken = default)
    {
        var command = new ActivateCreditCardCommand() 
        { 
            CreditCardNo = creditCardNo 
        };

        var result = await _mediator.Send(command);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok();
    }


    // PUT api/v1/credit-cards/deactivate/TR12345678
    [HttpPut(ApiRoutes.CreditCards.Deactivate)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeactivateCreditCard([FromRoute] string creditCardNo,
                                                        CancellationToken cancellationToken = default)
    {
        var command = new DeactivateCreditCardCommand() 
        { 
            CreditCardNo = creditCardNo 
        };

        var result = await _mediator.Send(command);

        if (result.IsError) HandleErrorResponse(result.Errors);

        return Ok();
    }
}